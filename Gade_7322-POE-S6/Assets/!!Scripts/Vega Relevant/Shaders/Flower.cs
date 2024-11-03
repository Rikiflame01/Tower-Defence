using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Flower : MonoBehaviour
{
    private void Start()
    {
        FlowerSwayManager manager = FindObjectOfType<FlowerSwayManager>();
        if (manager != null)
        {
            manager.RegisterFlower(GetComponent<MeshFilter>());
        }
        else
        {
            Debug.LogError("FlowerSwayManager not found in the scene.");
        }
    }
}
