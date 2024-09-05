/*
    The FoliageManager class is responsible for generating and placing various types of foliage 
    (grass, flowers, rocks, trees) in a scene based on Perlin noise and certain constraints.
    - Uses Perlin noise to determine where to place foliage.
    - Ensures foliage is not placed too close to paths, the TownHall, or existing trees.
    - Prevents overlapping foliage and manages occupied nodes.
    - Handles the instantiation and placement of foliage prefabs.
    - Provides methods for checking conditions around paths, the TownHall, and existing foliage.
    - Uses raycasting and sphere checks to determine if positions meet specific criteria.
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoliageManager : MonoBehaviour
{
    public GameObject grassPrefab;
    public GameObject flowerPrefab;
    public GameObject rockPrefab;
    public GameObject treePrefab;

    public float perlinScale = 10f;
    public float foliageDensity = 0.5f;
    public float minDistanceBetweenTrees = 2f;
    public float minDistanceFromPath = 3f;
    public float minDistanceFromTownHall = 10f;
    public LayerMask pathLayerMask;

    private Dictionary<Vector3, GameObject> occupiedNodes = new Dictionary<Vector3, GameObject>();
    private Transform townHallTransform;

    void Start()
    {
        GameObject townHall = GameObject.FindGameObjectWithTag("TownHall");
        if (townHall != null)
        {
            townHallTransform = townHall.transform;
        }
        else
        {
            Debug.LogError("No object with the 'TownHall' tag found in the scene.");
        }

        StartCoroutine(GenerateFoliage());
    }

    private IEnumerator GenerateFoliage()
    {
        yield return null;

        Node[] allNodes = FindObjectsOfType<Node>();
        if (allNodes == null || allNodes.Length == 0)
        {
            Debug.LogError("No nodes found in the scene.");
            yield break;
        }

        Debug.Log($"Found {allNodes.Length} nodes in the scene.");

        foreach (var node in allNodes)
        {
            Vector3 nodePosition = node.transform.position;

            PlaceGrass(node, nodePosition);

            if (node.isOccupied || IsPositionNearPath(nodePosition))
                continue;

            float noiseValue = Mathf.PerlinNoise(nodePosition.x / perlinScale, nodePosition.z / perlinScale);

            if (noiseValue > foliageDensity)
            {
                if (TryPlaceFoliage(node, nodePosition, noiseValue))
                {
                    continue;
                }
            }
        }
    }

    private void PlaceGrass(Node node, Vector3 position)
    {
        if (!IsDirectlyOnPath(position) && grassPrefab != null)
        {
            Instantiate(grassPrefab, position, Quaternion.identity, transform);
        }
    }

    private bool TryPlaceFoliage(Node node, Vector3 position, float noiseValue)
    {
        GameObject foliageInstance = null;

        if (noiseValue > 0.8f && !IsTreeNear(position))
        {
            if (treePrefab != null)
            {
                foliageInstance = Instantiate(treePrefab, position, GetRandomYRotation(), transform);
            }
            else
            {
                Debug.LogWarning("Tree prefab is not assigned.");
            }
            MarkNodesOccupied(position, minDistanceBetweenTrees);
        }
        else if (noiseValue > 0.6f)
        {
            if (rockPrefab != null && !IsPositionNearTownHall(position))
            {
                foliageInstance = Instantiate(rockPrefab, position, GetRandomYRotation(), transform);
            }
            else
            {
                Debug.LogWarning("Rock prefab is not assigned or too close to TownHall.");
            }
            MarkNodesOccupied(position, 1);
        }
        else if (noiseValue > 0.2f)
        {
            if (flowerPrefab != null)
            {
                foliageInstance = Instantiate(flowerPrefab, position, GetRandomYRotation(), transform);
            }
            else
            {
                Debug.LogWarning("Flower prefab is not assigned.");
            }
        }

        if (foliageInstance != null)
        {
            occupiedNodes[position] = foliageInstance;
            node.isOccupied = true;
            return true;
        }

        return false;
    }

    private bool IsDirectlyOnPath(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 0.1f, Vector3.down, out hit, 0.2f, pathLayerMask))
        {
            return true;
        }
        return false;
    }

    private bool IsPositionNearTownHall(Vector3 position)
    {
        if (townHallTransform != null)
        {
            float distanceToTownHall = Vector3.Distance(position, townHallTransform.position);
            bool nearTownHall = distanceToTownHall < minDistanceFromTownHall;

            return nearTownHall;
        }
        return false;
    }

    private bool IsPositionNearPath(Vector3 position)
    {
        bool nearPath = Physics.CheckSphere(position, minDistanceFromPath, pathLayerMask);
        return nearPath;
    }

    private bool IsTreeNear(Vector3 position)
    {
        foreach (var node in occupiedNodes)
        {
            if (node.Value != null && node.Value.CompareTag("Tree") && Vector3.Distance(node.Key, position) < minDistanceBetweenTrees)
            {
                Debug.Log($"Tree found near position {position}, cannot place another tree.");
                return true;
            }
        }
        return false;
    }

    private void MarkNodesOccupied(Vector3 position, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(position, radius);
        foreach (var collider in colliders)
        {
            Vector3 colliderPosition = collider.transform.position;
            if (!occupiedNodes.ContainsKey(colliderPosition))
            {
                occupiedNodes[colliderPosition] = null;
                Node node = collider.GetComponent<Node>();
                if (node != null)
                {
                    node.SetOccupied(true);
                }
            }
        }
    }

    private Quaternion GetRandomYRotation()
    {
        return Quaternion.Euler(0, Random.Range(0f, 360f), 0);
    }
}
