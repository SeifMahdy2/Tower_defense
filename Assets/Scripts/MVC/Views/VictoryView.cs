using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TowerDefense.Controllers;

namespace TowerDefense.Views
{
    public class VictoryView : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private TextMeshProUGUI enemiesDefeatedText;
        [SerializeField] private TextMeshProUGUI goldEarnedText;
        [SerializeField] private TextMeshProUGUI timeElapsedText;
        
        // Controllers reference
        private GameController gameController;
        private LevelController levelController;
        private WaveController waveController;
        
        // Level start time
        private float levelStartTime;
        
        private void Awake()
        {
            // Get controller references
            gameController = GameController.Instance;
            levelController = LevelController.Instance;
            waveController = FindObjectOfType<WaveController>();
            
            // Record level start time
            levelStartTime = Time.time;
            
            // Hide by default
            gameObject.SetActive(false);
            
            // Subscribe to level completion event
            if (waveController != null)
            {
                waveController.OnAllWavesCompleted += HandleVictory;
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (waveController != null)
            {
                waveController.OnAllWavesCompleted -= HandleVictory;
            }
        }
        
        private void HandleVictory()
        {
            // Only process if game is not over
            if (gameController != null && gameController.IsGameOver())
            {
                return;
            }
            
            // Complete the level in the controller
            if (levelController != null)
            {
                levelController.CompleteLevel();
            }
            
            // Show victory screen
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
            // Show enemies defeated
            if (enemiesDefeatedText != null && gameController != null)
            {
                enemiesDefeatedText.text = "Enemies Defeated: " + gameController.GetEnemiesDefeated();
            }
            
            // Show gold earned
            if (goldEarnedText != null && gameController != null)
            {
                goldEarnedText.text = "Gold Earned: " + gameController.GetTotalGoldEarned();
            }
            
            // Show time elapsed
            if (timeElapsedText != null)
            {
                float timeElapsed = Time.time - levelStartTime;
                int minutes = Mathf.FloorToInt(timeElapsed / 60f);
                int seconds = Mathf.FloorToInt(timeElapsed % 60f);
                timeElapsedText.text = "Time: " + minutes.ToString("00") + ":" + seconds.ToString("00");
            }
        }
        
        private void SetupButtons()
        {
            // Next level button
            if (nextLevelButton != null && levelController != null)
            {
                nextLevelButton.onClick.RemoveAllListeners();
                nextLevelButton.onClick.AddListener(() => {
                    Time.timeScale = 1f; // Resume normal time
                    levelController.LoadNextLevel();
                });
                
                // Check if this is the last level
                var levelData = levelController.GetCurrentLevelData();
                if (levelData != null && (levelData.IsLastLevel || levelData.NextLevelIndex <= 0))
                {
                    // No next level, hide the button or change its text
                    if (nextLevelButton.GetComponentInChildren<TextMeshProUGUI>() != null)
                    {
                        nextLevelButton.GetComponentInChildren<TextMeshProUGUI>().text = "Continue";
                    }
                }
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