using UnityEngine;

public class THProjectile : MonoBehaviour
{
    public TownHallProjectileSO projectileData;
    public float speed = 10f;
    private GameObject target;

    private void Start()
    {
        target = EnemyManager.instance.GetNearestEnemy(transform.position);

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        float damage = projectileData.GetDamage();

        if (projectileData.hasHoming)
        {
            Homing(target.transform);
        }
    }

    private void Update()
    {
        if (target != null && projectileData.hasHoming)
        {
            Homing(target.transform);
        }
    }

    private void Homing(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        transform.LookAt(target);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "KnightMeleeEnemy")
        {
            IHealth health = collision.gameObject.GetComponent<IHealth>();
            if (health != null)
            {
                health.TakeDamage(projectileData.GetDamage());
            }
            else
            {
                Debug.LogWarning("No IHealth component found on target.");
            }
            Destroy(gameObject);
        }

    }
}
