#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class RemoveMissingScripts : EditorWindow
{
    [MenuItem("Tools/Remove Missing Scripts in Scene")]
    public static void RemoveMissingScriptsInScene()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        int totalCount = 0;

        foreach (GameObject obj in allObjects)
        {
            totalCount += RemoveMissingScriptsRecursively(obj);
        }
    }

    [MenuItem("Tools/Remove Missing Scripts in Selected Objects")]
    public static void RemoveMissingScriptsInSelected()
    {
        int totalCount = 0;

        foreach (GameObject obj in Selection.gameObjects)
        {
            totalCount += RemoveMissingScriptsRecursively(obj);
        }
    }

    private static int RemoveMissingScriptsRecursively(GameObject obj)
    {
        int count = 0;

        count += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);

        foreach (Transform child in obj.transform)
        {
            count += RemoveMissingScriptsRecursively(child.gameObject);
        }

        return count;
    }
}

#endif