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

    private void Update()
    {
        AcquireAndFireAtTarget();
    }

    private void AcquireAndFireAtTarget()
    {
        List<GameObject> targets = EnemyManager.instance.GetAllEnemies();
        int projectilesToFire = projectilesPerLevel[level - 1];
        int spawnedProjectiles = 0;

        foreach (GameObject target in targets)
        {
            if (spawnedProjectiles >= projectilesToFire)
            {
                break;
            }

            if (target != null && Time.time >= lastAttackTime + attackCooldown)
            {
                FireAtTarget(target.transform);
                spawnedProjectiles++;
            }
        }

        if (spawnedProjectiles > 0)
        {
            lastAttackTime = Time.time;
        }
    }

    private void FireAtTarget(Transform target)
    {
        float damage = damagePerLevel[level - 1];

        foreach (Transform spawnPoint in projectileSpawnPoints)
        {
            Vector3 direction = (target.position - spawnPoint.position).normalized;
            if (Physics.Raycast(spawnPoint.position, direction, out RaycastHit hit, attackRange))
            {
                if (hit.transform == target)
                {
                    GameObject projectileInstance = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
                    BurstProjectile projectile = projectileInstance.GetComponent<BurstProjectile>();
                    if (projectile != null)
                    {
                        projectile.SetTarget(target, damage);
                    }
                    break;
                }
            }
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
}