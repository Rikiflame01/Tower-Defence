using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class PathGenerator : MonoBehaviour
{
    public GridGenerator gridGenerator;
    public GameObject pathPrefab;
    public float pathSpacing = 1.1f;
    public int numberOfPaths = 3;
    public Vector3 StartPathPosition { get; private set; }

    private List<Vector3> directions = new List<Vector3>
    {
        Vector3.forward,
        Vector3.back,
        Vector3.left,
        Vector3.right,
        Vector3.up,
        Vector3.down
    };

    public List<Vector3> startPositions = new List<Vector3>();
    private List<List<Vector3>> allPaths = new List<List<Vector3>>();
    private Dictionary<Vector3, GameObject> occupiedNodes = new Dictionary<Vector3, GameObject>();

    void Start()
    {
        StartCoroutine(GeneratePaths());
    }

    private IEnumerator GeneratePaths()
    {
        yield return new WaitUntil(() => gridGenerator != null && gridGenerator.IsGridGenerated());

        if (gridGenerator == null)
        {
            Debug.LogError("GridGenerator is not assigned.");
            yield break;
        }

        for (int i = 0; i < numberOfPaths; i++)
        {
            bool pathGenerated = false;
            int attempts = 0;
            while (!pathGenerated && attempts < 10)
            {
                StartPathPosition = GetRandomPerimeterPosition();
                if (IsPositionFarEnough(StartPathPosition, 4))
                {
                    Vector3 centerPosition = GetCenterPosition();
                    List<Vector3> path = FindPath(StartPathPosition, centerPosition);

                    if (path != null && path.Count > 0)
                    {
                        allPaths.Add(path);
                        startPositions.Add(StartPathPosition);
                        foreach (Vector3 position in path)
                        {
                            if (!occupiedNodes.ContainsKey(position))
                            {
                                Transform topNode = gridGenerator.GetTopNodeAtPosition(position);
                                if (topNode != null)
                                {
                                    GameObject pathInstance = Instantiate(pathPrefab, topNode.position, Quaternion.identity, transform);
                                    occupiedNodes[position] = pathInstance;
                                }
                                else
                                {
                                    Debug.LogError("Top node not found at position: " + position);
                                }
                            }
                        }

                        Debug.Log("Path " + (i + 1) + " generated successfully.");
                        pathGenerated = true;
                    }
                }

                attempts++;
            }

            if (!pathGenerated)
            {
                Debug.LogError("Failed to generate a non-overlapping path after 10 attempts.");
            }
        }

        //Ensures only one path has a NavMeshSurface component
        AssignSingleNavMeshSurface();
    }

    public Vector3 GetRandomPerimeterPosition()
    {
        List<Vector3> perimeterPositions = new List<Vector3>();
        int maxX = gridGenerator.gridWidth - 1;
        int maxY = gridGenerator.gridHeight - 1;
        int maxZ = gridGenerator.gridDepth - 1;

        // Adding perimeter positions
        for (int x = 0; x <= maxX; x++)
        {
            perimeterPositions.Add(new Vector3(x * pathSpacing, 0, 0));
            perimeterPositions.Add(new Vector3(x * pathSpacing, 0, maxZ * pathSpacing));
            perimeterPositions.Add(new Vector3(x * pathSpacing, maxY * pathSpacing, 0));
            perimeterPositions.Add(new Vector3(x * pathSpacing, maxY * pathSpacing, maxZ * pathSpacing));
        }

        for (int y = 0; y <= maxY; y++)
        {
            perimeterPositions.Add(new Vector3(0, y * pathSpacing, 0));
            perimeterPositions.Add(new Vector3(0, y * pathSpacing, maxZ * pathSpacing));
            perimeterPositions.Add(new Vector3(maxX * pathSpacing, y * pathSpacing, 0));
            perimeterPositions.Add(new Vector3(maxX * pathSpacing, y * pathSpacing, maxZ * pathSpacing));
        }

        for (int z = 0; z <= maxZ; z++)
        {
            perimeterPositions.Add(new Vector3(0, 0, z * pathSpacing));
            perimeterPositions.Add(new Vector3(maxX * pathSpacing, 0, z * pathSpacing));
            perimeterPositions.Add(new Vector3(0, maxY * pathSpacing, z * pathSpacing));
            perimeterPositions.Add(new Vector3(maxX * pathSpacing, maxY * pathSpacing, z * pathSpacing));
        }

        int randomIndex = Random.Range(0, perimeterPositions.Count);
        return perimeterPositions[randomIndex];
    }

    private bool IsPositionFarEnough(Vector3 newPosition, int minDistance)
    {
        foreach (Vector3 existingPosition in startPositions)
        {
            if (Vector3.Distance(existingPosition, newPosition) < minDistance * pathSpacing)
            {
                return false;
            }
        }
        return true;
    }

    public Vector3 GetCenterPosition()
    {
        float centerX = Mathf.Floor((gridGenerator.gridWidth - 1) / 2f) * pathSpacing;
        float centerY = Mathf.Floor((gridGenerator.gridHeight - 1) / 2f) * pathSpacing;
        float centerZ = Mathf.Floor((gridGenerator.gridDepth - 1) / 2f) * pathSpacing;
        return new Vector3(centerX, centerY, centerZ);
    }

    public List<Vector3> FindPath(Vector3 start, Vector3 goal)
    {
        List<Vector3> openSet = new List<Vector3> { start };
        Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
        Dictionary<Vector3, float> gScore = new Dictionary<Vector3, float>();
        gScore[start] = 0;
        Dictionary<Vector3, float> fScore = new Dictionary<Vector3, float>();
        fScore[start] = Vector3.Distance(start, goal);

        while (openSet.Count > 0)
        {
            Vector3 current = openSet[0];
            foreach (Vector3 pos in openSet)
            {
                if (fScore[pos] < fScore[current])
                    current = pos;
            }

            if (Vector3.Distance(current, goal) < pathSpacing)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);

            foreach (Vector3 direction in directions)
            {
                Vector3 neighbor = current + direction * pathSpacing;
                neighbor = new Vector3(
                    Mathf.Round(neighbor.x / pathSpacing) * pathSpacing,
                    Mathf.Round(neighbor.y / pathSpacing) * pathSpacing,
                    Mathf.Round(neighbor.z / pathSpacing) * pathSpacing
                );

                Transform topNode = gridGenerator.GetTopNodeAtPosition(neighbor);
                if (topNode == null || (topNode.GetComponent<Node>() != null && topNode.GetComponent<Node>().isOccupied))
                {
                    continue;
                }

                float tentativeGScore = gScore[current] + Vector3.Distance(current, neighbor);

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Vector3.Distance(neighbor, goal);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    List<Vector3> ReconstructPath(Dictionary<Vector3, Vector3> cameFrom, Vector3 current)
    {
        List<Vector3> totalPath = new List<Vector3> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        return totalPath;
    }

    private void AssignSingleNavMeshSurface()
    {
        if (occupiedNodes.Count == 0)
        {
            Debug.LogError("No paths generated to assign NavMeshSurface.");
            return;
        }

        int selectedIndex = Random.Range(0, occupiedNodes.Count);
        int currentIndex = 0;

        foreach (var node in occupiedNodes)
        {
            NavMeshSurface navMeshSurface = node.Value.GetComponent<NavMeshSurface>();
            if (navMeshSurface != null)
            {
                if (currentIndex == selectedIndex)
                {
                    navMeshSurface.BuildNavMesh();
                }
                else
                {
                    Destroy(navMeshSurface);
                }
            }
            currentIndex++;
        }
    }
}
