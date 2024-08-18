using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldTextCanvas : MonoBehaviour
{
    public TextMeshProUGUI goldText;

    private void Awake()
    {
        UpdateGoldText(100);
        EventManager.instance.onAddGold.AddListener(UpdateGoldText);
        EventManager.instance.onGoldSpend.AddListener(UpdateGoldText);
    }
    private void OnDisable()
    {
        EventManager.instance.onAddGold.RemoveListener(UpdateGoldText);
        EventManager.instance.onGoldSpend.RemoveListener(UpdateGoldText);
    }

    private void UpdateGoldText(int amount)
    {
        goldText.text = GoldManager.instance.currentGold.ToString();
    }
}
