/*
    The GoldTextCanvas class updates and displays the player's gold amount on the UI using TextMeshProUGUI. It listens to events for adding and spending gold to keep the display current.

    - Fields:
      - goldText: The TextMeshProUGUI component displaying the gold amount.

    - Methods:
      - Awake(): Initializes the gold text to 100 and sets up listeners for gold addition and spending events.
      - OnDisable(): Removes the event listeners to prevent memory leaks.
      - UpdateGoldText(int amount): Updates the displayed gold amount based on the current gold value from GoldManager.
*/


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
