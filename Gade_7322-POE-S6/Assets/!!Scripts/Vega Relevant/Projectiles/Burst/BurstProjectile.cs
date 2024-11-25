using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BurstProjectile : MonoBehaviour
{
    public float speed = 10f;
    private Transform target;
    private float damage;
    private Vector3 lastDirection;
    [SerializeField] private List<string> ignoreCollisionTags;

    public void SetTarget(Transform target, float damage, Vector3 direction)
    {
        this.target = target;
        this.damage = damage;
        this.lastDirection = direction;
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

        StartCoroutine(DestroyAfterTime(5f));
    }

     private System.Collections.IEnumerator DestroyAfterTime(float time)
    {
          yield return new WaitForSeconds(time);
          Destroy(gameObject);
     }

    private void Update()
    {
        if (target != null && target.gameObject.GetComponent<IHealth>().GetCurrentHealth() > 0)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            lastDirection = direction;
            transform.position += direction * speed * Time.deltaTime;
            transform.LookAt(target);
        }
        else if (lastDirection != Vector3.zero)
        {
            transform.position += lastDirection * speed * Time.deltaTime;
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
            if (health.GetCurrentHealth() <= 0)
            {
                collision.gameObject.tag = "Dead";
            }
            StartCoroutine(DestroyAfterTime(5f));
        }
    }
}