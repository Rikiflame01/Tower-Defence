using UnityEngine;
using UnityEngine.UI;

public class HealthCanvas : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, 0);

    private IHealth healthComponent;
    private Transform targetTransform;
    private Camera mainCamera;

    private void Awake()
    {
        healthComponent = GetComponentInParent<IHealth>();
        if (Debug.isDebugBuild)
        {
            if (healthComponent == null)
            {
                Debug.LogError("HealthCanvas: No IHealth component found on the parent GameObject.");
                return;
            }
        }

        targetTransform = GetComponentInParent<Transform>();

        if (Debug.isDebugBuild)
        {
            if (healthSlider == null)
            {
                healthSlider = GetComponentInChildren<Slider>();
                if (healthSlider == null)
                {
                    Debug.LogError("HealthCanvas: No Slider component found in children.");
                }
            }
        }
    }

    private void Start()
    {
        UpdateHealthUI();
        FindMainCamera();
    }

    private void Update()
    {
        if (healthComponent != null && healthSlider != null)
        {
            UpdateHealthUI();
        }

        if (mainCamera == null)
        {
            FindMainCamera();
        }

        if (mainCamera != null)
        {
            transform.LookAt(mainCamera.transform);
            transform.Rotate(0, 180, 0);
        }

        transform.position = targetTransform.position + offset;
    }

    public void UpdateHealthUI()
    {
        float currentHealth = healthComponent.GetCurrentHealth();
        healthSlider.value = currentHealth;
        healthSlider.maxValue = ((Health)healthComponent).MaxHealth;
    }

    private void FindMainCamera()
    {
        mainCamera = Camera.main;
        if (mainCamera == null && Debug.isDebugBuild)
        {
            Debug.LogError("HealthCanvas: No camera tagged as 'MainCamera' found in the scene.");
        }
    }
}
