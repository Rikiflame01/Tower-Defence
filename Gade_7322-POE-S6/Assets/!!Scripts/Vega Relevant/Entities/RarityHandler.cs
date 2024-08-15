using UnityEngine;

public class RarityHandler : MonoBehaviour
{
    public enum RarityType
    {
        Normal,
        Empowered,
        Mythic,
        Legendary,
        Godlike
    }

    [SerializeField] private RarityType rarity = RarityType.Normal;

    [SerializeField] private float baseHealth = 100f;
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float baseSpeed = 3.5f;

    private float health;
    private float damage;
    private float speed;

    private void Awake()
    {
        ApplyRarity();
    }

    private void ApplyRarity()
    {
        switch (rarity)
        {
            case RarityType.Normal:
                health = baseHealth;
                damage = baseDamage;
                speed = baseSpeed;
                break;
            case RarityType.Empowered:
                health = baseHealth * 1.5f;
                damage = baseDamage * 1.5f;
                speed = baseSpeed * 1.2f;
                Debug.Log("Empowered stats applied (Placeholder)");
                break;
            case RarityType.Mythic:
                health = baseHealth * 2f;
                damage = baseDamage * 2f;
                speed = baseSpeed * 1.3f;
                Debug.Log("Mythic stats applied (Placeholder)");
                break;
            case RarityType.Legendary:
                health = baseHealth * 3f;
                damage = baseDamage * 3f;
                speed = baseSpeed * 1.4f;
                Debug.Log("Legendary stats applied (Placeholder)");
                break;
            case RarityType.Godlike:
                health = baseHealth * 5f;
                damage = baseDamage * 5f;
                speed = baseSpeed * 1.5f;
                Debug.Log("Godlike stats applied (Placeholder)");
                break;
            default:
                Debug.LogError("Unknown rarity type");
                break;
        }

        IHealth healthComponent = GetComponent<IHealth>();
        if (healthComponent != null)
        {
            healthComponent.TakeDamage(-health);
        }
    }

    public float GetDamage()
    {
        return damage;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public RarityType GetRarity()
    {
        return rarity;
    }
}
