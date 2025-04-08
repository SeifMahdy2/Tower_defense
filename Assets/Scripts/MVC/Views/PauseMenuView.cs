using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TowerDefense.Controllers;

namespace TowerDefense.Views
{
    public class PauseMenuView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Button backFromSettingsButton;
        
        // Controllers reference
        private GameController gameController;
        private LevelController levelController;
        private SettingsController settingsController;
        
        private bool isPaused = false;
        
        private void Start()
        {
            // Get controller references
            gameController = GameController.Instance;
            levelController = LevelController.Instance;
            settingsController = SettingsController.Instance;
            
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
            if (restartButton != null && levelController != null)
            {
                restartButton.onClick.AddListener(() => {
                    ResumeGame(); // Ensure time scale is restored
                    levelController.RestartLevel();
                });
            }
            
            // Main menu button
            if (mainMenuButton != null && levelController != null)
            {
                mainMenuButton.onClick.AddListener(() => {
                    ResumeGame(); // Ensure time scale is restored
                    levelController.ReturnToMainMenu();
                });
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
            // Don't pause if game is already over
            if (gameController != null && gameController.IsGameOver())
            {
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
            PauseAudio(true);
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
            PauseAudio(false);
            
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
        
        private void PauseAudio(bool pause)
        {
            // Find audio manager
            AudioManager audioManager = FindObjectOfType<AudioManager>();
            if (audioManager != null)
            {
                audioManager.PauseAllAudio(pause);
            }
        }
    }
} 