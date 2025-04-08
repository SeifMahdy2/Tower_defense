using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using TowerDefense.Controllers;

namespace TowerDefense.Views
{
    public class GameOverView : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private TextMeshProUGUI wavesSurvivedText;
        [SerializeField] private TextMeshProUGUI enemiesDefeatedText;
        
        // Controllers reference
        private GameController gameController;
        private WaveController waveController;
        private LevelController levelController;
        
        private void Awake()
        {
            // Get controller references
            gameController = GameController.Instance;
            waveController = FindObjectOfType<WaveController>();
            levelController = LevelController.Instance;
            
            // Hide by default
            gameObject.SetActive(false);
            
            // Subscribe to game over event
            if (gameController != null)
            {
                gameController.OnGameOver += HandleGameOver;
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (gameController != null)
            {
                gameController.OnGameOver -= HandleGameOver;
            }
        }
        
        private void HandleGameOver()
        {
            // Show game over screen
            gameObject.SetActive(true);
            
            // Pause game
            Time.timeScale = 0f;
            
            // Setup UI
            SetupUI();
            
            // Setup buttons
            SetupButtons();
        }
        
        private void SetupUI()
        {
            // Display waves survived
            if (wavesSurvivedText != null && waveController != null)
            {
                int wavesCompleted = waveController.GetCurrentWaveIndex();
                wavesSurvivedText.text = "Waves Survived: " + wavesCompleted;
            }
            
            // Display enemies defeated
            if (enemiesDefeatedText != null && gameController != null)
            {
                int enemiesDefeated = gameController.GetEnemiesDefeated();
                enemiesDefeatedText.text = "Enemies Defeated: " + enemiesDefeated;
            }
        }
        
        private void SetupButtons()
        {
            // Restart button
            if (restartButton != null && levelController != null)
            {
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(() => {
                    Time.timeScale = 1f; // Resume normal time
                    levelController.RestartLevel();
                });
            }
            
            // Main menu button
            if (mainMenuButton != null && levelController != null)
            {
                mainMenuButton.onClick.RemoveAllListeners();
                mainMenuButton.onClick.AddListener(() => {
                    Time.timeScale = 1f; // Resume normal time
                    levelController.ReturnToMainMenu();
                });
            }
        }
    }
} 