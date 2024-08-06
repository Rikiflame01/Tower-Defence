using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;

public class DynamicPathNavMeshBaker : MonoBehaviour
{
    public float bakeDelay = 5.0f;

    void Start()
    {
        StartCoroutine(BakeNavMeshesAfterDelay(bakeDelay));
    }

    private IEnumerator BakeNavMeshesAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Find all NavMeshSurface components in the scene
        NavMeshSurface[] navMeshSurfaces = FindObjectsOfType<NavMeshSurface>();

        // Check if any NavMeshSurface components were found
        if (navMeshSurfaces.Length == 0)
        {
            Debug.LogWarning("No NavMeshSurface components found in the scene.");
            yield break;
        }

        // Iterate through each NavMeshSurface and bake the NavMesh
        foreach (NavMeshSurface surface in navMeshSurfaces)
        {
            Debug.Log($"Baking NavMesh for surface: {surface.gameObject.name}");

            if (surface != null)
            {
                surface.BuildNavMesh();
                Debug.Log($"NavMesh baked for surface: {surface.gameObject.name}");
            }
            else
            {
                Debug.LogError($"NavMeshSurface component is missing on the GameObject: {surface.gameObject.name}");
            }
        }

        Debug.Log("All NavMeshes baked successfully at runtime.");
    }
}
