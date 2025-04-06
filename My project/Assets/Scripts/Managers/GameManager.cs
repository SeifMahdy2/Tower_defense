using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }
    
    [Header("Game State")]
    [SerializeField] private GameState currentState = GameState.MainMenu;
    
    [Header("Player Stats")]
    [SerializeField] private int startingGold = 100;
    [SerializeField] private int startingLives = 10;
    
    [Header("Wave Settings")]
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private int currentWave = 0;
    [SerializeField] private bool isInWave = false;
    
    // Events
    public static UnityEvent<int> OnGoldChanged = new UnityEvent<int>();
    public static UnityEvent<int> OnLivesChanged = new UnityEvent<int>();
    public static UnityEvent<int> OnWaveChanged = new UnityEvent<int>();
    public static UnityEvent<GameState> OnGameStateChanged = new UnityEvent<GameState>();
    public static UnityEvent<bool> OnWaveStateChanged = new UnityEvent<bool>();
    
    // Properties
    private int playerGold;
    public int PlayerGold 
    { 
        get { return playerGold; }
        private set 
        { 
            playerGold = value;
            OnGoldChanged.Invoke(playerGold);
        }
    }
    
    private int playerLives;
    public int PlayerLives
    {
        get { return playerLives; }
        private set
        {
            playerLives = value;
            OnLivesChanged.Invoke(playerLives);
            if (playerLives <= 0) SetGameState(GameState.GameOver);
        }
    }
    
    public GameState CurrentState => currentState;
    public int CurrentWave => currentWave;
    public bool IsInWave => isInWave;
    
    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Register for events
        EnemyController.OnEnemyReachedEnd.AddListener(OnEnemyReachedEnd);
        EnemyController.OnEnemyDeath.AddListener(OnEnemyKilled);
    }
    
    private void Start()
    {
        // Initialize game
        ResetGame();
    }
    
    // Reset game to initial state
    public void ResetGame()
    {
        PlayerGold = startingGold;
        PlayerLives = startingLives;
        currentWave = 0;
        isInWave = false;
        
        OnWaveChanged.Invoke(currentWave);
        OnWaveStateChanged.Invoke(isInWave);
        
        // Set initial game state
        SetGameState(GameState.MainMenu);
    }
    
    // Change game state
    public void SetGameState(GameState newState)
    {
        if (currentState == newState) return;
        
        currentState = newState;
        OnGameStateChanged.Invoke(currentState);
        
        switch (currentState)
        {
            case GameState.MainMenu:
                Time.timeScale = 0f; // Pause game
                break;
                
            case GameState.Playing:
                Time.timeScale = 1f; // Resume game
                break;
                
            case GameState.Paused:
                Time.timeScale = 0f; // Pause game
                break;
                
            case GameState.GameOver:
                Time.timeScale = 0f; // Pause game
                Debug.Log("Game Over!");
                break;
                
            case GameState.Victory:
                Time.timeScale = 0f; // Pause game
                Debug.Log("Victory!");
                break;
        }
    }
    
    // Start the game
    public void StartGame()
    {
        SetGameState(GameState.Playing);
    }
    
    // Toggle pause state
    public void TogglePause()
    {
        if (currentState == GameState.Playing)
            SetGameState(GameState.Paused);
        else if (currentState == GameState.Paused)
            SetGameState(GameState.Playing);
    }
    
    // Start a new wave
    public void StartNextWave()
    {
        if (isInWave || currentState != GameState.Playing) return;
        
        currentWave++;
        OnWaveChanged.Invoke(currentWave);
        
        isInWave = true;
        OnWaveStateChanged.Invoke(isInWave);
        
        // Notify wave spawner to begin the wave
        if (WaveSpawner.Instance != null)
        {
            WaveSpawner.Instance.StartWave(currentWave);
        }
    }
    
    // End the current wave
    public void EndCurrentWave()
    {
        if (!isInWave) return;
        
        isInWave = false;
        OnWaveStateChanged.Invoke(isInWave);
        
        // Start timer for next wave
        StartCoroutine(WaitForNextWave());
    }
    
    // Wait before allowing the next wave to start
    private IEnumerator WaitForNextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        
        // Automatically start next wave (can be changed to manual)
        // StartNextWave();
    }
    
    // Attempt to spend gold (for buying/upgrading towers)
    public bool TrySpendGold(int amount)
    {
        if (playerGold >= amount)
        {
            PlayerGold -= amount;
            return true;
        }
        
        return false;
    }
    
    // Add gold (from killing enemies)
    public void AddGold(int amount)
    {
        PlayerGold += amount;
    }
    
    // Event handlers
    private void OnEnemyReachedEnd(EnemyController enemy)
    {
        PlayerLives -= enemy.EnemyData.damageToBase;
    }
    
    private void OnEnemyKilled(EnemyController enemy, int reward)
    {
        AddGold(reward);
    }
}

// Game state enum
public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver,
    Victory
} 