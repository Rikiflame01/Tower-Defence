using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private GameObject rarityCanvas;
    private Image rarityImage;

    [SerializeField] private float baseHealth = 100f;
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float baseSpeed = 3.5f;

    private float health;
    private float damage;
    private float speed;

    private void Awake()
    {
        if (rarityCanvas != null)
        {
            rarityImage = rarityCanvas.GetComponentInChildren<Image>();
            if (rarityImage == null && Debug.isDebugBuild)
            {
                Debug.LogError("No Image component found on the assigned Canvas.");
            }
        }
        else if (Debug.isDebugBuild)
        {
            Debug.LogWarning("Rarity canvas not assigned.");
        }

        AssignRandomRarity();
        ApplyRarity();
        UpdateRarityVisuals();
    }

    private void AssignRandomRarity()
    {
        float randomValue = Random.Range(0f, 100f);

        if (randomValue <= 0.5f)
        {
            rarity = RarityType.Godlike; // 0.5% chance
        }
        else if (randomValue <= 3.5f)
        {
            rarity = RarityType.Legendary; // 3% chance (0.5% + 3%)
        }
        else if (randomValue <= 13.5f)
        {
            rarity = RarityType.Mythic; // 10% chance (3.5% + 10%)
        }
        else if (randomValue <= 33.5f)
        {
            rarity = RarityType.Empowered; // 20% chance (13.5% + 20%)
        }
        else
        {
            rarity = RarityType.Normal; // Remaining percentage for Normal (balance)
        }
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
                Debug.Log("Empowered stats applied.");
                break;
            case RarityType.Mythic:
                health = baseHealth * 2f;
                damage = baseDamage * 2f;
                speed = baseSpeed * 1.3f;
                Debug.Log("Mythic stats applied.");
                break;
            case RarityType.Legendary:
                health = baseHealth * 3f;
                damage = baseDamage * 3f;
                speed = baseSpeed * 1.4f;
                Debug.Log("Legendary stats applied.");
                break;
            case RarityType.Godlike:
                health = baseHealth * 5f;
                damage = baseDamage * 5f;
                speed = baseSpeed * 1.5f;
                Debug.Log("Godlike stats applied.");
                break;
            default:
                Debug.LogError("Unknown rarity type");
                break;
        }

        //Apply the calculated health to the IHealth component
        IHealth healthComponent = GetComponent<IHealth>();
        if (healthComponent != null)
        {
            healthComponent.SetHealth(health);
        }
    }

    private void UpdateRarityVisuals()
    {
        if (rarityImage == null) return;

        Color rarityColor;

        switch (rarity)
        {
            case RarityType.Normal:
                rarityColor = new Color(0f, 0f, 0f, 0f); //Fully transparent
                break;
            case RarityType.Empowered:
                rarityColor = new Color(0.5f, 0.5f, 1f, 0.5f); //Light blue with transparency
                break;
            case RarityType.Mythic:
                rarityColor = new Color(0.8f, 0.2f, 1f, 0.6f); //Purple with more opacity
                break;
            case RarityType.Legendary:
                rarityColor = new Color(1f, 0.84f, 0f, 0.8f); //Gold with higher opacity
                break;
            case RarityType.Godlike:
                rarityColor = new Color(1f, 0f, 0f, 1f); //Fully opaque red
                break;
            default:
                rarityColor = new Color(0f, 0f, 0f, 0f); //Default to fully transparent
                break;
        }

        rarityImage.color = rarityColor;
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
