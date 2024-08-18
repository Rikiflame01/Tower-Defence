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
