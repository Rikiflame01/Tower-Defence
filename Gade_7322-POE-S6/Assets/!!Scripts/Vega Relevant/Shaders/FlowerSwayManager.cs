using UnityEngine;
using System.Collections.Generic;

public class FlowerSwayManager : MonoBehaviour
{
    public ComputeShader swayComputeShader;
    public float swayAmplitude = 0.1f;      
    public float swayFrequency = 1.0f;      

    private List<FlowerData> flowers = new List<FlowerData>();
    private ComputeBuffer vertexBuffer;
    private ComputeBuffer initialPositionsBuffer;
    private int totalVertices = 0;
    private int kernelHandle;
    private float time;

    void Start()
    {
        kernelHandle = swayComputeShader.FindKernel("CSMain");
    }

    public void RegisterFlower(MeshFilter flowerMeshFilter)
    {
        Vector3[] originalVertices = flowerMeshFilter.mesh.vertices;
        if (originalVertices.Length == 0)
        {
            Debug.LogWarning("Flower has no vertices: " + flowerMeshFilter.gameObject.name);
            return;
        }

        totalVertices += originalVertices.Length;

        var flowerData = new FlowerData
        {
            meshFilter = flowerMeshFilter,
            originalVertices = originalVertices,
            currentVertices = new Vector3[originalVertices.Length]
        };
        flowers.Add(flowerData);

        Debug.Log("Registered flower: " + flowerMeshFilter.gameObject.name + " with " + originalVertices.Length + " vertices.");
    }

    void Update()
    {
        if (totalVertices == 0) return;

        time += Time.deltaTime;

        if (vertexBuffer == null || vertexBuffer.count < totalVertices)
        {
            if (vertexBuffer != null) vertexBuffer.Release();
            if (initialPositionsBuffer != null) initialPositionsBuffer.Release();

            vertexBuffer = new ComputeBuffer(totalVertices, sizeof(float) * 3);
            initialPositionsBuffer = new ComputeBuffer(totalVertices, sizeof(float) * 3);
        }

        int vertexOffset = 0;
        foreach (var flower in flowers)
        {
            initialPositionsBuffer.SetData(flower.originalVertices, 0, vertexOffset, flower.originalVertices.Length);
            vertexOffset += flower.originalVertices.Length;
        }

        swayComputeShader.SetFloat("time", time);
        swayComputeShader.SetFloat("swayAmplitude", swayAmplitude);
        swayComputeShader.SetFloat("swayFrequency", swayFrequency);

        swayComputeShader.SetBuffer(kernelHandle, "vertices", vertexBuffer);
        swayComputeShader.SetBuffer(kernelHandle, "initialPositions", initialPositionsBuffer);

        swayComputeShader.Dispatch(kernelHandle, totalVertices / 256 + 1, 1, 1);

        vertexOffset = 0;
        foreach (var flower in flowers)
        {
            vertexBuffer.GetData(flower.currentVertices, 0, vertexOffset, flower.originalVertices.Length);
            flower.meshFilter.mesh.vertices = flower.currentVertices;
            flower.meshFilter.mesh.RecalculateNormals();
            vertexOffset += flower.originalVertices.Length;
        }
    }

    void OnDestroy()
    {
        if (vertexBuffer != null) vertexBuffer.Release();
        if (initialPositionsBuffer != null) initialPositionsBuffer.Release();
    }

    private class FlowerData
    {
        public MeshFilter meshFilter;
        public Vector3[] originalVertices;
        public Vector3[] currentVertices;
    }
}
