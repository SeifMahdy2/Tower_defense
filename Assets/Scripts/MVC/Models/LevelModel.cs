using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Models
{
    [System.Serializable]
    public class LevelData
    {
        public string LevelName;
        public int LevelIndex;
        public string SceneName;
        public bool IsUnlocked;
        public int NextLevelIndex;
        public bool IsLastLevel;
    }
    
    [System.Serializable]
    public class LevelModel
    {
        // Level properties
        public LevelData[] Levels { get; private set; }
        public int CurrentLevelIndex { get; private set; }
        public bool LevelCompleted { get; private set; }
        
        // Events
        public delegate void LevelStateChanged();
        public event LevelStateChanged OnLevelStateChanged;
        
        public delegate void LevelCompleted(int levelIndex);
        public event LevelCompleted OnLevelCompleted;
        
        // Constructor
        public LevelModel(LevelData[] levels)
        {
            this.Levels = levels;
            
            // Initialize
            CurrentLevelIndex = 0;
            LevelCompleted = false;
            LoadUnlockedLevels();
        }
        
        // Load information about which levels are unlocked
        private void LoadUnlockedLevels()
        {
            // Get highest unlocked level from PlayerPrefs
            int highestUnlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
            
            // Update level data
            for (int i = 0; i < Levels.Length; i++)
            {
                Levels[i].IsUnlocked = Levels[i].LevelIndex <= highestUnlockedLevel;
            }
            
            NotifyLevelStateChanged();
        }
        
        // Set current level
        public void SetCurrentLevel(int levelIndex)
        {
            // Find the level data for this index
            for (int i = 0; i < Levels.Length; i++)
            {
                if (Levels[i].LevelIndex == levelIndex)
                {
                    CurrentLevelIndex = i;
                    LevelCompleted = false;
                    break;
                }
            }
            
            NotifyLevelStateChanged();
        }
        
        // Complete the current level
        public void CompleteCurrentLevel()
        {
            if (LevelCompleted) return;
            
            LevelCompleted = true;
            
            // Get current level data
            LevelData currentLevel = GetCurrentLevelData();
            
            if (currentLevel != null)
            {
                // Unlock next level if not already unlocked and there is a next level
                if (!currentLevel.IsLastLevel && currentLevel.NextLevelIndex > 0)
                {
                    UnlockLevel(currentLevel.NextLevelIndex);
                }
                
                // Trigger event
                if (OnLevelCompleted != null)
                {
                    OnLevelCompleted(currentLevel.LevelIndex);
                }
            }
            
            NotifyLevelStateChanged();
        }
        
        // Unlock a specific level
        public void UnlockLevel(int levelIndex)
        {
            // Get currently unlocked level from PlayerPrefs
            int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
            
            // Unlock next level if not already unlocked
            if (levelIndex > unlockedLevel)
            {
                PlayerPrefs.SetInt("UnlockedLevel", levelIndex);
                PlayerPrefs.Save();
                
                // Update level data
                LoadUnlockedLevels();
            }
        }
        
        // Get current level data
        public LevelData GetCurrentLevelData()
        {
            if (CurrentLevelIndex >= 0 && CurrentLevelIndex < Levels.Length)
            {
                return Levels[CurrentLevelIndex];
            }
            
            return null;
        }
        
        // Get level data by index
        public LevelData GetLevelData(int levelIndex)
        {
            for (int i = 0; i < Levels.Length; i++)
            {
                if (Levels[i].LevelIndex == levelIndex)
                {
                    return Levels[i];
                }
            }
            
            return null;
        }
        
        // Get all available levels
        public LevelData[] GetAllLevels()
        {
            return Levels;
        }
        
        // Get all unlocked levels
        public LevelData[] GetUnlockedLevels()
        {
            List<LevelData> unlockedLevels = new List<LevelData>();
            
            foreach (LevelData level in Levels)
            {
                if (level.IsUnlocked)
                {
                    unlockedLevels.Add(level);
                }
            }
            
            return unlockedLevels.ToArray();
        }
        
        // Notify subscribers of level state change
        private void NotifyLevelStateChanged()
        {
            if (OnLevelStateChanged != null)
            {
                OnLevelStateChanged();
            }
        }
    }
} 