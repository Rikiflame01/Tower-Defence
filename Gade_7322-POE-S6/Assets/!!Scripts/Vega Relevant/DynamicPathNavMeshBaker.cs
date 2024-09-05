/*
    The DynamicPathNavMeshBaker class manages the runtime baking of NavMesh surfaces.
    - Waits for a specified delay before initiating the baking process.
    - Finds all NavMeshSurface components in the scene.
    - Iterates through each NavMeshSurface to build the NavMesh, logging warnings if none are found or errors if a component is missing.
    - Logs a success message after all NavMeshes are baked.
*/


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
}
