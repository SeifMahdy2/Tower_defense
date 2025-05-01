using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TD;

public class TowerUpgradePanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panelContainer;
    [SerializeField] private TextMeshProUGUI towerNameText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TextMeshProUGUI upgradeInfoText;
    [SerializeField] private GameObject maxLevelObject;
    [SerializeField] private Button closeButton;

    [Header("Settings")]
    [SerializeField] private Vector2 panelOffset = new Vector2(0, 100);
    [SerializeField] private float screenEdgeMargin = 50f;
    [SerializeField] private float autoCloseTime = 10f;

    private Tower selectedTower;
    private GameManager gameManager;
    private Camera mainCamera;
    private float lastInteractionTime;
    private RectTransform panelRect;
    private Canvas parentCanvas;

    private void Awake()
    {
        // Get references
        gameManager = FindObjectOfType<GameManager>();
        mainCamera = Camera.main;
        panelRect = panelContainer ? panelContainer.GetComponent<RectTransform>() : null;
        parentCanvas = GetComponentInParent<Canvas>();

        // Hide panel initially
        if (panelContainer)
            panelContainer.SetActive(false);

        // Setup buttons
        if (upgradeButton)
            upgradeButton.onClick.AddListener(UpgradeSelectedTower);

        // Create close button if needed
        if (!closeButton && panelContainer)
            AddCloseButton();
        else if (closeButton)
            closeButton.onClick.AddListener(Hide);
        
        SetupTouchHandlers();
    }
    
    private void Update()
    {
        // Auto-close panel after inactivity
        if (panelContainer && panelContainer.activeSelf && Time.time - lastInteractionTime > autoCloseTime)
            Hide();
    }
    
    private void AddCloseButton()
    {
        // Create close button object
        GameObject btnObj = new GameObject("CloseButton");
        btnObj.transform.SetParent(panelContainer.transform, false);
        
        // Add components
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        Image img = btnObj.AddComponent<Image>();
        closeButton = btnObj.AddComponent<Button>();
        
        // Set position and appearance
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(-10, -10);
        rect.sizeDelta = new Vector2(30, 30);
        img.color = new Color(0.8f, 0.2f, 0.2f);
        
        // Add X label
        GameObject xObj = new GameObject("X");
        xObj.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI xText = xObj.AddComponent<TextMeshProUGUI>();
        xText.text = "X";
        xText.fontSize = 20;
        xText.alignment = TextAlignmentOptions.Center;
        xText.color = Color.white;
        
        RectTransform xRect = xObj.GetComponent<RectTransform>();
        xRect.anchorMin = Vector2.zero;
        xRect.anchorMax = Vector2.one;
        xRect.offsetMin = Vector2.zero;
        xRect.offsetMax = Vector2.zero;
        
        // Add functionality
        closeButton.onClick.AddListener(Hide);
        btnObj.AddComponent<MobileUIButtonHandler>();
    }

    private void SetupTouchHandlers()
    {
        // Add touch handlers to all buttons
        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            if (!button.GetComponent<MobileUIButtonHandler>())
                button.gameObject.AddComponent<MobileUIButtonHandler>();
        }
    }

    public void ShowForTower(Tower tower)
    {
        if (!tower || !panelContainer)
            return;

        selectedTower = tower;
        lastInteractionTime = Time.time;
        
        PositionPanelAtTower();
        UpdateUIElements();
        
        panelContainer.SetActive(true);
        
        // Provide haptic feedback
        #if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
        #endif
    }

    private void PositionPanelAtTower()
    {
        if (!selectedTower || !mainCamera || !panelRect)
            return;

        // Get screen position
        Vector3 screenPos = mainCamera.WorldToScreenPoint(selectedTower.transform.position);
        screenPos.y += panelOffset.y;
        screenPos.x += panelOffset.x;
        
        KeepPanelOnScreen(ref screenPos);
        panelRect.position = screenPos;
    }
    
    private void KeepPanelOnScreen(ref Vector3 position)
    {
        if (!parentCanvas || parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            return;
            
        Vector2 size = panelRect.sizeDelta;
        
        // Get the safe area
        Rect safeArea = Screen.safeArea;
        
        // Check right edge
        if (position.x + size.x * 0.5f > safeArea.x + safeArea.width - screenEdgeMargin)
            position.x = safeArea.x + safeArea.width - screenEdgeMargin - size.x * 0.5f;
        
        // Check left edge
        if (position.x - size.x * 0.5f < safeArea.x + screenEdgeMargin)
            position.x = safeArea.x + screenEdgeMargin + size.x * 0.5f;
        
        // Check top edge
        if (position.y + size.y * 0.5f > safeArea.y + safeArea.height - screenEdgeMargin)
            position.y = safeArea.y + safeArea.height - screenEdgeMargin - size.y * 0.5f;
        
        // Check bottom edge
        if (position.y - size.y * 0.5f < safeArea.y + screenEdgeMargin)
            position.y = safeArea.y + screenEdgeMargin + size.y * 0.5f;
    }

    private void UpdateUIElements()
    {
        if (!selectedTower)
            return;

        // Update tower info
        if (towerNameText) towerNameText.text = selectedTower.towerName;
        if (levelText) levelText.text = $"Level: {selectedTower.upgradeLevel}";

        // Check if tower is already at maximum level
        bool isMaxLevel = selectedTower.upgradeLevel >= selectedTower.maxUpgradeLevel;
        Debug.Log($"Tower Level: {selectedTower.upgradeLevel}, Max Level: {selectedTower.maxUpgradeLevel}, Is Max Level: {isMaxLevel}");

        // Check player's gold
        bool hasEnoughGold = gameManager && gameManager.GetCurrentGold() >= selectedTower.upgradeCost;

        // Update UI elements based on upgrade availability
        if (upgradeCostText) 
            upgradeCostText.text = isMaxLevel ? "" : $"Cost: {selectedTower.upgradeCost}";
        
        if (upgradeButton) 
            upgradeButton.interactable = !isMaxLevel && hasEnoughGold;
        
        if (maxLevelObject) 
            maxLevelObject.SetActive(isMaxLevel);
        
        if (upgradeInfoText) 
            upgradeInfoText.text = GetUpgradeInfoForTower();
    }

    private string GetUpgradeInfoForTower()
    {
        if (!selectedTower)
            return "";

        // Check if tower is already at max level
        if (selectedTower.upgradeLevel >= selectedTower.maxUpgradeLevel)
            return "Maximum upgrade reached";
            
        // Add "One-time upgrade" prefix to all upgrade descriptions
        string baseInfo = "ONE-TIME UPGRADE: ";

        // Get type-specific upgrade info
        if (selectedTower is ArcherTower)
            return baseInfo + "Increases fire rate by 50% and arrow speed";
        else if (selectedTower is MageTower)
            return baseInfo + "Adds damage over time effect and increases splash radius";
        else if (selectedTower is FrostTower)
            return baseInfo + "Doubles slow duration and increases slow effect";
        else if (selectedTower is CannonTower)
            return baseInfo + "Increases explosion radius and knockback force";
        else
            return baseInfo + "Improves damage and range";
    }

    private void UpgradeSelectedTower()
    {
        if (!selectedTower || !gameManager)
            return;

        lastInteractionTime = Time.time;

        // Check max level more explicitly
        bool isMaxLevel = selectedTower.upgradeLevel >= selectedTower.maxUpgradeLevel;
        if (isMaxLevel)
        {
            Debug.Log("Cannot upgrade: Tower already at max level");
            // Update UI to show max level
            if (maxLevelObject) 
                maxLevelObject.SetActive(true);
            if (upgradeButton)
                upgradeButton.interactable = false;
            return;
        }

        // Try upgrade
        if (gameManager.SpendGold(selectedTower.upgradeCost))
        {
            Debug.Log($"Upgrading tower from level {selectedTower.upgradeLevel} to {selectedTower.upgradeLevel + 1}");
            
            // Apply upgrade
            selectedTower.Upgrade();
            
            // Play feedback
            if (AudioManager.Instance)
                AudioManager.Instance.PlayTowerUpgradeSound(selectedTower.transform.position);
            
            #if UNITY_IOS || UNITY_ANDROID
            Handheld.Vibrate();
            #endif
            
            // Update UI immediately after upgrade
            UpdateUIElements();
            
            // Force the max level UI to show if we reached max level
            isMaxLevel = selectedTower.upgradeLevel >= selectedTower.maxUpgradeLevel;
            if (isMaxLevel)
            {
                Debug.Log("Tower is now at max level - updating UI");
                if (maxLevelObject) 
                    maxLevelObject.SetActive(true);
                if (upgradeButton)
                    upgradeButton.interactable = false;
                if (upgradeCostText)
                    upgradeCostText.text = "";
            }
        }
    }

    public void Hide()
    {
        selectedTower = null;
        if (panelContainer)
            panelContainer.SetActive(false);
    }
} 