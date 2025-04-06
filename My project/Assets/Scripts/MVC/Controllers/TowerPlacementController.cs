using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TowerPlacementController : MonoBehaviour
{
    [Header("Tower Prefabs")]
    [SerializeField] private GameObject archerTowerPrefab;
    [SerializeField] private GameObject mageTowerPrefab;
    [SerializeField] private GameObject frostTowerPrefab;
    [SerializeField] private GameObject cannonTowerPrefab;
    
    [Header("Placement Settings")]
    [SerializeField] private LayerMask placementAreaLayer;
    [SerializeField] private LayerMask obstructionLayer;
    [SerializeField] private Material validPlacementMaterial;
    [SerializeField] private Material invalidPlacementMaterial;
    
    // Currently selected tower properties
    private GameObject selectedTowerPrefab;
    private TowerModel selectedTowerModel;
    private GameObject towerPreview;
    
    // Currently selected existing tower
    private TowerController selectedTower;
    
    // Current highlighted placement area
    private TowerPlacementArea highlightedArea;
    
    // Events
    public UnityEvent<TowerController> OnTowerSelected = new UnityEvent<TowerController>();
    public UnityEvent OnTowerDeselected = new UnityEvent();
    
    // Tower placement state
    private bool isPlacingTower = false;
    
    private void Start()
    {
        // Set up layer masks if not set
        if (placementAreaLayer == 0)
        {
            int placementLayer = LayerMask.NameToLayer("PlacementArea");
            if (placementLayer != -1)
            {
                placementAreaLayer = 1 << placementLayer;
            }
            else
            {
                Debug.LogWarning("PlacementArea layer not found. Please create it in the Tags & Layers settings.");
            }
        }
        
        if (obstructionLayer == 0)
        {
            int obstacleLayer = LayerMask.NameToLayer("Obstacles");
            if (obstacleLayer != -1)
            {
                obstructionLayer = 1 << obstacleLayer;
            }
        }
    }
    
    private void Update()
    {
        // Check for ESC to cancel tower placement
        if (isPlacingTower && Input.GetKeyDown(KeyCode.Escape))
        {
            CancelTowerPlacement();
            return;
        }
        
        // Handle tower placement preview movement
        if (isPlacingTower && towerPreview != null)
        {
            // Clear any previous highlight
            if (highlightedArea != null)
            {
                highlightedArea.Highlight(false);
                highlightedArea = null;
            }
            
            // Check if hovering over a placement area
            Vector3 mousePosition = GetMouseWorldPosition();
            TowerPlacementArea area = GetPlacementAreaAtPosition(mousePosition);
            
            if (area != null && !area.IsOccupied)
            {
                // Highlight the area
                highlightedArea = area;
                highlightedArea.Highlight(true);
                
                // Position the preview at the area
                towerPreview.transform.position = area.transform.position;
                
                // Set preview to valid appearance
                UpdateTowerPreview(true);
                
                // Handle click to place tower
                if (Input.GetMouseButtonDown(0))
                {
                    PlaceTower(area);
                }
            }
            else
            {
                // Position preview at mouse
                towerPreview.transform.position = mousePosition;
                
                // Set preview to invalid appearance
                UpdateTowerPreview(false);
            }
        }
        else
        {
            // Tower selection from existing towers
            if (Input.GetMouseButtonDown(0))
            {
                // Check if clicked on a tower
                TowerController tower = GetTowerAtMousePosition();
                
                if (tower != null)
                {
                    // Select this tower
                    SelectTower(tower);
                }
                else
                {
                    // Deselect if clicked elsewhere
                    DeselectTower();
                }
            }
        }
    }
    
    // Start placing a tower
    public void StartPlacingTower(TowerType towerType)
    {
        // Make sure we're not already placing a tower
        if (isPlacingTower)
        {
            CancelTowerPlacement();
        }
        
        // Deselect any selected tower
        DeselectTower();
        
        // Set the tower prefab and model based on type
        switch (towerType)
        {
            case TowerType.Archer:
                selectedTowerPrefab = archerTowerPrefab;
                selectedTowerModel = TowerModel.CreateArcherTower();
                break;
                
            case TowerType.Mage:
                selectedTowerPrefab = mageTowerPrefab;
                selectedTowerModel = TowerModel.CreateMageTower();
                break;
                
            case TowerType.Frost:
                selectedTowerPrefab = frostTowerPrefab;
                selectedTowerModel = TowerModel.CreateFrostTower();
                break;
                
            case TowerType.Cannon:
                selectedTowerPrefab = cannonTowerPrefab;
                selectedTowerModel = TowerModel.CreateCannonTower();
                break;
                
            default:
                Debug.LogError("Unknown tower type: " + towerType);
                return;
        }
        
        // Check if player has enough gold
        if (GameManager.Instance != null && 
            GameManager.Instance.PlayerGold < selectedTowerModel.buildCost)
        {
            Debug.Log("Not enough gold to build this tower!");
            return;
        }
        
        // Create preview
        CreateTowerPreview();
        
        // Set placing state
        isPlacingTower = true;
    }
    
    // Cancel tower placement
    public void CancelTowerPlacement()
    {
        if (towerPreview != null)
        {
            Destroy(towerPreview);
            towerPreview = null;
        }
        
        // Clear any highlight
        if (highlightedArea != null)
        {
            highlightedArea.Highlight(false);
            highlightedArea = null;
        }
        
        isPlacingTower = false;
        selectedTowerPrefab = null;
        selectedTowerModel = null;
    }
    
    // Select an existing tower
    public void SelectTower(TowerController tower)
    {
        // Deselect current tower if one is selected
        DeselectTower();
        
        // Cancel tower placement if active
        if (isPlacingTower)
        {
            CancelTowerPlacement();
        }
        
        // Select new tower
        selectedTower = tower;
        selectedTower.Select();
        
        // Trigger tower selected event
        OnTowerSelected.Invoke(selectedTower);
    }
    
    // Deselect the currently selected tower
    public void DeselectTower()
    {
        if (selectedTower != null)
        {
            selectedTower.Deselect();
            selectedTower = null;
            
            // Trigger tower deselected event
            OnTowerDeselected.Invoke();
        }
    }
    
    // Try to upgrade the selected tower
    public void TryUpgradeSelectedTower()
    {
        if (selectedTower == null || GameManager.Instance == null)
            return;
            
        int upgradeCost = selectedTower.TowerData.upgradeCost;
        
        if (GameManager.Instance.TrySpendGold(upgradeCost))
        {
            selectedTower.TryUpgrade(int.MaxValue); // Already spent the gold
        }
        else
        {
            Debug.Log("Not enough gold for upgrade!");
        }
    }
    
    // Sell the selected tower
    public void TrySellSelectedTower()
    {
        if (selectedTower == null || GameManager.Instance == null)
            return;
            
        // Calculate sell value (50% of total investment)
        int sellValue = (int)(selectedTower.TowerData.buildCost * 0.5f);
        
        // Add gold
        GameManager.Instance.AddGold(sellValue);
        
        // Find the placement area this tower is on
        TowerPlacementArea area = GetPlacementAreaAtPosition(selectedTower.transform.position);
        if (area != null)
        {
            area.RemoveTower();
        }
        
        // Destroy tower
        Destroy(selectedTower.gameObject);
        
        // Deselect tower
        DeselectTower();
    }
    
    // Create a preview of the tower being placed
    private void CreateTowerPreview()
    {
        if (selectedTowerPrefab == null) return;
        
        // Instantiate the preview
        towerPreview = Instantiate(selectedTowerPrefab);
        
        // Disable any scripts on the preview
        MonoBehaviour[] scripts = towerPreview.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = false;
        }
        
        // Set initial position
        towerPreview.transform.position = GetMouseWorldPosition();
        
        // Add a transparent material to show placement validity
        SpriteRenderer[] renderers = towerPreview.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers)
        {
            // Set initial material (invalid by default)
            renderer.material = invalidPlacementMaterial;
            
            // Make it semi-transparent
            Color color = renderer.color;
            color.a = 0.6f;
            renderer.color = color;
        }
    }
    
    // Update the tower preview appearance based on placement validity
    private void UpdateTowerPreview(bool isValid)
    {
        if (towerPreview == null) return;
        
        SpriteRenderer[] renderers = towerPreview.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.material = isValid ? validPlacementMaterial : invalidPlacementMaterial;
        }
    }
    
    // Place a tower at a specific placement area
    private void PlaceTower(TowerPlacementArea area)
    {
        if (selectedTowerPrefab == null || selectedTowerModel == null || 
            GameManager.Instance == null || area == null || area.IsOccupied) return;
        
        // Try to spend gold
        if (!GameManager.Instance.TrySpendGold(selectedTowerModel.buildCost))
        {
            Debug.Log("Not enough gold to place tower!");
            return;
        }
        
        // Remove preview
        Destroy(towerPreview);
        towerPreview = null;
        
        // Create actual tower
        GameObject towerObj = Instantiate(selectedTowerPrefab, area.transform.position, Quaternion.identity);
        TowerController tower = towerObj.GetComponent<TowerController>();
        
        if (tower != null)
        {
            // Initialize with the tower model
            tower.Initialize(selectedTowerModel);
            
            // Mark the area as occupied
            area.PlaceTower(tower);
        }
        
        // End placement mode
        isPlacingTower = false;
        
        // Clear highlight
        area.Highlight(false);
        highlightedArea = null;
    }
    
    // Get placement area at a position
    private TowerPlacementArea GetPlacementAreaAtPosition(Vector3 position)
    {
        Collider2D collider = Physics2D.OverlapPoint(position, placementAreaLayer);
        if (collider != null)
        {
            return collider.GetComponent<TowerPlacementArea>();
        }
        return null;
    }
    
    // Get tower at mouse position
    private TowerController GetTowerAtMousePosition()
    {
        Vector3 mousePos = GetMouseWorldPosition();
        Collider2D[] colliders = Physics2D.OverlapPointAll(mousePos);
        
        foreach (Collider2D collider in colliders)
        {
            TowerController tower = collider.GetComponent<TowerController>();
            if (tower != null)
            {
                return tower;
            }
        }
        
        return null;
    }
    
    // Get the mouse position in world space
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}

// Tower types enum
public enum TowerType
{
    Archer,
    Mage,
    Frost,
    Cannon
} 