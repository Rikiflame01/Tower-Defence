using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private Transform placementCameraTarget; // The target position and rotation for the camera during placement mode
    [SerializeField] private float cameraTransitionSpeed = 2f; // Speed of the camera transition

    private bool isFreeLookActive = false;
    private Vector3 lastMousePosition;
    private bool isInPlacementMode = false;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;

    private void OnEnable()
    {
        EventManager.instance.onDefenderPlaced.AddListener(TransitionBack);
    }

    private void OnDisable()
    {
        EventManager.instance.onDefenderPlaced.RemoveListener(TransitionBack);
    }

    private void TransitionBack()
    {
        isInPlacementMode = false;
        StartCoroutine(TransitionCamera(originalCameraPosition, originalCameraRotation));
    }

    private void Update()
    {
        HandlePause();
        HandleMovement();

        if (GameManager.instance.currentState == GameManager.GameState.Placement)
        {
            HandlePlacementCamera();
            return;
        }
        else
        {
            HandleFreeLook();
        }
    }

    private void HandlePause()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Placement)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EventManager.instance.TriggerPauseMode();
            }
        }
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            Vector3 moveDirection = transform.forward * vertical + transform.right * horizontal;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }

    private void HandleFreeLook()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse button down
        {
            isFreeLookActive = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(1)) // Right mouse button up
        {
            isFreeLookActive = false;
        }

        if (isFreeLookActive)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            // Rotate the camera based on mouse movement
            float yaw = mouseDelta.x * lookSpeed;
            float pitch = -mouseDelta.y * lookSpeed;

            transform.Rotate(0, yaw, 0, Space.World); // Rotate around the Y axis
            Camera.main.transform.Rotate(pitch, 0, 0, Space.Self); // Rotate the camera around its X axis
        }
    }

    private void HandlePlacementCamera()
    {
        if (!isInPlacementMode)
        {
            isInPlacementMode = true;
            originalCameraPosition = Camera.main.transform.position;
            originalCameraRotation = Camera.main.transform.rotation;

            if (placementCameraTarget != null)
            {
                StartCoroutine(TransitionCamera(placementCameraTarget.position, Quaternion.Euler(90f, 0f, 0f)));
            }
            else
            {
                Debug.LogWarning("Placement camera target is not set.");
            }
        }
    }

    private IEnumerator TransitionCamera(Vector3 targetPosition, Quaternion targetRotation)
    {
        float t = 0;
        Vector3 startPosition = Camera.main.transform.position;
        Quaternion startRotation = Camera.main.transform.rotation;

        while (t < 1)
        {
            t += Time.deltaTime * cameraTransitionSpeed;

            Camera.main.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            Camera.main.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            yield return null;
        }

        // Ensure final position and rotation are exactly set
        Camera.main.transform.position = targetPosition;
        Camera.main.transform.rotation = targetRotation;

        // Debugging rotation
        Debug.Log("Final Rotation: " + Camera.main.transform.rotation.eulerAngles);
    }
}
