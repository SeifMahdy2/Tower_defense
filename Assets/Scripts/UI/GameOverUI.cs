using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI wavesSurvivedText;
    [SerializeField] private TextMeshProUGUI enemiesDefeatedText;
    
    private GameManager gameManager;
    private WaveSpawner waveSpawner;
    
    private void OnEnable()
    {
        // Get references when the game over panel becomes active
        gameManager = FindObjectOfType<GameManager>();
        waveSpawner = FindObjectOfType<WaveSpawner>();
        
        // Setup UI
        SetupUI();
        
        // Add button listeners
        SetupButtons();
    }
    
    private void SetupUI()
    {
        // Display waves survived (current wave)
        if (wavesSurvivedText != null && waveSpawner != null)
        {
            int wavesCompleted = waveSpawner.GetCurrentWaveIndex();
            wavesSurvivedText.text = "Waves Survived: " + wavesCompleted;
        }
        
        // Display enemies defeated (could be tracked in GameManager)
        if (enemiesDefeatedText != null && gameManager != null)
        {
            int enemiesDefeated = gameManager.GetEnemiesDefeated();
            enemiesDefeatedText.text = "Enemies Defeated: " + enemiesDefeated;
        }
    }
    
    private void SetupButtons()
    {
        // Restart button
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
        }
        
        // Main menu button
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        }
    }
    
    private void RestartGame()
    {
        // Restart the current level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        // Ensure normal time scale
        Time.timeScale = 1f;
    }
    
    private void ReturnToMainMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene("MainMenu");
        
        // Ensure normal time scale
        Time.timeScale = 1f;
    }
} 