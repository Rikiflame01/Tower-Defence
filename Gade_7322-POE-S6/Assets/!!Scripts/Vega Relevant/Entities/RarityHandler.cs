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

    private int currentRound = 1;

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

        ApplyRarity();
        UpdateRarityVisuals();
    }

    public void SetAllowedRarities(int round)
    {
        currentRound = round;
        AssignRandomRarity();
        ApplyRarity();
        UpdateRarityVisuals();
    }

    private void AssignRandomRarity()
    {
        float randomValue = Random.Range(0f, 100f);

        if (currentRound <= 5)
        {
            rarity = RarityType.Normal;  //Only Normal for rounds 1-5
        }
        else if (currentRound <= 10)
        {
            //Empowered and Mythic introduced from rounds 6-10
            if (randomValue <= 20f)
            {
                rarity = RarityType.Empowered;  //20% chance for Empowered
            }
            else if (randomValue <= 30f)
            {
                rarity = RarityType.Mythic;     //10% chance for Mythic
            }
            else
            {
                rarity = RarityType.Normal;     //Remaining chance for Normal
            }
        }
        else
        {
            //Godlike and Legendary introduced from round 11 onward
            if (randomValue <= 0.5f)
            {
                rarity = RarityType.Godlike;    // 0.5% chance for Godlike
            }
            else if (randomValue <= 3.5f)
            {
                rarity = RarityType.Legendary;  // 3% chance for Legendary
            }
            else if (randomValue <= 13.5f)
            {
                rarity = RarityType.Mythic;     // 10% chance for Mythic
            }
            else if (randomValue <= 33.5f)
            {
                rarity = RarityType.Empowered;  // 20% chance for Empowered
            }
            else
            {
                rarity = RarityType.Normal;     //Remaining chance for Normal
            }
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
                break;
            case RarityType.Mythic:
                health = baseHealth * 2f;
                damage = baseDamage * 2f;
                speed = baseSpeed * 1.3f;
                break;
            case RarityType.Legendary:
                health = baseHealth * 3f;
                damage = baseDamage * 3f;
                speed = baseSpeed * 1.4f;
                break;
            case RarityType.Godlike:
                health = baseHealth * 5f;
                damage = baseDamage * 5f;
                speed = baseSpeed * 1.5f;
                break;
            default:
                Debug.LogError("Unknown rarity type");
                break;
        }

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
                rarityColor = new Color(0f, 0f, 0f, 0f); // Fully transparent
                break;
            case RarityType.Empowered:
                rarityColor = new Color(0.5f, 0.5f, 1f, 0.5f); // Light blue with transparency
                break;
            case RarityType.Mythic:
                rarityColor = new Color(0.8f, 0.2f, 1f, 0.6f); // Purple with more opacity
                break;
            case RarityType.Legendary:
                rarityColor = new Color(1f, 0.84f, 0f, 0.8f); // Gold with higher opacity
                break;
            case RarityType.Godlike:
                rarityColor = new Color(1f, 0f, 0f, 1f); // Fully opaque red
                break;
            default:
                rarityColor = new Color(0f, 0f, 0f, 0f); // Default to fully transparent
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
