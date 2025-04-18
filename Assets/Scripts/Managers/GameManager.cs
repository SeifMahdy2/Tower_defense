using UnityEngine;
using TMPro;

namespace TD
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private int startingGold = 500;
        [SerializeField] private int startingHealth = 100;

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI healthText;

        [Header("Game Over")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject victoryPanel;

        [Header("Visual Effects")]
        [SerializeField] private GameObject baseDamageEffect;
        [SerializeField] private Transform basePosition;

        private int currentGold;
        private int currentHealth;
        private int waveNumber = 0;
        private bool gameIsOver = false;

        void Start()
        {
            // Initialize game state
            currentGold = startingGold;
            currentHealth = startingHealth;
            
            // Update UI
            UpdateUI();
            
            // Hide end game panels initially
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
                
            if (victoryPanel != null)
                victoryPanel.SetActive(false);
        }

        public void EnemyReachedEnd(int damageAmount = 1)
        {
            if (gameIsOver)
                return;
                
            // Decrease player health when enemy reaches the end
            currentHealth -= damageAmount;
            
            // Show damage effect at base
            if (baseDamageEffect != null && basePosition != null)
            {
                GameObject effect = Instantiate(baseDamageEffect, basePosition.position, Quaternion.identity);
                Destroy(effect, 2f);
            }
            
            UpdateUI();
            
            // Check for game over
            if (currentHealth <= 0)
            {
                GameOver(false);
            }
        }

        public void AddGold(int amount)
        {
            if (gameIsOver)
                return;
                
            currentGold += amount;
            UpdateUI();
        }

        public bool SpendGold(int amount)
        {
            if (gameIsOver)
                return false;
                
            if (currentGold >= amount)
            {
                currentGold -= amount;
                UpdateUI();
                return true;
            }
            return false;
        }

        public void WaveCompleted()
        {
            if (gameIsOver)
                return;
                
            waveNumber++;
            
            // Add bonus gold for completing a wave
            AddGold(waveNumber * 25);
            
            // Check for victory condition (e.g., after wave 10)
            if (waveNumber >= 10)
            {
                GameOver(true);
            }
        }

        private void UpdateUI()
        {
            if (goldText != null)
                goldText.text = currentGold.ToString();
                
            if (healthText != null)
                healthText.text = currentHealth.ToString();
        }

        private void GameOver(bool victory)
        {
            gameIsOver = true;
            
            // Show appropriate panel
            if (victory && victoryPanel != null)
            {
                victoryPanel.SetActive(true);
            }
            else if (!victory && gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
            
            // Pause or slow down game
            Time.timeScale = 0.5f;
            
            if (victory)
                Debug.Log("Victory!");
            else
                Debug.Log("Game Over!");
        }

        // For tower placement to check if player can afford a tower
        public int GetCurrentGold()
        {
            return currentGold;
        }
        
        // Get current wave number
        public int GetWaveNumber()
        {
            return waveNumber;
        }
        public int GetHealth() { return currentHealth; }
        public int GetMaxHealth() { return startingHealth; }
    }
} 