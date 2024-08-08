using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState
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

    public GameState currentState;

    public Texture2D customCursorTexture;
    public Vector2 cursorHotspot = Vector2.zero;

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

    private void Start()
    {
        SetCustomCursor();
        SwitchState(GameState.Tutorial);
    }

    public void SwitchState(GameState newState)
    {
        currentState = newState;
        HandleStateChange(newState);
    }

    private void HandleStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.Tutorial:
                // Handle Tutorial mode
                Debug.Log("GameState set to: " + state + " Actual case executed: " + GameState.Tutorial.ToString());
                break;
            case GameState.Cooldown:
                // Handle Cooldown mode
                Debug.Log("GameState set to: " + state + " Actual case executed: " + GameState.Cooldown.ToString());
                break;
            case GameState.Placement:
                // Handle Placement mode
                Debug.Log("GameState set to: " + state + " Actual case executed: " + GameState.Placement.ToString());
                break;
            case GameState.Upgrade:
                // Handle Upgrade mode
                Debug.Log("GameState set to: " + state + " Actual case executed: " + GameState.Upgrade.ToString());
                break;
            case GameState.Wave:
                // Handle Wave mode
                Debug.Log("GameState set to: " + state + " Actual case executed: " + GameState.Wave.ToString());
                break;
            case GameState.Pause:
                // Handle Pause mode
                Debug.Log("GameState set to: " + state + " Actual case executed: " + GameState.Pause.ToString());
                break;
            case GameState.GameOver:
                // Handle Game Over mode
                Debug.Log("GameState set to: " + state + " Actual case executed: " + GameState.GameOver.ToString());
                break;
            case GameState.Victory:
                // Handle Victory mode
                Debug.Log("GameState set to: " + state + " Actual case executed: " + GameState.Victory.ToString());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public GameState GetCurrentState()
    {
        return currentState;
    }

    private void SetCustomCursor()
    {
        if (customCursorTexture != null)
        {
            Cursor.SetCursor(customCursorTexture, cursorHotspot, CursorMode.Auto);
        }
    }
}
