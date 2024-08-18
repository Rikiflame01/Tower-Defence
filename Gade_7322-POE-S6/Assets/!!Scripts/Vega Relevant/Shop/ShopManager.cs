using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ShopManager : MonoBehaviour
{
    public TownHallLevelSO townHallLevelData;

    public static ShopManager instance;

    public int shieldDefenderCost = 100;
    public GameObject shieldDefenderPrefab;

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

    private void OnEnable()
    {
        EventManager.instance.onButtonClicked.AddListener(HandleButtonClicked);
    }

    private void OnDisable()
    {
        EventManager.instance.onButtonClicked.RemoveListener(HandleButtonClicked);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            PurchaseShieldDefender();
        }
    }

    public void PurchaseShieldDefender()
    {
        if (GameManager.instance.GetCurrentState() == GameManager.GameState.Cooldown)
        {
            if (GoldManager.instance.HasEnoughGold(shieldDefenderCost))
            {
                GoldManager.instance.SpendGold(shieldDefenderCost);
                PrefabPlacementManager.instance.BeginPlacement(shieldDefenderPrefab);
            }
            else
            {
                Debug.Log("Not enough gold to purchase Shield Defender.");
            }
        }
        else
        {
            Debug.Log("Cannot purchase items outside of Cooldown state.");
        }
    }

    private void HandleButtonClicked(string buttonName)
    {
        if (buttonName == "ShieldBuyBttn")
        {
            Debug.Log("Shield BuyItemButton was clicked!");
            PurchaseShieldDefender();
        }
        else if (buttonName == "SellItemButton")
        {
            Debug.Log("SellItemButton was clicked!");
        }
    }
}
