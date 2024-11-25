using System.Collections.Generic;
using UnityEngine;

public class WizardProjectile : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private List<string> ignoreCollisionTags = new List<string>();
    private Collider projectileCollider;

    public new ParticleSystem particleSystem;
    
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

        if (ignoreCollisionTags != null && collision.gameObject != null && !string.IsNullOrEmpty(collision.gameObject.tag))
        {
            if (ignoreCollisionTags.Contains(collision.gameObject.tag))
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
                return;
            }
        }

        IHealth health = target.GetComponent<IHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
            StartCoroutine(DestroyProjectile());
            if (Debug.isDebugBuild)
            {
                Debug.Log($"{target.name} took {damage} damage.");
            }
        }
        else {
            Destroy(gameObject);
        }
    }

private System.Collections.IEnumerator DestroyProjectile()
{
    particleSystem = GetComponent<ParticleSystem>();
    if (particleSystem != null)
    {
        var mainModule = particleSystem.main;
        mainModule.startSizeMultiplier *= 0.5f;
        mainModule.startSpeedMultiplier *= 0.5f;
        SoundManager.Instance.PlaySFX("wizardprojectile", 0.5f);
        particleSystem.Play();
    }
    yield return new WaitForSeconds(1f);
    Destroy(gameObject);
}

}
