using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //PLAYER STATS
    private int lives = 1;
    private int bombSupply = 1;
    private int explodeRange = 1;
    private float moveSpeed = 4f;
    private int speedCounter = 1;

    [SerializeField] private int bombLimit = 3;
    [SerializeField] private int explodeRangeLimit = 4;
    [SerializeField] private int moveSpeedLimit = 5;
    private float speedIncrease = 0.4f;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerParentTransform;
    [SerializeField] private PlayerAgent playerAgent;
    [SerializeField] private GameObject player;
    private PlayerController currentPlayer;

    //[SerializeField] private float spawnDelayDifference = 1f;

    [SerializeField] private CameraController myCamera;

    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI bombSupplyText;
    [SerializeField] private TextMeshProUGUI explodeRangeText;
    [SerializeField] private TextMeshProUGUI moveSpeedText;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI winTimeText;

    private bool isPaused = false;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winGamePanel;

    private int enemiesThisLevel = 0;

    [SerializeField] private bool isLastLevel = false;

    private LineRecorder lineRecorder;
    private GameTimer gameTimer;

    // Const string variables for our PlayerPref keys
    const string CurrentLivesKey = "CurrentLives";
    const string BombSupplyKey = "BombSupply";
    const string ExplodeRangeKey = "ExplodeRange";
    const string MoveSpeedKey = "MoveSpeed";
    const string SpeedCounterKey = "SpeedCounter";

    void Awake()
    {
        LoadPlayerPrefs();
        lineRecorder = Object.FindAnyObjectByType<LineRecorder>();
        gameTimer = GameTimer.Instance;
        if (gameTimer == null)
        {
            Debug.LogError("GameTimer not found in the scene during GameManager Awake.");
            // Optionally create the GameTimer if it doesn't exist
            GameObject timerObject = new GameObject("GameTimer");
            gameTimer = timerObject.AddComponent<GameTimer>();
            Debug.Log("Created new GameTimer instance.");
        }
        else
        {
            Debug.Log("GameManager initialized. GameTimer instance: " + gameTimer);
        }
    }

    private void Start()
    {
        if (player != null)
        {
            currentPlayer = player.GetComponent<PlayerController>();
            if (currentPlayer == null)
            {
                Debug.LogError("Failed to assign PlayerController in GameManager.");
            }
            currentPlayer.InitializePlayer(bombSupply, moveSpeed);
            myCamera.SetPlayer(player);
            UpdateLivesText();
            lineRecorder.SetPlayer(player);
        }
        else
        {
            Debug.LogError("Player GameObject reference is missing in GameManager.");
        }

        enemiesThisLevel = GetEnemyCount();
        Debug.Log("GameManager started. GameTimer instance: " + gameTimer);

        lineRecorder.SetLevel(SceneManager.GetActiveScene().name);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            PauseButton();
        }
    }

    public void PlayerDied()
    {
        playerAgent.AddReward(-2.0f);
        if (lives > 1)
        {
            lives--;

            // Spawn new player
            //Invoke("SpawnPlayer", currentPlayer.GetDestroyDelayTime() + spawnDelayDifference);
        }
        else
        {
            lineRecorder.StopRecording();
            Invoke("GameOver", 2.5f);
            playerAgent.EndEpisode();
        }
    }

    /*private void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity, playerParentTransform);
        player.tag = "Player";
        currentPlayer = player.GetComponent<PlayerController>();
        if (currentPlayer == null)
        {
            Debug.LogError("Failed to assign PlayerController in GameManager.");
        }
        currentPlayer.InitializePlayer(bombSupply, moveSpeed);
        myCamera.SetPlayer(player);
        UpdateLivesText();
        lineRecorder.SetPlayer(player);
    }*/

    public void WinGame()
    {
        Debug.Log("WinGame called.");
        lineRecorder.StopRecording();
        //winGamePanel.SetActive(true);
        playerAgent.AddReward(10.0f);  // Large reward for winning the game
        playerAgent.EndEpisode();

        if (gameTimer != null)
        {
            gameTimer.StopTimer();
            winTimeText.text = gameTimer.GetElapsedTimeFormatted();
            gameTimer.ResetTimer();
            Debug.Log("GameTimer stopped.");
        }
        else
        {
            Debug.LogError("GameTimer instance is null.");
        }
    }

    private void UpdateLivesText()
    {
        livesText.text = lives.ToString("D2");
    }

    private void UpdateBombSupplyText()
    {
        bombSupplyText.text = bombSupply.ToString("D2");
    }

    private void UpdateExplodeRangeText()
    {
        explodeRangeText.text = explodeRange.ToString("D2");
    }
    private void UpdateMoveSpeedText()
    {
        moveSpeedText.text = speedCounter.ToString("D2");
    }

    private void UpdateLevelText()
    {
        levelNameText.text = SceneManager.GetActiveScene().name;
    }

    public void PauseButton()
    {
        if (isPaused)
        {
            pausePanel.SetActive(false);
            currentPlayer.SetPaused(false);
            isPaused = false;
            Time.timeScale = 1f;
        }
        else
        {
            pausePanel.SetActive(true);
            currentPlayer.SetPaused(true);
            isPaused = true;
            Time.timeScale = 0f;
        }
    }

    private void GameOver()
    {
        lineRecorder.StopRecording();
        //gameOverPanel.SetActive(true);
        gameTimer.StopTimer();
        gameTimer.ResetTimer();
        Debug.Log("Game over. Timer stopped and reset.");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    private int GetEnemyCount()
    {
        int count = GameObject.FindGameObjectsWithTag("Enemy").Length;
        return count;
    }

    public void EnemyHasDied()
    {
        playerAgent.AddReward(20f);
        enemiesThisLevel--;
        if (enemiesThisLevel <= 0)
        {
            if (isLastLevel)
            {
                // Tell the player to play victory animation
                currentPlayer.PlayerVictory();

                WinGame();
                
                if (playerAgent != null)
                {
                    playerAgent.AddVictoryReward();
                }
            }
            else
            {
                // Tell the player to play victory animation
                currentPlayer.PlayerVictory();

                // Save all of the player data to PlayerPrefs so that it can be loaded in the next level
                SavePlayerData();

                Invoke("LoadNextLevel", 3f);
            }
        }
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        lineRecorder.StopRecording();
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void IncreaseBombSupply()
    {
        bombSupply++;
        bombSupply = Mathf.Clamp(bombSupply, 0, bombLimit);
        UpdateBombSupplyText();
        currentPlayer.InitializePlayer(bombSupply, moveSpeed);
        Debug.Log("Increased bomb supply: " + bombSupply);
    }

    public void IncreaseSpeed()
    {
        if (speedCounter < moveSpeedLimit)
        {
            moveSpeed += speedIncrease;
            Debug.Log("Pre-clamp speed: " + moveSpeed);
            //moveSpeed = Mathf.Clamp(moveSpeed, 4, moveSpeedLimit);
            Debug.Log("Post-clamp speed: " + moveSpeed);
            speedCounter++;
            UpdateMoveSpeedText();
            currentPlayer.InitializePlayer(bombSupply, moveSpeed);
            Debug.Log("Increased speed: " + moveSpeed);
        }
    }

    public void IncreaseBombRange()
    {
        explodeRange++;
        Debug.Log("Pre-clamp range: " + explodeRange);
        explodeRange = Mathf.Clamp(explodeRange, 0, explodeRangeLimit);
        Debug.Log("Post-clamp range: " + explodeRange);
        UpdateExplodeRangeText();
        Debug.Log("Increased bomb range: " + explodeRange);
    }

    // Bombs can call this method to know how much they should set the explode range to
    public int GetExplodeRange()
    {
        return explodeRange;
    }

    private void SavePlayerData()
    {
        PlayerPrefs.SetInt(CurrentLivesKey, lives);
        PlayerPrefs.SetInt(BombSupplyKey, bombSupply);
        PlayerPrefs.SetInt(ExplodeRangeKey, explodeRange);
        PlayerPrefs.SetFloat(MoveSpeedKey, moveSpeed);
        PlayerPrefs.SetInt(SpeedCounterKey, speedCounter);
    }

    private void LoadPlayerPrefs()
    {

        if (PlayerPrefs.HasKey(CurrentLivesKey))
        {
            lives = PlayerPrefs.GetInt(CurrentLivesKey);
            Debug.Log("Loaded lives: " + lives);
        }

        if (PlayerPrefs.HasKey(BombSupplyKey))
        {
            bombSupply = PlayerPrefs.GetInt(BombSupplyKey);
            Debug.Log("Loaded bomb supply: " + bombSupply);
        }

        if (PlayerPrefs.HasKey(ExplodeRangeKey))
        {
            explodeRange = PlayerPrefs.GetInt(ExplodeRangeKey);
            Debug.Log("Loaded explode range: " + explodeRange);
        }

        if (PlayerPrefs.HasKey(MoveSpeedKey))
        {
            moveSpeed = PlayerPrefs.GetFloat(MoveSpeedKey);
            Debug.Log("Loaded move speed: " + moveSpeed);
        }
        if (PlayerPrefs.HasKey(SpeedCounterKey))
        {
            speedCounter = PlayerPrefs.GetInt(SpeedCounterKey);
            Debug.Log("Loaded speed counter: " + speedCounter);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}