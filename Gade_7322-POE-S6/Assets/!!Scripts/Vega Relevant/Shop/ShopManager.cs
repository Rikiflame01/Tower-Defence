/*
    The ShopManager class manages in-game shop functionalities, including purchasing items, upgrading buildings and projectiles, and healing objects.

    - Fields:
      - townHallLevelData: Data for town hall levels.
      - townHallProjectileData: Data for projectile upgrades.
      - shieldDefenderCost: Cost of the shield defender.
      - shieldDefenderPrefab: Prefab for the shield defender.
      - InitialChoicePanel, ShopOptionsPanel, TowerUpgradeOptionsPanel: Panels for different shop views.
      - townHallLevel: Current level of the town hall.
      - healingCosts: Dictionary containing healing costs for different object types.

    - Methods:
      - Awake(): Ensures there is only one instance of ShopManager.
      - OnEnable()/OnDisable(): Subscribes and unsubscribes to button click events.
      - Start(): Finds and assigns panel GameObjects.
      - PurchaseShieldDefender(): Initiates the purchase process for the shield defender.
      - UpgradeTownHall(): Upgrades the town hall and updates its level.
      - UpgradeProjectile(): Upgrades the projectile and updates its level.
      - HealObjectsWithTags(): Heals objects with specific tags if enough gold is available.
      - HandleButtonClicked(): Handles button click events and triggers corresponding actions.
      - GetUpgradeCost()/GetProjectileUpgradeCost(): Calculates the cost for upgrading the town hall and projectile.
      - OpenPanel()/ClosePanel(): Manages the visibility of shop panels.
*/


using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public TownHallLevelSO townHallLevelData;
    public TownHallProjectileSO townHallProjectileData;
    public static ShopManager instance;

    public int shieldDefenderCost = 100;
    public int burstDefenderCost = 300;
    public int catapultDefenderCost = 1000;

    public GameObject shieldDefenderPrefab;
    public GameObject burstDefenderPrefab;
    public GameObject catapultDefenderPrefab;

    private GameObject InitialChoicePanel;
    private GameObject ShopOptionsPanel;
    private GameObject TowerUpgradeOptionsPanel;
    private GameObject CoinPusherPanel;

    private int townHallLevel = 0;
    private Dictionary<string, int> healingCosts = new Dictionary<string, int>
    {
        { "Ally", 50 },
        { "TownHall", 500 },
        { "ShieldDefender", 1000},
        { "BurstDefender", 1000},
        { "CatapultDefender", 1000 }
    };

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

    private void Start()
    {
        InitialChoicePanel = GameObject.Find("InitialChoice");
        ShopOptionsPanel = GameObject.Find("ShopOptions");
        TowerUpgradeOptionsPanel = GameObject.Find("TowerUpgradeOptions");
        CoinPusherPanel = GameObject.Find("CoinPusherPanel");
    }

    public void PurchaseShieldDefender()
    {
        ConfirmationManager.instance.ShowConfirmation(shieldDefenderCost, () =>
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
        });
    }

    public void PurchaseBurstDefender()
    {
        ConfirmationManager.instance.ShowConfirmation(burstDefenderCost, () =>
        {
            if (GoldManager.instance.HasEnoughGold(burstDefenderCost))
            {
                GoldManager.instance.SpendGold(burstDefenderCost);
                PrefabPlacementManager.instance.BeginPlacement(burstDefenderPrefab);
            }
            else
            {
                Debug.Log("Not enough gold to purchase Burst Defender.");
            }
        });
    }

    public void PurchaseCatapultDefender()
    {
        ConfirmationManager.instance.ShowConfirmation(catapultDefenderCost, () =>
        {
            if (GoldManager.instance.HasEnoughGold(catapultDefenderCost))
            {
                GoldManager.instance.SpendGold(catapultDefenderCost);
                PrefabPlacementManager.instance.BeginPlacement(catapultDefenderPrefab);
            }
            else
            {
                Debug.Log("Not enough gold to purchase Catapult Defender.");
            }
        });
    }

    public void UpgradeTownHall()
    {
        int cost = GetUpgradeCost();
        ConfirmationManager.instance.ShowConfirmation(cost, () =>
        {
            if (GoldManager.instance.HasEnoughGold(cost))
            {
                GoldManager.instance.SpendGold(cost);
                townHallLevel++;
                townHallLevelData.ConfigureLevel(townHallLevel);
                Debug.Log($"Town Hall upgraded to level {townHallLevel}. New cost: {GetUpgradeCost()}");
            }
            else
            {
                Debug.Log("Not enough gold to upgrade Town Hall.");
            }
        });
    }

    public void UpgradeProjectile()
    {
        int cost = GetProjectileUpgradeCost();
        ConfirmationManager.instance.ShowConfirmation(cost, () =>
        {
            if (GoldManager.instance.HasEnoughGold(cost))
            {
                GoldManager.instance.SpendGold(cost);
                townHallProjectileData.level++;
                Debug.Log($"Projectile upgraded to level {townHallProjectileData.level}. New cost: {GetProjectileUpgradeCost()}");
            }
            else
            {
                Debug.Log("Not enough gold to upgrade projectile.");
            }
        });
    }

    public void HealObjectsWithTags(List<string> tags)
    {
        int totalCost = 0;
        foreach (string tag in tags)
        {
            if (healingCosts.TryGetValue(tag, out int cost))
            {
                totalCost += cost;
            }
        }

        ConfirmationManager.instance.ShowConfirmation(totalCost, () =>
        {
            foreach (string tag in tags)
            {
                if (healingCosts.TryGetValue(tag, out int cost))
                {
                    if (GoldManager.instance.HasEnoughGold(cost))
                    {
                        GoldManager.instance.SpendGold(cost);
                        var objectsToHeal = GameObject.FindGameObjectsWithTag(tag);
                        foreach (var obj in objectsToHeal)
                        {
                            var health = obj.GetComponent<IHealth>();
                            if (health != null)
                            {
                                health.Heal();
                            }
                        }
                        Debug.Log($"{tag} objects healed for {cost} gold.");
                    }
                    else
                    {
                        Debug.Log($"Not enough gold to heal {tag} objects.");
                    }
                }
                else
                {
                    Debug.LogWarning($"No healing cost defined for tag: {tag}");
                }
            }
        });
    }

    public void HandleButtonClicked(string buttonName)
    {
        switch (buttonName)
        {
            case "ShieldBuyBttn":
                PurchaseShieldDefender();
                SoundManager.Instance.PlaySFX("Page Turn 2");
                break;
            case "BurstBuyBttn":
                PurchaseBurstDefender();
                SoundManager.Instance.PlaySFX("Page Turn 2");
                break;
            case "CatapultBuyBttn":
                PurchaseCatapultDefender();
                SoundManager.Instance.PlaySFX("Page Turn 2");
                break;
            case "UpgradeTownHallBttn":
                UpgradeTownHall();
                SoundManager.Instance.PlaySFX("Page Turn 2");
                break;
            case "UpgradeProjectileBttn":
                UpgradeProjectile();
                SoundManager.Instance.PlaySFX("Page Turn 2");
                break;
            case "HealAllyBttn":
                HealObjectsWithTags(new List<string> { "TownHall", "ShieldDefender", "Defender" });
                SoundManager.Instance.PlaySFX("Page Turn 2");
                break;
            case "ShopPanelBttn":
                ClosePanel(InitialChoicePanel);
                OpenPanel(ShopOptionsPanel);
                SoundManager.Instance.PlaySFX("Page Turn 2");
                break;
            case "TUpgradesBttn":
                ClosePanel(InitialChoicePanel);
                OpenPanel(TowerUpgradeOptionsPanel);
                SoundManager.Instance.PlaySFX("Page Turn 2");
                break;
            case "CoinPusherBttn":
                CoinPusherPanel.SetActive(true);
                EventManager.instance.TriggerCoinPusherBttn();
                SoundManager.Instance.PlaySFX("Page Turn 2");
                break;
            case "DropCoinBttn":
                EventManager.instance.TriggerDropCoin();
                break;
            case "TransitionBack":
                EventManager.instance.TriggerTransitionBack();
                SoundManager.Instance.PlaySFX("Page Turn 2");
                CoinPusherPanel.SetActive(false);
                break;
            default:
                Debug.LogWarning($"Unhandled button clicked: {buttonName}");
                break;
        }
    }

    private int GetUpgradeCost()
    {
        switch (townHallLevel)
        {
            case 0: return 800;
            case 1: return 1600;
            case 2: return 3000;
            case 3: return 6000;
            default: return 10000;
        }
    }

    private int GetProjectileUpgradeCost()
    {
        switch (townHallProjectileData.level)
        {
            case 0: return 600;
            case 1: return 1200;
            case 2: return 2000;
            case 3: return 3000;
            case 4: return 4500;
            default: return 10000;
        }
    }

    public void OpenPanel(GameObject panel)
    {
        if (!panel.activeSelf)
        {
            panel.SetActive(true);
        }
    }

    public void ClosePanel(GameObject panel)
    {
        if (panel.activeSelf)
        {
            panel.SetActive(false);
        }
    }

}
