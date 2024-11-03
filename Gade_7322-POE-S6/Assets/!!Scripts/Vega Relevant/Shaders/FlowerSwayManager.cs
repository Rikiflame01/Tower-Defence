using System.Collections.Generic;
using UnityEngine;

public class FlowerSwayManager : MonoBehaviour
{
    public ComputeShader swayComputeShader;
    public float swayAmplitude = 0.1f;
    public float swayFrequency = 1.0f;
    public float swayHeightPercentage = 0.5f;
    public float blendZoneHeight = 0.1f;


    private List<FlowerData> flowers = new List<FlowerData>();
    private ComputeBuffer vertexBuffer;
    private ComputeBuffer initialPositionsBuffer;
    private ComputeBuffer maxHeightBuffer;
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

        float maxHeight = float.MinValue;
        foreach (Vector3 vertex in originalVertices)
        {
            if (vertex.y > maxHeight)
                maxHeight = vertex.y;
        }

        totalVertices += originalVertices.Length;

        var flowerData = new FlowerData
        {
            meshFilter = flowerMeshFilter,
            originalVertices = originalVertices,
            currentVertices = new Vector3[originalVertices.Length],
            maxHeight = maxHeight
        };
        flowers.Add(flowerData);
    }

    void Update()
    {
        if (totalVertices == 0) return;

        time += Time.deltaTime;

        if (vertexBuffer == null || vertexBuffer.count < totalVertices)
        {
            if (vertexBuffer != null) vertexBuffer.Release();
            if (initialPositionsBuffer != null) initialPositionsBuffer.Release();
            if (maxHeightBuffer != null) maxHeightBuffer.Release();

            vertexBuffer = new ComputeBuffer(totalVertices, sizeof(float) * 3);
            initialPositionsBuffer = new ComputeBuffer(totalVertices, sizeof(float) * 3);
            maxHeightBuffer = new ComputeBuffer(flowers.Count, sizeof(float));
        }

        int vertexOffset = 0;
        List<float> maxHeights = new List<float>();

        foreach (var flower in flowers)
        {
            initialPositionsBuffer.SetData(flower.originalVertices, 0, vertexOffset, flower.originalVertices.Length);
            maxHeights.Add(flower.maxHeight);
            vertexOffset += flower.originalVertices.Length;
        }
        maxHeightBuffer.SetData(maxHeights.ToArray());

        swayComputeShader.SetFloat("time", time);
        swayComputeShader.SetFloat("swayAmplitude", swayAmplitude);
        swayComputeShader.SetFloat("swayFrequency", swayFrequency);
        swayComputeShader.SetFloat("swayHeightPercentage", swayHeightPercentage);
        swayComputeShader.SetFloat("blendZoneHeight", blendZoneHeight);

        swayComputeShader.SetBuffer(kernelHandle, "vertices", vertexBuffer);
        swayComputeShader.SetBuffer(kernelHandle, "initialPositions", initialPositionsBuffer);
        swayComputeShader.SetBuffer(kernelHandle, "maxHeights", maxHeightBuffer);

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
        if (maxHeightBuffer != null) maxHeightBuffer.Release();
    }

    private class FlowerData
    {
        public MeshFilter meshFilter;
        public Vector3[] originalVertices;
        public Vector3[] currentVertices;
        public float maxHeight;
    }
}
