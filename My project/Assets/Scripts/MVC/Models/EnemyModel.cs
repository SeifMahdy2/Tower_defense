using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyModel
{
    public string enemyName;
    public float maxHealth;
    public float currentHealth;
    public float moveSpeed;
    public int goldReward;
    public int damageToBase;
    
    // Resistances (can be used for different tower types)
    public float magicResistance;
    public float armorResistance;
    public float frostResistance;
    
    // Initialize with default values
    public EnemyModel()
    {
        enemyName = "Enemy";
        maxHealth = 100f;
        currentHealth = maxHealth;
        moveSpeed = 2f;
        goldReward = 10;
        damageToBase = 1;
        magicResistance = 0f;
        armorResistance = 0f;
        frostResistance = 0f;
    }
    
    // Constructor with custom parameters
    public EnemyModel(string name, float health, float speed, int reward, int damage)
    {
        enemyName = name;
        maxHealth = health;
        currentHealth = maxHealth;
        moveSpeed = speed;
        goldReward = reward;
        damageToBase = damage;
        magicResistance = 0f;
        armorResistance = 0f;
        frostResistance = 0f;
    }
    
    // Take damage
    public float TakeDamage(float amount, DamageType damageType)
    {
        float finalDamage = amount;
        
        // Apply resistances based on damage type
        switch (damageType)
        {
            case DamageType.Physical:
                finalDamage *= (1f - armorResistance);
                break;
            case DamageType.Magic:
                finalDamage *= (1f - magicResistance);
                break;
            case DamageType.Frost:
                finalDamage *= (1f - frostResistance);
                break;
        }
        
        currentHealth -= finalDamage;
        currentHealth = Mathf.Max(0f, currentHealth); // Prevent negative health
        
        return finalDamage;
    }
    
    // Check if the enemy is dead
    public bool IsDead()
    {
        return currentHealth <= 0;
    }
    
    // Reset health to max (for pooling)
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}

// Enum for different damage types
public enum DamageType
{
    Physical,
    Magic,
    Frost,
    Pure  // Ignores all resistances
} 