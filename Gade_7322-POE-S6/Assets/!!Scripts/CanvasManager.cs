/*
    The CanvasManager class controls game canvases based on different game modes.
    - Manages canvases for Tutorial, Cooldown, Placement, Upgrade, Wave, Pause, GameOver, and Victory.
    - Uses a singleton pattern for instance management.
    - Sets active canvas according to the current game state.
    - Shows or hides canvases with optional delays.
    - Toggles game pause state with delay, affecting time scale and resuming previous state.
    - Listens to events from EventManager to update canvases.
*/

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager instance;

    public GameObject[] canvases;

    private GameObject tutorialCanvas;
    public GameObject cooldownCanvas;
    public GameObject placementCanvas;
    public GameObject upgradeCanvas;
    public GameObject waveCanvas;
    private GameObject pauseCanvas;
    private GameObject gameOverCanvas;
    private GameObject victoryCanvas;

    private bool isPaused;

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
            return;
        }

        pauseCanvas = GameObject.Find("Canv_Pause");
        gameOverCanvas = GameObject.Find("Canv_Lose");
        victoryCanvas = GameObject.Find("Canv_Win");
        tutorialCanvas = GameObject.Find("Canv_Tutorial");


        if (pauseCanvas == null) Debug.LogWarning("Pause Canvas not found!");
        if (gameOverCanvas == null) Debug.LogWarning("Game Over Canvas not found!");
        if (victoryCanvas == null) Debug.LogWarning("Victory Canvas not found!");
        if (tutorialCanvas == null) Debug.LogWarning("Tutorial Canvas not found!");
    }

    private void OnEnable()
    {
        if (EventManager.instance != null)
        {
            EventManager.instance.onTutorialMode?.AddListener(() => SetGameMode(GameMode.Tutorial));
            EventManager.instance.onCooldownMode?.AddListener(() => SetGameMode(GameMode.Cooldown));
            EventManager.instance.onPlacementMode?.AddListener(() => SetGameMode(GameMode.Placement));
            EventManager.instance.onUpgradeMode?.AddListener(() => SetGameMode(GameMode.Upgrade));
            EventManager.instance.onWaveMode?.AddListener(() => SetGameMode(GameMode.Wave));
            EventManager.instance.onPauseMode?.AddListener(() => SetGameMode(GameMode.Pause));
            EventManager.instance.onGameOverMode?.AddListener(() => SetGameMode(GameMode.GameOver));
            EventManager.instance.onVictoryMode?.AddListener(() => SetGameMode(GameMode.Victory));
        }
        else if (Debug.isDebugBuild)
        {
            Debug.LogWarning("EventManager instance not found!");
        }
    }

    private void OnDisable()
    {
        if (EventManager.instance != null)
        {
            EventManager.instance.onTutorialMode?.RemoveListener(() => SetGameMode(GameMode.Tutorial));
            EventManager.instance.onCooldownMode?.RemoveListener(() => SetGameMode(GameMode.Cooldown));
            EventManager.instance.onPlacementMode?.RemoveListener(() => SetGameMode(GameMode.Placement));
            EventManager.instance.onUpgradeMode?.RemoveListener(() => SetGameMode(GameMode.Upgrade));
            EventManager.instance.onWaveMode?.RemoveListener(() => SetGameMode(GameMode.Wave));
            EventManager.instance.onPauseMode?.RemoveListener(() => SetGameMode(GameMode.Pause));
            EventManager.instance.onGameOverMode?.RemoveListener(() => SetGameMode(GameMode.GameOver));
            EventManager.instance.onVictoryMode?.RemoveListener(() => SetGameMode(GameMode.Victory));
        }
    }

    public void SetGameMode(GameMode mode)
    {
        if (GameManager.instance == null)
        {
            if (Debug.isDebugBuild)
            {
                Debug.LogWarning("GameManager instance not found!"); 
            }
            return;
        }

        switch (mode)
        {
            case GameMode.Tutorial:
                if (GameManager.instance.currentState == GameManager.GameState.Tutorial)
                    StartCoroutine(ShowCanvasWithDelay(tutorialCanvas, 0.5f));
                else
                    Debug.LogWarning("GameMode.Tutorial should not be executing in this state: " + mode);
                break;
            case GameMode.Cooldown:
                if (GameManager.instance.currentState == GameManager.GameState.Cooldown)
                    ShowCanvas(cooldownCanvas);
                else
                    Debug.LogWarning("GameMode.Cooldown should not be executing in this state: " + mode);
                break;
            case GameMode.Placement:
                if (GameManager.instance.currentState == GameManager.GameState.Placement)
                    ShowCanvas(placementCanvas);
                else
                    Debug.LogWarning("GameMode.Placement should not be executing in this state: " + mode);
                break;
            case GameMode.Upgrade:
                if (GameManager.instance.currentState == GameManager.GameState.Upgrade)
                    ShowCanvas(upgradeCanvas);
                else
                    Debug.LogWarning("GameMode.Upgrade should not be executing in this state: " + mode);
                break;
            case GameMode.Wave:
                if (GameManager.instance.currentState == GameManager.GameState.Wave)
                    ShowCanvas(waveCanvas);
                else
                    Debug.LogWarning("GameMode.Wave should not be executing in this state: " + mode);
                break;
            case GameMode.Pause:
                PauseToggle();
                break;
            case GameMode.GameOver:
                if (GameManager.instance.currentState == GameManager.GameState.GameOver)
                    ShowCanvas(gameOverCanvas);
                else
                    Debug.LogWarning("GameMode.GameOver should not be executing in this state: " + mode);
                break;
            case GameMode.Victory:
                if (GameManager.instance.currentState == GameManager.GameState.Victory)
                    ShowCanvas(victoryCanvas);
                else
                    Debug.LogWarning("GameMode.Victory should not be executing in this state: " + mode);
                break;
            default:
                Debug.LogWarning("Unhandled game mode: " + mode);
                break;
        }
    }

    private void ShowCanvas(GameObject canvas)
    {
        if (canvas == null)
        {
            Debug.LogWarning("Attempted to show a null canvas.");
            return;
        }

        foreach (GameObject canvasObject in canvases)
        {
            if (canvasObject != null)
            {
                canvasObject.SetActive(false);
            }
        }

        canvas.SetActive(true);
    }

    private IEnumerator ShowCanvasWithDelay(GameObject canvas, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        ShowCanvas(canvas);
        Debug.Log("Showing canvas: " + canvas.name);
    }

    public void HideCanvases()
    {
        foreach (GameObject canvas in canvases)
        {
            if (canvas != null)
            {
                canvas.SetActive(false);
            }
        }
    }

    private void PauseToggle()
    {
        if (isPaused)
        {
            StartCoroutine(UnpauseAfterDelay(0.1f));
        }
        else
        {
            StartCoroutine(PauseAfterDelay(0.1f));
        }
    }

    private IEnumerator PauseAfterDelay(float delay)
    {
        if (pauseCanvas != null)
        {
            ShowCanvas(pauseCanvas);
        }
        else
        {
            Debug.LogWarning("Pause canvas is null, cannot show pause screen.");
        }

        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 0;
        isPaused = true;
    }

    private IEnumerator UnpauseAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1;
        HideCanvases();

        if (GameManager.instance != null)
        {
            GameManager.instance.ResumePreviousState();
            Debug.Log("Resuming previous state: " + GameManager.instance.GetCurrentState().ToString());
        }
        else
        {
            Debug.LogWarning("GameManager instance is null, cannot resume previous state.");
        }

        isPaused = false;
    }
}
