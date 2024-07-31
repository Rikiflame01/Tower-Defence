using UnityEditor;
using UnityEngine;

public class GameManagerEditor : EditorWindow
{
    [MenuItem("Window/Game State Manager")]
    public static void ShowWindow()
    {
        GetWindow<GameManagerEditor>("Game State Manager");
    }

    private void OnGUI()
    {
        GUILayout.Label("Game State Manager", EditorStyles.boldLabel);

        if (GUILayout.Button("Tutorial Mode"))
        {
            EventManager.instance.TriggerTutorialMode();
        }

        if (GUILayout.Button("Cooldown Mode"))
        {
            EventManager.instance.TriggerCooldownMode();
        }

        if (GUILayout.Button("Placement Mode"))
        {
            EventManager.instance.TriggerPlacementMode();
        }

        if (GUILayout.Button("Upgrade Mode"))
        {
            EventManager.instance.TriggerUpgradeMode();
        }

        if (GUILayout.Button("Wave Mode"))
        {
            EventManager.instance.TriggerWaveMode();
        }

        if (GUILayout.Button("Pause Mode"))
        {
            EventManager.instance.TriggerPauseMode();
        }

        if (GUILayout.Button("Game Over Mode"))
        {
            EventManager.instance.TriggerGameOverMode();
        }

        if (GUILayout.Button("Victory Mode"))
        {
            EventManager.instance.TriggerVictoryMode();
        }
    }
}
