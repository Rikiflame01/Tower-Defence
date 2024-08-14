using UnityEngine;

public interface IHealth
{
    void TakeDamage(float amount);
    void Heal(float amount);
    float GetCurrentHealth();
}

public class Health : MonoBehaviour, IHealth
{
    [SerializeField] private float maxHealth = 5f;
    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"{gameObject.name} took {amount} damage. Current health: {currentHealth}");

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
        Debug.Log($"{gameObject.name} has died.");
        if (gameObject.CompareTag("TownHall"))
        {
            EventManager.instance.TriggerGameOverMode();
            Destroy(gameObject);
        }
        if (gameObject.CompareTag("KnightMeleeEnemy"))
        {
            EnemyManager.instance.RemoveEnemy(this.gameObject);
        }
    }
}
