using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using TowerDefense.Controllers;
using TowerDefense.Models;

namespace TowerDefense.Views
{
    public class TowerSelectionView : MonoBehaviour
    {
        [Header("Tower Buttons")]
        [SerializeField] private Button archerButton;
        [SerializeField] private Button mageButton;
        [SerializeField] private Button frostButton;
        [SerializeField] private Button cannonButton;
        
        [Header("Tower Costs")]
        [SerializeField] private TextMeshProUGUI archerCostText;
        [SerializeField] private TextMeshProUGUI mageCostText;
        [SerializeField] private TextMeshProUGUI frostCostText;
        [SerializeField] private TextMeshProUGUI cannonCostText;
        
        [Header("Tower Info")]
        [SerializeField] private GameObject towerInfoPanel;
        [SerializeField] private TextMeshProUGUI towerNameText;
        [SerializeField] private TextMeshProUGUI towerDescriptionText;
        [SerializeField] private Image towerImage;
        
        // Controllers reference
        private TowerController towerController;
        private GameController gameController;
        
        private void Start()
        {
            // Get controller references
            towerController = TowerController.Instance;
            gameController = GameController.Instance;
            
            // Hide tower info panel by default
            if (towerInfoPanel != null)
            {
                towerInfoPanel.SetActive(false);
            }
            
            // Setup buttons
            SetupTowerButtons();
            
            // Update tower costs
            UpdateTowerCosts();
            
            // Subscribe to controller events
            if (gameController != null)
            {
                gameController.OnGameStateChanged += UpdateButtonInteractability;
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (gameController != null)
            {
                gameController.OnGameStateChanged -= UpdateButtonInteractability;
            }
        }
        
        private void SetupTowerButtons()
        {
            if (towerController == null) return;
            
            SetupTowerButton(archerButton, TowerType.Archer);
            SetupTowerButton(mageButton, TowerType.Mage);
            SetupTowerButton(frostButton, TowerType.Frost);
            SetupTowerButton(cannonButton, TowerType.Cannon);
        }
        
        private void SetupTowerButton(Button button, TowerType towerType)
        {
            if (button != null)
            {
                // Clear existing listeners
                button.onClick.RemoveAllListeners();
                
                // Add listener for tower selection
                button.onClick.AddListener(() => SelectTower(towerType));
                
                // Add listener for tower info on pointer enter
                EventTriggerHelper.AddEventTrigger(button.gameObject, EventTriggerType.PointerEnter, 
                    (data) => ShowTowerInfo(towerType));
                
                // Add listener to hide info on pointer exit
                EventTriggerHelper.AddEventTrigger(button.gameObject, EventTriggerType.PointerExit, 
                    (data) => HideTowerInfo());
            }
        }
        
        private void SelectTower(TowerType towerType)
        {
            // Tell tower controller to select this tower
            if (towerController != null)
            {
                towerController.SelectTowerToBuild(towerType);
            }
        }
        
        private void ShowTowerInfo(TowerType towerType)
        {
            if (towerInfoPanel != null && towerNameText != null && towerDescriptionText != null && towerController != null)
            {
                // Get tower data
                TowerData towerData = towerController.GetTowerData(towerType);
                
                if (towerData == null) return;
                
                // Set name
                towerNameText.text = towerData.Type.ToString();
                
                // Set description
                towerDescriptionText.text = towerData.Description;
                
                // Set image if available
                if (towerImage != null && towerData.Prefab != null)
                {
                    SpriteRenderer spriteRenderer = towerData.Prefab.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        towerImage.sprite = spriteRenderer.sprite;
                    }
                }
                
                // Show panel
                towerInfoPanel.SetActive(true);
            }
        }
        
        private void HideTowerInfo()
        {
            if (towerInfoPanel != null)
            {
                towerInfoPanel.SetActive(false);
            }
        }
        
        private void UpdateTowerCosts()
        {
            if (towerController == null) return;
            
            // Update costs for each tower type
            UpdateCostText(archerCostText, TowerType.Archer);
            UpdateCostText(mageCostText, TowerType.Mage);
            UpdateCostText(frostCostText, TowerType.Frost);
            UpdateCostText(cannonCostText, TowerType.Cannon);
        }
        
        private void UpdateCostText(TextMeshProUGUI costText, TowerType towerType)
        {
            if (costText != null)
            {
                // Get tower data
                TowerData towerData = towerController.GetTowerData(towerType);
                
                if (towerData != null)
                {
                    costText.text = towerData.Cost.ToString();
                }
            }
        }
        
        private void Update()
        {
            // Update button interactability based on available gold
            UpdateButtonInteractability();
        }
        
        private void UpdateButtonInteractability()
        {
            if (towerController == null || gameController == null) return;
            
            // Update each button based on available gold
            UpdateButtonInteractable(archerButton, TowerType.Archer);
            UpdateButtonInteractable(mageButton, TowerType.Mage);
            UpdateButtonInteractable(frostButton, TowerType.Frost);
            UpdateButtonInteractable(cannonButton, TowerType.Cannon);
        }
        
        private void UpdateButtonInteractable(Button button, TowerType towerType)
        {
            if (button != null)
            {
                // Get tower data
                TowerData towerData = towerController.GetTowerData(towerType);
                
                if (towerData != null)
                {
                    // Set interactable based on gold
                    button.interactable = gameController.HasEnoughGold(towerData.Cost);
                }
            }
        }
    }
    
    // Helper class for easily adding event triggers
    public static class EventTriggerHelper
    {
        public static void AddEventTrigger(GameObject obj, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = obj.AddComponent<EventTrigger>();
            }
            
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener(action);
            trigger.triggers.Add(entry);
        }
    }
} 