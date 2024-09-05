/*
    The MainMenuManager class handles scene loading and application quitting from the main menu.

    - Methods:
      - LoadScene(string sceneName): Loads the specified scene. Clears all objects in the "DontDestroyOnLoad" scene and destroys instances of CanvasManager and GameManager.
      - ExitApplication(): Exits the application. In the editor, stops play mode.
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        if (sceneName == null)
        {
            Debug.LogError("ButtonManager: Scene name is not assigned.");
            return;
        }

        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.scene.name == "DontDestroyOnLoad")
            {
                Destroy(obj);
            }
        }

        if (CanvasManager.instance != null)
        {
            Destroy(CanvasManager.instance.gameObject);
            CanvasManager.instance = null;
        }

        if (GameManager.instance != null)
        {
            Destroy(GameManager.instance.gameObject);
            GameManager.instance = null;
        }

        SceneManager.LoadScene(sceneName);

    }

    public void ExitApplication()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }
}
