using UnityEngine;
using TMPro;
using System.Collections;

namespace TD
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private int startingGold = 500;
        [SerializeField] private int startingHealth = 100;

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI goldText;

        [Header("Game Over")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject victoryPanel;

        [Header("Visual Effects")]
        [SerializeField] private GameObject baseDamageEffect;
        [SerializeField] private Transform basePosition;
        [SerializeField] private float hitFlashDuration = 0.2f;
        [SerializeField] private Color hitColor = Color.red;

        private int currentGold;
        private int currentHealth;
        private int waveNumber = 0;
        private bool gameIsOver = false;
        
        // Track previous gold value to avoid excessive logging
        private int previousGold;
        
        // Reference to the base sprite renderer for flash effect
        private SpriteRenderer baseSpriteRenderer;
        private Color originalBaseColor;

        void Start()
        {
            // Initialize game state
            currentGold = startingGold;
            previousGold = currentGold; // Initialize previous gold
            currentHealth = startingHealth;
            
            // Get base sprite renderer if it exists
            if (basePosition != null)
            {
                baseSpriteRenderer = basePosition.GetComponent<SpriteRenderer>();
                if (baseSpriteRenderer != null)
                {
                    originalBaseColor = baseSpriteRenderer.color;
                }
            }
            
            // Update UI
            UpdateUI();
            
            // Hide end game panels initially
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
                
            if (victoryPanel != null)
                victoryPanel.SetActive(false);
                
            // Log initial values for debugging
            Debug.Log("GameManager initialized: Gold=" + currentGold + ", Health=" + currentHealth);
            
            // Force an immediate update of all UI elements
            ForceUIUpdate();
            
            // Schedule regular UI updates to ensure sync
            InvokeRepeating("ForceUIUpdate", 1f, 1f);
        }
        
        // Called every frame to ensure UI stays in sync but no logging
        void Update()
        {
            // Only update UI, no need to call this every frame
            // Keep for now for robustness, but without logging
        }

        // Late update ensures this runs after all other Updates
        void LateUpdate()
        {
            // Update UI silently without logging
            UpdateUISilently();
        }

        public void EnemyReachedEnd(int damageAmount = 1)
        {
            if (gameIsOver)
                return;
                
            Debug.Log("Enemy reached end! Applying " + damageAmount + " damage to base");
                
            // Decrease player health when enemy reaches the end
            currentHealth -= damageAmount;
            
            // Ensure health doesn't go below 0
            if (currentHealth < 0)
                currentHealth = 0;
            
            // Show damage effect at base
            if (baseSpriteRenderer != null)
            {
                StopAllCoroutines();
                StartCoroutine(FlashEffect());
                Debug.Log("Base damage flash effect started");
            }
            else if (baseDamageEffect != null && basePosition != null)
            {
                GameObject effect = Instantiate(baseDamageEffect, basePosition.position, Quaternion.identity);
                Destroy(effect, 2f);
                Debug.Log("Base damage effect created at " + basePosition.position);
            }
            else
            {
                Debug.LogWarning("Base damage effect or base position is null! Visual effect won't be shown.");
            }
            
            // Update UI with new health
            UpdateUI();
            
            Debug.Log("Base health reduced to " + currentHealth + "/" + startingHealth);
            
            // Check for game over
            if (currentHealth <= 0)
            {
                GameOver(false);
            }
        }
        
        IEnumerator FlashEffect()
        {
            // Change to hit color
            baseSpriteRenderer.color = hitColor;
            
            // Wait for duration
            yield return new WaitForSeconds(hitFlashDuration);
            
            // Change back to original color
            baseSpriteRenderer.color = originalBaseColor;
        }

        public void AddGold(int amount)
        {
            if (gameIsOver)
                return;
            
            // Store previous gold
            previousGold = currentGold;
            
            // Add gold
            currentGold += amount;
            
            // Only log if gold changed
            if (previousGold != currentGold) {
                Debug.Log("Added " + amount + " gold. New total: " + currentGold);
            }
            
            UpdateUI();
        }

        public bool SpendGold(int amount)
        {
            if (gameIsOver)
                return false;
                
            if (currentGold >= amount)
            {
                // Store previous gold
                previousGold = currentGold;
                
                // Spend gold
                currentGold -= amount;
                
                // Only log if gold changed
                if (previousGold != currentGold) {
                    Debug.Log("Spent " + amount + " gold. Remaining: " + currentGold);
                }
                
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
            
            // Show victory panel after completing exactly wave 3
            if (waveNumber == 3)
            {
                // Show victory notification without ending the game
                ShowVictoryNotification();
            }
        }

        // Show victory notification without ending game
        private void ShowVictoryNotification()
        {
            Debug.Log("All waves completed! Victory achieved but gameplay continues.");
            
            // Show victory panel
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
                
                // Optional: Hide the panel after a few seconds
                StartCoroutine(HideVictoryPanelAfterDelay(5f));
            }
        }

        private IEnumerator HideVictoryPanelAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(false);
            }
        }

        // A silent version that doesn't log anything
        private void UpdateUISilently()
        {
            // Find the gold text directly each time to ensure we're getting the right object
            GameObject goldTextObject = GameObject.Find("Gold_Overall");
            if (goldTextObject != null)
            {
                TextMeshProUGUI goldTextComponent = goldTextObject.GetComponent<TextMeshProUGUI>();
                if (goldTextComponent != null)
                {
                    goldTextComponent.text = currentGold.ToString();
                    goldTextComponent.SetText(currentGold.ToString());  // Force text update with SetText method
                    
                    // Force a canvas refresh using the static method
                    Canvas.ForceUpdateCanvases();
                }
            }
        }
        
        private void UpdateUI()
        {
            // Find the gold text directly each time to ensure we're getting the right object
            GameObject goldTextObject = GameObject.Find("Gold_Overall");
            if (goldTextObject != null)
            {
                TextMeshProUGUI goldTextComponent = goldTextObject.GetComponent<TextMeshProUGUI>();
                if (goldTextComponent != null)
                {
                    goldTextComponent.text = currentGold.ToString();
                    goldTextComponent.SetText(currentGold.ToString());  // Force text update with SetText method
                    
                    // Force a canvas refresh using the static method
                    Canvas.ForceUpdateCanvases();
                    
                    // Only log if gold actually changed to reduce console spam
                    if (previousGold != currentGold) {
                        Debug.Log("Updated gold display to: " + currentGold);
                        previousGold = currentGold;
                    }
                }
                else
                {
                    Debug.LogError("Found Gold_Overall object but it has no TextMeshProUGUI component!");
                }
            }
            else
            {
                Debug.LogError("Could not find Gold_Overall object in scene!");
            }
        }
        
        // This method forces an update of all UI elements
        // It can be called from other scripts to ensure UI is in sync
        public void ForceUIUpdate()
        {
            // Find the gold text if it's not assigned
            if (goldText == null)
            {
                // Try to find by name
                GameObject goldTextObj = GameObject.Find("Gold_Overall");
                if (goldTextObj != null)
                {
                    goldText = goldTextObj.GetComponent<TextMeshProUGUI>();
                    // Only log once when found
                    if (goldText != null) {
                        Debug.Log("Found Gold_Overall text component");
                    }
                }
            }
            
            // Update the UI
            UpdateUI();
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
                
                // Only slow down time for defeat, not for victory
                Time.timeScale = 0.5f;
            }
            
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