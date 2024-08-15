using UnityEngine;

public class RandomRotator : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public float changeDirectionInterval = 2f;

    private Vector3 randomRotationAxis;
    private float timeSinceLastChange;

    private void Start()
    {
        SetRandomRotation();
    }

    private void Update()
    {
        timeSinceLastChange += Time.deltaTime;

        if (timeSinceLastChange >= changeDirectionInterval)
        {
            SetRandomRotation();
            timeSinceLastChange = 0f;
        }

        transform.Rotate(randomRotationAxis * rotationSpeed * Time.deltaTime);
    }

    private void SetRandomRotation()
    {
        randomRotationAxis = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;
    }
}
