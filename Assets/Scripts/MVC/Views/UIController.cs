using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameHUDPanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject towerInfoPanel;
    
    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI enemiesRemainingText;
    [SerializeField] private Button startWaveButton;
    
    [Header("Tower Selection UI")]
    [SerializeField] private Button archerTowerButton;
    [SerializeField] private Button mageTowerButton;
    [SerializeField] private Button frostTowerButton;
    [SerializeField] private Button cannonTowerButton;
    
    [Header("Tower Info Panel")]
    [SerializeField] private TextMeshProUGUI towerNameText;
    [SerializeField] private TextMeshProUGUI towerLevelText;
    [SerializeField] private TextMeshProUGUI towerDamageText;
    [SerializeField] private TextMeshProUGUI towerRangeText;
    [SerializeField] private TextMeshProUGUI towerSpeedText;
    [SerializeField] private TextMeshProUGUI upgradeButtonText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button sellButton;
    
    [Header("References")]
    [SerializeField] private TowerPlacementController towerPlacement;
    
    private void Start()
    {
        // Register for events
        GameManager.OnGoldChanged.AddListener(UpdateGoldText);
        GameManager.OnLivesChanged.AddListener(UpdateLivesText);
        GameManager.OnWaveChanged.AddListener(UpdateWaveText);
        GameManager.OnGameStateChanged.AddListener(UpdateGameState);
        GameManager.OnWaveStateChanged.AddListener(UpdateWaveButton);
        
        // Tower selection events
        if (towerPlacement != null)
        {
            towerPlacement.OnTowerSelected.AddListener(ShowTowerInfo);
            towerPlacement.OnTowerDeselected.AddListener(HideTowerInfo);
        }
        
        // Initialize UI
        InitializeUI();
        
        // Set up button listeners
        SetupButtonListeners();
    }
    
    private void Update()
    {
        // Update enemies remaining count if in a wave
        if (GameManager.Instance != null && 
            GameManager.Instance.CurrentState == GameState.Playing && 
            GameManager.Instance.IsInWave)
        {
            UpdateEnemiesRemainingText();
        }
    }
    
    // Initialize UI elements
    private void InitializeUI()
    {
        // If we have references to the game manager, update initial values
        if (GameManager.Instance != null)
        {
            UpdateGoldText(GameManager.Instance.PlayerGold);
            UpdateLivesText(GameManager.Instance.PlayerLives);
            UpdateWaveText(GameManager.Instance.CurrentWave);
            UpdateGameState(GameManager.Instance.CurrentState);
            UpdateWaveButton(GameManager.Instance.IsInWave);
        }
        
        // Hide tower info panel initially
        if (towerInfoPanel != null)
            towerInfoPanel.SetActive(false);
    }
    
    // Set up button click listeners
    private void SetupButtonListeners()
    {
        // Tower selection buttons
        if (archerTowerButton != null)
            archerTowerButton.onClick.AddListener(() => OnTowerButtonClicked(TowerType.Archer));
            
        if (mageTowerButton != null)
            mageTowerButton.onClick.AddListener(() => OnTowerButtonClicked(TowerType.Mage));
            
        if (frostTowerButton != null)
            frostTowerButton.onClick.AddListener(() => OnTowerButtonClicked(TowerType.Frost));
            
        if (cannonTowerButton != null)
            cannonTowerButton.onClick.AddListener(() => OnTowerButtonClicked(TowerType.Cannon));
            
        // Wave start button
        if (startWaveButton != null)
            startWaveButton.onClick.AddListener(OnStartWaveButtonClicked);
            
        // Tower action buttons
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
            
        if (sellButton != null)
            sellButton.onClick.AddListener(OnSellButtonClicked);
    }
    
    // Update UI based on game state
    private void UpdateGameState(GameState state)
    {
        // Show/hide panels based on game state
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(state == GameState.MainMenu);
            
        if (gameHUDPanel != null)
            gameHUDPanel.SetActive(state == GameState.Playing || state == GameState.Paused);
            
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(state == GameState.Paused);
            
        if (gameOverPanel != null)
            gameOverPanel.SetActive(state == GameState.GameOver);
            
        if (victoryPanel != null)
            victoryPanel.SetActive(state == GameState.Victory);
    }
    
    // UI update methods
    private void UpdateGoldText(int amount)
    {
        if (goldText != null)
            goldText.text = "Gold: " + amount;
    }
    
    private void UpdateLivesText(int amount)
    {
        if (livesText != null)
            livesText.text = "Lives: " + amount;
    }
    
    private void UpdateWaveText(int wave)
    {
        if (waveText != null)
            waveText.text = "Wave: " + wave;
    }
    
    private void UpdateEnemiesRemainingText()
    {
        if (enemiesRemainingText != null && WaveSpawner.Instance != null)
        {
            int count = WaveSpawner.Instance.GetEnemiesRemaining();
            enemiesRemainingText.text = "Enemies: " + count;
        }
    }
    
    private void UpdateWaveButton(bool isInWave)
    {
        if (startWaveButton != null)
        {
            startWaveButton.interactable = !isInWave;
            startWaveButton.GetComponentInChildren<TextMeshProUGUI>().text = 
                isInWave ? "Wave In Progress" : "Start Wave";
        }
    }
    
    // Show tower info when a tower is selected
    private void ShowTowerInfo(TowerController tower)
    {
        if (towerInfoPanel == null || tower == null) return;
        
        // Show the panel
        towerInfoPanel.SetActive(true);
        
        // Update tower info
        TowerModel data = tower.TowerData;
        
        if (towerNameText != null)
            towerNameText.text = data.towerName;
            
        if (towerLevelText != null)
            towerLevelText.text = "Level: " + data.level;
            
        if (towerDamageText != null)
            towerDamageText.text = "Damage: " + data.damage.ToString("F1");
            
        if (towerRangeText != null)
            towerRangeText.text = "Range: " + data.attackRange.ToString("F1");
            
        if (towerSpeedText != null)
            towerSpeedText.text = "Speed: " + data.attackSpeed.ToString("F1");
            
        // Update upgrade button
        if (upgradeButton != null && upgradeButtonText != null)
        {
            upgradeButtonText.text = "Upgrade (" + data.upgradeCost + " Gold)";
            
            // Disable if player doesn't have enough gold
            bool canAfford = GameManager.Instance != null && 
                             GameManager.Instance.PlayerGold >= data.upgradeCost;
            upgradeButton.interactable = canAfford;
        }
    }
    
    // Hide tower info panel
    private void HideTowerInfo()
    {
        if (towerInfoPanel != null)
            towerInfoPanel.SetActive(false);
    }
    
    // Button click handlers
    private void OnTowerButtonClicked(TowerType type)
    {
        if (towerPlacement != null)
            towerPlacement.StartPlacingTower(type);
    }
    
    private void OnStartWaveButtonClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StartNextWave();
    }
    
    private void OnUpgradeButtonClicked()
    {
        if (towerPlacement != null)
            towerPlacement.TryUpgradeSelectedTower();
    }
    
    private void OnSellButtonClicked()
    {
        if (towerPlacement != null)
            towerPlacement.TrySellSelectedTower();
    }
    
    // Game control buttons
    public void OnStartGameClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StartGame();
    }
    
    public void OnPauseButtonClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.TogglePause();
    }
    
    public void OnResumeButtonClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.TogglePause();
    }
    
    public void OnRestartButtonClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ResetGame();
    }
    
    public void OnQuitButtonClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
} 