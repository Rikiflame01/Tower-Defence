using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ButtonManager : MonoBehaviour
{
    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;

    private void Start()
    {
        eventSystem = FindObjectOfType<EventSystem>();

        if (eventSystem == null)
        {
            Debug.LogError("ButtonManager: No EventSystem found in the scene.");
            return;
        }

        raycaster = FindObjectOfType<GraphicRaycaster>();

        if (raycaster == null)
        {
            Debug.LogError("ButtonManager: No GraphicRaycaster found in the scene.");
            return;
        }

        Button[] allButtons = FindObjectsOfType<Button>();

        foreach (Button button in allButtons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button));
        }
    }

    private void OnButtonClicked(Button button)
    {
        if (GameManager.instance.GetCurrentState() == GameManager.GameState.Cooldown || 
            GameManager.instance.GetCurrentState() == GameManager.GameState.Tutorial)
        {
            if (IsPointerOnUIButton())
            {
                EventManager.instance.TriggerButtonClicked(button.name);
            }
        }
        else
        {
            Debug.Log("Button clicks are disabled unless in Cooldown state.");
        }
    }

    private bool IsPointerOnUIButton()
    {
        PointerEventData pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.GetComponent<Button>() != null)
            {
                return true;
            }
        }

        return false;
    }
}
