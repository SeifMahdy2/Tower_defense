using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TowerDefense.Controllers;

namespace TowerDefense.Views
{
    public class HUDView : MonoBehaviour
    {
        [Header("Game Speed")]
        [SerializeField] private Button normalSpeedButton;
        [SerializeField] private Button fastSpeedButton;
        [SerializeField] private Button superSpeedButton;
        
        [Header("Info Display")]
        [SerializeField] private TextMeshProUGUI waveInfoText;
        [SerializeField] private TextMeshProUGUI enemiesRemainingText;
        [SerializeField] private TextMeshProUGUI goldInfoText;
        [SerializeField] private TextMeshProUGUI healthInfoText;
        
        // Controllers reference
        private GameController gameController;
        private WaveController waveController;
        
        // Speed multipliers
        private float normalSpeed = 1f;
        private float fastSpeed = 2f;
        private float superSpeed = 3f;
        
        private void Start()
        {
            // Get controller references
            gameController = GameController.Instance;
            waveController = FindObjectOfType<WaveController>();
            
            // Setup speed buttons
            SetupSpeedButtons();
            
            // Set normal speed by default
            SetGameSpeed(normalSpeed);
            UpdateSpeedButtonsVisual(normalSpeed);
            
            // Subscribe to controller events
            if (gameController != null)
            {
                gameController.OnGameStateChanged += UpdateGameInfo;
            }
            
            if (waveController != null)
            {
                waveController.OnWaveStateChanged += UpdateWaveInfo;
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (gameController != null)
            {
                gameController.OnGameStateChanged -= UpdateGameInfo;
            }
            
            if (waveController != null)
            {
                waveController.OnWaveStateChanged -= UpdateWaveInfo;
            }
        }
        
        private void SetupSpeedButtons()
        {
            if (normalSpeedButton != null)
            {
                normalSpeedButton.onClick.AddListener(() => {
                    SetGameSpeed(normalSpeed);
                    UpdateSpeedButtonsVisual(normalSpeed);
                });
            }
            
            if (fastSpeedButton != null)
            {
                fastSpeedButton.onClick.AddListener(() => {
                    SetGameSpeed(fastSpeed);
                    UpdateSpeedButtonsVisual(fastSpeed);
                });
            }
            
            if (superSpeedButton != null)
            {
                superSpeedButton.onClick.AddListener(() => {
                    SetGameSpeed(superSpeed);
                    UpdateSpeedButtonsVisual(superSpeed);
                });
            }
        }
        
        private void SetGameSpeed(float speed)
        {
            // Set game time scale
            Time.timeScale = speed;
        }
        
        private void UpdateSpeedButtonsVisual(float currentSpeed)
        {
            // Update button visuals to indicate which speed is active
            if (normalSpeedButton != null)
            {
                normalSpeedButton.interactable = currentSpeed != normalSpeed;
            }
            
            if (fastSpeedButton != null)
            {
                fastSpeedButton.interactable = currentSpeed != fastSpeed;
            }
            
            if (superSpeedButton != null)
            {
                superSpeedButton.interactable = currentSpeed != superSpeed;
            }
        }
        
        // Update game info text (gold, health)
        private void UpdateGameInfo()
        {
            // Update gold info
            if (goldInfoText != null && gameController != null)
            {
                goldInfoText.text = "Gold: " + gameController.GetCurrentGold();
            }
            
            // Update health info
            if (healthInfoText != null && gameController != null)
            {
                healthInfoText.text = "Health: " + gameController.GetCurrentHealth();
            }
        }
        
        // Update wave info text
        private void UpdateWaveInfo()
        {
            // Update wave info
            if (waveInfoText != null && waveController != null)
            {
                waveInfoText.text = "Wave: " + (waveController.GetCurrentWaveIndex() + 1) + "/" + waveController.GetTotalWaves();
            }
            
            // Update enemies remaining
            if (enemiesRemainingText != null && waveController != null)
            {
                enemiesRemainingText.text = "Enemies: " + waveController.GetRemainingEnemies();
            }
        }
        
        private void Update()
        {
            // Update HUD info each frame (for real-time updates)
            UpdateGameInfo();
            UpdateWaveInfo();
        }
    }
} 