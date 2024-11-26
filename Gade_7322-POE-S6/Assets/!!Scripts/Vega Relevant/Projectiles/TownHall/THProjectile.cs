/*
    The THProjectile class controls a projectile's behavior, including homing and damage application.

    - Fields:
      - projectileData: Data related to the projectile's properties.
      - speed: Movement speed of the projectile.
      - target: Current target for the projectile.

    - Methods:
      - Start(): Acquires the initial target. Destroys the projectile if no target is found.
      - Update(): Handles homing towards the target if applicable; reacquires target if lost.
      - AcquireTarget(): Finds the nearest enemy as the target; destroys the projectile if no target is found.
      - Homing(Transform targetTransform): Moves the projectile towards and looks at the target.
      - OnCollisionEnter(Collision collision): Applies damage to the target on collision and handles projectile destruction.
      - StartDespawn(): Coroutine to delay projectile destruction if not homing.
*/


using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

public class THProjectile : MonoBehaviour
{
    public TownHallProjectileSO projectileData;
    public float speed = 10f;
    private GameObject target;
    private float lastSoundTime;
    private float soundCooldown = 0.1f;

    private void Start()
    {
        AcquireTarget();

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        if (target != null && projectileData.hasHoming)
        {
            Homing(target.transform);
        }
        else if (target == null && projectileData.hasHoming)
        {
            AcquireTarget();
        }
    }

    private void AcquireTarget()
    {
        target = EnemyManager.instance.GetNearestEnemy(transform.position);

        if (target == null)
        {
            Destroy(gameObject);
        }
    }

    private void Homing(Transform targetTransform)
    {
        Vector3 direction = (targetTransform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        transform.LookAt(targetTransform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "ShieldDefender" && collision.gameObject.tag != 
            "TownHall" && collision.gameObject.tag != "BurstDefender" && collision.gameObject.tag != "CatapultDefender")
        {

            IHealth health = collision.gameObject.GetComponent<IHealth>();
            if (health != null)
            {
                health.TakeDamage(projectileData.GetDamage());
                StartCoroutine(StartDespawn());
            }
        }
        if (collision.gameObject.tag == "HKnightMeleeEnemy" || collision.gameObject.tag == "KnightMeleeEnemy" || collision.gameObject.tag == "WizardRangedEnemy")
        {
                        if (Time.time >= lastSoundTime + soundCooldown)
            {
                int randomChance = Random.Range(0, 20);
                if (randomChance == 0)
                {
                    SoundManager.Instance.PlaySFX("BowlingStrike", 0.5f);
                }
                else
                {
                    SoundManager.Instance.PlaySFX("Impact1", 0.5f);
                }
                lastSoundTime = Time.time;
            }
        }

        if (!projectileData.hasHoming)
        {
            StartCoroutine(StartDespawn());
        }
    }

    private IEnumerator StartDespawn()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
