using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

public class THProjectile : MonoBehaviour
{
    public TownHallProjectileSO projectileData;
    public float speed = 10f;
    private GameObject target;

    private void Start()
    {
        AcquireTarget();

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        if (target != null && projectileData.hasHoming)
        {
            Homing(target.transform);
        }
        else if (target == null && projectileData.hasHoming)
        {
            AcquireTarget();
        }
    }

    private void AcquireTarget()
    {
        target = EnemyManager.instance.GetNearestEnemy(transform.position);

        if (target == null)
        {
            Destroy(gameObject);
        }
    }

    private void Homing(Transform targetTransform)
    {
        Vector3 direction = (targetTransform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        transform.LookAt(targetTransform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == target)
        {
            IHealth health = collision.gameObject.GetComponent<IHealth>();
            if (health != null)
            {
                health.TakeDamage(projectileData.GetDamage());
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("No IHealth component found on target.");
            }
        }

        if (projectileData.hasHoming == false) { 
            StartCoroutine(StartDespawn());
        }
    }

    private IEnumerator StartDespawn()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
