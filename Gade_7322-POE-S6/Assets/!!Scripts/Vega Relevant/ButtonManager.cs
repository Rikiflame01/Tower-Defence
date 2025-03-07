/*
    The ButtonManager class handles UI button interactions and panel management.
    - Manages singleton instance and ensures necessary components are present.
    - Adds click listeners to all buttons and handles their interactions based on game state.
    - Provides methods to open, close, and manage UI panels.
    - Toggles specific canvases and handles scene transitions, including cleaning up previous instances and unloading resources.
    - Includes functionality to exit the application and load new scenes while managing object destruction.
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public static ButtonManager instance;

    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;

    private GameObject shopCanvas;
    private GameObject InitialChoicePanel;
    private GameObject ShopOptionsPanel;
    private GameObject TowerUpgradeOptionsPanel;
    private GameObject CoinPusherPanel;

    private List<GameObject> openPanels = new List<GameObject>();

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
        if (CanvasManager.instance != null)
        {
            CanvasManager.instance.HideCanvases();
        }

    }

    private void Start()
    {
        shopCanvas = GameObject.Find("ShopCanvas");
        InitialChoicePanel = GameObject.Find("InitialChoice");
        ShopOptionsPanel = GameObject.Find("ShopOptions");
        TowerUpgradeOptionsPanel = GameObject.Find("TowerUpgradeOptions");
        CoinPusherPanel = GameObject.Find("CoinPusherPanel");

        Button[] allButtons = FindObjectsOfType<Button>();

        foreach (Button button in allButtons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button));
        }

        CoinPusherPanel.SetActive(false);
        ShopOptionsPanel.SetActive(false);
        TowerUpgradeOptionsPanel.SetActive(false);

        eventSystem = FindObjectOfType<EventSystem>();

        if (eventSystem == null)
        {
            if (Debug.isDebugBuild)
            {
                Debug.LogError("ButtonManager: No EventSystem found in the scene."); 
            }
            return;
        }

        raycaster = FindObjectOfType<GraphicRaycaster>();

        if (raycaster == null)
        {
            if (Debug.isDebugBuild)
            {
                Debug.LogError("ButtonManager: No GraphicRaycaster found in the scene."); 
            }
            return;
        }


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleSpecificCanvas();
        }
    }

    private void OnButtonClicked(Button button)
    {
        if (GameManager.instance.GetCurrentState() == GameManager.GameState.Cooldown ||
            GameManager.instance.GetCurrentState() == GameManager.GameState.Tutorial ||
            GameManager.instance.GetCurrentState() == GameManager.GameState.Upgrade)
        {
            if (IsPointerOnUIButton())
            {
                EventManager.instance.TriggerButtonClicked(button.name);
            }
        }
        else if (Debug.isDebugBuild)
        {
            Debug.Log("Button clicks are disabled unless in Cooldown state.");
        }
    }

    private bool IsPointerOnUIButton()
    {
        PointerEventData pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.GetComponent<Button>() != null)
            {
                return true;
            }
        }

        return false;
    }

    public void OpenPanel(GameObject panel)
    {
        if (!panel.activeSelf)
        {
            panel.SetActive(true);
            openPanels.Add(panel);
        }
    }

    public void ClosePanel(GameObject panel)
    {
        if (panel.activeSelf)
        {
            panel.SetActive(false);
            openPanels.Remove(panel);
        }
    }

    public void CloseAllPanels()
    {
        foreach (GameObject panel in openPanels)
        {
            panel.SetActive(false);
        }
        openPanels.Clear();
    }

    private void CloseCoinPusherPanel()
    {
        if (CoinPusherPanel != null)
        {
            CoinPusherPanel.SetActive(false);
        }
        else if (Debug.isDebugBuild)
        {
            Debug.LogError("ButtonManager: Coin Pusher panel is not assigned.");
        }
    }

    private void ToggleSpecificCanvas()
    {
        if (shopCanvas != null)
        {
            bool isActive = shopCanvas.activeSelf;
            shopCanvas.SetActive(!isActive);
            InitialChoicePanel.SetActive(true);
            ShopOptionsPanel.SetActive(false);
            TowerUpgradeOptionsPanel.SetActive(false);
        }
        else if (Debug.isDebugBuild)
        {
            Debug.LogError("ButtonManager: Specific canvas is not assigned.");
        }
    }
    public void ExitApplication()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }

    public void LoadScene(string sceneName)
    {
        GameObject camera = GameObject.Find("Camera");
        Destroy(camera);

        if (sceneName == null)
        {
            if (Debug.isDebugBuild)
            {
                Debug.LogError("ButtonManager: Scene name is not assigned."); 
            }
            return;
        }

        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.scene.name == "DontDestroyOnLoad")
            {
                Destroy(obj);
            }
        }

        if (CanvasManager.instance != null)
        {
            Destroy(CanvasManager.instance.gameObject);
            CanvasManager.instance = null;
        }

        if (GameManager.instance != null)
        {
            Destroy(GameManager.instance.gameObject);
            GameManager.instance = null;
        }

        SceneManager.LoadScene(sceneName);

    }
}
