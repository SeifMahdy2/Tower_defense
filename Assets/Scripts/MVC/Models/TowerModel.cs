using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Models
{
    public enum TowerType
    {
        Archer,
        Mage,
        Frost,
        Cannon
    }
    
    [System.Serializable]
    public class TowerData
    {
        public TowerType Type;
        public int Cost;
        public float Range;
        public float FireRate;
        public int Damage;
        public GameObject Prefab;
        public string Description;
        public Sprite Icon;
    }
    
    [System.Serializable]
    public class TowerModel
    {
        // Dictionary of tower data by tower type
        private Dictionary<TowerType, TowerData> towerDataDict;
        
        // Events
        public delegate void TowerDataChanged();
        public event TowerDataChanged OnTowerDataChanged;
        
        // Constructor
        public TowerModel(TowerData[] towerData)
        {
            // Initialize dictionary
            towerDataDict = new Dictionary<TowerType, TowerData>();
            
            // Add tower data to dictionary
            if (towerData != null)
            {
                foreach (TowerData data in towerData)
                {
                    towerDataDict[data.Type] = data;
                }
            }
        }
        
        // Get tower data by type
        public TowerData GetTowerData(TowerType type)
        {
            if (towerDataDict.ContainsKey(type))
            {
                return towerDataDict[type];
            }
            
            return null;
        }
        
        // Get all tower types
        public TowerType[] GetAllTowerTypes()
        {
            TowerType[] types = new TowerType[towerDataDict.Count];
            int index = 0;
            
            foreach (KeyValuePair<TowerType, TowerData> pair in towerDataDict)
            {
                types[index] = pair.Key;
                index++;
            }
            
            return types;
        }
        
        // Get all tower data
        public TowerData[] GetAllTowerData()
        {
            TowerData[] data = new TowerData[towerDataDict.Count];
            int index = 0;
            
            foreach (KeyValuePair<TowerType, TowerData> pair in towerDataDict)
            {
                data[index] = pair.Value;
                index++;
            }
            
            return data;
        }
        
        // Add or update tower data
        public void SetTowerData(TowerData data)
        {
            towerDataDict[data.Type] = data;
            
            // Notify subscribers
            if (OnTowerDataChanged != null)
            {
                OnTowerDataChanged();
            }
        }
    }
} 