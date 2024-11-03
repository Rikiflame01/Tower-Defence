using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlowZoneTower : MonoBehaviour, IHealth
{
    [SerializeField] private List<BoxCollider> slowZones;
    [SerializeField] private int level = 1;
    [SerializeField] private float[] slowEffectsPerLevel = { 0f, 0.2f, 0.4f, 0.6f };
    [SerializeField] private int[] upgradeCosts = { 1000, 2000, 4000 };
    [SerializeField] private float[] healthPerLevel = { 100f, 200f, 350f, 500f };
    [SerializeField] private LayerMask ignoredLayers;

    private List<NavMeshAgent> targetsInZone = new List<NavMeshAgent>();
    private Dictionary<NavMeshAgent, float> originalSpeeds = new Dictionary<NavMeshAgent, float>();
    private Renderer objectRenderer;
    private Color originalEmissionColor;
    private Material materialInstance;
    private EnemyManager enemyManager;
    private Health healthComponent;

    private void Start()
    {
        objectRenderer = GetComponentInChildren<Renderer>();
        healthComponent = GetComponent<Health>();
        enemyManager = EnemyManager.instance;

        if (healthComponent != null)
        {
            healthComponent.SetHealth(healthPerLevel[level - 1]);
        }

        if (objectRenderer != null)
        {
            materialInstance = objectRenderer.material;

            if (materialInstance.HasProperty("_EmissionColor"))
            {
                originalEmissionColor = materialInstance.GetColor("_EmissionColor");
            }
            else
            {
                originalEmissionColor = Color.black;
            }

            materialInstance.EnableKeyword("_EMISSION");
        }
    }

    private void Update()
    {
        targetsInZone.RemoveAll(target => target == null || !target.gameObject.activeInHierarchy);

        ApplySlowEffect();

        if (level < upgradeCosts.Length && GoldManager.instance.HasEnoughGold(upgradeCosts[level - 1]))
        {
            StartPulsatingEffect();
        }
        else
        {
            StopPulsatingEffect();
        }
    }

    private void ApplySlowEffect()
    {
        if (enemyManager == null) return;

        List<GameObject> activeEnemies = enemyManager.GetAllEnemies();
        float slowEffect = slowEffectsPerLevel[level - 1];

        foreach (var target in targetsInZone)
        {
            if (target != null && activeEnemies.Contains(target.gameObject))
            {
                if (!originalSpeeds.ContainsKey(target))
                {
                    originalSpeeds[target] = target.speed;
                }

                target.speed = originalSpeeds[target] * (1 - slowEffect);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
        if (agent != null && !targetsInZone.Contains(agent))
        {
            targetsInZone.Add(agent);
            Debug.Log("Target entered slow zone: " + agent.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
        if (agent != null && targetsInZone.Contains(agent))
        {
            if (originalSpeeds.ContainsKey(agent))
            {
                agent.speed = originalSpeeds[agent];
                originalSpeeds.Remove(agent);
            }

            targetsInZone.Remove(agent);
            Debug.Log("Target exited slow zone: " + agent.name);
        }
    }

    private void OnMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if ((ignoredLayers.value & (1 << hit.collider.gameObject.layer)) != 0)
            {
                Debug.Log("Click ignored on layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));
                return;
            }
        }

        if (level < healthPerLevel.Length)
        {
            int upgradeCost = upgradeCosts[level - 1];
            Debug.Log("Attempting upgrade. Cost: " + upgradeCost + ", Current Gold: " + GoldManager.instance.currentGold);
            ConfirmationManager.instance.ShowConfirmation(upgradeCost, UpgradeTower);
        }
        else
        {
            Debug.Log("Already at maximum level. Cannot upgrade further.");
        }
    }

    private void UpgradeTower()
    {
        if (GoldManager.instance.HasEnoughGold(upgradeCosts[level - 1]))
        {
            GoldManager.instance.SpendGold(upgradeCosts[level - 1]);
            level++;
            VFXManager.Instance.SpawnVFX("CoinBurst", transform.position, 1.0f);

            if (healthComponent != null)
            {
                healthComponent.SetHealth(healthPerLevel[level - 1]);
            }

            Debug.Log("Tower upgraded to level " + level);
        }
        else
        {
            Debug.Log("Not enough gold to upgrade tower.");
        }
    }

    private void StartPulsatingEffect()
    {
        if (materialInstance != null)
        {
            float pulse = Mathf.Abs(Mathf.Sin(Time.time * 5)) * 0.5f + 0.5f;
            Color pulseColor = Color.Lerp(originalEmissionColor, Color.white, pulse);
            materialInstance.SetColor("_EmissionColor", pulseColor);
        }
    }

    private void StopPulsatingEffect()
    {
        if (materialInstance != null)
        {
            materialInstance.SetColor("_EmissionColor", originalEmissionColor);
        }
    }

    public void TakeDamage(float amount)
    {
        if (healthComponent != null)
        {
            healthComponent.TakeDamage(amount);
        }
    }

    public void Heal(float amount)
    {
        if (healthComponent != null)
        {
            healthComponent.Heal(amount);
        }
    }

    public float GetCurrentHealth()
    {
        return healthComponent != null ? healthComponent.GetCurrentHealth() : 0f;
    }

    public float MaxHealth => healthComponent != null ? healthComponent.MaxHealth : 0f;

    public void Heal()
    {
        if (healthComponent != null)
        {
            healthComponent.Heal();
        }
    }

    public void SetHealth(float value)
    {
        if (healthComponent != null)
        {
            healthComponent.SetHealth(value);
        }
    }
}
