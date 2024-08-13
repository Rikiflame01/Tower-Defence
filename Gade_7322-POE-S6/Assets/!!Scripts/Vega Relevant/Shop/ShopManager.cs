using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ShopManager : MonoBehaviour
{
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
}

[CustomEditor(typeof(GoldManager))]
public class GoldManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GoldManager goldManager = (GoldManager)target;

        GUILayout.Space(10);
        GUILayout.Label("Testing Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Add 100 Gold"))
        {
            goldManager.AddGold(100);
            Debug.Log("Added 100 gold. Current gold: " + goldManager.currentGold);
        }

        if (GUILayout.Button("Add 1000 Gold"))
        {
            goldManager.AddGold(1000);
            Debug.Log("Added 1000 gold. Current gold: " + goldManager.currentGold);
        }
    }
}
[CustomEditor(typeof(ShopManager))]
public class ShopManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ShopManager shopManager = (ShopManager)target;

        GUILayout.Space(10);
        GUILayout.Label("Testing Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Purchase Shield Defender"))
        {
            shopManager.PurchaseShieldDefender();
        }
    }
}
