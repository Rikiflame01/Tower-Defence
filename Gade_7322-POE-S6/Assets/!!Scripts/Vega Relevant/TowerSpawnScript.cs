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
