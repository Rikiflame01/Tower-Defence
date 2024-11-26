using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CatapultTower : MonoBehaviour, IHealth
{
    [SerializeField] private BoxCollider detectionZone;
    [SerializeField] private float attackCooldown = 5f;
    [SerializeField] private float launchDelay = 1f;
    [SerializeField] private float launchInterval = 0.5f;
    [SerializeField] private int level = 1;
    [SerializeField] private int[] targetsPerLevel = { 4, 6, 8, 10 };
    [SerializeField] private float damagePerLaunch = 1000f;
    [SerializeField] private int[] upgradeCosts = { 2000, 3000, 4000 };
    [SerializeField] private float[] healthPerLevel = { 150f, 300f, 500f, 750f };
    [SerializeField] private LayerMask ignoredLayers;

    private float lastAttackTime;
    private List<NavMeshAgent> targetsInZone = new List<NavMeshAgent>();
    private Renderer objectRenderer;
    private Color originalEmissionColor;
    private Material materialInstance;
    private Health healthComponent;

    public GameObject Level2Objects;
    public GameObject Level3Objects;
    public GameObject Level4Objects;

    public new ParticleSystem particleSystem;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Stop();

        objectRenderer = GetComponentInChildren<Renderer>();
        healthComponent = GetComponent<Health>();

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

        if (Time.time >= lastAttackTime + attackCooldown && targetsInZone.Count > 0)
        {
            Debug.Log("Launching targets. Targets in zone: " + targetsInZone.Count);
            StartCoroutine(LaunchTargets());
            lastAttackTime = Time.time;
        }

        if (level < upgradeCosts.Length && GoldManager.instance.HasEnoughGold(upgradeCosts[level - 1]))
        {
            StartPulsatingEffect();
        }
        else
        {
            StopPulsatingEffect();
        }
    }

    private System.Collections.IEnumerator LaunchTargets()
    {
        int targetsToLaunch = Mathf.Min(targetsPerLevel[level - 1], targetsInZone.Count);
        targetsToLaunch = Mathf.Max(1, targetsToLaunch);

        Debug.Log("Launching " + targetsToLaunch + " targets at level " + level);

        List<NavMeshAgent> targetsToLaunchList = new List<NavMeshAgent>(targetsInZone);

        for (int i = 0; i < targetsToLaunch && i < targetsToLaunchList.Count; i++)
        {
            NavMeshAgent target = targetsToLaunchList[i];
            if (target != null)
            {
                Debug.Log("Launching target: " + target.name);
                StartCoroutine(LaunchAfterDelay(target, launchDelay));
            }
            else
            {
                Debug.LogWarning("Target is null. Skipping launch.");
            }
        }

        yield return null;
    }

    private System.Collections.IEnumerator LaunchAfterDelay(NavMeshAgent target, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (target != null && target.gameObject.activeInHierarchy)
        {
            SoundManager.Instance.PlaySFX("Catapult", 0.5f);
            particleSystem.Play();

            Rigidbody targetRb = target.GetComponent<Rigidbody>();
            NavMeshAgent targetAgent = target.GetComponent<NavMeshAgent>();

            if (targetAgent != null)
            {
                targetAgent.enabled = false;
                Debug.Log("NavMeshAgent disabled for target: " + target.name);
            }

            if (targetRb != null)
            {
                targetRb.isKinematic = false;
                targetRb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
                Debug.Log("Applied force to target: " + target.name);
                StartCoroutine(ApplyDamageAfterDelay(target, 1f));
            }
            else
            {
                Debug.LogWarning("Rigidbody not found on target: " + target.name);
            }
        }
        else
        {
            Debug.LogWarning("Target is null or inactive when attempting to launch.");
        }
        yield return new WaitForSeconds(1f);
        particleSystem.Stop();
    }

    private System.Collections.IEnumerator ApplyDamageAfterDelay(NavMeshAgent target, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (target != null && target.gameObject.activeInHierarchy)
        {
            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damagePerLaunch);
                Debug.Log("Dealt " + damagePerLaunch + " damage to target: " + target.name);
            }
            else
            {
                Debug.LogWarning("Health component not found on target: " + target.name);
            }
        }
        else
        {
            Debug.LogWarning("Target is null or inactive when attempting to apply damage.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
        if (agent != null && !targetsInZone.Contains(agent))
        {
            targetsInZone.Add(agent);
            Debug.Log("Target entered detection zone: " + agent.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
        if (agent != null && targetsInZone.Contains(agent))
        {
            targetsInZone.Remove(agent);
            Debug.Log("Target exited detection zone: " + agent.name);
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

            if (level == 2)
            {
                Level2Objects.SetActive(true);
            }
            if (level == 3)
            {
                Level3Objects.SetActive(true);
            }
            if (level == 4)
            {
                Level4Objects.SetActive(true);
            }

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

    public float GetCurrentHealth()
    {
        return healthComponent != null ? healthComponent.GetCurrentHealth() : 0f;
    }

    public float MaxHealth => healthComponent != null ? healthComponent.MaxHealth : 0f;

    public void SetHealth(float value)
    {
        if (healthComponent != null)
        {
            healthComponent.SetHealth(value);
        }
    }

    public void Heal(float amount)
    {
        if (healthComponent != null)
        {
            healthComponent.Heal(amount);
        }
    }
    public void Heal()
    {
        if (healthComponent != null)
        {
            healthComponent.Heal();
        }
    }
}
