using UnityEngine;
using UnityEngine.UI;

public class HealthCanvas : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, 0);

    private IHealth healthComponent;
    private Transform targetTransform;

    private void Awake()
    {
        healthComponent = GetComponentInParent<IHealth>();
        if (healthComponent == null)
        {
            Debug.LogError("HealthCanvas: No IHealth component found on the parent GameObject.");
            return;
        }

        targetTransform = GetComponentInParent<Transform>();

        if (healthSlider == null)
        {
            healthSlider = GetComponentInChildren<Slider>();
            if (healthSlider == null)
            {
                Debug.LogError("HealthCanvas: No Slider component found in children.");
            }
        }
    }

    private void Start()
    {
        UpdateHealthUI();
    }

    private void Update()
    {
        if (healthComponent != null && healthSlider != null)
        {
            UpdateHealthUI();
        }

        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
        transform.position = targetTransform.position + offset;
    }

    public void UpdateHealthUI()
    {
        float currentHealth = healthComponent.GetCurrentHealth();
        healthSlider.value = currentHealth;
        healthSlider.maxValue = ((Health)healthComponent).MaxHealth;
    }
}
