using System;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    public UnityEvent onTutorialMode;
    public UnityEvent onCooldownMode;
    public UnityEvent onPlacementMode;
    public UnityEvent onUpgradeMode;
    public UnityEvent onWaveMode;
    public UnityEvent onPauseMode;
    public UnityEvent onGameOverMode;
    public UnityEvent onVictoryMode;

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
        if (onTutorialMode == null) onTutorialMode = new UnityEvent();
        if (onCooldownMode == null) onCooldownMode = new UnityEvent();
        if (onPlacementMode == null) onPlacementMode = new UnityEvent();
        if (onUpgradeMode == null) onUpgradeMode = new UnityEvent();
        if (onWaveMode == null) onWaveMode = new UnityEvent();
        if (onPauseMode == null) onPauseMode = new UnityEvent();
        if (onGameOverMode == null) onGameOverMode = new UnityEvent();
        if (onVictoryMode == null) onVictoryMode = new UnityEvent();
    }

    public void TriggerTutorialMode() => onTutorialMode.Invoke();
    public void TriggerCooldownMode() => onCooldownMode.Invoke();
    public void TriggerPlacementMode() => onPlacementMode.Invoke();
    public void TriggerUpgradeMode() => onUpgradeMode.Invoke();
    public void TriggerWaveMode() => onWaveMode.Invoke();
    public void TriggerPauseMode() => onPauseMode.Invoke();
    public void TriggerGameOverMode() => onGameOverMode.Invoke();
    public void TriggerVictoryMode() => onVictoryMode.Invoke();
}
