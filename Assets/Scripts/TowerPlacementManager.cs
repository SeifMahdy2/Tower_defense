using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using TD;

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
    private Vector3 touchOffset;
    private SpriteRenderer ghostSpriteRenderer;
    private Tower ghostTowerComponent;
    private bool canPlace = false;

    // Gold management
    private int gold = 300; // Starting gold
    private GameManager gameManager; // Add reference to the GameManager

    // Camera reference for converting screen to world position
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        
        // Find the GameManager in the scene
        gameManager = FindObjectOfType<GameManager>();
        
        // If there's no GameManager, use the default gold value
        if (gameManager == null)
        {
            Debug.LogWarning("No GameManager found! Using default gold value.");
        }
        else
        {
            // If GameManager exists, use its gold instead
            gold = gameManager.GetCurrentGold();
            Debug.Log("Found GameManager. Starting gold: " + gold);
        }
        
        // Always update gold text at start
        UpdateGoldText();
        
        // Log the current gold text value for debugging
        if (goldText != null)
        {
            Debug.Log("Gold text initialized to: " + goldText.text);
        }
        else
        {
            Debug.LogError("Gold text reference is missing! Gold display won't work.");
        }
        
        rangeIndicator.SetActive(false);
    }

    private void Update()
    {
        // Check if the game manager exists and game is over
        if (gameManager != null && GetComponent<TowerPlacementManager>().enabled)
        {
            // Check if goldText isn't being updated properly
            if (goldText != null && int.TryParse(goldText.text, out int displayedGold))
            {
                if (displayedGold != gameManager.GetCurrentGold())
                {
                    // Force update if the displayed gold doesn't match the actual gold
                    UpdateGoldText();
                }
            }
        }
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

        // First try to find the Tower component
        Tower towerComponent = selectedTowerPrefab.GetComponent<Tower>();
        
        // If Tower component is not found, try to find any derived type (like ArcherTower)
        if (towerComponent == null)
        {
            towerComponent = selectedTowerPrefab.GetComponent<ArcherTower>();
            
            if (towerComponent == null)
            {
                Debug.LogError("The selected tower prefab does not have a Tower component!");
                return;
            }
        }

        // Create the ghost tower
        ghostTower = Instantiate(selectedTowerPrefab, Vector3.zero, Quaternion.identity);
        
        // Make it semi-transparent
        ApplyGhostMaterial(ghostTower);
        
        // Disable any scripts
        MonoBehaviour[] scripts = ghostTower.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = false;
        }
        
        // Set ghost tower inactive initially
        ghostTower.SetActive(false);
    }

    private void ApplyGhostMaterial(GameObject tower)
    {
        // Get the sprite renderer and set opacity
        ghostSpriteRenderer = tower.GetComponent<SpriteRenderer>();
        if (ghostSpriteRenderer != null)
        {
            Color ghostColor = ghostSpriteRenderer.color;
            ghostColor.a = 0.6f;
            ghostSpriteRenderer.color = ghostColor;
        }
        else
        {
            Debug.LogWarning("Ghost tower has no SpriteRenderer component");
        }
        
        // Get child sprite renderers if any
        SpriteRenderer[] childRenderers = tower.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in childRenderers)
        {
            if (renderer != ghostSpriteRenderer) // Don't process the main one twice
            {
                Color color = renderer.color;
                color.a = 0.6f;
                renderer.color = color;
            }
        }
        
        // Show and configure range indicator
        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(true);
            
            // Get tower range and set indicator scale
            float range = 5f; // Default range
            
            // Try to get the range from the tower component
            Tower towerComponent = tower.GetComponent<Tower>();
            if (towerComponent != null)
            {
                range = towerComponent.GetRange();
                ghostTowerComponent = towerComponent; // Store reference for later use
            }
            else
            {
                // Try with specific tower types if base class not found
                ArcherTower archerTower = tower.GetComponent<ArcherTower>();
                if (archerTower != null)
                {
                    range = archerTower.GetRange();
                }
                
                MageTower mageTower = tower.GetComponent<MageTower>();
                if (mageTower != null)
                {
                    range = mageTower.GetRange();
                }
                
                FrostTower frostTower = tower.GetComponent<FrostTower>();
                if (frostTower != null)
                {
                    range = frostTower.GetRange();
                }
                
                CannonTower cannonTower = tower.GetComponent<CannonTower>();
                if (cannonTower != null)
                {
                    range = cannonTower.GetRange();
                }
            }
            
            // Set the range indicator scale
            rangeIndicator.transform.localScale = new Vector3(range * 2, range * 2, 1);
            
            // Set initial color
            SpriteRenderer rangeRenderer = rangeIndicator.GetComponent<SpriteRenderer>();
            if (rangeRenderer != null)
            {
                Color rangeColor = validPlacementColor;
                rangeColor.a = 0.3f;
                rangeRenderer.color = rangeColor;
            }
        }
        
        // Activate the ghost tower
        tower.SetActive(true);
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
        // Get the current position
        Vector2 pos = new Vector2(position.x, position.y);
        
        // Check if placement is in a valid area for towers
        bool isValidArea = Physics2D.OverlapPoint(pos, placementAreaLayer);
        
        // Check if position is already occupied by another tower or obstacle
        bool isOccupied = Physics2D.OverlapCircle(pos, 0.6f, obstacleLayer);

        // Check if we can afford it
        bool canAfford = CanAffordSelectedTower();
        
        // Don't check if game is over - allow placement regardless of wave status
        
        return isValidArea && !isOccupied && canAfford;
    }

    private void TryPlaceTower()
    {
        if (canPlace)
        {
            // Deduct gold
            bool goldSpent = false;
            
            if (gameManager != null)
            {
                // Use GameManager's gold system
                goldSpent = gameManager.SpendGold(selectedTowerCost);
            }
            else
            {
                // Fallback to internal gold
                gold -= selectedTowerCost;
                goldSpent = true;
            }
            
            // Always update the gold text immediately after spending gold
            UpdateGoldText();
            
            if (goldSpent)
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
                
                // Make sure gold display is updated again after tower placement
                UpdateGoldText();
                
                // Play placement sound
                // audioSource.PlayOneShot(placementSound);
                
                // Add haptic feedback for mobile
                #if UNITY_IOS || UNITY_ANDROID
                Handheld.Vibrate();
                #endif
            }
        }
        
        // Clean up ghost tower
        Destroy(ghostTower);
        ghostTower = null;
        rangeIndicator.SetActive(false);
        
        // Deselect tower
        DeselectTower();
        
        // Update gold text one final time
        UpdateGoldText();
    }

    public void SelectArcherTower()
    {
        selectedTowerPrefab = archerTowerPrefab;
        selectedTowerCost = archerTowerCost;
        CleanupGhost();
        UpdateGoldText();
    }

    public void SelectMageTower()
    {
        selectedTowerPrefab = mageTowerPrefab;
        selectedTowerCost = mageTowerCost;
        CleanupGhost();
        UpdateGoldText();
    }

    public void SelectFrostTower()
    {
        selectedTowerPrefab = frostTowerPrefab;
        selectedTowerCost = frostTowerCost;
        CleanupGhost();
        UpdateGoldText();
    }

    public void SelectCannonTower()
    {
        selectedTowerPrefab = cannonTowerPrefab;
        selectedTowerCost = cannonTowerCost;
        CleanupGhost();
        UpdateGoldText();
    }

    public void DeselectTower()
    {
        selectedTowerPrefab = null;
        selectedTowerCost = 0;
        CleanupGhost();
        UpdateGoldText();
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
            if (gameManager != null)
            {
                goldText.text = gameManager.GetCurrentGold().ToString();
                // Also force the UI update in GameManager to ensure consistency
                gameManager.ForceUIUpdate();
            }
            else
            {
                goldText.text = gold.ToString();
            }
        }
        else
        {
            Debug.LogWarning("TowerPlacementManager: Gold text reference is missing!");
        }
    }

    // Method to check if the player can afford a tower
    public bool CanAffordTower(int cost)
    {
        if (gameManager != null)
        {
            return gameManager.GetCurrentGold() >= cost;
        }
        else
        {
            return gold >= cost;
        }
    }

    private bool CanAffordSelectedTower()
    {
        if (gameManager != null)
        {
            return gameManager.GetCurrentGold() >= selectedTowerCost;
        }
        else
        {
            return gold >= selectedTowerCost;
        }
    }
} 