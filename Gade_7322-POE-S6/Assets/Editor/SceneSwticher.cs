using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.IO;

[Overlay(typeof(SceneView), "Scene Switcher")]
public class SceneSwitcher : Overlay
{
    private List<string> scenePaths;

    public override void OnCreated()
    {
        base.OnCreated();
        scenePaths = GetAllScenePaths();
    }

    public override VisualElement CreatePanelContent()
    {
        var container = new VisualElement();
        container.style.flexDirection = FlexDirection.Column;

        if (scenePaths == null || scenePaths.Count == 0)
        {
            container.Add(new Label("No scenes found in the project."));
            return container;
        }

        foreach (string scenePath in scenePaths)
        {
            var button = new Button(() => OpenScene(scenePath))
            {
                text = Path.GetFileNameWithoutExtension(scenePath)
            };
            container.Add(button);
        }

        return container;
    }

    private List<string> GetAllScenePaths()
    {
        List<string> scenes = new List<string>();
        string[] guids = AssetDatabase.FindAssets("t:Scene");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            scenes.Add(path);
        }

        return scenes;
    }

    private void OpenScene(string scenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }
}
