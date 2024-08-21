using Unity.AI.Navigation;
using UnityEngine;

/// <summary>
/// Manages the placement of building prefabs in the game. Ensures that buildings are placed on valid ground locations, 
/// sufficiently far from the path, and that placement is properly handled with preview and rotation functionalities.
/// Automatically refunds gold if the game state changes before placement is confirmed.
/// </summary>
public class PrefabPlacementManager : MonoBehaviour
{
    public static PrefabPlacementManager instance;

    [SerializeField] private float placementRadius = 1f;
    [SerializeField] private float requiredDistanceFromPath = 1.0f;

    private GameObject currentPrefab;
    private GameObject previewInstance;
    private Vector3 placementPosition;

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
        if (previewInstance == null)
        {
            Debug.LogError("Preview instance is null, cannot update preview.");
            return;
        }

        Vector3 newPosition = GetMouseWorldPosition();

        if (IsValidPlacementPosition(newPosition))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                Transform bottomTransform = previewInstance.transform.Find("Bottom");
                if (bottomTransform != null)
                {
                    Vector3 offset = previewInstance.transform.position - bottomTransform.position;
                    newPosition = hit.point + offset;
                    previewInstance.transform.position = newPosition;
                    previewInstance.SetActive(true);
                }
                else
                {
                    Debug.LogError("No 'Bottom' child object found on the preview instance.");
                }
            }
            else
            {
                previewInstance.SetActive(false);
            }
        }
        else
        {
            previewInstance.SetActive(false);
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

                Instantiate(currentPrefab, finalPosition, previewInstance.transform.rotation);

                Destroy(previewInstance);
                previewInstance = null;
                currentPrefab = null;
                RebakeNavMesh();

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
                    return true;
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
        if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
        }
        currentPrefab = null;
        GoldManager.instance.AddGold(ShopManager.instance.shieldDefenderCost);
        EventManager.instance.TriggerUpgradeMode();
    }
}
