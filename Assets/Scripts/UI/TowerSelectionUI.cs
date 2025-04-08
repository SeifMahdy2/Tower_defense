using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TowerSelectionUI : MonoBehaviour
{
    [Header("Tower Placement")]
    [SerializeField] private TowerPlacement towerPlacement;
    
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
    
    [Header("Tower Descriptions")]
    [SerializeField] private string archerDescription = "Basic tower with good rate of fire.";
    [SerializeField] private string mageDescription = "High damage but slower fire rate.";
    [SerializeField] private string frostDescription = "Slows enemies in range.";
    [SerializeField] private string cannonDescription = "Area damage to multiple enemies.";
    
    // Tower prefabs for cost reference
    [SerializeField] private GameObject archerPrefab;
    [SerializeField] private GameObject magePrefab;
    [SerializeField] private GameObject frostPrefab;
    [SerializeField] private GameObject cannonPrefab;
    
    private GameManager gameManager;
    
    private void Start()
    {
        // Get references
        gameManager = FindObjectOfType<GameManager>();
        
        // Hide tower info panel by default
        if (towerInfoPanel != null)
        {
            towerInfoPanel.SetActive(false);
        }
        
        // Setup buttons
        SetupTowerButton(archerButton, TowerType.Archer, archerPrefab);
        SetupTowerButton(mageButton, TowerType.Mage, magePrefab);
        SetupTowerButton(frostButton, TowerType.Frost, frostPrefab);
        SetupTowerButton(cannonButton, TowerType.Cannon, cannonPrefab);
        
        // Update tower costs
        UpdateTowerCosts();
    }
    
    private void SetupTowerButton(Button button, TowerType towerType, GameObject towerPrefab)
    {
        if (button != null)
        {
            // Clear existing listeners
            button.onClick.RemoveAllListeners();
            
            // Add listener for tower selection
            button.onClick.AddListener(() => SelectTower(towerType));
            
            // Add listener for tower info on pointer enter
            EventTriggerHelper.AddEventTrigger(button.gameObject, EventTriggerType.PointerEnter, 
                (data) => ShowTowerInfo(towerType, towerPrefab));
            
            // Add listener to hide info on pointer exit
            EventTriggerHelper.AddEventTrigger(button.gameObject, EventTriggerType.PointerExit, 
                (data) => HideTowerInfo());
        }
    }
    
    private void SelectTower(TowerType towerType)
    {
        // Tell tower placement to start placing
        if (towerPlacement != null)
        {
            towerPlacement.SelectTowerToBuild(towerType);
        }
    }
    
    private void ShowTowerInfo(TowerType towerType, GameObject towerPrefab)
    {
        if (towerInfoPanel != null && towerNameText != null && towerDescriptionText != null)
        {
            // Set name
            towerNameText.text = towerType.ToString();
            
            // Set description
            switch (towerType)
            {
                case TowerType.Archer:
                    towerDescriptionText.text = archerDescription;
                    break;
                case TowerType.Mage:
                    towerDescriptionText.text = mageDescription;
                    break;
                case TowerType.Frost:
                    towerDescriptionText.text = frostDescription;
                    break;
                case TowerType.Cannon:
                    towerDescriptionText.text = cannonDescription;
                    break;
            }
            
            // Set image if available
            if (towerImage != null && towerPrefab != null)
            {
                SpriteRenderer spriteRenderer = towerPrefab.GetComponent<SpriteRenderer>();
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
        // Update costs from prefabs
        UpdateCostText(archerCostText, archerPrefab, "Archer");
        UpdateCostText(mageCostText, magePrefab, "Mage");
        UpdateCostText(frostCostText, frostPrefab, "Frost");
        UpdateCostText(cannonCostText, cannonPrefab, "Cannon");
    }
    
    private void UpdateCostText(TextMeshProUGUI costText, GameObject towerPrefab, string towerName)
    {
        if (costText != null && towerPrefab != null)
        {
            int cost = 0;
            
            // Try to get cost from tower scripts
            switch (towerName)
            {
                case "Archer":
                    Archer archer = towerPrefab.GetComponent<Archer>();
                    if (archer != null)
                        cost = archer.GetCost();
                    break;
                case "Mage":
                    Mage mage = towerPrefab.GetComponent<Mage>();
                    if (mage != null)
                        cost = mage.GetCost();
                    break;
                case "Frost":
                    Frost frost = towerPrefab.GetComponent<Frost>();
                    if (frost != null)
                        cost = frost.GetCost();
                    break;
                case "Cannon":
                    Cannon cannon = towerPrefab.GetComponent<Cannon>();
                    if (cannon != null)
                        cost = cannon.GetCost();
                    break;
            }
            
            costText.text = cost.ToString();
        }
    }
    
    private void Update()
    {
        // Update button interactability based on available gold
        UpdateButtonInteractability();
    }
    
    private void UpdateButtonInteractability()
    {
        if (gameManager == null)
            return;
        
        // Update each button based on available gold
        UpdateButtonInteractable(archerButton, archerPrefab, "Archer");
        UpdateButtonInteractable(mageButton, magePrefab, "Mage");
        UpdateButtonInteractable(frostButton, frostPrefab, "Frost");
        UpdateButtonInteractable(cannonButton, cannonPrefab, "Cannon");
    }
    
    private void UpdateButtonInteractable(Button button, GameObject towerPrefab, string towerName)
    {
        if (button != null && towerPrefab != null && gameManager != null)
        {
            int cost = 0;
            
            // Get cost
            switch (towerName)
            {
                case "Archer":
                    Archer archer = towerPrefab.GetComponent<Archer>();
                    if (archer != null)
                        cost = archer.GetCost();
                    break;
                case "Mage":
                    Mage mage = towerPrefab.GetComponent<Mage>();
                    if (mage != null)
                        cost = mage.GetCost();
                    break;
                case "Frost":
                    Frost frost = towerPrefab.GetComponent<Frost>();
                    if (frost != null)
                        cost = frost.GetCost();
                    break;
                case "Cannon":
                    Cannon cannon = towerPrefab.GetComponent<Cannon>();
                    if (cannon != null)
                        cost = cannon.GetCost();
                    break;
            }
            
            // Set interactable based on gold
            button.interactable = gameManager.HasEnoughGold(cost);
        }
    }
} 