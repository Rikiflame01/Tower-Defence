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

    // References to the particle prefabs for each rarity (except Normal)
    [SerializeField] private GameObject empoweredParticlePrefab;
    [SerializeField] private GameObject mythicParticlePrefab;
    [SerializeField] private GameObject legendaryParticlePrefab;
    [SerializeField] private GameObject godlikeParticlePrefab;

    // Adjustable Y offset for particle positioning
    [SerializeField] private float yOffset = 0f;

    [SerializeField] private float baseHealth = 100f;
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float baseSpeed = 3.5f;

    private float health;
    private float damage;
    private float speed;

    private int currentRound = 1;

    // Reference to the currently spawned particle effect
    private GameObject currentParticleEffect;

    private void Awake()
    {
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
            rarity = RarityType.Normal; // Only Normal for rounds 1-5
        }
        else if (currentRound <= 10)
        {
            // Empowered and Mythic introduced from rounds 6-10
            if (randomValue <= 20f)
            {
                rarity = RarityType.Empowered; // 20% chance for Empowered
            }
            else if (randomValue <= 30f)
            {
                rarity = RarityType.Mythic; // 10% chance for Mythic
            }
            else
            {
                rarity = RarityType.Normal; // Remaining chance for Normal
            }
        }
        else
        {
            // Godlike and Legendary introduced from round 11 onward
            if (randomValue <= 0.5f)
            {
                rarity = RarityType.Godlike; // 0.5% chance for Godlike
            }
            else if (randomValue <= 3.5f)
            {
                rarity = RarityType.Legendary; // 3% chance for Legendary
            }
            else if (randomValue <= 13.5f)
            {
                rarity = RarityType.Mythic; // 10% chance for Mythic
            }
            else if (randomValue <= 33.5f)
            {
                rarity = RarityType.Empowered; // 20% chance for Empowered
            }
            else
            {
                rarity = RarityType.Normal; // Remaining chance for Normal
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
        // Destroy existing particle effect
        if (currentParticleEffect != null)
        {
            Destroy(currentParticleEffect);
        }

        GameObject particlePrefab = null;

        switch (rarity)
        {
            case RarityType.Empowered:
                particlePrefab = empoweredParticlePrefab;
                break;
            case RarityType.Mythic:
                particlePrefab = mythicParticlePrefab;
                break;
            case RarityType.Legendary:
                particlePrefab = legendaryParticlePrefab;
                break;
            case RarityType.Godlike:
                particlePrefab = godlikeParticlePrefab;
                break;
            default:
                // Do nothing for Normal rarity
                return;
        }

        if (particlePrefab != null)
        {
            // Instantiate the particle effect as a child of this object
            currentParticleEffect = Instantiate(particlePrefab, transform);
            // Adjust the local position with the Y offset
            currentParticleEffect.transform.localPosition = new Vector3(0f, yOffset, 0f);
        }
        else
        {
            Debug.LogWarning("Particle prefab not assigned for rarity: " + rarity.ToString());
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
