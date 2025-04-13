using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TowerPlacementManager : MonoBehaviour
{
    [Header("Tower Prefabs")]
    [SerializeField] private GameObject archerTowerPrefab;
    [SerializeField] private GameObject mageTowerPrefab;
    [SerializeField] private GameObject frostTowerPrefab;
    [SerializeField] private GameObject cannonTowerPrefab;

    [Header("Tower Costs")]
    [SerializeField] private int archerTowerCost = 100;
    [SerializeField] private int mageTowerCost = 150;
    [SerializeField] private int frostTowerCost = 125;
    [SerializeField] private int cannonTowerCost = 200;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private GameObject rangeIndicator;
    [SerializeField] private Color validPlacementColor = Color.green;
    [SerializeField] private Color invalidPlacementColor = Color.red;

    [Header("Placement Settings")]
    [SerializeField] private LayerMask placementAreaLayer;
    [SerializeField] private LayerMask obstacleLayer;

    // Tower selection and placement variables
    private GameObject selectedTowerPrefab;
    private GameObject ghostTower;
    private int selectedTowerCost;
    private bool isDragging = false;
    private Vector3 touchOffset;
    private SpriteRenderer ghostSpriteRenderer;
    private Tower ghostTowerComponent;
    private bool canPlace = false;

    // Gold management
    private int gold = 300; // Starting gold

    // Camera reference for converting screen to world position
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        UpdateGoldText();
        rangeIndicator.SetActive(false);
    }

    private void Update()
    {
        // Direct tower placement code is kept for testing purposes
        // but is now triggered by the drag and drop system
    }

    // Called by TowerButton when dragging begins
    public void UpdateDragPosition(Vector2 screenPosition)
    {
        // Convert screen position to world position
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0;

        // Create ghost tower if it doesn't exist
        if (ghostTower == null && selectedTowerPrefab != null)
        {
            CreateGhostTower();
        }

        if (ghostTower != null)
        {
            // Update ghost tower position
            ghostTower.transform.position = worldPosition;
            
            // Check placement validity
            CheckPlacementValidity();
        }
    }

    // Called by TowerButton when drag ends
    public void TryPlaceTowerAtPosition(Vector2 screenPosition)
    {
        // Convert screen position to world position
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0;

        // Update ghost position one last time
        if (ghostTower != null)
        {
            ghostTower.transform.position = worldPosition;
            CheckPlacementValidity();
            
            // Try to place the tower
            TryPlaceTower();
        }
    }

    private void CreateGhostTower()
    {
        if (selectedTowerPrefab == null)
        {
            Debug.LogError("No tower prefab selected!");
            return;
        }

        // Instantiate the ghost tower from the selected prefab
        ghostTower = Instantiate(selectedTowerPrefab, Vector3.zero, Quaternion.identity);
        
        // Get the sprite renderer and set opacity
        ghostSpriteRenderer = ghostTower.GetComponent<SpriteRenderer>();
        Color ghostColor = ghostSpriteRenderer.color;
        ghostColor.a = 0.6f;
        ghostSpriteRenderer.color = ghostColor;
        
        // Get the tower component for range display
        ghostTowerComponent = ghostTower.GetComponent<Tower>();
        if (ghostTowerComponent == null)
        {
            Debug.LogError("The selected tower prefab does not have a Tower component!");
            Destroy(ghostTower);
            return;
        }
        
        // Show range indicator
        rangeIndicator.SetActive(true);
        rangeIndicator.transform.localScale = new Vector3(
            ghostTowerComponent.GetRange() * 2, 
            ghostTowerComponent.GetRange() * 2, 
            1
        );
        
        // Add "Ghost" tag for identification
        // ghostTower.tag = "Ghost"; // Using tags requires defining them in Tag Manager
        ghostTower.name = "GhostTower"; // Using name instead of tag
        
        // Disable colliders and scripts
        Collider2D[] colliders = ghostTower.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
        
        MonoBehaviour[] scripts = ghostTower.GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != ghostTowerComponent)
                script.enabled = false;
        }
    }

    private void CheckPlacementValidity()
    {
        // Update range indicator position
        rangeIndicator.transform.position = ghostTower.transform.position;
        
        // Check if placement is valid
        canPlace = IsPlacementValid(ghostTower.transform.position);
        
        // Update ghost tower color based on placement validity
        Color currentColor = ghostSpriteRenderer.color;
        currentColor = canPlace ? validPlacementColor : invalidPlacementColor;
        currentColor.a = 0.6f;
        ghostSpriteRenderer.color = currentColor;
        
        // Update range indicator color
        SpriteRenderer rangeImage = rangeIndicator.GetComponent<SpriteRenderer>();
        Color rangeColor = canPlace ? validPlacementColor : invalidPlacementColor;
        rangeColor.a = 0.3f;
        rangeImage.color = rangeColor;
    }

    private bool IsPlacementValid(Vector3 position)
    {
        // Check if position is on a valid placement area
        Collider2D placementArea = Physics2D.OverlapCircle(position, 0.3f, placementAreaLayer);
        if (placementArea == null)
        {
            Debug.Log("No valid placement area found");
            return false;
        }
        
        // Check for any towers or obstacles using the Tower layer
        Collider2D[] obstacles = Physics2D.OverlapCircleAll(position, 1.5f, obstacleLayer);
        if (obstacles.Length > 0)
        {
            Debug.Log("Found " + obstacles.Length + " obstacles at position " + position);
            foreach (var obstacle in obstacles)
            {
                Debug.Log("Obstacle: " + obstacle.name + " on layer: " + LayerMask.LayerToName(obstacle.gameObject.layer));
            }
            return false;
        }
        
        // Check if player has enough gold
        if (gold < selectedTowerCost)
            return false;
        
        return true;
    }

    private void TryPlaceTower()
    {
        if (canPlace)
        {
            // Place actual tower with proper z-position
            Vector3 placementPosition = ghostTower.transform.position;
            placementPosition.z = 0f; // Ensure z is 0 to appear in front of the map
            GameObject tower = Instantiate(selectedTowerPrefab, placementPosition, Quaternion.identity);
            
            // Explicitly set the layer to Tower layer to prevent future placement
            tower.layer = LayerMask.NameToLayer("Tower");
            Debug.Log("Set tower layer to: " + tower.layer + " Layer name: " + LayerMask.LayerToName(tower.layer));
            
            // Make sure all child objects are also on the Tower layer
            foreach (Transform child in tower.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Tower");
            }
            
            // Add a collider if it doesn't have one
            if (tower.GetComponent<Collider2D>() == null)
            {
                CircleCollider2D collider = tower.AddComponent<CircleCollider2D>();
                collider.radius = 0.6f;
                Debug.Log("Added collider with radius: " + collider.radius);
            }
            
            // Deduct gold
            gold -= selectedTowerCost;
            UpdateGoldText();
            
            // Play placement sound
            // audioSource.PlayOneShot(placementSound);
            
            // Add haptic feedback for mobile
            #if UNITY_IOS || UNITY_ANDROID
            Handheld.Vibrate();
            #endif
        }
        
        // Clean up ghost tower
        Destroy(ghostTower);
        ghostTower = null;
        rangeIndicator.SetActive(false);
        
        // Deselect tower
        DeselectTower();
    }

    public void SelectArcherTower()
    {
        selectedTowerPrefab = archerTowerPrefab;
        selectedTowerCost = archerTowerCost;
        CleanupGhost();
    }

    public void SelectMageTower()
    {
        selectedTowerPrefab = mageTowerPrefab;
        selectedTowerCost = mageTowerCost;
        CleanupGhost();
    }

    public void SelectFrostTower()
    {
        selectedTowerPrefab = frostTowerPrefab;
        selectedTowerCost = frostTowerCost;
        CleanupGhost();
    }

    public void SelectCannonTower()
    {
        selectedTowerPrefab = cannonTowerPrefab;
        selectedTowerCost = cannonTowerCost;
        CleanupGhost();
    }

    public void DeselectTower()
    {
        selectedTowerPrefab = null;
        selectedTowerCost = 0;
        CleanupGhost();
    }

    private void CleanupGhost()
    {
        if (ghostTower != null)
        {
            Destroy(ghostTower);
            ghostTower = null;
        }
        rangeIndicator.SetActive(false);
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldText();
    }

    private void UpdateGoldText()
    {
        if (goldText != null)
        {
            goldText.text = gold.ToString();
        }
    }

    // Method to check if the player can afford a tower
    public bool CanAffordTower(int towerCost)
    {
        return gold >= towerCost;
    }
} 