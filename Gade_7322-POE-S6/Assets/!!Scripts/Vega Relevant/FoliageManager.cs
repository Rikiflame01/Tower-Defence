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
    public LayerMask pathLayerMask;

    private Dictionary<Vector3, GameObject> occupiedNodes = new Dictionary<Vector3, GameObject>();

    void Start()
    {
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

            if (node.isOccupied)
            {
                occupiedNodes[nodePosition] = node.gameObject;
                continue;
            }

            if (IsPositionNearPath(nodePosition))
                continue;

            float noiseValue = Mathf.PerlinNoise(nodePosition.x / perlinScale, nodePosition.z / perlinScale);
            Debug.Log($"Noise value at {nodePosition}: {noiseValue}");

            if (noiseValue > foliageDensity)
            {
                if (TryPlaceFoliage(node, nodePosition, noiseValue))
                {
                    Debug.Log($"Foliage placed at {nodePosition}");
                    continue;
                }
            }
        }
    }

    private bool TryPlaceFoliage(Node node, Vector3 position, float noiseValue)
    {
        GameObject foliageInstance = null;

        if (noiseValue > 0.8f && !IsTreeNear(position))
        {
            if (treePrefab != null)
            {
                foliageInstance = Instantiate(treePrefab, position, Quaternion.identity, transform);
            }
            else
            {
                Debug.LogWarning("Tree prefab is not assigned.");
            }
            MarkNodesOccupied(position, minDistanceBetweenTrees);
        }
        else if (noiseValue > 0.6f)
        {
            if (rockPrefab != null)
            {
                foliageInstance = Instantiate(rockPrefab, position, Quaternion.identity, transform);
            }
            else
            {
                Debug.LogWarning("Rock prefab is not assigned.");
            }
            MarkNodesOccupied(position, 1);
        }
        else if (noiseValue > 0.4f)
        {
            if (grassPrefab != null)
            {
                foliageInstance = Instantiate(grassPrefab, position, Quaternion.identity, transform);
            }
            else
            {
                Debug.LogWarning("Grass prefab is not assigned.");
            }
        }
        else if (noiseValue > 0.2f)
        {
            if (flowerPrefab != null)
            {
                foliageInstance = Instantiate(flowerPrefab, position, Quaternion.identity, transform);
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

    private bool IsPositionNearPath(Vector3 position)
    {
        bool nearPath = Physics.CheckSphere(position, minDistanceFromPath, pathLayerMask);
        if (nearPath)
        {
            Debug.Log($"Position {position} is near a path.");
        }
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
                    Debug.Log($"Marked node at {colliderPosition} as occupied.");
                }
            }
        }
    }
}
