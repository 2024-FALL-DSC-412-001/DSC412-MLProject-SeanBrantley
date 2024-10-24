using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }

    private float startTime;
    private float elapsedTime = 0;
    private bool isTiming = true;

    private TextMeshProUGUI timerText;

    private const string ElapsedTimeKey = "ElapsedTime";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameTimer initialized and set to DontDestroyOnLoad.");
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            Debug.Log("Duplicate GameTimer instance destroyed.");
        }
    }

    void Start()
    {
        // Dynamically find and assign the TimerText component
        timerText = GameObject.Find("Timer Text (TMP)").GetComponent<TextMeshProUGUI>();

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
            UpdateTimerText();
        }
    }

    public void StopTimer()
    {
        isTiming = false;
        // Save elapsed time to PlayerPrefs
        PlayerPrefs.SetFloat(ElapsedTimeKey, elapsedTime);
        Debug.Log($"Stopped timer. Saved elapsed time: {elapsedTime}");
    }

    public void ResetTimer()
    {
        elapsedTime = 0;
        PlayerPrefs.SetFloat(ElapsedTimeKey, elapsedTime);
        Debug.Log("Reset timer and saved elapsed time as 0.");
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

    public void UpdateTimerTextReference(string timerTextObjectName)
    {
        timerText = GameObject.Find(timerTextObjectName).GetComponent<TextMeshProUGUI>();
        Debug.Log("Updated TimerText reference.");
    }
}