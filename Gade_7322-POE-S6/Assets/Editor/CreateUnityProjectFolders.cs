using UnityEditor;
using UnityEngine;
using System.IO;

public class CreateUnityProjectFolders : MonoBehaviour
{
    [MenuItem("Tools/Create Project Folders")]
    private static void CreateProjectFolders()
    {
        // Check if the application is in the editor
        if (!Application.isEditor)
        {
            Debug.LogWarning("This script should only be run in the Unity Editor.");
            return;
        }

        string basePath = Application.dataPath;
        string[] folders = new string[]
        {
            "Animations",
            "Audio",
            "Materials",
            "Models",
            "Prefabs",
            "Resources",
            "Scripts",
            "Scenes",
            "Shaders",
            "Sprites",
            "Textures"
        };

        try
        {
            foreach (string folder in folders)
            {
                string folderPath = Path.Combine(basePath, folder);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    Debug.Log($"Created folder: {folderPath}");
                }
                else
                {
                    Debug.LogWarning($"Folder already exists: {folderPath}");
                }
            }
            Debug.Log("Unity project folder structure created successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"An error occurred: {e.Message}");
        }
    }
}
