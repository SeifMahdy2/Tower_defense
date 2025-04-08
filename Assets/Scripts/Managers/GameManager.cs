using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int startingGold = 150;
    [SerializeField] private int startingHealth = 100;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private GameObject gameOverPanel;
    
    // Game state
    private int currentGold;
    private int currentHealth;
    private bool gameIsOver = false;
    private int enemiesDefeated = 0;
    private int totalGoldEarned = 0;
    
    // Singleton instance
    public static GameManager Instance { get; private set; }
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        // Initialize game state
        currentGold = startingGold;
        currentHealth = startingHealth;
        
        // Hide game over panel if available
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // Update UI
        UpdateUI();
    }
    
    // Called when an enemy reaches the end of the path
    public void EnemyReachedEnd(int damage)
    {
        if (gameIsOver) return;
        
        currentHealth -= damage;
        
        // Check if game over
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GameOver();
        }
        
        UpdateUI();
    }
    
    // Add gold (when killing enemies)
    public void AddGold(int amount)
    {
        if (gameIsOver) return;
        
        currentGold += amount;
        totalGoldEarned += amount;
        UpdateUI();
    }
    
    // Increment enemies defeated counter
    public void IncrementEnemiesDefeated()
    {
        enemiesDefeated++;
    }
    
    // Get the number of enemies defeated
    public int GetEnemiesDefeated()
    {
        return enemiesDefeated;
    }
    
    // Try to spend gold (for tower placement)
    public bool SpendGold(int amount)
    {
        if (gameIsOver || currentGold < amount) return false;
        
        currentGold -= amount;
        UpdateUI();
        return true;
    }
    
    // Check if player has enough gold
    public bool HasEnoughGold(int amount)
    {
        return currentGold >= amount;
    }
    
    // Update UI elements
    private void UpdateUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth;
        }
        
        if (goldText != null)
        {
            goldText.text = "Gold: " + currentGold;
        }
    }
    
    // Game over handling
    private void GameOver()
    {
        gameIsOver = true;
        
        // Show game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        Debug.Log("Game Over!");
        
        // Optionally pause the game
        //Time.timeScale = 0f;
    }
    
    // Check if game is over
    public bool IsGameOver()
    {
        return gameIsOver;
    }
    
    // Reset the game (called from UI)
    public void RestartGame()
    {
        // Reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        
        // Reset timescale in case it was paused
        Time.timeScale = 1f;
    }
    
    // Get the total gold earned throughout the level
    public int GetTotalGoldEarned()
    {
        return totalGoldEarned;
    }
    
    // Get current gold
    public int GetCurrentGold()
    {
        return currentGold;
    }
    
    // Get current health
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
} 