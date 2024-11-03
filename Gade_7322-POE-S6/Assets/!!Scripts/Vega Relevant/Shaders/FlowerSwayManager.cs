
using UnityEngine;
using System.Collections.Generic;

public class FlowerSwayManager : MonoBehaviour
{

    private MaterialPropertyBlock propertyBlock;

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
        propertyBlock = new MaterialPropertyBlock();
        InvokeRepeating(nameof(UpdateSway), 0.1f, updateInterval);
    }

    private void UpdateSway()
    {
        time += Time.deltaTime;

        propertyBlock.SetFloat("_SwayAmplitude", swayAmplitude);
        propertyBlock.SetFloat("_SwayFrequency", swayFrequency);
        propertyBlock.SetFloat("_SwayHeightPercentage", swayHeightPercentage);
        propertyBlock.SetFloat("_BlendZoneHeight", blendZoneHeight);
        propertyBlock.SetFloat("_TimeOffset", time);

        foreach (var flower in flowers)
        {
            if (flower.renderer != null)
            {
                flower.renderer.SetPropertyBlock(propertyBlock);
            }
        }
    }



    public void RegisterFlower(MeshFilter flowerMeshFilter)
    {
        Renderer flowerRenderer = flowerMeshFilter.GetComponent<Renderer>();
        if (flowerRenderer == null)
        {
            Debug.LogWarning("Flower has no renderer: " + flowerMeshFilter.gameObject.name);
            return;
        }

        var flowerData = new FlowerData
        {
            meshFilter = flowerMeshFilter,
            renderer = flowerRenderer,
            originalVertices = flowerMeshFilter.mesh.vertices,
        };
        flowers.Add(flowerData);
    }

    private class FlowerData
    {
        public Renderer renderer;
        public MeshFilter meshFilter;
        public Vector3[] originalVertices;
        public float maxHeight;
    }
}
