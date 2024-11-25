/*
    The IHealth interface defines methods for managing health.
    - TakeDamage(float amount): Reduces health by the specified amount.
    - Heal(float amount): Increases health by the specified amount.
    - GetCurrentHealth(): Returns the current health value.
    - MaxHealth: Property that gets the maximum health value.
    - Heal(): Resets health to the maximum value if below it.
    - SetHealth(float value): Sets the current health to a specified value, clamped between 0 and maxHealth.

    The Health class implements the IHealth interface and manages the health of a game object.
    - maxHealth: The maximum health value, set to 5 by default.
    - currentHealth: The current health value, initialized to maxHealth on start.
    - TakeDamage(float amount): Reduces health, clamps it to 0, and triggers Die() if health drops to 0.
    - Heal(float amount): Increases health, clamps it to the maximum value.
    - GetCurrentHealth(): Returns the current health value.
    - SetHealth(float value): Sets the current health directly, clamped between 0 and maxHealth.
    - Die(): Handles different behaviors based on the object's tag (e.g., triggers game over, drops gold, or destroys the object).
    - Heal(): Sets health to maximum if below it.
*/

using UnityEngine;
using UnityEngine.AI;

public interface IHealth
{
    void TakeDamage(float amount);
    void Heal(float amount);
    float GetCurrentHealth();
    float MaxHealth { get; }

    void Heal();
    void SetHealth(float value);
}

public class Health : MonoBehaviour, IHealth
{
    [SerializeField] private float maxHealth = 5f;
    private float currentHealth;

    public float MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public void SetHealth(float value)
    {
        maxHealth = value;
        currentHealth = Mathf.Clamp(value, 0, maxHealth);
    }

private void Die()
{
    if (gameObject.CompareTag("TownHall"))
    {
        EventManager.instance.TriggerGameOverMode();

        StartCoroutine(HandleBuildingDestruction());
    }
    else if (gameObject.CompareTag("KnightMeleeEnemy") || gameObject.CompareTag("HKnightMeleeEnemy") || gameObject.CompareTag("WizardRangedEnemy"))
    {
        StartCoroutine(HandleRagdollAndDestroy());

        GoldDropper goldDropper = GetComponent<GoldDropper>();
        if (goldDropper != null)
        {
            goldDropper.DropGold(3);
        }
    }
    else if (gameObject.CompareTag("ShieldDefender") || gameObject.CompareTag("BurstDefender") || gameObject.CompareTag("CatapultDefender"))
    {
        SoundManager.Instance.PlaySFX("BuildingDestroyed", 0.5f);

        StartCoroutine(HandleBuildingDestruction());
    }
    else if (Debug.isDebugBuild)
    {
        Debug.LogWarning("No behavior defined for death of " + gameObject.name);
    }
}


private System.Collections.IEnumerator HandleRagdollAndDestroy()
{
    NavMeshAgent agent = GetComponent<NavMeshAgent>();
    if (agent != null) agent.enabled = false;

    Animator animator = GetComponent<Animator>();
    if (animator != null) animator.enabled = false;

    Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
    foreach (var rb in rigidbodies)
    {
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    Collider[] colliders = GetComponentsInChildren<Collider>();
    foreach (var col in colliders)
    {
        col.enabled = true;
    }

foreach (var rb in rigidbodies)
{
    Vector3 randomDirection = new Vector3(
        Random.Range(-1f, 1f), 
        Random.Range(0.5f, 1f), 
        Random.Range(-1f, 1f)
    ).normalized;
    rb.AddForce(randomDirection * 500);
}
EnemyManager.instance.RemoveEnemy(this.gameObject);

    yield return new WaitForSeconds(2f);
    Destroy(gameObject);
}

private System.Collections.IEnumerator HandleBuildingDestruction()
{
    NavMeshAgent agent = GetComponent<NavMeshAgent>();
    if (agent != null) agent.enabled = false;

    Animator animator = GetComponent<Animator>();
    if (animator != null) animator.enabled = false;

    foreach (Transform child in transform)
    {

        Rigidbody rb = child.gameObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = child.gameObject.AddComponent<Rigidbody>();
            rb.mass = 40f;
        }

        Collider col = child.gameObject.GetComponent<Collider>();
        if (col == null)
        {
            col = child.gameObject.AddComponent<BoxCollider>();
        }

        Vector3 randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(0.5f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;

        rb.AddForce(randomDirection * 50, ForceMode.Impulse);
    }
    SoundManager.Instance.PlaySFX("BuildingDestroyed", 0.5f);
    yield return new WaitForSeconds(1f);
    Destroy(gameObject);
}

    public void Heal()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
