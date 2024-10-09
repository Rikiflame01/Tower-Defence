using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyAI : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private List<GameObject> targets = new List<GameObject>();
    private Animator animator;

    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileForce = 1000f;
    private float lastAttackTime;

    private RarityHandler rarityHandler;

    [SerializeField] private List<string> targetTags;
    [SerializeField] private float navMeshSearchDistance = 2.0f;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rarityHandler = GetComponent<RarityHandler>();

        if (rarityHandler != null)
        {
            navMeshAgent.speed = rarityHandler.GetSpeed();
        }
        else if (Debug.isDebugBuild)
        {
            Debug.LogError("RarityHandler not found on " + gameObject.name);
        }

        navMeshAgent.stoppingDistance = attackRange - 0.1f;

        FindTargets();
    }

    private void Update()
    {
        if (targets.Count == 0)
        {
            FindTargets();
            animator.SetBool("IsRunning", false);
            return;
        }

        GameObject nearestTarget = GetNearestTarget();
        if (nearestTarget != null)
        {
            Vector3 targetPosition = GetClosestPointOnTarget(nearestTarget);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPosition, out hit, navMeshSearchDistance, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position);
                float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

                if (distanceToTarget <= attackRange)
                {
                    if (Time.time >= lastAttackTime + attackCooldown)
                    {
                        AttackTarget(nearestTarget);
                    }
                    else
                    {
                        animator.SetBool("IsRunning", false);
                    }
                }
                else
                {
                    animator.SetBool("IsRunning", true);
                    animator.SetBool("IsAttacking", false);
                }
            }
            else if (Debug.isDebugBuild)
            {
                Debug.LogWarning("No valid NavMesh position found near target.");
            }
        }
    }

    private void FindTargets()
    {
        targets.Clear();

        foreach (string tag in targetTags)
        {
            GameObject[] foundTargets = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject target in foundTargets)
            {
                targets.Add(target);
            }
        }

        if (Debug.isDebugBuild)
        {
            if (targets.Count == 0)
            {
                Debug.LogWarning("No targets found with specified tags.");
            } 
        }
    }

    private GameObject GetNearestTarget()
    {
        GameObject nearestTarget = null;
        float closestDistance = Mathf.Infinity;

        for (int i = targets.Count - 1; i >= 0; i--)
        {
            GameObject target = targets[i];
            if (target == null || !target.activeInHierarchy)
            {
                targets.RemoveAt(i);
                continue;
            }

            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestTarget = target;
            }
        }

        return nearestTarget;
    }

    private Vector3 GetClosestPointOnTarget(GameObject target)
    {
        Collider targetCollider = target.GetComponent<Collider>();
        if (targetCollider != null)
        {
            return targetCollider.ClosestPoint(transform.position);
        }

        return target.transform.position;
    }

    private void AttackTarget(GameObject target)
    {
        if (projectilePrefab != null)
        {
            animator.SetBool("IsAttacking", true);
            animator.SetBool("IsRunning", false);
            Invoke(nameof(SpawnProjectile), 2.5f);
            lastAttackTime = Time.time;
        }
        else if (Debug.isDebugBuild)
        {
            Debug.LogError("Projectile prefab is not assigned.");
        }
    }

    private void SpawnProjectile()
    {
        if (animator != null)
        {
            animator.SetBool("IsAttacking", false);
        }

        Vector3 spawnPosition = transform.position + Vector3.up * 0.5f;
        GameObject projectileInstance = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        Rigidbody rb = projectileInstance.GetComponent<Rigidbody>();

        if (rb != null)
        {
            GameObject target = GetNearestTarget();
            if (target != null)
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                rb.AddForce(direction * projectileForce);
            }
        }

        animator.SetBool("IsRunning", true);
    }
}