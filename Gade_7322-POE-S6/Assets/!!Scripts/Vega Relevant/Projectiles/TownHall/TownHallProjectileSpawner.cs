using System.Collections;
using UnityEngine;

public class TownHallProjectileSpawner : MonoBehaviour
{
    public TownHallLevelSO townHallLevelData;
    public Transform[] shootPoints;
    public GameObject projectilePrefab;
    public float projectileForce = 500f;

    private float lastShotTime;
    private bool isBurstActive = false;
    private float burstCooldownEndTime = 0f;

    private void Start()
    {
        lastShotTime = Time.time;
        townHallLevelData.ConfigureLevel(3);

        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab is not assigned in the TownHall script.");
        }
    }

    private void Update()
    {
        GameObject nearestEnemy = EnemyManager.instance.GetNearestEnemy(transform.position);
        if (nearestEnemy != null)
        {
            if (Time.time - lastShotTime >= townHallLevelData.interval)
            {
                Shoot(nearestEnemy.transform);
                lastShotTime = Time.time;
            }

            if (townHallLevelData.hasBurstAbility && Time.time >= burstCooldownEndTime && !isBurstActive)
            {
                if (Input.GetKeyDown(KeyCode.B))
                {
                    StartCoroutine(ActivateBurst(nearestEnemy.transform));
                }
            }
        }
    }

    private void Shoot(Transform target)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab is not assigned. Cannot shoot.");
            return;
        }

        if (townHallLevelData.canShootFromAllPoints)
        {
            foreach (Transform shootPoint in shootPoints)
            {
                FireProjectile(shootPoint, target);
            }
        }
        else
        {
            int randomIndex = Random.Range(0, shootPoints.Length);
            FireProjectile(shootPoints[randomIndex], target);
        }
    }

    private void FireProjectile(Transform shootPoint, Transform target)
    {
        GameObject projectileInstance = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody rb = projectileInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (target.position - shootPoint.position).normalized;
            rb.AddForce(direction * projectileForce);
        }
    }

    private IEnumerator ActivateBurst(Transform target)
    {
        isBurstActive = true;
        yield return new WaitForSeconds(townHallLevelData.burstActivationTime);

        for (int i = 0; i < townHallLevelData.burstCount; i++)
        {
            Shoot(target);
            yield return new WaitForSeconds(townHallLevelData.interval);
        }

        isBurstActive = false;
        burstCooldownEndTime = Time.time + townHallLevelData.burstCooldown;
    }

    public void UpgradeTownHall(int level)
    {
        townHallLevelData.ConfigureLevel(level);
    }
}
