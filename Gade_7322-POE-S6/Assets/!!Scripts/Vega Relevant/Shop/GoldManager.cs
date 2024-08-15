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
        }
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
    }

}
