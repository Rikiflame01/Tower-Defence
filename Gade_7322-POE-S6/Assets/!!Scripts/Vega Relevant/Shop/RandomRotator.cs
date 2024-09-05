/*
    The RandomRotator class applies continuous random rotation to a GameObject.

    - Fields:
      - rotationSpeed: Speed of rotation in degrees per second.
      - changeDirectionInterval: Time interval (in seconds) for changing the rotation axis.
      - randomRotationAxis: The current axis of rotation, randomly generated.
      - timeSinceLastChange: Timer to track the interval for changing the rotation direction.

    - Methods:
      - Start(): Initializes the random rotation axis.
      - Update(): Updates the rotation of the GameObject and checks if it's time to change the rotation direction.
      - SetRandomRotation(): Sets a new random axis for rotation.
*/

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
