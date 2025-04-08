using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefense.Models;

namespace TowerDefense.Controllers
{
    public class GameController : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private int startingGold = 150;
        [SerializeField] private int startingHealth = 100;
        
        // Models
        private GameModel gameModel;
        
        // Singleton instance
        public static GameController Instance { get; private set; }
        
        // Events relayed from model
        public delegate void GameStateChanged();
        public event GameStateChanged OnGameStateChanged;
        
        public delegate void GameOverTriggered();
        public event GameOverTriggered OnGameOver;
        
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
            
            // Initialize model
            gameModel = new GameModel(startingGold, startingHealth);
            
            // Subscribe to model events
            gameModel.OnGameStateChanged += HandleGameStateChanged;
            gameModel.OnGameOver += HandleGameOver;
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from model events
            if (gameModel != null)
            {
                gameModel.OnGameStateChanged -= HandleGameStateChanged;
                gameModel.OnGameOver -= HandleGameOver;
            }
        }
        
        // Relay game state changed event
        private void HandleGameStateChanged()
        {
            if (OnGameStateChanged != null)
            {
                OnGameStateChanged();
            }
        }
        
        // Relay game over event
        private void HandleGameOver()
        {
            if (OnGameOver != null)
            {
                OnGameOver();
            }
        }
        
        // Called when an enemy reaches the end of the path
        public void EnemyReachedEnd(int damage)
        {
            gameModel.EnemyReachedEnd(damage);
        }
        
        // Add gold (when killing enemies)
        public void AddGold(int amount)
        {
            gameModel.AddGold(amount);
        }
        
        // Increment enemies defeated counter
        public void IncrementEnemiesDefeated()
        {
            gameModel.IncrementEnemiesDefeated();
        }
        
        // Try to spend gold (for tower placement)
        public bool SpendGold(int amount)
        {
            return gameModel.SpendGold(amount);
        }
        
        // Check if player has enough gold
        public bool HasEnoughGold(int amount)
        {
            return gameModel.HasEnoughGold(amount);
        }
        
        // Reset the game
        public void RestartGame()
        {
            gameModel.ResetGame();
        }
        
        // Getters for model properties
        public int GetCurrentGold()
        {
            return gameModel.CurrentGold;
        }
        
        public int GetCurrentHealth()
        {
            return gameModel.CurrentHealth;
        }
        
        public int GetEnemiesDefeated()
        {
            return gameModel.EnemiesDefeated;
        }
        
        public int GetTotalGoldEarned()
        {
            return gameModel.TotalGoldEarned;
        }
        
        public bool IsGameOver()
        {
            return gameModel.IsGameOver;
        }
    }
} 