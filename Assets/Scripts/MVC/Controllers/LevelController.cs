using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TowerDefense.Models;

namespace TowerDefense.Controllers
{
    public class LevelController : MonoBehaviour
    {
        [Header("Level Settings")]
        [SerializeField] private LevelData[] levelData;
        [SerializeField] private bool isLastLevel = false;
        [SerializeField] private int nextLevelIndex = 0; // 0 to return to menu
        
        // Models
        private LevelModel levelModel;
        
        // Singleton instance
        public static LevelController Instance { get; private set; }
        
        // Events relayed from model
        public delegate void LevelStateChanged();
        public event LevelStateChanged OnLevelStateChanged;
        
        public delegate void LevelCompleted(int levelIndex);
        public event LevelCompleted OnLevelCompleted;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            // Initialize model if level data is provided
            if (levelData == null || levelData.Length == 0)
            {
                // Create a default single level data if none provided
                levelData = new LevelData[1];
                levelData[0] = new LevelData
                {
                    LevelName = SceneManager.GetActiveScene().name,
                    LevelIndex = SceneManager.GetActiveScene().buildIndex,
                    SceneName = SceneManager.GetActiveScene().name,
                    IsUnlocked = true,
                    NextLevelIndex = nextLevelIndex,
                    IsLastLevel = isLastLevel
                };
            }
            
            levelModel = new LevelModel(levelData);
            
            // Subscribe to model events
            levelModel.OnLevelStateChanged += HandleLevelStateChanged;
            levelModel.OnLevelCompleted += HandleLevelCompleted;
            
            // Subscribe to wave completion
            WaveController.OnWaveCompleted += CheckForLevelCompletion;
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from model events
            if (levelModel != null)
            {
                levelModel.OnLevelStateChanged -= HandleLevelStateChanged;
                levelModel.OnLevelCompleted -= HandleLevelCompleted;
            }
            
            // Unsubscribe from wave completion
            WaveController.OnWaveCompleted -= CheckForLevelCompletion;
        }
        
        // Relay level state changed event
        private void HandleLevelStateChanged()
        {
            if (OnLevelStateChanged != null)
            {
                OnLevelStateChanged();
            }
        }
        
        // Relay level completed event
        private void HandleLevelCompleted(int levelIndex)
        {
            if (OnLevelCompleted != null)
            {
                OnLevelCompleted(levelIndex);
            }
        }
        
        // Check if level should be completed when waves are done
        private void CheckForLevelCompletion(int waveNumber)
        {
            WaveController waveController = FindObjectOfType<WaveController>();
            
            if (waveController != null)
            {
                // Check if all waves are completed
                if (waveNumber >= waveController.GetTotalWaves() - 1)
                {
                    // Level completed
                    CompleteLevel();
                }
            }
        }
        
        // Complete the current level
        public void CompleteLevel()
        {
            levelModel.CompleteCurrentLevel();
        }
        
        // Load next level
        public void LoadNextLevel()
        {
            // Get current level data
            LevelData currentLevel = levelModel.GetCurrentLevelData();
            
            if (currentLevel == null)
            {
                Debug.LogError("No current level data found!");
                return;
            }
            
            // If this is the last level, return to main menu
            if (currentLevel.IsLastLevel || currentLevel.NextLevelIndex <= 0)
            {
                ReturnToMainMenu();
                return;
            }
            
            // Find level data for next level
            LevelData nextLevel = levelModel.GetLevelData(currentLevel.NextLevelIndex);
            
            if (nextLevel != null)
            {
                // Load next level scene
                SceneManager.LoadScene(nextLevel.SceneName);
            }
            else
            {
                Debug.LogWarning("Next level data not found!");
                // Fallback to scene index
                if (currentLevel.NextLevelIndex > 0)
                {
                    SceneManager.LoadScene("Level" + currentLevel.NextLevelIndex);
                }
            }
        }
        
        // Restart current level
        public void RestartLevel()
        {
            // Reload current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        // Return to main menu
        public void ReturnToMainMenu()
        {
            // Load main menu
            SceneManager.LoadScene("MainMenu");
        }
        
        // Get level info
        public LevelData GetCurrentLevelData()
        {
            return levelModel.GetCurrentLevelData();
        }
        
        public LevelData[] GetAllLevels()
        {
            return levelModel.GetAllLevels();
        }
        
        public LevelData[] GetUnlockedLevels()
        {
            return levelModel.GetUnlockedLevels();
        }
        
        public bool IsLevelCompleted()
        {
            return levelModel.LevelCompleted;
        }
    }
} 