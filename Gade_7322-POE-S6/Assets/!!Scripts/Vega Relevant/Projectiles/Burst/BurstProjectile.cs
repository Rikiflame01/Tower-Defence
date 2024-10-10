using System.Collections.Generic;
using UnityEngine;

public class BurstProjectile : MonoBehaviour
{
    public float speed = 10f;
    private Transform target;
    private float damage;
    [SerializeField] private List<string> ignoreCollisionTags;

    public void SetTarget(Transform target, float damage)
    {
        this.target = target;
        this.damage = damage;
    }

    private void Start()
    {
        Collider projectileCollider = GetComponent<Collider>();
        if (projectileCollider != null)
        {
            foreach (string tag in ignoreCollisionTags)
            {
                GameObject[] objectsToIgnore = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject obj in objectsToIgnore)
                {
                    Collider objCollider = obj.GetComponent<Collider>();
                    if (objCollider != null)
                    {
                        Physics.IgnoreCollision(projectileCollider, objCollider);
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            transform.LookAt(target);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (ignoreCollisionTags.Contains(collision.gameObject.tag))
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
            return;
        }

        IHealth health = collision.gameObject.GetComponent<IHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }
}