using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class TowerButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Tower Type")]
    [SerializeField] private TowerType towerType;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image towerImage;
    
    [Header("Lock Visuals")]
    [SerializeField] private GameObject lockIcon;  // Assign a lock icon image in the inspector
    [SerializeField] private float lockedAlpha = 0.4f; // Alpha value for locked towers
    
    private TowerPlacementManager towerPlacementManager;
    private Canvas mainCanvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private GameObject dragIcon;
    private bool isLocked = false;
    
    public enum TowerType
    {
        Archer,
        Mage,
        Frost,
        Cannon
    }
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        // Add CanvasGroup if it doesn't exist
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // Find the main canvas
        mainCanvas = GetComponentInParent<Canvas>();
        if (mainCanvas == null)
        {
            mainCanvas = FindObjectOfType<Canvas>();
        }
        
        // Create a lock icon if it doesn't exist yet
        if (lockIcon == null)
        {
            // Create lock container
            lockIcon = new GameObject("LockIcon");
            lockIcon.transform.SetParent(transform);
            lockIcon.transform.localPosition = Vector3.zero;
            RectTransform lockRT = lockIcon.AddComponent<RectTransform>();
            lockRT.anchoredPosition = Vector2.zero;
            lockRT.sizeDelta = rectTransform.sizeDelta;
            
            // Create background overlay
            GameObject overlay = new GameObject("Overlay");
            overlay.transform.SetParent(lockIcon.transform);
            Image overlayImage = overlay.AddComponent<Image>();
            overlayImage.color = new Color(0, 0, 0, 0.5f);  // Semi-transparent black
            RectTransform overlayRT = overlay.GetComponent<RectTransform>();
            overlayRT.anchorMin = Vector2.zero;
            overlayRT.anchorMax = Vector2.one;
            overlayRT.offsetMin = Vector2.zero;
            overlayRT.offsetMax = Vector2.zero;
            
            // Create lock body (padlock body)
            GameObject lockBody = new GameObject("LockBody");
            lockBody.transform.SetParent(lockIcon.transform);
            Image bodyImage = lockBody.AddComponent<Image>();
            bodyImage.color = Color.white;
            bodyImage.raycastTarget = false;
            RectTransform bodyRT = lockBody.GetComponent<RectTransform>();
            
            // Size and position the lock body
            float size = Mathf.Min(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y) * 0.4f;
            bodyRT.sizeDelta = new Vector2(size, size);
            bodyRT.anchoredPosition = new Vector2(0, -size * 0.1f);
            
            // Create lock shackle (the top curved part)
            GameObject lockShackle = new GameObject("LockShackle");
            lockShackle.transform.SetParent(lockIcon.transform);
            Image shackleImage = lockShackle.AddComponent<Image>();
            shackleImage.color = Color.white;
            shackleImage.raycastTarget = false;
            
            // Make it a rounded shape for the top part of the lock
            shackleImage.sprite = Resources.Load<Sprite>("UI/Circle");
            if (shackleImage.sprite == null)
            {
                // If no circle sprite found, use a mask with an image
                shackleImage.color = Color.clear;
                lockShackle.AddComponent<Mask>();
                lockShackle.GetComponent<Mask>().showMaskGraphic = false;
                
                // Add a child image that will be the actual shackle
                GameObject shackleGraphic = new GameObject("ShackleGraphic");
                shackleGraphic.transform.SetParent(lockShackle.transform);
                Image shackleGraphicImage = shackleGraphic.AddComponent<Image>();
                shackleGraphicImage.color = Color.white;
                
                RectTransform shackleGraphicRT = shackleGraphic.GetComponent<RectTransform>();
                shackleGraphicRT.anchorMin = new Vector2(0, 0);
                shackleGraphicRT.anchorMax = new Vector2(1, 0.5f);
                shackleGraphicRT.offsetMin = Vector2.zero;
                shackleGraphicRT.offsetMax = Vector2.zero;
            }
            
            RectTransform shackleRT = lockShackle.GetComponent<RectTransform>();
            shackleRT.sizeDelta = new Vector2(size * 0.8f, size * 0.5f);
            shackleRT.anchoredPosition = new Vector2(0, size * 0.35f);
            
            // Create a simple Text "LOCKED" for clarity
            GameObject lockText = new GameObject("LockText");
            lockText.transform.SetParent(lockIcon.transform);
            TextMeshProUGUI lockTextComponent = lockText.AddComponent<TextMeshProUGUI>();
            lockTextComponent.text = "LOCKED";
            lockTextComponent.fontSize = size * 0.35f;
            lockTextComponent.alignment = TextAlignmentOptions.Center;
            lockTextComponent.color = Color.white;
            lockTextComponent.raycastTarget = false;
            
            RectTransform textRT = lockText.GetComponent<RectTransform>();
            textRT.sizeDelta = new Vector2(size * 2, size * 0.5f);
            textRT.anchoredPosition = new Vector2(0, -size * 0.6f);
            
            // Hide by default
            lockIcon.SetActive(false);
        }
    }
    
    void Start()
    {
        // Find the tower placement manager in the scene
        towerPlacementManager = FindObjectOfType<TowerPlacementManager>();
        
        // Check if tower should be locked based on current scene
        CheckTowerLockStatus();
        
        // Set the cost text based on the tower type
        UpdateCostText();
        
        // Save original position
        originalPosition = rectTransform.anchoredPosition;
    }
    
    void Update()
    {
        // First check if tower is locked
        if (isLocked)
        {
            canvasGroup.interactable = false;
            canvasGroup.alpha = lockedAlpha;
            return;
        }
        
        // Check if player can afford this tower and update button interactability
        int cost = GetTowerCost();
        bool canAfford = towerPlacementManager.CanAffordTower(cost);
        canvasGroup.interactable = canAfford;
        canvasGroup.alpha = canAfford ? 1f : 0.5f;
    }
    
    private void CheckTowerLockStatus()
    {
        // Get current scene name
        string currentScene = SceneManager.GetActiveScene().name;
        
        // Mage and Cannon towers are locked in Level_1
        if (currentScene.Contains("Level_1"))
        {
            if (towerType == TowerType.Mage || towerType == TowerType.Cannon)
            {
                LockTower();
            }
            else
            {
                UnlockTower();
            }
        }
        // All towers are unlocked in Level_2
        else if (currentScene.Contains("Level_2"))
        {
            UnlockTower();
        }
    }
    
    private void LockTower()
    {
        isLocked = true;
        canvasGroup.interactable = false;
        canvasGroup.alpha = lockedAlpha;
        if (lockIcon != null)
        {
            lockIcon.SetActive(true);
        }
    }
    
    private void UnlockTower()
    {
        isLocked = false;
        if (lockIcon != null)
        {
            lockIcon.SetActive(false);
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canvasGroup.interactable || isLocked)
            return;

        // Create drag icon
        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(mainCanvas.transform);
        Image dragImage = dragIcon.AddComponent<Image>();
        dragImage.sprite = towerImage.sprite;
        dragImage.raycastTarget = false;
        
        // Size the drag icon
        RectTransform dragRect = dragIcon.GetComponent<RectTransform>();
        dragRect.sizeDelta = rectTransform.sizeDelta;
        
        // Position at touch/mouse position
        dragRect.position = eventData.position;
        
        // Tell the placement manager which tower is being dragged
        SelectTowerForPlacement();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!canvasGroup.interactable || dragIcon == null || isLocked)
            return;
            
        // Move the drag icon to the pointer position
        dragIcon.GetComponent<RectTransform>().position = eventData.position;
        
        // Update tower preview position in world space
        towerPlacementManager.UpdateDragPosition(eventData.position);
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canvasGroup.interactable || dragIcon == null || isLocked)
            return;
            
        // Clean up drag icon
        Destroy(dragIcon);
        
        // Try to place the tower at the final position
        towerPlacementManager.TryPlaceTowerAtPosition(eventData.position);
    }
    
    private void SelectTowerForPlacement()
    {
        // Call the appropriate method on the tower placement manager
        switch (towerType)
        {
            case TowerType.Archer:
                towerPlacementManager.SelectArcherTower();
                break;
            case TowerType.Mage:
                towerPlacementManager.SelectMageTower();
                break;
            case TowerType.Frost:
                towerPlacementManager.SelectFrostTower();
                break;
            case TowerType.Cannon:
                towerPlacementManager.SelectCannonTower();
                break;
        }
        
        // Add haptic feedback for mobile
        #if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
        #endif
    }
    
    private int GetTowerCost()
    {
        // Return the cost based on tower type
        switch (towerType)
        {
            case TowerType.Archer:
                return 100;
            case TowerType.Mage:
                return 150;
            case TowerType.Frost:
                return 125;
            case TowerType.Cannon:
                return 200;
            default:
                return 0;
        }
    }
    
    private void UpdateCostText()
    {
        if (costText != null)
        {
            costText.text = GetTowerCost().ToString();
        }
    }
} 