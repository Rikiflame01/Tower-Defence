/*
    The ConfirmationManager class handles displaying a confirmation UI for actions that require user approval. 
    It manages a singleton instance and provides methods to show and handle confirmation dialogs.

    - Fields:
      - instance: The singleton instance of ConfirmationManager.
      - confirmationCanvas: The GameObject for the confirmation UI.
      - costText: The TextMeshProUGUI component displaying the cost message.
      - onConfirmAction: The action to invoke if the user confirms.

    - Methods:
      - Awake(): Sets up the singleton pattern for the ConfirmationManager.
      - ShowConfirmation(int cost, System.Action onConfirm): Displays the confirmation UI with the specified cost 
        and stores the action to invoke upon confirmation.
      - OnYesButtonClicked(): Executes the stored confirmation action and hides the confirmation UI.
      - OnNoButtonClicked(): Hides the confirmation UI without executing any action.
*/


using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmationManager : MonoBehaviour
{
    public static ConfirmationManager instance;

    public GameObject confirmationCanvas;
    public TextMeshProUGUI costText;
    private System.Action onConfirmAction;

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

    public void ShowConfirmation(int cost, System.Action onConfirm)
    {
        costText.text = $"Are you sure you want to proceed? The cost will be {cost} gold.";
        onConfirmAction = onConfirm;
        confirmationCanvas.SetActive(true);
    }

    public void OnYesButtonClicked()
    {
        onConfirmAction?.Invoke();
        confirmationCanvas.SetActive(false);
    }

    public void OnNoButtonClicked()
    {
        confirmationCanvas.SetActive(false);
    }
}
