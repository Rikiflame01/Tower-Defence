using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyAI : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private List<GameObject> targets = new List<GameObject>();
    private Animator animator;

    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f;
    private float damage;
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
            damage = rarityHandler.GetDamage();
            navMeshAgent.speed = rarityHandler.GetSpeed();
        }
        else
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
                if (distanceToTarget <= attackRange + 0.1f)
                {
                    if (!animator.GetBool("IsAttacking"))
                    {
                        AttackTarget(nearestTarget);
                    }
                }
                else
                {
                    if (animator.GetBool("IsAttacking"))
                    {
                        animator.SetBool("IsAttacking", false);
                    }
                }
            }
            else
            {
                Debug.LogWarning("No valid NavMesh position found near target.");
            }
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            animator.SetBool("IsAttacking", false);
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

        if (targets.Count == 0)
        {
            Debug.LogWarning("No targets found with specified tags.");
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
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            IHealth health = target.GetComponent<IHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                lastAttackTime = Time.time;
                animator.SetBool("IsAttacking", true);
            }
            else
            {
                Debug.LogWarning("Target does not have an IHealth component.");
            }
        }
    }
}
