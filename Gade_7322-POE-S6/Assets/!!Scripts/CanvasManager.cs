using UnityEngine;
using UnityEngine.Events;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager instance;

    public GameObject[] canvases;

    public GameObject tutorialCanvas;
    public GameObject cooldownCanvas;
    public GameObject placementCanvas;
    public GameObject upgradeCanvas;
    public GameObject waveCanvas;
    public GameObject pauseCanvas;
    public GameObject gameOverCanvas;
    public GameObject victoryCanvas;

    public enum GameMode
    {
        Tutorial,
        Cooldown,
        Placement,
        Upgrade,
        Wave,
        Pause,
        GameOver,
        Victory
    }

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
        EventManager.instance.onTutorialMode.AddListener(() => SetGameMode(GameMode.Tutorial));
        EventManager.instance.onCooldownMode.AddListener(() => SetGameMode(GameMode.Cooldown));
        EventManager.instance.onPlacementMode.AddListener(() => SetGameMode(GameMode.Placement));
        EventManager.instance.onUpgradeMode.AddListener(() => SetGameMode(GameMode.Upgrade));
        EventManager.instance.onWaveMode.AddListener(() => SetGameMode(GameMode.Wave));
        EventManager.instance.onPauseMode.AddListener(() => SetGameMode(GameMode.Pause));
        EventManager.instance.onGameOverMode.AddListener(() => SetGameMode(GameMode.GameOver));
        EventManager.instance.onVictoryMode.AddListener(() => SetGameMode(GameMode.Victory));
    }

    private void OnDisable()
    {
        EventManager.instance.onTutorialMode.RemoveListener(() => SetGameMode(GameMode.Tutorial));
        EventManager.instance.onCooldownMode.RemoveListener(() => SetGameMode(GameMode.Cooldown));
        EventManager.instance.onPlacementMode.RemoveListener(() => SetGameMode(GameMode.Placement));
        EventManager.instance.onUpgradeMode.RemoveListener(() => SetGameMode(GameMode.Upgrade));
        EventManager.instance.onWaveMode.RemoveListener(() => SetGameMode(GameMode.Wave));
        EventManager.instance.onPauseMode.RemoveListener(() => SetGameMode(GameMode.Pause));
        EventManager.instance.onGameOverMode.RemoveListener(() => SetGameMode(GameMode.GameOver));
        EventManager.instance.onVictoryMode.RemoveListener(() => SetGameMode(GameMode.Victory));
    }

    private void SetGameMode(GameMode mode)
    {
        switch (mode)
        {
            case GameMode.Tutorial:
                if (GameManager.instance.currentState == GameManager.GameState.Tutorial) { ShowCanvas(tutorialCanvas);}
                else { Debug.Log("This should not be executing: " + mode); }
                break;
            case GameMode.Cooldown:
                if (GameManager.instance.currentState == GameManager.GameState.Cooldown) { ShowCanvas(cooldownCanvas); }
                else { Debug.Log("This should not be executing: " + mode); }
                break;
            case GameMode.Placement:
                if (GameManager.instance.currentState == GameManager.GameState.Placement) { ShowCanvas(placementCanvas); }
                else { Debug.Log("This should not be executing: " + mode); }
                break;
            case GameMode.Upgrade:
                if (GameManager.instance.currentState == GameManager.GameState.Upgrade) { ShowCanvas(upgradeCanvas); }
                else { Debug.Log("This should not be executing: " + mode); }
                break;
            case GameMode.Wave:
                if (GameManager.instance.currentState == GameManager.GameState.Wave) { ShowCanvas(waveCanvas); }
                else { Debug.Log("This should not be executing: " + mode); }
                break;
            case GameMode.Pause:
                if (GameManager.instance.currentState == GameManager.GameState.Pause) { ShowCanvas(pauseCanvas); }
                else { Debug.Log("This should not be executing: " + mode); }
                break;
            case GameMode.GameOver:
                if (GameManager.instance.currentState == GameManager.GameState.GameOver) { ShowCanvas(gameOverCanvas); }
                else { Debug.Log("This should not be executing: " + mode); }
                break;
            case GameMode.Victory:
                if (GameManager.instance.currentState == GameManager.GameState.Victory) { ShowCanvas(victoryCanvas); }
                else { Debug.Log("This should not be executing: " + mode); }
                break;
        }
    }

    private void ShowCanvas(GameObject canvas)
    {
        if (canvas == null)
        {
            Debug.LogError("Canvas is not assigned." + canvas.ToString());
            return;
        }
        foreach (GameObject canvasObject in canvases)
        {
            canvasObject.SetActive(false);
        }

        canvas.SetActive(true);
    }
    private void HideCanvases()
    {
        foreach (GameObject canvas in canvases)
        {
            canvas.SetActive(false);
        }
    }
}
