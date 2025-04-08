using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
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
    
    private WaveSpawner waveSpawner;
    private GameManager gameManager;
    
    // Speed multipliers
    private float normalSpeed = 1f;
    private float fastSpeed = 2f;
    private float superSpeed = 3f;
    
    private void Start()
    {
        // Get references
        waveSpawner = FindObjectOfType<WaveSpawner>();
        gameManager = FindObjectOfType<GameManager>();
        
        // Setup speed buttons
        SetupSpeedButtons();
        
        // Set normal speed by default
        SetGameSpeed(normalSpeed);
        UpdateSpeedButtonsVisual(normalSpeed);
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
    
    private void Update()
    {
        // Update HUD info each frame
        UpdateHUDInfo();
    }
    
    private void UpdateHUDInfo()
    {
        // Update wave info
        if (waveInfoText != null && waveSpawner != null)
        {
            waveInfoText.text = "Wave: " + (waveSpawner.GetCurrentWaveIndex() + 1) + "/" + waveSpawner.GetTotalWaves();
        }
        
        // Update enemies remaining
        if (enemiesRemainingText != null && waveSpawner != null)
        {
            enemiesRemainingText.text = "Enemies: " + waveSpawner.GetRemainingEnemies();
        }
        
        // Update gold info
        if (goldInfoText != null && gameManager != null)
        {
            goldInfoText.text = "Gold: " + gameManager.GetCurrentGold();
        }
        
        // Update health info
        if (healthInfoText != null && gameManager != null)
        {
            healthInfoText.text = "Health: " + gameManager.GetCurrentHealth();
        }
    }
} 