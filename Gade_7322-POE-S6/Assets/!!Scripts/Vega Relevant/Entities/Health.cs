/*
    The IHealth interface defines methods for managing health.
    - TakeDamage(float amount): Reduces health by the specified amount.
    - Heal(float amount): Increases health by the specified amount.
    - GetCurrentHealth(): Returns the current health value.
    - MaxHealth: Property that gets the maximum health value.
    - Heal(): Resets health to the maximum value if below it.

    The Health class implements the IHealth interface and manages the health of a game object.
    - maxHealth: The maximum health value, set to 5 by default.
    - currentHealth: The current health value, initialized to maxHealth on start.
    - TakeDamage(float amount): Reduces health, clamps it to 0, and triggers Die() if health drops to 0.
    - Heal(float amount): Increases health, clamps it to the maximum value.
    - GetCurrentHealth(): Returns the current health value.
    - Die(): Handles different behaviors based on the object's tag (e.g., triggers game over, drops gold, or destroys the object).
    - Heal(): Sets health to maximum if below it.
*/
using UnityEngine;

public interface IHealth
{
    void TakeDamage(float amount);
    void Heal(float amount);
    float GetCurrentHealth();
    float MaxHealth { get; }

    void Heal();
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

        //Debug.Log($"{gameObject.name} took {amount} damage. Current health: {currentHealth}");

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

    private void Die()
    {
        if (gameObject.CompareTag("TownHall"))
        {
            EventManager.instance.TriggerGameOverMode();
            Destroy(gameObject);
        }
        if (gameObject.CompareTag("KnightMeleeEnemy"))
        {
            GoldDropper goldDropper = GetComponent<GoldDropper>();
            if (goldDropper != null)
            {
                goldDropper.DropGold(3);
            }
            EnemyManager.instance.RemoveEnemy(this.gameObject);
        }
        if (gameObject.CompareTag("HKnightMeleeEnemy"))
        {
            GoldDropper goldDropper = GetComponent<GoldDropper>();
            if (goldDropper != null)
            {
                goldDropper.DropGold(3);
            }
            EnemyManager.instance.RemoveEnemy(this.gameObject);
        }
        if (gameObject.CompareTag("WizardRangedEnemy"))
        {
            GoldDropper goldDropper = GetComponent<GoldDropper>();
            if (goldDropper != null)
            {
                goldDropper.DropGold(3);
            }
            EnemyManager.instance.RemoveEnemy(this.gameObject);
        }
        if (gameObject.CompareTag("ShieldDefender"))
        {
            Destroy(gameObject);
        }
        if (gameObject.CompareTag("BurstDefender"))
        {
            Destroy(gameObject);
        }
        if (gameObject.CompareTag("CatapultDefender"))
        {
            Destroy(gameObject);
        }
        else if (Debug.isDebugBuild)
        {
            Debug.LogWarning("No behavior defined for death of " + gameObject.name);
        }
    }

    public void Heal()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
