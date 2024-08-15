using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyAI : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private GameObject target;
    private Animator animator;

    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f;
    private float damage;
    private float lastAttackTime;

    private RarityHandler rarityHandler;


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
        FindTarget();
    }

    private void Update()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }

        navMeshAgent.SetDestination(target.transform.position);

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if (distanceToTarget <= attackRange)
        {
            AttackTarget();
        }
        else
        {
            animator.SetBool("IsAttacking", false);
        }
    }

    private void FindTarget()
    {
        target = GameObject.FindWithTag("TownHall");

        if (target == null)
        {
            Debug.LogWarning("No object with tag 'TownHall' found in the scene.");
        }
    }

    private void AttackTarget()
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
