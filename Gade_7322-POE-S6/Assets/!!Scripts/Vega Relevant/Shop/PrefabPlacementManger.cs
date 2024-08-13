using UnityEngine;

public class PrefabPlacementManager : MonoBehaviour
{
    public static PrefabPlacementManager instance;

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

        placementPosition = GetMouseWorldPosition();
        previewInstance.transform.position = placementPosition;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }

        Debug.LogWarning("Mouse position not hitting anything, returning zero vector.");
        return Vector3.zero;
    }

    private void HandlePlacementInput()
    {
        if (Input.GetMouseButtonDown(0))
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

    private void PlacePrefab()
    {
        if (previewInstance != null)
        {
            Instantiate(currentPrefab, placementPosition, previewInstance.transform.rotation);
            Destroy(previewInstance);
            previewInstance = null;
            currentPrefab = null;
            GameManager.instance.ResumePreviousState();
        }
        else
        {
            Debug.LogError("Preview instance is null, cannot place prefab.");
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
        GameManager.instance.ResumePreviousState();
    }
}
