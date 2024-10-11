using System.Collections.Generic;
using UnityEngine;

public class WizardProjectile : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private List<string> ignoreCollisionTags = new List<string>();
    private Collider projectileCollider;

    private void Start()
    {
        projectileCollider = GetComponent<Collider>();
        if (Debug.isDebugBuild && projectileCollider == null)
        {
            Debug.LogError("No Collider attached to the projectile.");
        }
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject target = collision.gameObject;

        if (ignoreCollisionTags.Contains(target.tag))
        {
            Physics.IgnoreCollision(projectileCollider, collision.collider);
            return;
        }

        IHealth health = target.GetComponent<IHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
            Destroy(gameObject);
            if (Debug.isDebugBuild)
            {
                Debug.Log($"{target.name} took {damage} damage.");
            }
        }
    }
}
