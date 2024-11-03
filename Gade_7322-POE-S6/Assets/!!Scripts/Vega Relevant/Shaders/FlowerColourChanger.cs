using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlowerColourChanger : MonoBehaviour
{
    public Color[] possibleColors;
    private List<FlowerData> flowerDataList = new List<FlowerData>();
    private float changeInterval = 120f;
    private float lastChangeTime;

    void Start()
    {
        StartCoroutine(InitializeFlowersWithDelay(2f));
    }

    private IEnumerator InitializeFlowersWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] flowers = GameObject.FindGameObjectsWithTag("Flower");
        Debug.Log("Number of flowers found after delay: " + flowers.Length);

        foreach (GameObject flower in flowers)
        {
            Renderer rend = flower.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                rend.material = new Material(rend.material);
                flowerDataList.Add(new FlowerData(rend, rend.material.color));
            }
            else
            {
                Debug.LogWarning("No Renderer found in " + flower.name + " or its children.");
            }
        }

        lastChangeTime = Time.time;
    }

    void Update()
    {
        if (Time.time - lastChangeTime >= changeInterval)
        {
            foreach (FlowerData flowerData in flowerDataList)
            {
                if (Random.value < 0.1f)
                {
                    Color newTargetColor = possibleColors[Random.Range(0, possibleColors.Length)];
                    flowerData.StartLerp(newTargetColor, 2f);
                }
            }
            lastChangeTime = Time.time;
        }

        foreach (FlowerData flowerData in flowerDataList)
        {
            flowerData.UpdateLerp();
        }
    }

    private class FlowerData
    {
        public Renderer Renderer { get; private set; }
        private Color startColor;
        private Color targetColor;
        private float lerpDuration;
        private float lerpStartTime;
        private bool isLerping;

        public FlowerData(Renderer renderer, Color initialColor)
        {
            Renderer = renderer;
            startColor = initialColor;
            targetColor = initialColor;
            Renderer.material.SetColor("_Color", initialColor);
        }

        public void StartLerp(Color newTargetColor, float duration)
        {
            startColor = Renderer.material.GetColor("_Color");
            targetColor = newTargetColor;
            lerpDuration = duration;
            lerpStartTime = Time.time;
            isLerping = true;
        }

        public void UpdateLerp()
        {
            if (isLerping)
            {
                float lerpProgress = (Time.time - lerpStartTime) / lerpDuration;
                if (lerpProgress >= 1f)
                {
                    lerpProgress = 1f;
                    isLerping = false;
                }
                Color currentColor = Color.Lerp(startColor, targetColor, lerpProgress);
                Renderer.material.SetColor("_Color", currentColor);
            }
        }
    }
}
