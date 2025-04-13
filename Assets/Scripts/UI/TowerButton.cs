using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TowerButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Tower Type")]
    [SerializeField] private TowerType towerType;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image towerImage;
    
    private TowerPlacementManager towerPlacementManager;
    private Canvas mainCanvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private GameObject dragIcon;
    
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
    }
    
    void Start()
    {
        // Find the tower placement manager in the scene
        towerPlacementManager = FindObjectOfType<TowerPlacementManager>();
        
        // Set the cost text based on the tower type
        UpdateCostText();
        
        // Save original position
        originalPosition = rectTransform.anchoredPosition;
    }
    
    void Update()
    {
        // Check if player can afford this tower and update button interactability
        int cost = GetTowerCost();
        bool canAfford = towerPlacementManager.CanAffordTower(cost);
        canvasGroup.interactable = canAfford;
        canvasGroup.alpha = canAfford ? 1f : 0.5f;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canvasGroup.interactable)
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
        if (!canvasGroup.interactable || dragIcon == null)
            return;
            
        // Move the drag icon to the pointer position
        dragIcon.GetComponent<RectTransform>().position = eventData.position;
        
        // Update tower preview position in world space
        towerPlacementManager.UpdateDragPosition(eventData.position);
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canvasGroup.interactable || dragIcon == null)
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