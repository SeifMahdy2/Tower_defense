using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefense.Models;

namespace TowerDefense.Controllers
{
    public class TowerController : MonoBehaviour
    {
        [Header("Tower Prefabs")]
        [SerializeField] private GameObject archerPrefab;
        [SerializeField] private GameObject magePrefab;
        [SerializeField] private GameObject frostPrefab;
        [SerializeField] private GameObject cannonPrefab;
        
        [Header("Tower Descriptions")]
        [SerializeField] private string archerDescription = "Basic tower with good rate of fire.";
        [SerializeField] private string mageDescription = "High damage but slower fire rate.";
        [SerializeField] private string frostDescription = "Slows enemies in range.";
        [SerializeField] private string cannonDescription = "Area damage to multiple enemies.";
        
        // Selected tower
        private TowerType selectedTowerType;
        private bool isBuildingTower = false;
        
        // Models
        private TowerModel towerModel;
        
        // Singleton instance
        public static TowerController Instance { get; private set; }
        
        // Events
        public delegate void TowerSelected(TowerType towerType);
        public event TowerSelected OnTowerSelected;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            // Initialize tower data
            InitializeTowerData();
        }
        
        private void InitializeTowerData()
        {
            // Create tower data array
            TowerData[] towerData = new TowerData[4];
            
            // Archer Tower
            towerData[0] = new TowerData {
                Type = TowerType.Archer,
                Cost = GetTowerCost(archerPrefab, "Archer"),
                Range = 6f,
                FireRate = 1f,
                Damage = 10,
                Prefab = archerPrefab,
                Description = archerDescription
            };
            
            // Mage Tower
            towerData[1] = new TowerData {
                Type = TowerType.Mage,
                Cost = GetTowerCost(magePrefab, "Mage"),
                Range = 5f,
                FireRate = 1.5f,
                Damage = 25,
                Prefab = magePrefab,
                Description = mageDescription
            };
            
            // Frost Tower
            towerData[2] = new TowerData {
                Type = TowerType.Frost,
                Cost = GetTowerCost(frostPrefab, "Frost"),
                Range = 4f,
                FireRate = 1.2f,
                Damage = 5,
                Prefab = frostPrefab,
                Description = frostDescription
            };
            
            // Cannon Tower
            towerData[3] = new TowerData {
                Type = TowerType.Cannon,
                Cost = GetTowerCost(cannonPrefab, "Cannon"),
                Range = 5f,
                FireRate = 2f,
                Damage = 15,
                Prefab = cannonPrefab,
                Description = cannonDescription
            };
            
            // Create tower model
            towerModel = new TowerModel(towerData);
        }
        
        // Helper method to extract tower cost from prefabs
        private int GetTowerCost(GameObject towerPrefab, string towerName)
        {
            int cost = 100; // Default cost
            
            if (towerPrefab != null)
            {
                // Try to get cost based on tower type
                switch (towerName)
                {
                    case "Archer":
                        Archer archer = towerPrefab.GetComponent<Archer>();
                        if (archer != null) cost = archer.GetCost();
                        break;
                    case "Mage":
                        Mage mage = towerPrefab.GetComponent<Mage>();
                        if (mage != null) cost = mage.GetCost();
                        break;
                    case "Frost":
                        Frost frost = towerPrefab.GetComponent<Frost>();
                        if (frost != null) cost = frost.GetCost();
                        break;
                    case "Cannon":
                        Cannon cannon = towerPrefab.GetComponent<Cannon>();
                        if (cannon != null) cost = cannon.GetCost();
                        break;
                }
            }
            
            return cost;
        }
        
        // Select tower to build
        public void SelectTowerToBuild(TowerType towerType)
        {
            selectedTowerType = towerType;
            isBuildingTower = true;
            
            // Notify listeners
            if (OnTowerSelected != null)
            {
                OnTowerSelected(towerType);
            }
        }
        
        // Cancel tower building
        public void CancelTowerBuilding()
        {
            isBuildingTower = false;
        }
        
        // Place tower at position
        public bool PlaceTower(Vector3 position)
        {
            if (!isBuildingTower) return false;
            
            // Get tower data
            TowerData towerData = towerModel.GetTowerData(selectedTowerType);
            
            if (towerData == null)
            {
                Debug.LogError("Tower data not found for type: " + selectedTowerType);
                return false;
            }
            
            // Check if player has enough gold
            if (GameController.Instance != null && !GameController.Instance.HasEnoughGold(towerData.Cost))
            {
                Debug.Log("Not enough gold to build this tower!");
                return false;
            }
            
            // Spend gold
            if (GameController.Instance != null)
            {
                GameController.Instance.SpendGold(towerData.Cost);
            }
            
            // Create tower prefab
            GameObject tower = Instantiate(towerData.Prefab, position, Quaternion.identity);
            
            // Reset building state
            isBuildingTower = false;
            
            return true;
        }
        
        // Check if currently building a tower
        public bool IsBuildingTower()
        {
            return isBuildingTower;
        }
        
        // Get currently selected tower type
        public TowerType GetSelectedTowerType()
        {
            return selectedTowerType;
        }
        
        // Get tower data
        public TowerData GetTowerData(TowerType towerType)
        {
            return towerModel.GetTowerData(towerType);
        }
        
        // Get all tower data
        public TowerData[] GetAllTowerData()
        {
            return towerModel.GetAllTowerData();
        }
    }
} 