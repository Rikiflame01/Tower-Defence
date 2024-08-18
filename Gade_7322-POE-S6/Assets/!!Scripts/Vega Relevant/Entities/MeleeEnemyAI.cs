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
            navMeshAgent.SetDestination(nearestTarget.transform.position);

            float distanceToTarget = Vector3.Distance(transform.position, nearestTarget.transform.position);
            if (distanceToTarget <= attackRange)
            {
                AttackTarget(nearestTarget);
            }
            else
            {
                animator.SetBool("IsAttacking", false);
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

        if (targets.Count == 0)
        {
            Debug.LogWarning("No targets found with specified tags.");
        }
    }

    private GameObject GetNearestTarget()
    {
        GameObject nearestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject target in targets)
        {
            if (target == null)
            {
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
