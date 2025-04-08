using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [Header("Tower Prefabs")]
    [SerializeField] private GameObject archerPrefab;
    [SerializeField] private GameObject magePrefab;
    [SerializeField] private GameObject frostPrefab;
    [SerializeField] private GameObject cannonPrefab;
    
    [Header("UI References")]
    [SerializeField] private GameObject selectionIndicator;
    
    // References
    private Camera mainCamera;
    private GameManager gameManager;
    
    // Tower selection
    private GameObject selectedTowerPrefab;
    private int selectedTowerCost;
    
    // Placement state
    private bool isPlacingTower = false;
    private GameObject previewTower;
    private SpriteRenderer previewRenderer;
    private Color validPlacementColor = new Color(0, 1, 0, 0.5f);
    private Color invalidPlacementColor = new Color(1, 0, 0, 0.5f);
    
    private void Start()
    {
        // Get references
        mainCamera = Camera.main;
        gameManager = FindObjectOfType<GameManager>();
        
        // Hide selection indicator if exists
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (isPlacingTower)
        {
            // Update preview tower position to follow mouse
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            previewTower.transform.position = mouseWorldPos;
            
            // Check if this is a valid placement position
            bool validPlacement = IsValidPlacement(mouseWorldPos);
            
            // Update preview color
            if (previewRenderer != null)
            {
                previewRenderer.color = validPlacement ? validPlacementColor : invalidPlacementColor;
            }
            
            // Show range preview
            ShowRangePreview(previewTower, true);
            
            // Check for click to place tower
            if (Input.GetMouseButtonDown(0) && validPlacement)
            {
                PlaceTower(mouseWorldPos);
            }
            
            // Cancel placement with right-click
            if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
            }
        }
    }
    
    public void SelectTowerToBuild(TowerType towerType)
    {
        // Cancel any existing placement
        if (isPlacingTower)
        {
            CancelPlacement();
        }
        
        GameObject towerPrefab = GetTowerPrefab(towerType);
        int towerCost = GetTowerCost(towerType);
        
        if (towerPrefab != null && gameManager != null && gameManager.HasEnoughGold(towerCost))
        {
            selectedTowerPrefab = towerPrefab;
            selectedTowerCost = towerCost;
            
            // Create preview tower
            previewTower = Instantiate(selectedTowerPrefab, Vector3.zero, Quaternion.identity);
            
            // Setup preview
            SetupPreviewTower(previewTower);
            
            // Enable placement mode
            isPlacingTower = true;
            
            // Hide selection indicator
            if (selectionIndicator != null)
            {
                selectionIndicator.SetActive(false);
            }
        }
        else
        {
            // Not enough gold
            Debug.Log("Not enough gold to build this tower!");
        }
    }
    
    private void SetupPreviewTower(GameObject tower)
    {
        // Disable all colliders
        Collider2D[] colliders = tower.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
        
        // Get sprite renderer
        previewRenderer = tower.GetComponent<SpriteRenderer>();
        if (previewRenderer != null)
        {
            previewRenderer.color = validPlacementColor;
        }
        
        // Disable any scripts
        MonoBehaviour[] scripts = tower.GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = false;
        }
        
        // Name this as preview
        tower.name = "Tower Preview";
    }
    
    private bool IsValidPlacement(Vector3 position)
    {
        // Check if position overlaps with other towers or obstacles
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, 0.5f);
        foreach (Collider2D collider in hitColliders)
        {
            // Check if it's a tower, obstacle, or path
            if (collider.CompareTag("Tower") || collider.CompareTag("Obstacle") || collider.CompareTag("Path"))
            {
                return false;
            }
        }
        
        // Check if player has enough gold
        if (gameManager != null && !gameManager.HasEnoughGold(selectedTowerCost))
        {
            return false;
        }
        
        // Check if position is on a valid tile (could do a raycast against a tilemap)
        return true;
    }
    
    private void PlaceTower(Vector3 position)
    {
        if (gameManager != null && gameManager.SpendGold(selectedTowerCost))
        {
            // Destroy preview
            Destroy(previewTower);
            
            // Create actual tower
            GameObject newTower = Instantiate(selectedTowerPrefab, position, Quaternion.identity);
            newTower.name = selectedTowerPrefab.name;
            
            // Enable tower functionality
            MonoBehaviour[] scripts = newTower.GetComponentsInChildren<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                script.enabled = true;
            }
            
            // Enable colliders
            Collider2D[] colliders = newTower.GetComponentsInChildren<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = true;
            }
            
            // Tag as tower
            newTower.tag = "Tower";
            
            // Reset state
            isPlacingTower = false;
            selectedTowerPrefab = null;
            previewTower = null;
            previewRenderer = null;
        }
    }
    
    private void CancelPlacement()
    {
        if (previewTower != null)
        {
            Destroy(previewTower);
        }
        
        isPlacingTower = false;
        selectedTowerPrefab = null;
        previewTower = null;
        previewRenderer = null;
    }
    
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -mainCamera.transform.position.z;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0;
        return worldPosition;
    }
    
    private GameObject GetTowerPrefab(TowerType type)
    {
        switch (type)
        {
            case TowerType.Archer:
                return archerPrefab;
            case TowerType.Mage:
                return magePrefab;
            case TowerType.Frost:
                return frostPrefab;
            case TowerType.Cannon:
                return cannonPrefab;
            default:
                return null;
        }
    }
    
    private int GetTowerCost(TowerType type)
    {
        switch (type)
        {
            case TowerType.Archer:
                return archerPrefab.GetComponent<Archer>()?.GetCost() ?? 100;
            case TowerType.Mage:
                return magePrefab.GetComponent<Mage>()?.GetCost() ?? 175;
            case TowerType.Frost:
                return frostPrefab.GetComponent<Frost>()?.GetCost() ?? 150;
            case TowerType.Cannon:
                return cannonPrefab.GetComponent<Cannon>()?.GetCost() ?? 200;
            default:
                return 100;
        }
    }
    
    private void ShowRangePreview(GameObject tower, bool show)
    {
        // Show range for various tower types
        Archer archer = tower.GetComponent<Archer>();
        if (archer != null)
        {
            archer.ShowRange(show);
            return;
        }
        
        Mage mage = tower.GetComponent<Mage>();
        if (mage != null)
        {
            mage.ShowRange(show);
            return;
        }
        
        Frost frost = tower.GetComponent<Frost>();
        if (frost != null)
        {
            frost.ShowRange(show);
            return;
        }
        
        Cannon cannon = tower.GetComponent<Cannon>();
        if (cannon != null)
        {
            cannon.ShowRange(show);
            return;
        }
    }
}

// Tower types enum for the tower selection UI
public enum TowerType
{
    Archer,
    Mage,
    Frost,
    Cannon
} 