using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefense.Controllers;
using TowerDefense.Models;

namespace TowerDefense.Views
{
    public class TowerPlacementView : MonoBehaviour
    {
        [Header("Placement Settings")]
        [SerializeField] private LayerMask placementLayer;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private Color validPlacementColor = Color.green;
        [SerializeField] private Color invalidPlacementColor = Color.red;
        
        // Placement visualization
        private GameObject placementIndicator;
        private SpriteRenderer indicatorRenderer;
        private bool canPlaceHere = false;
        
        // Controllers reference
        private TowerController towerController;
        
        private void Start()
        {
            // Get controller reference
            towerController = TowerController.Instance;
            
            // Create placement indicator
            CreatePlacementIndicator();
            
            // Subscribe to tower selection events
            if (towerController != null)
            {
                towerController.OnTowerSelected += HandleTowerSelected;
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (towerController != null)
            {
                towerController.OnTowerSelected -= HandleTowerSelected;
            }
        }
        
        private void CreatePlacementIndicator()
        {
            // Create indicator object
            placementIndicator = new GameObject("PlacementIndicator");
            indicatorRenderer = placementIndicator.AddComponent<SpriteRenderer>();
            
            // Set default properties
            indicatorRenderer.color = invalidPlacementColor;
            
            // Hide by default
            placementIndicator.SetActive(false);
        }
        
        private void HandleTowerSelected(TowerType towerType)
        {
            // Tower selected, update indicator
            if (towerController.IsBuildingTower() && placementIndicator != null)
            {
                // Get tower data
                TowerData towerData = towerController.GetTowerData(towerType);
                
                if (towerData != null && towerData.Prefab != null)
                {
                    // Get sprite from prefab
                    SpriteRenderer towerRenderer = towerData.Prefab.GetComponent<SpriteRenderer>();
                    if (towerRenderer != null && indicatorRenderer != null)
                    {
                        indicatorRenderer.sprite = towerRenderer.sprite;
                        
                        // Adjust alpha for transparency
                        Color color = indicatorRenderer.color;
                        color.a = 0.5f;
                        indicatorRenderer.color = color;
                    }
                    
                    // Show indicator
                    placementIndicator.SetActive(true);
                }
            }
            else
            {
                // Hide indicator if not building
                if (placementIndicator != null)
                {
                    placementIndicator.SetActive(false);
                }
            }
        }
        
        private void Update()
        {
            // Update placement position and check if we can place here
            UpdatePlacementPosition();
            
            // Check for placement input
            CheckPlacementInput();
            
            // Check for cancellation
            CheckCancellation();
        }
        
        private void UpdatePlacementPosition()
        {
            if (!towerController.IsBuildingTower() || placementIndicator == null)
            {
                return;
            }
            
            // Get mouse position in world space
            Vector3 mousePosition = GetMouseWorldPosition();
            
            // Set indicator position
            placementIndicator.transform.position = mousePosition;
            
            // Check if placement is valid
            canPlaceHere = IsValidPlacement(mousePosition);
            
            // Update indicator color
            if (indicatorRenderer != null)
            {
                indicatorRenderer.color = canPlaceHere ? validPlacementColor : invalidPlacementColor;
                
                // Keep alpha at 0.5
                Color color = indicatorRenderer.color;
                color.a = 0.5f;
                indicatorRenderer.color = color;
            }
        }
        
        private Vector3 GetMouseWorldPosition()
        {
            // Get mouse position in screen space
            Vector3 mousePos = Input.mousePosition;
            
            // Convert to world position
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.z = 0; // Keep at z=0 for 2D
            
            return worldPos;
        }
        
        private bool IsValidPlacement(Vector3 position)
        {
            // Check if position is on placement layer
            Collider2D placementCollider = Physics2D.OverlapPoint(position, placementLayer);
            
            // Check if position is not on obstacle layer
            Collider2D obstacleCollider = Physics2D.OverlapPoint(position, obstacleLayer);
            
            // Valid if on placement layer and not on obstacle
            return placementCollider != null && obstacleCollider == null;
        }
        
        private void CheckPlacementInput()
        {
            if (!towerController.IsBuildingTower())
            {
                return;
            }
            
            // Check for left mouse button click
            if (Input.GetMouseButtonDown(0))
            {
                // Try to place tower
                if (canPlaceHere)
                {
                    PlaceTower();
                }
            }
        }
        
        private void PlaceTower()
        {
            // Get position
            Vector3 position = GetMouseWorldPosition();
            
            // Place tower via controller
            bool success = towerController.PlaceTower(position);
            
            if (success)
            {
                // Tower placed, hide indicator
                if (placementIndicator != null)
                {
                    placementIndicator.SetActive(false);
                }
            }
        }
        
        private void CheckCancellation()
        {
            if (!towerController.IsBuildingTower())
            {
                return;
            }
            
            // Check for right mouse button click or escape key
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                // Cancel tower building
                towerController.CancelTowerBuilding();
                
                // Hide indicator
                if (placementIndicator != null)
                {
                    placementIndicator.SetActive(false);
                }
            }
        }
    }
} 