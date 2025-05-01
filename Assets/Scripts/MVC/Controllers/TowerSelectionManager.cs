using UnityEngine;
using UnityEngine.EventSystems;
using TD;

public class TowerSelectionManager : MonoBehaviour
{
    [SerializeField] private LayerMask towerLayer;
    [SerializeField] private TowerUpgradePanel upgradePanel;
    [SerializeField] private float touchRadius = 0.5f; // Works for both mouse and touch

    private Tower selectedTower;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        
        // If tower layer mask is not set, try to find the Tower layer
        if (towerLayer.value == 0)
        {
            int towerLayerIndex = LayerMask.NameToLayer("Tower");
            if (towerLayerIndex != -1)
            {
                towerLayer = 1 << towerLayerIndex;
                Debug.Log("Found Tower layer automatically");
            }
            else
            {
                Debug.LogWarning("Tower layer not found! Create a layer named 'Tower' for tower selection to work.");
            }
        }
        
        // Make sure we have a reference to the upgrade panel
        if (upgradePanel == null)
        {
            upgradePanel = FindObjectOfType<TowerUpgradePanel>();
            if (upgradePanel == null)
            {
                Debug.LogError("TowerUpgradePanel not found in scene! Tower selection won't work properly.");
            }
        }
    }

    private void Update()
    {
        // Handle mouse or touch input
        bool userClicked = Input.GetMouseButtonDown(0);
        Vector2 inputPosition = Input.mousePosition;
        
        // Use touch position if available (on mobile)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            userClicked = true;
            inputPosition = Input.GetTouch(0).position;
        }
        
        if (userClicked)
        {
            // Skip if over UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;
                
            // Convert to world point
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(inputPosition);
            
            // Use CircleCast for better touch accuracy (works well for mouse too)
            RaycastHit2D hit = Physics2D.CircleCast(worldPoint, touchRadius, Vector2.zero, 0f, towerLayer);
            
            if (hit.collider != null)
            {
                Tower tower = hit.collider.GetComponent<Tower>();
                if (tower != null)
                {
                    // Select the tower
                    selectedTower = tower;
                    
                    // Show upgrade panel for this tower
                    if (upgradePanel != null)
                    {
                        upgradePanel.ShowForTower(tower);
                    }
                    
                    Debug.Log($"Selected tower: {tower.towerName}");
                }
            }
            else
            {
                // Clicked on empty space, deselect tower
                if (selectedTower != null)
                {
                    selectedTower = null;
                    
                    // Hide upgrade panel
                    if (upgradePanel != null)
                    {
                        upgradePanel.Hide();
                    }
                }
            }
        }
    }
} 