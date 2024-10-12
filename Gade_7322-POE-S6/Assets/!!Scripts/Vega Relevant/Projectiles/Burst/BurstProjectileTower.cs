using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class BurstProjectileTower : MonoBehaviour
{
    [SerializeField] private List<Transform> projectileSpawnPoints;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float attackRange = 15f;
    [SerializeField] private float attackCooldown = 5f;
    [SerializeField] private int level = 1;
    [SerializeField] private int[] projectilesPerLevel = { 3, 6, 9, 15 };
    [SerializeField] private float[] damagePerLevel = { 5f, 6f, 8f, 15f };
    [SerializeField] private int[] upgradeCosts = { 1000, 2000, 4000 };

    private float lastAttackTime;
    private Renderer objectRenderer;
    private Color originalEmissionColor;
    private Material materialInstance;

    private void Start()
    {
        objectRenderer = GetComponentInChildren<Renderer>();

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
        AcquireAndFireAtTarget();

        if (level < upgradeCosts.Length && GoldManager.instance.HasEnoughGold(upgradeCosts[level - 1]))
        {
            StartPulsatingEffect();
        }
        else
        {
            StopPulsatingEffect();
        }
    }

    private void AcquireAndFireAtTarget()
    {
        List<GameObject> targets = EnemyManager.instance.GetAllEnemies();
        int projectilesToFire = projectilesPerLevel[level - 1];

        if (targets.Count == 0 || Time.time < lastAttackTime + attackCooldown)
        {
            return;
        }

        int spawnedProjectiles = 0;
        foreach (GameObject target in targets)
        {
            if (target != null && spawnedProjectiles < projectilesToFire)
            {
                foreach (Transform spawnPoint in projectileSpawnPoints)
                {
                    FireAtTarget(target.transform, spawnPoint);
                    spawnedProjectiles++;
                    if (spawnedProjectiles >= projectilesToFire)
                    {
                        break;
                    }
                }
            }
        }

        if (spawnedProjectiles > 0)
        {
            lastAttackTime = Time.time;
        }
    }

    private void FireAtTarget(Transform target, Transform spawnPoint)
    {
        float damage = damagePerLevel[level - 1];

        Vector3 direction = (target.position - spawnPoint.position).normalized;

        GameObject projectileInstance = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        BurstProjectile projectile = projectileInstance.GetComponent<BurstProjectile>();
        if (projectile != null)
        {
            projectile.SetTarget(target, damage, direction);
        }
    }


    private void OnMouseDown()
    {
        if (level < projectilesPerLevel.Length)
        {
            int upgradeCost = upgradeCosts[level - 1];
            ConfirmationManager.instance.ShowConfirmation(upgradeCost, UpgradeTower);
        }
    }

    private void UpgradeTower()
    {
        if (GoldManager.instance.HasEnoughGold(upgradeCosts[level - 1]))
        {
            GoldManager.instance.SpendGold(upgradeCosts[level - 1]);
            level++;
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
}
