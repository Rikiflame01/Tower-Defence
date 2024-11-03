using UnityEngine;
using System.Collections.Generic;

public class FlowerSwayManager : MonoBehaviour
{
    public Material flowerMaterial;
    public float swayAmplitude = 0.1f;
    public float swayFrequency = 1.0f;
    public float swayHeightPercentage = 0.5f;
    public float blendZoneHeight = 0.1f;
    public float updateInterval = 0.1f;

    private List<FlowerData> flowers = new List<FlowerData>();
    private float time;

    void Start()
    {
        InvokeRepeating(nameof(UpdateSway), 0.1f, updateInterval);
    }

    private void UpdateSway()
    {
        time += Time.deltaTime;

        foreach (var flower in flowers)
        {
            Renderer renderer = flower.meshFilter.GetComponent<Renderer>();
            if (renderer != null)
            {
                MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(propertyBlock);

                propertyBlock.SetFloat("_SwayAmplitude", swayAmplitude);
                propertyBlock.SetFloat("_SwayFrequency", swayFrequency);
                propertyBlock.SetFloat("_SwayHeightPercentage", swayHeightPercentage);
                propertyBlock.SetFloat("_BlendZoneHeight", blendZoneHeight);
                propertyBlock.SetFloat("_TimeOffset", time);

                renderer.SetPropertyBlock(propertyBlock);
            }
        }
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

        var flowerData = new FlowerData
        {
            meshFilter = flowerMeshFilter,
            originalVertices = originalVertices,
            maxHeight = maxHeight
        };
        flowers.Add(flowerData);
    }

    private class FlowerData
    {
        public MeshFilter meshFilter;
        public Vector3[] originalVertices;
        public float maxHeight;
    }
}
