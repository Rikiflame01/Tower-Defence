/*
    The TowerSpawnScript handles the spawning of a tower prefab at the center of a grid.
    - On startup, it checks if the grid is generated using GridGenerator.
    - If the grid is valid, it calculates the center position of the grid, adjusts 
      the y-coordinate by a specified offset, and instantiates the tower prefab at this position.
    - If the grid is not generated or GridGenerator is not assigned, an error message is logged.

    Key methods:
    - SpawnTowerAtCenter: Instantiates the tower prefab at the calculated center position with an optional y-offset.
    - GetCenterPosition: Computes the center position of the grid based on grid dimensions and spacing.
*/

using UnityEngine;

public class TowerSpawnScript : MonoBehaviour
{
    public GridGenerator gridGenerator;
    public GameObject towerPrefab;
    public float yOffset = 0f; 

    private void Start()
    {
        SpawnTowerAtCenter();
    }

    private void SpawnTowerAtCenter()
    {
        if (gridGenerator != null && gridGenerator.IsGridGenerated())
        {
            Vector3 centerPosition = GetCenterPosition();
            centerPosition.y += yOffset;
            Instantiate(towerPrefab, centerPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("GridGenerator is not assigned or grid is not generated.");
        }
    }

    private Vector3 GetCenterPosition()
    {
        float centerX = Mathf.Floor((gridGenerator.gridWidth - 1) / 2f) * gridGenerator.spacing;
        float centerY = Mathf.Floor((gridGenerator.gridHeight - 1) / 2f) * gridGenerator.spacing;
        float centerZ = Mathf.Floor((gridGenerator.gridDepth - 1) / 2f) * gridGenerator.spacing;

        return new Vector3(centerX, centerY, centerZ);
    }
}
