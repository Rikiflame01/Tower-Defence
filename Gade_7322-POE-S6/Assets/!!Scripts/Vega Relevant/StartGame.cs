/*
    The StartGame class handles user interaction and spinning behavior for a game object.
    - The object spins when clicked, with spin speed easing in and out over a specified duration.
    - The object’s position is locked to a specified Transform position.
    - The spinning effect is achieved by rotating the object with an eased speed.
    - When clicked, the object starts spinning and triggers cooldown behavior if applicable.
    
    Key methods:
    - StartSpinning: Starts the spinning effect and resets the elapsed time.
    - SkipCooldown: Adjusts game state or timer based on the wave counter and game state.
    - EaseInOutCubic: Applies cubic easing to control the spin speed.
*/

using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] private float maxSpinSpeed = 900f;
    [SerializeField] private Transform lockTransform;
    private bool isSpinning = false;
    private float spinDuration = 2f;
    private float spinTimeElapsed = 0f;
    private Quaternion initialRotation;

    private WaveManager waveManager;
    private TimerScript timerScript;

    private void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        timerScript = FindObjectOfType<TimerScript>();
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        if (lockTransform != null)
        {
            transform.position = lockTransform.position;
        }

        if (isSpinning)
        {
            spinTimeElapsed += Time.deltaTime;

            float normalizedTime = spinTimeElapsed / spinDuration;

            float easedSpeed = EaseInOutCubic(normalizedTime) * maxSpinSpeed;

            transform.Rotate(Vector3.up, easedSpeed * Time.deltaTime);

            if (spinTimeElapsed >= spinDuration)
            {
                isSpinning = false;
                spinTimeElapsed = 0f;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.transform == transform)
                {
                    Debug.Log("Object clicked: " + gameObject.name);
                    StartSpinning();
                    SkipCooldown();
                }
            }
        }
    }

    private void StartSpinning()
    {
        Debug.Log("StartSpinning called, object should start spinning.");
        isSpinning = true;
        spinTimeElapsed = 0f;
        initialRotation = transform.rotation;
    }

    private void SkipCooldown()
    {
        if (waveManager.roundCounter == 0 && GameManager.instance.currentState != GameManager.GameState.Cooldown)
        {
            EventManager.instance.TriggerCooldownMode();
        }
        else
        {
            timerScript.currentTime = 0;
            timerScript.UpdateTimerText();
        }
    }

    private float EaseInOutCubic(float t)
    {
        return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }
}
