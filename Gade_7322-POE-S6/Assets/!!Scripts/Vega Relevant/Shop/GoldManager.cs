/*
    The GoldManager class manages the player's gold, including tracking the current amount and handling transactions.

    - Fields:
      - instance: Singleton instance of the GoldManager.
      - currentGold: The amount of gold currently held by the player.

    - Methods:
      - Awake(): Initializes the singleton instance of GoldManager. Ensures only one instance persists across scenes.
      - HasEnoughGold(int amount): Checks if the player has enough gold for a given amount. Returns true if the currentGold is greater than or equal to the specified amount.
      - SpendGold(int amount): Deducts a specified amount of gold if the player has enough. Triggers the `onGoldSpend` event if successful.
      - AddGold(int amount): Increases the currentGold by a specified amount. Triggers the `onAddGold` event.
*/

using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager instance;

    public int currentGold;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool HasEnoughGold(int amount)
    {
        return currentGold >= amount;
    }

    public void SpendGold(int amount)
    {
        if (HasEnoughGold(amount))
        {
            currentGold -= amount;

            EventManager.instance.onGoldSpend.Invoke(amount);
        }
    }
    public void AddGold(int amount)
    {
        currentGold += amount;

        EventManager.instance.onAddGold.Invoke(amount);
    }

}
