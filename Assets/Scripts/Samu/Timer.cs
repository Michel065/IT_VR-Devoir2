using UnityEngine;
using TMPro;

public class VRTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public float startTime = 60f;
    private float currentTime;
    private bool isRunning = false;

    void Start()
    {
        currentTime = startTime;
        UpdateDisplay();
        StartTimer();
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;
        currentTime = Mathf.Max(currentTime, 0);
        UpdateDisplay();

        if (currentTime <= 0)
        {
            isRunning = false;
            OnTimerEnd();
        }
    }

    void UpdateDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        currentTime = startTime;
        UpdateDisplay();
    }

    void OnTimerEnd()
    {
        Debug.Log("⏰ Timer terminé !");
        // vibration, son, événement, etc.
    }
}
