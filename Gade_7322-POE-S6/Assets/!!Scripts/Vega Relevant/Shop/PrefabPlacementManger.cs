/*
    The PrefabPlacementManager class handles the placement of prefabs in the game world, including previewing, rotating, and validating placement.

    - Fields:
      - instance: Singleton instance of the PrefabPlacementManager.
      - previewRadiusMultiplier: Multiplier for the preview radius.
      - placementRadius: Radius for placement validation.
      - requiredDistanceFromPath: Minimum distance from paths required for placement.
      - requiredMinimumDistanceFromPath: Minimum distance from paths required to avoid invalid placement.
      - foliageLayerMask: Layer mask to identify foliage objects.

    - Methods:
      - Awake(): Initializes the singleton instance of PrefabPlacementManager. Ensures only one instance persists across scenes.
      - Update(): Handles input for placement and preview updating, and manages state changes.
      - BeginPlacement(GameObject prefab): Initiates the placement process by creating a preview instance of the given prefab.
      - UpdatePreview(): Updates the position of the preview instance based on mouse position and checks for valid placement.
      - PreviewFoliageRemoval(Vector3 position, float radius): Temporarily removes foliage objects in the preview area.
      - PlacePrefab(): Finalizes the placement of the prefab, removes existing foliage, and rebakes the NavMesh.
      - RemoveExistingFoliage(Vector3 position, float radius): Permanently removes foliage objects in the placement area.
      - RebakeNavMesh(): Rebuilds the NavMesh surfaces in the scene to reflect changes.
      - GetMouseWorldPosition(): Retrieves the world position of the mouse click.
      - IsValidPlacementPosition(Vector3 position): Checks if the placement position is valid based on distance from paths.
      - HandlePlacementInput(): Handles input for placing, rotating, or canceling placement.
      - RotatePrefab(): Rotates the preview instance by 90 degrees.
      - CancelPlacement(): Cancels the placement process, restores foliage, and refunds gold.
*/


using Unity.AI.Navigation;
using UnityEngine;
using System.Collections.Generic;

public class PrefabPlacementManager : MonoBehaviour
{
    public static PrefabPlacementManager instance;

    [SerializeField] private float previewRadiusMultiplier = 2f;
    [SerializeField] private float placementRadius = 1f;
    [SerializeField] private float requiredDistanceFromPath = 1.0f;
    [SerializeField] private float requiredMinimumDistanceFromPath = 1.0f;
    [SerializeField] private LayerMask foliageLayerMask;

    private GameObject currentPrefab;
    private GameObject previewInstance;
    private Vector3 placementPosition;
    private List<GameObject> foliageToRemove = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (GameManager.instance.GetCurrentState() == GameManager.GameState.Placement && previewInstance != null)
        {
            HandlePlacementInput();
            UpdatePreview();
        }
        else if (previewInstance != null)
        {
            CancelPlacement();
            Debug.LogWarning("Placement canceled: Game state changed before placement was confirmed.");
        }
    }

    public void BeginPlacement(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab is null, cannot initiate placement.");
            return;
        }

        currentPrefab = prefab;
        previewInstance = Instantiate(currentPrefab);

        if (previewInstance != null)
        {
            previewInstance.GetComponent<Collider>().enabled = false;
            EventManager.instance.TriggerPlacementMode();
            Debug.Log("Preview instance successfully created.");
        }
        else
        {
            Debug.LogError("Failed to instantiate preview instance.");
        }
    }

    private void UpdatePreview()
    {
        Vector3 newPosition = GetMouseWorldPosition();

        if (IsValidPlacementPosition(newPosition) )
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")) && previewInstance != null)
            {
                Transform bottomTransform = previewInstance.transform.Find("Bottom");
                if (bottomTransform != null)
                {
                    Vector3 offset = previewInstance.transform.position - bottomTransform.position;
                    newPosition = hit.point + offset;
                    previewInstance.transform.position = newPosition;
                    previewInstance.SetActive(true);

                    PreviewFoliageRemoval(newPosition, placementRadius * previewRadiusMultiplier);
                }
                else
                {
                    Debug.LogError("No 'Bottom' child object found on the preview instance.");
                }
            }
            else if (previewInstance != null)

            {
                previewInstance.SetActive(false);
            }
        }
        else
        {
            if (previewInstance != null)
            {
                previewInstance.SetActive(false);
            }
        }
    }

    private void PreviewFoliageRemoval(Vector3 position, float radius)
    {
        foreach (GameObject foliage in foliageToRemove)
        {
            if (foliage != null)
            {
                foliage.SetActive(true);
            }
        }
        foliageToRemove.Clear();

        Collider[] colliders = Physics.OverlapSphere(position, radius, foliageLayerMask);

        foreach (Collider collider in colliders)
        {
            GameObject foliageObject = collider.gameObject;
            if (foliageObject != null)
            {
                foliageObject.SetActive(false);
                foliageToRemove.Add(foliageObject);
            }
        }
    }
    private void PlacePrefab()
    {
        if (previewInstance != null && previewInstance.activeSelf)
        {
            Transform bottomTransform = previewInstance.transform.Find("Bottom");
            if (bottomTransform != null)
            {
                Vector3 finalPosition = previewInstance.transform.position;

                RemoveExistingFoliage(finalPosition, placementRadius);

                Instantiate(currentPrefab, finalPosition, previewInstance.transform.rotation);

                Destroy(previewInstance);
                previewInstance = null;
                currentPrefab = null;
                RebakeNavMesh();

                EventManager.instance.TriggerDefenderPlaced();

                EventManager.instance.TriggerUpgradeMode();
            }
            else
            {
                Debug.LogError("Cannot place prefab: Preview instance has no 'Bottom' child object.");
            }
        }
        else
        {
            Debug.LogError("Preview instance is null or inactive, cannot place prefab.");
        }
    }

    private void RemoveExistingFoliage(Vector3 position, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(position, radius, foliageLayerMask);

        foreach (Collider collider in colliders)
        {
            GameObject foliageObject = collider.gameObject;
            if (foliageObject != null)
            {
                Debug.Log($"Removing existing foliage object: {foliageObject.name}");
                Destroy(foliageObject);
            }
        }
    }

    private void RebakeNavMesh()
    {
        NavMeshSurface[] navMeshSurfaces = FindObjectsOfType<NavMeshSurface>();

        if (navMeshSurfaces.Length == 0)
        {
            Debug.LogWarning("No NavMeshSurface components found in the scene.");
            return;
        }

        foreach (NavMeshSurface surface in navMeshSurfaces)
        {
            if (surface != null)
            {
                surface.BuildNavMesh();
            }
            else
            {
                Debug.LogError($"NavMeshSurface component is missing on the GameObject: {surface.gameObject.name}");
            }
        }

        Debug.Log("All NavMeshes baked successfully at runtime.");
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            return hit.point;
        }

        Debug.LogWarning("Mouse position not hitting Ground, returning zero vector.");
        return Vector3.zero;
    }

    private bool IsValidPlacementPosition(Vector3 position)
    {
        Ray groundRay = new Ray(position + Vector3.up * 10, Vector3.down);
        if (Physics.Raycast(groundRay, out RaycastHit groundHit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            if (groundHit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Collider[] nearbyPathColliders = Physics.OverlapSphere(position, requiredDistanceFromPath, LayerMask.GetMask("Path"));
                if (nearbyPathColliders.Length == 0)
                {
                    Collider[] withinMinimumDistanceColliders = Physics.OverlapSphere(position, requiredMinimumDistanceFromPath, LayerMask.GetMask("Path"));
                    if (withinMinimumDistanceColliders.Length > 0)
                    {
                        return true;
                    }
                    else
                    {
                        Debug.Log("Invalid placement position: too far away from the Path.");
                        return false;
                    }
                }
            }
        }

        Debug.Log("Invalid placement position: too close to or directly on Path.");
        return false;
    }
    private void HandlePlacementInput()
    {
        if (Input.GetMouseButtonDown(0) && previewInstance.activeSelf)
        {
            PlacePrefab();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            RotatePrefab();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }
    }

    private void RotatePrefab()
    {
        if (previewInstance != null)
        {
            previewInstance.transform.Rotate(0, 90, 0);
        }
        else
        {
            Debug.LogError("Preview instance is null, cannot rotate prefab.");
        }
    }

    private void CancelPlacement()
    {
        foreach (GameObject foliage in foliageToRemove)
        {
            if (foliage != null)
            {
                foliage.SetActive(true);
            }
        }
        foliageToRemove.Clear();

        if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
        }
        currentPrefab = null;
        GoldManager.instance.AddGold(ShopManager.instance.shieldDefenderCost);
        EventManager.instance.TriggerUpgradeMode();
        EventManager.instance.TriggerDefenderPlaced();
    }
}
