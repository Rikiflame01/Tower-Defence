using UnityEngine;

public class GoldDropper : MonoBehaviour
{
    [SerializeField] private GameObject goldPrefab;
    private RarityHandler rarityHandler;

    private void Awake()
    {
        rarityHandler = GetComponent<RarityHandler>();
        if (rarityHandler == null)
        {
            Debug.LogError("RarityHandler not found on " + gameObject.name);
        }
    }

    public void DropGold(int dropCount)
    {
        if (goldPrefab != null && rarityHandler != null)
        {
            int goldAmount = CalculateGoldAmount();

            for (int i = 0; i < dropCount; i++)
            {
                GameObject goldInstance = Instantiate(goldPrefab, transform.position, Quaternion.identity);
                Gold gold = goldInstance.GetComponent<Gold>();
                if (gold != null)
                {
                    gold.Initialize(goldAmount);
                }
                else
                {
                    Debug.LogError("No Gold script found on the spawned gold object.");
                }
            }
        }
        else
        {
            Debug.LogError("GoldPrefab or RarityHandler is missing.");
        }
    }

    private int CalculateGoldAmount()
    {
        int baseGold = 15;
        switch (rarityHandler.GetRarity())
        {
            case RarityHandler.RarityType.Normal:
                return baseGold;
            case RarityHandler.RarityType.Empowered:
                return baseGold * 2;
            case RarityHandler.RarityType.Mythic:
                return baseGold * 3;
            case RarityHandler.RarityType.Legendary:
                return baseGold * 5;
            case RarityHandler.RarityType.Godlike:
                return baseGold * 10;
            default:
                return baseGold;
        }
    }
}
