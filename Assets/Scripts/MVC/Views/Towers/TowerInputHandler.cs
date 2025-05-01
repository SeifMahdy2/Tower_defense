using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TD;

public class TowerInputHandler : MonoBehaviour
{
    [Header("Touch Settings")]
    [SerializeField] private LayerMask towerLayer;
    [SerializeField] private LayerMask placementLayer;
    
    private Camera mainCamera;
    private Tower selectedTower;
    
    void Awake()
    {
        mainCamera = Camera.main;
    }
    
    void Update()
    {
            HandleDesktopInput();
    }
    
    private void HandleDesktopInput()
    {
        // Left click for selection
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 100f, towerLayer))
            {
                Tower tower = hit.collider.GetComponent<Tower>();
                if (tower != null)
                {
                    SelectTower(tower);
                }
            }
        }
    }
    
    
    private void SelectTower(Tower tower)
    {
        selectedTower = tower;
        
        // Show tower upgrade panel if available
        TowerUpgradePanel upgradePanel = FindObjectOfType<TowerUpgradePanel>();
        if (upgradePanel != null)
        {
            upgradePanel.ShowForTower(tower);
        }
    }
    
    private bool IsPointerOverUI()
    {
        // For desktop
        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject())
            return true;
        
        return false;
    }
} 