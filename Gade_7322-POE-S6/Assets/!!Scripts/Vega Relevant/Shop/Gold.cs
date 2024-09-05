/*
    The Gold class manages the behavior of collectible gold items in the game. It handles the gold's movement towards the town hall, applies explosion effects upon collection, and updates the player's gold count.

    - Fields:
      - amount: The amount of gold the object represents.
      - flySpeed: The speed at which the gold flies towards the town hall.
      - explosionForce: The force applied to the gold when it explodes.
      - explosionRadius: The radius of the explosion effect.
      - townHallTransform: The transform of the town hall to move towards.
      - rb: The Rigidbody component of the gold.
      - isCollected: A flag indicating if the gold has been collected.

    - Methods:
      - Initialize(int goldAmount): Sets the gold amount, initializes the Rigidbody, and finds the town hall's transform.
      - TriggerCollection(): Starts the collection process if not already collected, applying an explosion force and initiating the collection sequence.
      - ApplyExplosionForce(): Applies an explosion force to the gold using Rigidbody.
      - StartCollecting(): Sets the Rigidbody to kinematic mode to stop physics interactions.
      - Update(): Moves the gold towards the town hall if it is collected and kinematic.
      - FlyTowardsTownHall(): Moves the gold towards the town hall and calls CollectGold() when close.
      - CollectGold(): Adds the gold amount to the player's total and destroys the gold object.
*/


using UnityEngine;

public class Gold : MonoBehaviour
{
    public int amount;
    public float flySpeed = 25f;
    public float explosionForce = 100f;
    public float explosionRadius = 20f;
    private Transform townHallTransform;
    private Rigidbody rb;
    private bool isCollected = false;

    public void Initialize(int goldAmount)
    {
        amount = goldAmount;
        rb = GetComponent<Rigidbody>();
        townHallTransform = GameObject.FindWithTag("TownHall").transform;
    }

    public void TriggerCollection()
    {
        if (!isCollected)
        {
            ApplyExplosionForce();
            Invoke("StartCollecting", 0.5f);
            isCollected = true;
        }
    }

    private void ApplyExplosionForce()
    {
        if (rb != null)
        {
            Vector3 explosionDirection = Random.insideUnitSphere;
            explosionDirection.y = Mathf.Abs(explosionDirection.y);

            rb.AddExplosionForce(explosionForce, transform.position - explosionDirection, explosionRadius, 1f, ForceMode.Impulse);
        }
    }

    private void StartCollecting()
    {
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        if (isCollected && rb.isKinematic)
        {
            FlyTowardsTownHall();
        }
    }

    private void FlyTowardsTownHall()
    {
        if (townHallTransform == null) return;

        transform.position = Vector3.MoveTowards(transform.position, townHallTransform.position, flySpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, townHallTransform.position) < 0.1f)
        {
            CollectGold();
        }
    }

    private void CollectGold()
    {
        GoldManager.instance.AddGold(amount);
        //Debug.Log($"Collected {amount} gold. Total gold: {GoldManager.instance.currentGold}");
        Destroy(gameObject);
    }
}
