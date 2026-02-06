using UnityEngine;
using TMPro;

public class VRTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public float startTime;
    private float currentTime;
    private bool isRunning = false;
    public Transform teleportTarget;
    Rigidbody rb;

    public string sceneToLoad;

    void Start()
    {
        ResetTimer();
        currentTime = startTime;
        UpdateDisplay();
        StartTimer();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;
        Debug.Log(currentTime);
        currentTime = Mathf.Max(currentTime, 0);
        UpdateDisplay();

        
        if (currentTime <= 0)
        {
            Debug.Log("Timer arrivé à zéro");
            isRunning = false;
            OnTimerEnd();
        }
    }

    void UpdateDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt((currentTime) % 60);
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
        Debug.Log("Timer terminé !");
        // vibration, son, événement, etc. 
        if (teleportTarget == null || rb == null) return;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = teleportTarget.position;
        rb.rotation = teleportTarget.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = teleportTarget.position;
            rb.rotation = teleportTarget.rotation;
        }
    }
}

