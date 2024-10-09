/*
    The HealthCanvas class manages the UI representation of an object's health using a Slider component.
    
    Fields:
    - healthSlider: The UI Slider component displaying the health value.
    - offset: A Vector3 offset applied to the health canvas position relative to the target.
    
    Private Fields:
    - healthComponent: The IHealth component attached to the parent GameObject, which provides health data.
    - targetTransform: The transform of the parent GameObject used to position the health canvas.
    
    Methods:
    - Awake(): Initializes healthComponent and healthSlider. Logs errors if components are not found.
    - Start(): Calls UpdateHealthUI() to set initial health values on the slider.
    - Update(): Updates the health UI and adjusts the health canvas position and rotation to face the camera.
    - UpdateHealthUI(): Sets the Slider's value to the current health and its max value to the object's maximum health.
*/

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
