using UnityEngine;
using TMPro;
using System.Collections;

namespace TD
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private int startingGold = 300;
        [SerializeField] private int startingHealth = 100;

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI goldText;

        [Header("Game Over")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject victoryPanel;

        [Header("Visual Effects")]
        [SerializeField] private Transform basePosition;
        [SerializeField] private float hitFlashDuration = 0.2f;
        [SerializeField] private Color hitColor = Color.red;

        private int currentGold;
        private int currentHealth;
        private int waveNumber = 0;
        private bool gameIsOver = false;
        private EnemySpawner enemySpawner;
        private int previousGold;
        private SpriteRenderer baseSpriteRenderer;
        private Color originalBaseColor;

        void Start()
        {
            // Set starting gold based on level
            string currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (currentLevel.Contains("Level_2"))
            {
                startingGold = 350; // Higher starting gold for Level 2
            }

            // Initialize game state
            currentGold = startingGold;
            previousGold = currentGold;
            currentHealth = startingHealth;
            
            // Get base renderer for visual effects
            if (basePosition != null)
            {
                baseSpriteRenderer = basePosition.GetComponent<SpriteRenderer>();
                if (baseSpriteRenderer != null)
                    originalBaseColor = baseSpriteRenderer.color;
            }
            
            // Get references
            enemySpawner = FindObjectOfType<EnemySpawner>();
            
            // Initialize UI
            UpdateUI();
            
            // Hide end game panels
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
                
            if (victoryPanel != null)
                victoryPanel.SetActive(false);
            
            // Schedule regular UI updates
            InvokeRepeating("ForceUIUpdate", 1f, 1f);
        }

        void LateUpdate()
        {
            UpdateUISilently();
        }

        public void EnemyReachedEnd(int damageAmount = 1)
        {
            if (gameIsOver)
                return;
                
            // Apply damage to base
            currentHealth -= damageAmount;
            
            // Prevent negative health
            if (currentHealth < 0)
                currentHealth = 0;
            
            // Show damage visual effects
            if (baseSpriteRenderer != null)
            {
                StopAllCoroutines();
                StartCoroutine(FlashEffect());
            }
            
            // Update UI and check game over
            UpdateUI();
            
            if (currentHealth <= 0)
                GameOver(false);
        }
        
        IEnumerator FlashEffect()
        {
            baseSpriteRenderer.color = hitColor;
            yield return new WaitForSeconds(hitFlashDuration);
            baseSpriteRenderer.color = originalBaseColor;
        }

        public void AddGold(int amount)
        {
            if (gameIsOver)
                return;
            
            previousGold = currentGold;
            currentGold += amount;
            UpdateUI();
        }

        public bool SpendGold(int amount)
        {
            if (gameIsOver || currentGold < amount)
                return false;
                
            previousGold = currentGold;
            currentGold -= amount;
            UpdateUI();
            return true;
        }

        public void WaveCompleted()
        {
            if (gameIsOver)
                return;
                
            waveNumber++;
            
            // Add bonus gold for completing wave
            int waveBonus = 0;
            
            if (waveNumber == 1)
                waveBonus = 100; // First wave bonus: 100 gold
            else if (waveNumber == 2)
                waveBonus = 150; // Second wave bonus: 150 gold
            else
                waveBonus = 200; // Final wave bonus (higher reward)
            
            AddGold(waveBonus);
            
            // Show notification of bonus
            StartCoroutine(ShowBonusNotification(waveNumber, waveBonus));
            
            // Check for victory condition - only when final wave is completed and all enemies are eliminated
            if (enemySpawner != null && waveNumber == enemySpawner.GetTotalWaves() && !enemySpawner.IsWaveInProgress())
            {
                ShowVictoryNotification();
            }
        }

        private IEnumerator ShowBonusNotification(int waveNum, int bonus)
        {
            // You can implement a UI notification here if desired
            Debug.Log($"Wave {waveNum} completed! Bonus: {bonus} gold");
            yield return null;
        }

        private void ShowVictoryNotification()
        {
            if (victoryPanel == null)
                return;
                
            victoryPanel.SetActive(true);
            
            // Save level completion
            SaveLevelCompletion();
            
            // Stop enemies
            DespawnAllEnemies();
            
            // Pause the game to prevent further actions
            Time.timeScale = 0.0f;
            
            // Remove auto-hide functionality - panel will stay until user makes a choice
            // Make sure your victory panel has buttons like "Next Level" and "Main Menu"
        }

        // Add a method to save level completion status
        private void SaveLevelCompletion()
        {
            // Get current level number from scene name
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (currentScene.Contains("Level_"))
            {
                int currentLevel = 1;
                if (int.TryParse(currentScene.Substring(currentScene.IndexOf("_") + 1), out currentLevel))
                {
                    Debug.Log("Saving completion for Level " + currentLevel);
                    
                    // Save via PlayerProgressManager if available
                    if (PlayerProgressManager.Instance != null)
                    {
                        PlayerProgressManager.Instance.SetLevelCompleted(currentLevel);
                    }
                    else
                    {
                        // Direct fallback
                        PlayerPrefs.SetInt("Level_" + currentLevel + "_Completed", 1);
                        PlayerPrefs.Save();
                    }
                    
                    // Verify save
                    int saved = PlayerPrefs.GetInt("Level_" + currentLevel + "_Completed", 0);
                    Debug.Log("Level " + currentLevel + " completion status saved: " + saved);
                }
            }
        }

        private void UpdateUISilently()
        {
            GameObject goldObj = GameObject.Find("Gold_Overall");
            if (goldObj == null)
                return;
                
            TextMeshProUGUI goldComp = goldObj.GetComponent<TextMeshProUGUI>();
            if (goldComp != null)
            {
                goldComp.text = currentGold.ToString();
                goldComp.SetText(currentGold.ToString());
                Canvas.ForceUpdateCanvases();
            }
        }
        
        private void UpdateUI()
        {
            GameObject goldObj = GameObject.Find("Gold_Overall");
            if (goldObj == null)
                return;
                
            TextMeshProUGUI goldComp = goldObj.GetComponent<TextMeshProUGUI>();
            if (goldComp != null)
            {
                goldComp.text = currentGold.ToString();
                goldComp.SetText(currentGold.ToString());
                Canvas.ForceUpdateCanvases();
                
                if (previousGold != currentGold)
                    previousGold = currentGold;
            }
        }
        
        public void ForceUIUpdate()
        {
            // Find gold text if not assigned
            if (goldText == null)
            {
                GameObject goldObj = GameObject.Find("Gold_Overall");
                if (goldObj != null)
                    goldText = goldObj.GetComponent<TextMeshProUGUI>();
            }
            
            UpdateUI();
        }

        private void GameOver(bool victory)
        {
            gameIsOver = true;
            
            // Stop enemy spawning
            DespawnAllEnemies();
            
            // Show appropriate panel
            if (victory && victoryPanel != null)
                victoryPanel.SetActive(true);
            else if (!victory && gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                Time.timeScale = 0.5f; // Slow down for defeat
            }
        }
        
        private void DespawnAllEnemies()
        {
            // Use dedicated method if available
            if (enemySpawner != null)
            {
                enemySpawner.StopAndClearEnemies();
                return;
            }
            
            // Fallback: Destroy all enemies
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
                Destroy(enemy);
        }

        // Getters for other systems
        public int GetCurrentGold() { return currentGold; }
        public int GetWaveNumber() { return waveNumber; }
        public int GetHealth() { return currentHealth; }
        public int GetMaxHealth() { return startingHealth; }

        // Add these public methods for buttons on the victory panel
        public void OnVictoryNextLevel()
        {
            // Resume normal time scale
            Time.timeScale = 1.0f;
            
            // Hide victory panel
            if (victoryPanel != null)
                victoryPanel.SetActive(false);
            
            // Get current scene name and extract level number
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (currentScene.Contains("Level_"))
            {
                int currentLevel = 1;
                int.TryParse(currentScene.Substring(currentScene.IndexOf("_") + 1), out currentLevel);
                
                // Load next level
                string nextScene = "Level_" + (currentLevel + 1);
                SceneLoader.Instance.LoadScene(nextScene);
            }
            else
            {
                // Default to main menu if can't determine next level
                OnVictoryMainMenu();
            }
        }

        public void OnVictoryMainMenu()
        {
            // Resume normal time scale
            Time.timeScale = 1.0f;
            
            // Hide victory panel
            if (victoryPanel != null)
                victoryPanel.SetActive(false);
            
            // Load main menu
            SceneLoader.Instance.LoadScene("Main_Menu");
        }

        public void OnVictoryRetry()
        {
            // Resume normal time scale
            Time.timeScale = 1.0f;
            
            // Hide victory panel
            if (victoryPanel != null)
                victoryPanel.SetActive(false);
            
            // Reload current level
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            SceneLoader.Instance.LoadScene(currentScene);
        }
    }
} 