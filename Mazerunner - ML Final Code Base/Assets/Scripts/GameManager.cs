using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //PLAYER STAT
    private float moveSpeed = 4f;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerParentTransform;
    private PlayerController currentPlayer;

    [SerializeField] private CameraController myCamera;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject winGamePanel;

    private LineRecorder lineRecorder;
    private int winCounter = 0;
    private float bestCompletionTime = float.MaxValue;
    [SerializeField] private GameTimer gameTimer;
    PlayerAgent playerAgent;

    // Const string variables for our PlayerPref keys
    const string MoveSpeedKey = "MoveSpeed";

    void Awake()
    {
        LoadPlayerPrefs();
        lineRecorder = Object.FindFirstObjectByType<LineRecorder>();
        /*gameTimer = GameTimer.Instance;

        if (gameTimer == null)
        {
            Debug.LogError("GameTimer not found in the scene during GameManager Awake.");
            // Optionally create the GameTimer if it doesn't exist
            GameObject timerObject = new GameObject("GameTimer");
            gameTimer = timerObject.AddComponent<GameTimer>();
        }
        else
        {
            Debug.Log("GameManager initialized. GameTimer instance: " + gameTimer);
        }*/
    }

    private void Start()
    {
        currentPlayer = Object.FindFirstObjectByType<PlayerController>();
        SpawnPlayer();
        Debug.Log("GameManager started. GameTimer instance: " + gameTimer);

        lineRecorder.SetLevel(SceneManager.GetActiveScene().name);
    }

    private void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity, playerParentTransform);
        player.tag = "Player";
        playerAgent = Object.FindFirstObjectByType<PlayerAgent>();
        currentPlayer = player.GetComponent<PlayerController>();
        currentPlayer.InitializePlayer(moveSpeed);
        myCamera.SetPlayer(player);
        lineRecorder.SetPlayer(player);
    }

    public void WinGame()
    {
        Debug.Log("Win Game called.");
        //lineRecorder.StopRecording();

        /*float elapsedTime = GetElapsedTime();

        if (winCounter >= 1 && elapsedTime < bestCompletionTime)
        {
            bestCompletionTime = elapsedTime;
            playerAgent.AddCompletionBonusReward(10000.0f);
        }*/

        //winCounter++;

        //gameTimer.StopTimer();
        //gameTimer.ResetTimer();
    }

    public float GetElapsedTime()
    {
        return gameTimer.GetElapsedTime();
    }

    private void LoadPlayerPrefs()
    {

        if (PlayerPrefs.HasKey(MoveSpeedKey))
        {
            moveSpeed = PlayerPrefs.GetFloat(MoveSpeedKey);
            Debug.Log("Loaded move speed: " + moveSpeed);
        }
    }

    public void PlayerReachedWaypoint()
    {
        playerAgent.AddReward(10.0f); // Reward for reaching a waypoint
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}