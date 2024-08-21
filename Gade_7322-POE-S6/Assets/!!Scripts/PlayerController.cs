using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void Update()
    {
        HandlePause();
    }

    private void HandlePause()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Placement)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EventManager.instance.TriggerPauseMode();
            }
        }

    }
}
