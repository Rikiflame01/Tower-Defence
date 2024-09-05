/*
    The GridGenerator class creates and manages a 3D grid of cubes in Unity.
    - Instantiates cubes in a grid based on specified width, height, depth, and spacing.
    - Stores each cube in a dictionary with its position as the key.
    - Provides methods to get a cube at a specific position, check if the grid is generated, 
      and retrieve the top node of a cube.
    - Retrieves all nodes from the grid by accessing the Node component of each cube.
*/

using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GameObject cubePrefab;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public int gridDepth = 10;
    public float spacing = 1.1f;

    private Dictionary<Vector3, GameObject> cubeGrid = new Dictionary<Vector3, GameObject>();
    private bool gridGenerated = false;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                for (int z = 0; z < gridDepth; z++)
                {
                    Vector3 position = new Vector3(x * spacing, y * spacing, z * spacing);
                    GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
                    cube.transform.parent = transform;
                    cubeGrid.Add(position, cube);
                }
            }
        }
        gridGenerated = true;
    }

    public GameObject GetCubeAtPosition(Vector3 position)
    {
        if (cubeGrid.ContainsKey(position))
        {
            return cubeGrid[position];
        }
        return null;
    }

    public bool IsGridGenerated()
    {
        return gridGenerated;
    }

    public Transform GetTopNodeAtPosition(Vector3 position)
    {
        GameObject cube = GetCubeAtPosition(position);
        if (cube != null)
        {
            CubeController cubeController = cube.GetComponent<CubeController>();
            if (cubeController != null)
            {
                return cubeController.GetTopNode();
            }
        }
        return null;
    }

    public List<Node> GetAllNodes()
    {
        List<Node> nodes = new List<Node>();

        foreach (var cube in cubeGrid.Values)
        {
            Node node = cube.GetComponent<Node>();
            if (node != null)
            {
                nodes.Add(node);
            }
        }

        return nodes;
    }
}
