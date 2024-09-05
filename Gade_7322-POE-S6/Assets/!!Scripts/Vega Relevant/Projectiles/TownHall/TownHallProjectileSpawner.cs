/*
    The TownHallProjectileSpawner class manages projectile spawning from multiple shoot points, handling targeting, shooting, and burst firing.

    - Fields:
      - townHallLevelData: Configuration data for the town hall level.
      - shootPoints: Points from which projectiles are fired.
      - projectilePrefab: Prefab for the projectile.
      - projectileForce: Force applied to the projectile.
      - obstacleLayerMask: Layer mask to detect obstacles.

    - Private Fields:
      - lastShotTime: Time of the last shot.
      - isBurstActive: Flag for burst firing state.
      - burstCooldownEndTime: End time for burst cooldown.

    - Methods:
      - Start(): Initializes and configures the level; logs error if prefab is missing.
      - Update(): Checks for enemies and shoots if the time interval has passed.
      - Shoot(Transform target): Fires projectiles at the target if line of sight is clear.
      - HasLineOfSight(Transform shootPoint, Transform target): Checks if there is a clear line of sight to the target.
      - FireProjectile(Transform shootPoint, Transform target): Instantiates and fires the projectile.
      - ActivateBurst(Transform target): Coroutine for burst firing.
      - UpgradeTownHall(int level): Updates the town hall level and its configuration.
*/


using System.Collections;
using UnityEngine;

public class TownHallProjectileSpawner : MonoBehaviour
{
    public TownHallLevelSO townHallLevelData;
    public Transform[] shootPoints;
    public GameObject projectilePrefab;
    public float projectileForce = 500f;
    public LayerMask obstacleLayerMask;

    private float lastShotTime;
    private bool isBurstActive = false;
    private float burstCooldownEndTime = 0f;

    private void Start()
    {
        lastShotTime = Time.time;
        townHallLevelData.ConfigureLevel(0);

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
        }
    }

    private void Shoot(Transform target)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab is not assigned. Cannot shoot.");
            return;
        }

        bool hasFired = false;

        foreach (Transform shootPoint in shootPoints)
        {
            if (HasLineOfSight(shootPoint, target))
            {
                FireProjectile(shootPoint, target);
                hasFired = true;
            }
        }

        if (!hasFired)
        {
            Debug.Log("No shoot points with line of sight to the target.");
        }
    }

    private bool HasLineOfSight(Transform shootPoint, Transform target)
    {
        Vector3 direction = (target.position - shootPoint.position).normalized;
        RaycastHit hit;

        if (Physics.Raycast(shootPoint.position, direction, out hit, Mathf.Infinity, obstacleLayerMask))
        {
            if (hit.transform == target)
            {
                return true;
            }
        }

        return false;
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
