/*
    The EventManager class manages various UnityEvents for game state changes and interactions.
    - Implements a singleton pattern for global access.
    - Defines events for game modes: Tutorial, Cooldown, Placement, Upgrade, Wave, Pause, GameOver, and Victory.
    - Includes events for gold transactions, enemy spawning, defender placement, and button clicks.
    - Initializes UnityEvents in the Start method if they are null.
    - Provides methods to trigger these events and switch game states using GameManager.
*/

using System;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    #region GameMode Events
    public UnityEvent onTutorialMode;
    public UnityEvent onCooldownMode;
    public UnityEvent onPlacementMode;
    public UnityEvent onUpgradeMode;
    public UnityEvent onWaveMode;
    public UnityEvent onPauseMode;
    public UnityEvent onGameOverMode;
    public UnityEvent onVictoryMode;
    public UnityEvent<int> onAddGold;
    public UnityEvent<int> onGoldSpend;
    public UnityEvent onDefenderPlaced;
    public UnityEvent<GameObject> onEnemySpawned;
    public UnityEvent<string> onButtonClicked;
    #endregion

    //Singleton
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

    //Initialize UnityEvents if they are null
    private void Start()
    {
        if (onTutorialMode == null) onTutorialMode = new UnityEvent();
        if (onCooldownMode == null) onCooldownMode = new UnityEvent();
        if (onPlacementMode == null) onPlacementMode = new UnityEvent();
        if (onUpgradeMode == null) onUpgradeMode = new UnityEvent();
        if (onWaveMode == null) onWaveMode = new UnityEvent();
        if (onPauseMode == null) onPauseMode = new UnityEvent();
        if (onGameOverMode == null) onGameOverMode = new UnityEvent();
        if (onVictoryMode == null) onVictoryMode = new UnityEvent();
        if (onEnemySpawned == null) onEnemySpawned = new UnityEvent<GameObject>();
        if (onAddGold == null) onAddGold = new UnityEvent<int>();
        if (onGoldSpend == null) onGoldSpend = new UnityEvent<int>();
        if (onButtonClicked == null) onButtonClicked = new UnityEvent<string>();
        if (onDefenderPlaced == null) onDefenderPlaced = new UnityEvent();
    }

    //Trigger events
    public void TriggerDefenderPlaced()
    {
        onDefenderPlaced.Invoke();
    }
    public void TriggerButtonClicked(string buttonName)
    {
        onButtonClicked.Invoke(buttonName);
    }
    public void TriggerTutorialMode()
    {
        GameManager.instance.SwitchState(GameManager.GameState.Tutorial);
        onTutorialMode.Invoke();
    }
    public void TriggerCooldownMode()
    {
        GameManager.instance.SwitchState(GameManager.GameState.Cooldown);
        onCooldownMode.Invoke();
    }
    public void TriggerPlacementMode() { 
        GameManager.instance.SwitchState(GameManager.GameState.Placement);
        onPlacementMode.Invoke(); 
    }
    public void TriggerUpgradeMode()
    {
        GameManager.instance.SwitchState(GameManager.GameState.Upgrade);
        onUpgradeMode.Invoke();
    }
    public void TriggerWaveMode() { 
        GameManager.instance.SwitchState(GameManager.GameState.Wave);
        onWaveMode.Invoke(); 
    }
    public void TriggerEnemySpawned(GameObject enemy)
    {
        onEnemySpawned.Invoke(enemy);
    }
    public void TriggerPauseMode()
    {
        GameManager.instance.SwitchState(GameManager.GameState.Pause);
        onPauseMode.Invoke();
    }
    public void TriggerGameOverMode()
    {
        GameManager.instance.SwitchState(GameManager.GameState.GameOver);
        onGameOverMode.Invoke();
    }
    public void TriggerVictoryMode()
    {
        GameManager.instance.SwitchState(GameManager.GameState.Victory);
        onVictoryMode.Invoke();
    }
    public void TriggerAddGold(int amount)
    {
        onAddGold.Invoke(amount);
    }
    public void TriggerGoldSpend(int amount)
    {
        onGoldSpend.Invoke(amount);
    }
}
