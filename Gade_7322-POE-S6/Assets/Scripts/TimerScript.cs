using System.Collections;
using UnityEngine;
using TMPro;

public class TimerScript : MonoBehaviour
{
    public float countdownTime = 20f;
    private float currentTime;
    public TextMeshProUGUI timerText;

    private void Start()
    {
        EventManager.instance.onCooldownMode.AddListener(StartTimer);
    }

    private void StartTimer()
    {
        currentTime = countdownTime;
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerText();
            yield return null;
        }
        TimerEnded();
    }

    private void UpdateTimerText()
    {
        timerText.text = currentTime.ToString("F2");
    }

    private void TimerEnded()
    {
        EventManager.instance.TriggerWaveMode();
    }
}
