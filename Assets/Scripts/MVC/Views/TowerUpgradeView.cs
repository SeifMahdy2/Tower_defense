using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TowerDefense.Controllers;

namespace TowerDefense.Views
{
    public class TowerUpgradeView : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject upgradePanel;
        [SerializeField] private TextMeshProUGUI towerNameText;
        [SerializeField] private TextMeshProUGUI towerLevelText;
        [SerializeField] private TextMeshProUGUI towerStatsText;
        
        [Header("Upgrade Buttons")]
        [SerializeField] private Button damageUpgradeButton;
        [SerializeField] private Button rangeUpgradeButton;
        [SerializeField] private Button attackSpeedUpgradeButton;
        [SerializeField] private Button specialAbilityUpgradeButton;
        
        [Header("Upgrade Costs")]
        [SerializeField] private TextMeshProUGUI damageUpgradeCostText;
        [SerializeField] private TextMeshProUGUI rangeUpgradeCostText;
        [SerializeField] private TextMeshProUGUI attackSpeedUpgradeCostText;
        [SerializeField] private TextMeshProUGUI specialAbilityUpgradeCostText;
        
        [Header("Other Buttons")]
        [SerializeField] private Button sellTowerButton;
        [SerializeField] private TextMeshProUGUI sellValueText;
        [SerializeField] private Button closeButton;
        
        // Controllers reference
        private TowerController towerController;
        private GameController gameController;
        
        // Current selected tower
        private GameObject selectedTower;
        
        private void Start()
        {
            // Get controller references
            towerController = TowerController.Instance;
            gameController = GameController.Instance;
            
            // Initially hide the panel
            HideUpgradePanel();
            
            // Setup button listeners
            SetupButtons();
            
            // Subscribe to tower selection events
            if (towerController != null)
            {
                towerController.OnTowerSelected += ShowUpgradePanel;
                towerController.OnTowerDeselected += HideUpgradePanel;
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (towerController != null)
            {
                towerController.OnTowerSelected -= ShowUpgradePanel;
                towerController.OnTowerDeselected -= HideUpgradePanel;
            }
        }
        
        private void SetupButtons()
        {
            if (damageUpgradeButton != null)
            {
                damageUpgradeButton.onClick.AddListener(() => UpgradeTower("damage"));
            }
            
            if (rangeUpgradeButton != null)
            {
                rangeUpgradeButton.onClick.AddListener(() => UpgradeTower("range"));
            }
            
            if (attackSpeedUpgradeButton != null)
            {
                attackSpeedUpgradeButton.onClick.AddListener(() => UpgradeTower("attackSpeed"));
            }
            
            if (specialAbilityUpgradeButton != null)
            {
                specialAbilityUpgradeButton.onClick.AddListener(() => UpgradeTower("specialAbility"));
            }
            
            if (sellTowerButton != null)
            {
                sellTowerButton.onClick.AddListener(SellTower);
            }
            
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(() => {
                    if (towerController != null)
                    {
                        towerController.DeselectTower();
                    }
                });
            }
        }
        
        private void ShowUpgradePanel(GameObject tower)
        {
            if (tower == null || upgradePanel == null) return;
            
            selectedTower = tower;
            upgradePanel.SetActive(true);
            
            // Update UI with tower information
            UpdateTowerInfo();
            
            // Update button states
            UpdateUpgradeButtons();
        }
        
        private void HideUpgradePanel()
        {
            if (upgradePanel == null) return;
            
            selectedTower = null;
            upgradePanel.SetActive(false);
        }
        
        private void UpdateTowerInfo()
        {
            if (selectedTower == null || towerController == null) return;
            
            // Get tower data
            string towerName = towerController.GetTowerName(selectedTower);
            int towerLevel = towerController.GetTowerLevel(selectedTower);
            string towerStats = towerController.GetTowerStats(selectedTower);
            int sellValue = towerController.GetTowerSellValue(selectedTower);
            
            // Update UI texts
            if (towerNameText != null) towerNameText.text = towerName;
            if (towerLevelText != null) towerLevelText.text = "Level " + towerLevel;
            if (towerStatsText != null) towerStatsText.text = towerStats;
            if (sellValueText != null) sellValueText.text = "Sell: " + sellValue + " Gold";
            
            // Update upgrade costs
            UpdateUpgradeCosts();
        }
        
        private void UpdateUpgradeCosts()
        {
            if (selectedTower == null || towerController == null) return;
            
            // Get upgrade costs
            int damageCost = towerController.GetUpgradeCost(selectedTower, "damage");
            int rangeCost = towerController.GetUpgradeCost(selectedTower, "range");
            int attackSpeedCost = towerController.GetUpgradeCost(selectedTower, "attackSpeed");
            int specialAbilityCost = towerController.GetUpgradeCost(selectedTower, "specialAbility");
            
            // Update cost texts
            if (damageUpgradeCostText != null) damageUpgradeCostText.text = damageCost + " Gold";
            if (rangeUpgradeCostText != null) rangeUpgradeCostText.text = rangeCost + " Gold";
            if (attackSpeedUpgradeCostText != null) attackSpeedUpgradeCostText.text = attackSpeedCost + " Gold";
            if (specialAbilityUpgradeCostText != null) specialAbilityUpgradeCostText.text = specialAbilityCost + " Gold";
        }
        
        private void UpdateUpgradeButtons()
        {
            if (selectedTower == null || towerController == null || gameController == null) return;
            
            int playerGold = gameController.GetGold();
            
            // Update button interactability based on player gold and max upgrade levels
            if (damageUpgradeButton != null)
            {
                int cost = towerController.GetUpgradeCost(selectedTower, "damage");
                bool canUpgrade = cost > 0 && playerGold >= cost;
                damageUpgradeButton.interactable = canUpgrade;
            }
            
            if (rangeUpgradeButton != null)
            {
                int cost = towerController.GetUpgradeCost(selectedTower, "range");
                bool canUpgrade = cost > 0 && playerGold >= cost;
                rangeUpgradeButton.interactable = canUpgrade;
            }
            
            if (attackSpeedUpgradeButton != null)
            {
                int cost = towerController.GetUpgradeCost(selectedTower, "attackSpeed");
                bool canUpgrade = cost > 0 && playerGold >= cost;
                attackSpeedUpgradeButton.interactable = canUpgrade;
            }
            
            if (specialAbilityUpgradeButton != null)
            {
                int cost = towerController.GetUpgradeCost(selectedTower, "specialAbility");
                bool canUpgrade = cost > 0 && playerGold >= cost;
                specialAbilityUpgradeButton.interactable = canUpgrade;
            }
        }
        
        private void UpgradeTower(string upgradeType)
        {
            if (selectedTower == null || towerController == null || gameController == null) return;
            
            // Get upgrade cost
            int cost = towerController.GetUpgradeCost(selectedTower, upgradeType);
            int playerGold = gameController.GetGold();
            
            // Check if player has enough gold
            if (playerGold < cost) return;
            
            // Perform upgrade
            bool success = towerController.UpgradeTower(selectedTower, upgradeType);
            
            if (success)
            {
                // Spend gold
                gameController.SpendGold(cost);
                
                // Update UI
                UpdateTowerInfo();
                UpdateUpgradeButtons();
            }
        }
        
        private void SellTower()
        {
            if (selectedTower == null || towerController == null || gameController == null) return;
            
            int sellValue = towerController.GetTowerSellValue(selectedTower);
            
            // Sell tower
            bool success = towerController.SellTower(selectedTower);
            
            if (success)
            {
                // Add gold
                gameController.AddGold(sellValue);
                
                // Hide panel
                HideUpgradePanel();
            }
        }
        
        // Update is called once per frame
        private void Update()
        {
            // Check for gold changes to update button states
            if (selectedTower != null && gameController != null)
            {
                UpdateUpgradeButtons();
            }
        }
    }
} 