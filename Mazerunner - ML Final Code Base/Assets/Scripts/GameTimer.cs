using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private bool debugMode = false;

    private float startTime;
    private float elapsedTime = 0;
    private bool isTiming = true;

    private const string ElapsedTimeKey = "ElapsedTime";
    private int lastDisplayedSecond = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
            //Debug.Log("GameTimer initialized and set to DontDestroyOnLoad.");
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            Debug.Log("Duplicate GameTimer instance destroyed.");
        }

        Debug.Log($"GameTimer Instance: {Instance}, IsTiming: {isTiming}, ElapsedTime: {elapsedTime}");
    }

    void Start()
    {
        if (timerText == null)
        {
            timerText = GameObject.Find("Timer Text (TMP)")?.GetComponent<TextMeshProUGUI>();
            if (timerText == null)
            {
                Debug.LogWarning("Timer text reference is missing or could not be found.");
            }
        }

        // Load elapsed time from PlayerPrefs
        if (PlayerPrefs.HasKey(ElapsedTimeKey))
        {
            elapsedTime = PlayerPrefs.GetFloat(ElapsedTimeKey);
            Debug.Log($"Loaded elapsed time: {elapsedTime}");
        }
        else
        {
            Debug.Log("No elapsed time found in PlayerPrefs.");
        }

        startTime = Time.time;
    }

    void Update()
    {
        if (isTiming)
        {
            elapsedTime += Time.deltaTime;
            int currentSecond = Mathf.FloorToInt(elapsedTime);
            if (currentSecond != lastDisplayedSecond)
            {
                lastDisplayedSecond = currentSecond;
                UpdateTimerText();
            }
        }
    }

    public void StopTimer()
    {
        isTiming = false;
        // Save elapsed time to PlayerPrefs
        PlayerPrefs.SetFloat(ElapsedTimeKey, elapsedTime);
        LogDebug($"Stopped timer. Saved elapsed time: {elapsedTime}");
    }

    public void ResetTimer()
    {
        //StopTimer();
        elapsedTime = 0;
        PlayerPrefs.SetFloat(ElapsedTimeKey, elapsedTime);
        LogDebug("Reset timer and saved elapsed time as 0.");
        UpdateTimerText();
    }

    public void UpdateTimerText()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            timerText.text = string.Format("Timer : {0:00}:{1:00}", minutes, seconds);
        }
    }

    public string GetElapsedTimeFormatted()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        return string.Format("Time : {0:00}:{1:00}", minutes, seconds);
    }

    private void LogDebug(string message)
    {
        if (debugMode) Debug.Log(message);
    }

    // New method to get elapsed time in seconds
    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}