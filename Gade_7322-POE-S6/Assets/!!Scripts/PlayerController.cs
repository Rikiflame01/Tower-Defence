/*
    The PlayerController class manages player movement and camera behavior.
    - Handles player movement based on input.
    - Manages camera transitions for placement mode and free look mode.
    - Listens to defender placement events to transition back to the default camera view.
    - Handles pausing the game when the Escape key is pressed (if not in Placement mode).
    - Uses coroutines for smooth camera transitions.
    - Adjusts the camera's field of view and position during transitions.
    - Switches between placement and free look camera modes based on game state and input.
*/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private Transform placementCameraTarget;
    [SerializeField] private Transform defaultCameraTarget;
    [SerializeField] private float cameraTransitionSpeed = 2f;

    private bool isFreeLookActive = false;
    private Vector3 lastMousePosition;
    private bool isInPlacementMode = false;

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

        if (defaultCameraTarget != null)
        {
            Quaternion targetRotation = Quaternion.Euler(defaultCameraTarget.rotation.eulerAngles.x, 90f, defaultCameraTarget.rotation.eulerAngles.z);
            StartCoroutine(TransitionCamera(defaultCameraTarget.position, targetRotation, 60f));
        }
        else
        {
            Debug.LogWarning("Default camera target is not set.");
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "GameScene" && GameManager.instance != null)
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

    }

    private void HandlePause()
    {
        if (GameManager.instance != null)
        {
            if (GameManager.instance.currentState != GameManager.GameState.Placement)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    EventManager.instance.TriggerPauseMode();
                }
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
        if (Input.GetMouseButtonDown(1))
        {
            isFreeLookActive = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isFreeLookActive = false;
        }

        if (isFreeLookActive)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            float yaw = mouseDelta.x * lookSpeed;
            float pitch = -mouseDelta.y * lookSpeed;

            transform.Rotate(0, yaw, 0, Space.World);
            Camera.main.transform.Rotate(pitch, 0, 0, Space.Self);
        }
    }

    private void HandlePlacementCamera()
    {
        if (!isInPlacementMode)
        {
            isInPlacementMode = true;

            if (placementCameraTarget != null)
            {
                StartCoroutine(TransitionCamera(placementCameraTarget.position, Quaternion.Euler(90f, 0f, 0f), 97f));
            }
            else
            {
                Debug.LogWarning("Placement camera target is not set.");
            }
        }
    }

    private IEnumerator TransitionCamera(Vector3 targetPosition, Quaternion targetRotation, float targetFOV)
    {
        float t = 0;
        Vector3 startPosition = Camera.main.transform.position;
        Quaternion startRotation = Camera.main.transform.rotation;
        float startFOV = Camera.main.fieldOfView;

        while (t < 1)
        {
            t += Time.deltaTime * cameraTransitionSpeed;

            Camera.main.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            Camera.main.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            Camera.main.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);

            yield return null;
        }

        Camera.main.transform.position = targetPosition;
        Camera.main.transform.rotation = targetRotation;
        Camera.main.fieldOfView = targetFOV;

        Debug.Log("Final Rotation: " + Camera.main.transform.rotation.eulerAngles);
    }
}
