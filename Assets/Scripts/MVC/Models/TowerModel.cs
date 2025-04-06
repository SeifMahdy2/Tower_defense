using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TowerModel
{
    // Basic tower properties
    public string towerName;
    public int level = 1;
    public int buildCost;
    public int upgradeCost;
    public float attackRange;
    public float attackSpeed;  // Attacks per second
    public float damage;
    public DamageType damageType;
    
    // Upgrade-related properties
    public float rangeUpgradeMultiplier = 1.2f;
    public float speedUpgradeMultiplier = 1.15f;
    public float damageUpgradeMultiplier = 1.5f;
    public float upgradeCostMultiplier = 1.8f;
    
    // Special effects
    public float slowPercentage = 0f;      // For Frost Tower
    public float slowDuration = 0f;        // For Frost Tower
    public float splashRadius = 0f;        // For Mage Tower
    
    // Constructor with default values
    public TowerModel()
    {
        towerName = "Basic Tower";
        buildCost = 50;
        upgradeCost = 75;
        attackRange = 3f;
        attackSpeed = 1f;
        damage = 10f;
        damageType = DamageType.Physical;
    }
    
    // Constructor with specific values
    public TowerModel(string name, int cost, float range, float speed, float dmg, DamageType type)
    {
        towerName = name;
        buildCost = cost;
        upgradeCost = Mathf.RoundToInt(cost * upgradeCostMultiplier);
        attackRange = range;
        attackSpeed = speed;
        damage = dmg;
        damageType = type;
    }
    
    // Method to upgrade the tower
    public virtual void Upgrade()
    {
        level++;
        attackRange *= rangeUpgradeMultiplier;
        attackSpeed *= speedUpgradeMultiplier;
        damage *= damageUpgradeMultiplier;
        
        // Calculate new upgrade cost
        upgradeCost = Mathf.RoundToInt(upgradeCost * upgradeCostMultiplier);
    }
    
    // Create a tower model for Archer Tower
    public static TowerModel CreateArcherTower()
    {
        return new TowerModel(
            "Archer Tower",
            50,   // Cost
            3.5f, // Range
            1.5f, // Attack speed
            12f,  // Damage
            DamageType.Physical
        );
    }
    
    // Create a tower model for Mage Tower
    public static TowerModel CreateMageTower()
    {
        TowerModel model = new TowerModel(
            "Mage Tower",
            75,   // Cost
            3.0f, // Range
            0.8f, // Attack speed
            20f,  // Damage
            DamageType.Magic
        );
        model.splashRadius = 1.5f;
        return model;
    }
    
    // Create a tower model for Frost Tower
    public static TowerModel CreateFrostTower()
    {
        TowerModel model = new TowerModel(
            "Frost Tower",
            60,   // Cost
            3.0f, // Range
            1.0f, // Attack speed
            8f,   // Damage
            DamageType.Frost
        );
        model.slowPercentage = 0.3f;  // 30% slow
        model.slowDuration = 2.0f;    // 2 seconds slow
        return model;
    }
    
    // Create a tower model for Cannon Tower
    public static TowerModel CreateCannonTower()
    {
        TowerModel model = new TowerModel(
            "Cannon Tower",
            80,   // Cost
            2.5f, // Range
            0.5f, // Attack speed
            35f,  // Damage
            DamageType.Physical
        );
        model.splashRadius = 1.0f;
        return model;
    }
} 