using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Models
{
    [System.Serializable]
    public class GameModel
    {
        // Game state
        public int CurrentGold { get; private set; }
        public int CurrentHealth { get; private set; }
        public int EnemiesDefeated { get; private set; }
        public int TotalGoldEarned { get; private set; }
        public bool IsGameOver { get; private set; }
        
        // Default settings
        private int startingGold;
        private int startingHealth;
        
        // Events
        public delegate void GameStateChanged();
        public event GameStateChanged OnGameStateChanged;
        
        public delegate void GameOverTriggered();
        public event GameOverTriggered OnGameOver;
        
        // Constructor with default values
        public GameModel(int startingGold = 150, int startingHealth = 100)
        {
            this.startingGold = startingGold;
            this.startingHealth = startingHealth;
            
            // Initialize game state
            ResetGame();
        }
        
        // Reset game state
        public void ResetGame()
        {
            CurrentGold = startingGold;
            CurrentHealth = startingHealth;
            EnemiesDefeated = 0;
            TotalGoldEarned = 0;
            IsGameOver = false;
            
            NotifyGameStateChanged();
        }
        
        // Called when an enemy reaches the end of the path
        public void EnemyReachedEnd(int damage)
        {
            if (IsGameOver) return;
            
            CurrentHealth -= damage;
            
            // Check if game over
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                GameOver();
            }
            
            NotifyGameStateChanged();
        }
        
        // Add gold (when killing enemies)
        public void AddGold(int amount)
        {
            if (IsGameOver) return;
            
            CurrentGold += amount;
            TotalGoldEarned += amount;
            
            NotifyGameStateChanged();
        }
        
        // Increment enemies defeated counter
        public void IncrementEnemiesDefeated()
        {
            EnemiesDefeated++;
            NotifyGameStateChanged();
        }
        
        // Try to spend gold (for tower placement)
        public bool SpendGold(int amount)
        {
            if (IsGameOver || CurrentGold < amount) return false;
            
            CurrentGold -= amount;
            NotifyGameStateChanged();
            return true;
        }
        
        // Check if player has enough gold
        public bool HasEnoughGold(int amount)
        {
            return CurrentGold >= amount;
        }
        
        // Game over handling
        private void GameOver()
        {
            IsGameOver = true;
            
            // Trigger game over event
            if (OnGameOver != null)
            {
                OnGameOver();
            }
            
            NotifyGameStateChanged();
        }
        
        // Notify subscribers of game state change
        private void NotifyGameStateChanged()
        {
            if (OnGameStateChanged != null)
            {
                OnGameStateChanged();
            }
        }
    }
} 