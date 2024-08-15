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
