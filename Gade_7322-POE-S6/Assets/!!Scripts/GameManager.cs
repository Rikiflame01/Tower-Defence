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

    private GameState currentState;

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
                break;
            case GameState.Cooldown:
                // Handle Cooldown mode
                break;
            case GameState.Placement:
                // Handle Placement mode
                break;
            case GameState.Upgrade:
                // Handle Upgrade mode
                break;
            case GameState.Wave:
                // Handle Wave mode
                break;
            case GameState.Pause:
                // Handle Pause mode
                break;
            case GameState.GameOver:
                // Handle Game Over mode
                break;
            case GameState.Victory:
                // Handle Victory mode
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }

        Debug.Log("Game state changed to: " + state);
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
