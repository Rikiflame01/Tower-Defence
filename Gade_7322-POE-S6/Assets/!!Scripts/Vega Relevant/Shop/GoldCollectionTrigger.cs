/*
    The GoldCollectionTrigger class handles the collection of gold objects within a certain radius when the user clicks on the screen, 
    excluding clicks on UI elements and specific spinning objects.

    - Fields:
      - collectionRadius: The radius within which gold can be collected.
      - groundOrPathLayerMask: Layer mask for detecting ground or path layers.
      - clickableLayerMask: Layer mask for detecting clickable objects.

    - Methods:
      - Start(): Initializes the clickableLayerMask with the "ClickableObject" layer.
      - Update(): Checks for mouse clicks and determines if the click should trigger gold collection or skip based on whether it hits a spinning object.
      - IsClickOnSpinningObject(): Performs a raycast to check if the click hits a spinning object and returns true if it does.
      - CollectGoldInRadius(): Collects gold within the specified radius if the current game state allows it. Uses raycasting to get the click position 
        and overlap sphere to detect gold objects.
      - GetMouseWorldPosition(): Performs a raycast to get the world position of the mouse click, based on ground or path layers.
      - IsPointerOverUI(): Checks if the mouse pointer is over a UI element to prevent interaction with the UI.
*/

using UnityEngine;
using UnityEngine.EventSystems;

public class GoldCollectionTrigger : MonoBehaviour
{
    public float collectionRadius = 5f;
    public LayerMask groundOrPathLayerMask;

    public LayerMask clickableLayerMask;

    private void Start()
    {
        clickableLayerMask = LayerMask.GetMask("ClickableObject");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            if (!IsClickOnSpinningObject())
            {
                CollectGoldInRadius();
            }
            else
            {
                Debug.Log("Click was on spinning object, skipping gold collection.");
            }
        }
    }

    private bool IsClickOnSpinningObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, clickableLayerMask))
        {
            StartGame spinObject = hit.transform.GetComponent<StartGame>();
            if (spinObject != null)
            {
                Debug.Log("Raycast hit spinning object: " + spinObject.gameObject.name);
                return true;
            }
        }
        return false;
    }

    private void CollectGoldInRadius()
    {
        if (GameManager.instance.GetCurrentState() == GameManager.GameState.Cooldown ||
            GameManager.instance.GetCurrentState() == GameManager.GameState.Wave ||
            GameManager.instance.GetCurrentState() == GameManager.GameState.Upgrade)
        {
            Vector3 clickPosition = GetMouseWorldPosition();

            if (clickPosition != Vector3.zero)
            {
                Collider[] goldColliders = Physics.OverlapSphere(clickPosition, collectionRadius, LayerMask.GetMask("Gold"));
                foreach (Collider collider in goldColliders)
                {
                    Gold gold = collider.GetComponent<Gold>();
                    if (gold != null)
                    {   
                        gold.TriggerCollection();
                    }
                }
                if (goldColliders.Length > 0){                
                    SoundManager.Instance.PlaySFXRandomTiming("CoinDrop3", 1f);}

            }
            else
            {
                Debug.Log("Click was not on the Ground or Path layers.");
            }
        }
        else
        {
            Debug.Log("Gold collection is not allowed in the current game state.");
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, groundOrPathLayerMask))
        {
            return hit.point;
        }

        Debug.LogWarning("Raycast did not hit the Ground or Path layers.");
        return Vector3.zero;
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
