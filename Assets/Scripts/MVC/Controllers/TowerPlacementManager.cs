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

    [Header("Mobile Touch")]
    [SerializeField] private bool touchPlacementEnabled = true;
    [SerializeField] private GameObject touchTargetIndicator;

    // Tower and placement data
    private GameObject selectedTowerPrefab;
    private GameObject ghostTower;
    private int selectedTowerCost;
    private SpriteRenderer ghostSpriteRenderer;
    private Tower ghostTowerComponent;
    private bool canPlace = false;
    private int gold = 300;
    private GameManager gameManager;
    private Camera mainCamera;

    // Touch variables
    private bool isTouchPlacementMode = false;
    private TowerButton.TowerType selectedTouchTowerType;
    private Coroutine touchPlacementCoroutine;

    private void Start()
    {
        mainCamera = Camera.main;
        gameManager = FindObjectOfType<GameManager>();
        
        if (gameManager != null)
            gold = gameManager.GetCurrentGold();
            
        UpdateGoldText();
        rangeIndicator.SetActive(false);
    }

    private void Update()
    {
        if (gameManager != null && goldText != null)
        {
            if (int.TryParse(goldText.text, out int displayedGold) && 
                displayedGold != gameManager.GetCurrentGold())
            {
                UpdateGoldText();
            }
        }
    }

    public void UpdateDragPosition(Vector2 screenPosition)
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0;

        if (ghostTower == null && selectedTowerPrefab != null)
            CreateGhostTower();

        if (ghostTower != null)
        {
            ghostTower.transform.position = worldPosition;
            CheckPlacementValidity();
        }
    }

    public bool TryPlaceTowerAtPosition(Vector2 screenPosition)
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0;

        if (ghostTower != null)
        {
            ghostTower.transform.position = worldPosition;
            CheckPlacementValidity();
            return PlaceTower();
        }
        
        return false;
    }

    private void CreateGhostTower()
    {
        if (selectedTowerPrefab == null)
            return;

        Tower towerComponent = selectedTowerPrefab.GetComponent<Tower>() ?? 
                               selectedTowerPrefab.GetComponent<ArcherTower>();
        
        if (towerComponent == null)
            return;

        ghostTower = Instantiate(selectedTowerPrefab, Vector3.zero, Quaternion.identity);
        
        // Make ghost transparent and disable scripts
        ApplyGhostMaterial(ghostTower);
        foreach (MonoBehaviour script in ghostTower.GetComponents<MonoBehaviour>())
            script.enabled = false;
            
        ghostTower.SetActive(false);
    }

    private void ApplyGhostMaterial(GameObject tower)
    {
        // Set ghost tower transparency
        ghostSpriteRenderer = tower.GetComponent<SpriteRenderer>();
        if (ghostSpriteRenderer != null)
        {
            Color ghostColor = ghostSpriteRenderer.color;
            ghostColor.a = 0.6f;
            ghostSpriteRenderer.color = ghostColor;
        }
        
        // Set child renderers transparency
        foreach (SpriteRenderer renderer in tower.GetComponentsInChildren<SpriteRenderer>())
        {
            if (renderer != ghostSpriteRenderer)
            {
                Color color = renderer.color;
                color.a = 0.6f;
                renderer.color = color;
            }
        }
        
        // Setup range indicator
        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(true);
            
            // Get tower range
            float range = 5f;
            Tower towerComp = tower.GetComponent<Tower>();
            
            if (towerComp != null)
            {
                range = towerComp.GetRange();
                ghostTowerComponent = towerComp;
            }
            else
            {
                // Try with specific tower types
                if (tower.GetComponent<ArcherTower>() != null)
                    range = tower.GetComponent<ArcherTower>().GetRange();
                else if (tower.GetComponent<MageTower>() != null)
                    range = tower.GetComponent<MageTower>().GetRange();
                else if (tower.GetComponent<FrostTower>() != null)
                    range = tower.GetComponent<FrostTower>().GetRange();
                else if (tower.GetComponent<CannonTower>() != null)
                    range = tower.GetComponent<CannonTower>().GetRange();
            }
            
            // Set indicator size and color
            rangeIndicator.transform.localScale = new Vector3(range * 2, range * 2, 1);
            
            SpriteRenderer rangeRenderer = rangeIndicator.GetComponent<SpriteRenderer>();
            if (rangeRenderer != null)
            {
                Color rangeColor = validPlacementColor;
                rangeColor.a = 0.3f;
                rangeRenderer.color = rangeColor;
            }
        }
        
        tower.SetActive(true);
    }

    private void CheckPlacementValidity()
    {
        rangeIndicator.transform.position = ghostTower.transform.position;
        canPlace = IsPlacementValid(ghostTower.transform.position);
        
        // Update ghost color based on validity
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
        Vector2 pos = new Vector2(position.x, position.y);
        
        bool isValidArea = Physics2D.OverlapPoint(pos, placementAreaLayer);
        bool isOccupied = Physics2D.OverlapCircle(pos, 0.6f, obstacleLayer);
        bool canAfford = CanAffordSelectedTower();
        
        return isValidArea && !isOccupied && canAfford;
    }

    private bool PlaceTower()
    {
        if (!canPlace)
        {
            CleanupPlacement();
            return false;
        }
        
        // Spend gold
        bool goldSpent = gameManager != null ? 
                         gameManager.SpendGold(selectedTowerCost) : 
                         SpendGoldInternal(selectedTowerCost);
        
        UpdateGoldText();
        
        if (goldSpent)
        {
            // Place the tower
            Vector3 position = ghostTower.transform.position;
            position.z = 0f;
            GameObject tower = Instantiate(selectedTowerPrefab, position, Quaternion.identity);
            
            // Setup tower layer for collision detection
            tower.layer = LayerMask.NameToLayer("Tower");
            foreach (Transform child in tower.transform)
                child.gameObject.layer = tower.layer;
            
            // Add collider if needed
            if (tower.GetComponent<Collider2D>() == null)
            {
                CircleCollider2D collider = tower.AddComponent<CircleCollider2D>();
                collider.radius = 0.6f;
            }
            
            // Provide placement feedback
            #if UNITY_IOS || UNITY_ANDROID
            Handheld.Vibrate();
            #endif
            
            CleanupPlacement();
            return true;
        }
        
        CleanupPlacement();
        return false;
    }
    
    private void CleanupPlacement()
    {
        if (ghostTower != null)
        {
            Destroy(ghostTower);
            ghostTower = null;
        }
        
        rangeIndicator.SetActive(false);
        DeselectTower();
        isTouchPlacementMode = false;
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
    
    private bool SpendGoldInternal(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }
        return false;
    }

    private void UpdateGoldText()
    {
        if (goldText != null)
        {
            if (gameManager != null)
            {
                goldText.text = gameManager.GetCurrentGold().ToString();
                gameManager.ForceUIUpdate();
            }
            else
                goldText.text = gold.ToString();
        }
    }

    public bool CanAffordTower(int cost)
    {
        return gameManager != null ? 
               gameManager.GetCurrentGold() >= cost : 
               gold >= cost;
    }

    private bool CanAffordSelectedTower()
    {
        return CanAffordTower(selectedTowerCost);
    }

    public void EnableTouchPlacementMode(TowerButton.TowerType towerType)
    {
        if (!(Application.platform == RuntimePlatform.Android || 
              Application.platform == RuntimePlatform.IPhonePlayer) && !touchPlacementEnabled)
            return;
            
        selectedTouchTowerType = towerType;
        
        // Select the appropriate tower
        switch(towerType)
        {
            case TowerButton.TowerType.Archer: SelectArcherTower(); break;
            case TowerButton.TowerType.Mage: SelectMageTower(); break;
            case TowerButton.TowerType.Frost: SelectFrostTower(); break;
            case TowerButton.TowerType.Cannon: SelectCannonTower(); break;
        }
        
        isTouchPlacementMode = true;
        
        // Create ghost and target indicator
        if (ghostTower == null && selectedTowerPrefab != null)
            CreateGhostTower();
            
        SetupTouchTargetIndicator();
        
        // Start touch tracking
        if (touchPlacementCoroutine != null)
            StopCoroutine(touchPlacementCoroutine);
            
        touchPlacementCoroutine = StartCoroutine(TrackTouchInput());
    }
    
    private void SetupTouchTargetIndicator()
    {
        if (touchTargetIndicator != null)
            return;
            
        touchTargetIndicator = new GameObject("TouchTargetIndicator");
        SpriteRenderer sr = touchTargetIndicator.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("UI/TouchTarget") ?? CreateCircleSprite();
        sr.color = new Color(1f, 1f, 1f, 0.5f);
        touchTargetIndicator.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }
    
    private IEnumerator TrackTouchInput()
    {
        // Show indicators
        if (ghostTower != null)
            ghostTower.SetActive(true);
            
        if (touchTargetIndicator != null)
            touchTargetIndicator.SetActive(true);
        
        // Track touch input
        while (isTouchPlacementMode)
        {
            // Get input position
            Vector3 inputPosition;
            
            if (Input.touchCount > 0)
            {
                inputPosition = Input.GetTouch(0).position;
                
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    TryPlaceTowerAtPosition(inputPosition);
                    break;
                }
            }
            else if (Input.GetMouseButton(0))
            {
                inputPosition = Input.mousePosition;
                
                if (Input.GetMouseButtonUp(0))
                {
                    TryPlaceTowerAtPosition(inputPosition);
                    break;
                }
            }
            else
                inputPosition = Input.mousePosition;
            
            // Update ghost position
            UpdateDragPosition(inputPosition);
            
            // Check for cancellation
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
            {
                CancelTouchPlacement();
                break;
            }
            
            yield return null;
        }
        
        CancelTouchPlacement();
    }
    
    private void CancelTouchPlacement()
    {
        isTouchPlacementMode = false;
        
        if (ghostTower != null)
        {
            Destroy(ghostTower);
            ghostTower = null;
        }
        
        if (rangeIndicator != null)
            rangeIndicator.SetActive(false);
            
        if (touchTargetIndicator != null)
            touchTargetIndicator.SetActive(false);
    }
    
    private Sprite CreateCircleSprite()
    {
        int resolution = 64;
        Texture2D texture = new Texture2D(resolution, resolution);
        Color[] colors = new Color[resolution * resolution];
        
        float centerX = resolution / 2f;
        float centerY = resolution / 2f;
        float radius = resolution / 2f - 2f;
        
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float distance = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                colors[y * resolution + x] = distance < radius ? Color.white : Color.clear;
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f));
    }
} 