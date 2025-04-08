using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private bool isLastLevel = false;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private int nextLevelIndex = 0; // 0 to return to menu
    
    private WaveSpawner waveSpawner;
    private GameManager gameManager;
    private int totalWaves;
    private bool levelCompleted = false;
    
    private void Start()
    {
        // Get references
        waveSpawner = FindObjectOfType<WaveSpawner>();
        gameManager = FindObjectOfType<GameManager>();
        
        // Hide victory panel
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
        
        // Subscribe to wave completed event
        WaveSpawner.OnWaveCompleted += CheckLevelCompletion;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from event
        WaveSpawner.OnWaveCompleted -= CheckLevelCompletion;
    }
    
    private void CheckLevelCompletion(int waveNumber)
    {
        if (waveSpawner == null) return;
        
        // Check if all waves have been completed
        if (waveNumber >= waveSpawner.GetTotalWaves() - 1)
        {
            // Level completed
            LevelCompleted();
        }
    }
    
    private void LevelCompleted()
    {
        if (levelCompleted) return;
        
        levelCompleted = true;
        
        // Show victory panel
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
        
        // Play victory sound
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlaySound("Victory");
        }
        
        // Save progress (unlock next level)
        UnlockNextLevel();
        
        Debug.Log("Level completed!");
    }
    
    private void UnlockNextLevel()
    {
        if (isLastLevel) return;
        
        // Get currently unlocked level
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        
        // Unlock next level if not already unlocked
        if (nextLevelIndex > unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextLevelIndex);
            PlayerPrefs.Save();
        }
    }
    
    public void LoadNextLevel()
    {
        // If this is the last level, return to main menu
        if (isLastLevel || nextLevelIndex <= 0)
        {
            ReturnToMainMenu();
            return;
        }
        
        // Load next level
        SceneManager.LoadScene("Level" + nextLevelIndex);
    }
    
    public void RestartLevel()
    {
        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void ReturnToMainMenu()
    {
        // Load main menu
        SceneManager.LoadScene("MainMenu");
    }
} 