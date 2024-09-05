/*
    The TutorialCanvasManager class manages the tutorial steps in the UI.
    - Handles navigation between tutorial steps using Next and Previous buttons.
    - Shows or hides tutorial steps based on the current step index.
    - Provides methods for navigating steps and ending the tutorial.
    - Disables the GameObject if no tutorial steps are assigned.
    - Updates button visibility based on the current step position.
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCanvasManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> tutorialSteps = new List<GameObject>();
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    private int currentStepIndex = 0;

    private void Start()
    {
        if (tutorialSteps.Count == 0)
        {
            Debug.LogError("No tutorial steps assigned to the TutorialCanvasManager.");
            gameObject.SetActive(false);
            return;
        }

        nextButton.onClick.AddListener(OnNextButtonClicked);
        previousButton.onClick.AddListener(OnPreviousButtonClicked);

        UpdateTutorialSteps();
    }

    private void UpdateTutorialSteps()
    {
        foreach (GameObject step in tutorialSteps)
        {
            step.SetActive(false);
        }

        tutorialSteps[currentStepIndex].SetActive(true);

        previousButton.gameObject.SetActive(currentStepIndex > 0);
        nextButton.gameObject.SetActive(true);
    }

    public void OnNextButtonClicked()
    {
        currentStepIndex++;
        if (currentStepIndex >= tutorialSteps.Count)
        {
            EndTutorial();
        }
        else
        {
            UpdateTutorialSteps();
        }
    }

    public void OnPreviousButtonClicked()
    {
        if (currentStepIndex > 0)
        {
            currentStepIndex--;
            UpdateTutorialSteps();
        }
    }

    private void EndTutorial()
    {
        gameObject.SetActive(false);
        Debug.Log("Tutorial completed.");
    }
}
