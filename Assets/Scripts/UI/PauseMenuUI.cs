using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button backFromSettingsButton;
    
    private GameManager gameManager;
    private AudioManager audioManager;
    private bool isPaused = false;
    
    private void Start()
    {
        // Get references
        gameManager = FindObjectOfType<GameManager>();
        audioManager = FindObjectOfType<AudioManager>();
        
        // Hide panels by default
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        
        // Setup button listeners
        SetupButtons();
    }
    
    private void SetupButtons()
    {
        // Resume button
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }
        
        // Restart button
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        // Main menu button
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        }
        
        // Settings button
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettings);
        }
        
        // Back from settings button
        if (backFromSettingsButton != null)
        {
            backFromSettingsButton.onClick.AddListener(CloseSettings);
        }
    }
    
    private void Update()
    {
        // Check for pause input (typically Escape key)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    public void TogglePause()
    {
        if (gameManager != null && gameManager.IsGameOver())
        {
            // Don't pause if game is already over
            return;
        }
        
        isPaused = !isPaused;
        
        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }
    
    private void PauseGame()
    {
        // Pause time
        Time.timeScale = 0f;
        
        // Show pause menu
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
        
        // Pause audio
        if (audioManager != null)
        {
            audioManager.PauseAllAudio(true);
        }
    }
    
    private void ResumeGame()
    {
        // Resume time
        Time.timeScale = 1f;
        
        // Hide pause menu
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        // Hide settings panel if open
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        
        // Resume audio
        if (audioManager != null)
        {
            audioManager.PauseAllAudio(false);
        }
        
        isPaused = false;
    }
    
    private void RestartGame()
    {
        // Resume time
        Time.timeScale = 1f;
        
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        isPaused = false;
    }
    
    private void ReturnToMainMenu()
    {
        // Resume time
        Time.timeScale = 1f;
        
        // Load main menu
        SceneManager.LoadScene("MainMenu");
        
        isPaused = false;
    }
    
    private void OpenSettings()
    {
        // Hide pause menu
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        // Show settings panel
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }
    
    private void CloseSettings()
    {
        // Hide settings panel
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        
        // Show pause menu
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
    }
} 