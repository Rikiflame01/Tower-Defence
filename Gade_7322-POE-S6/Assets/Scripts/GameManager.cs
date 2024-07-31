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
        SwitchState(GameState.Tutorial); //Initial state

        //Listens to the following events
        EventManager.instance.onTutorialMode.AddListener(() => SwitchState(GameState.Tutorial));
        EventManager.instance.onCooldownMode.AddListener(() => SwitchState(GameState.Cooldown));
        EventManager.instance.onPlacementMode.AddListener(() => SwitchState(GameState.Placement));
        EventManager.instance.onUpgradeMode.AddListener(() => SwitchState(GameState.Upgrade));
        EventManager.instance.onWaveMode.AddListener(() => SwitchState(GameState.Wave));
        EventManager.instance.onPauseMode.AddListener(() => SwitchState(GameState.Pause));
        EventManager.instance.onGameOverMode.AddListener(() => SwitchState(GameState.GameOver));
        EventManager.instance.onVictoryMode.AddListener(() => SwitchState(GameState.Victory));
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
}
