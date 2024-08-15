using UnityEngine;

public class GoldCollectionTrigger : MonoBehaviour
{
    public float collectionRadius = 5f;
    public LayerMask groundOrPathLayerMask;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CollectGoldInRadius();
        }
    }

    private void CollectGoldInRadius()
    {
        if (GameManager.instance.GetCurrentState() == GameManager.GameState.Cooldown ||
            GameManager.instance.GetCurrentState() == GameManager.GameState.Wave)
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
            //Debug.Log($"Hit {hit.collider.name} on layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
            return hit.point;
        }

        Debug.LogWarning("Raycast did not hit the Ground or Path layers.");
        return Vector3.zero;
    }
}
