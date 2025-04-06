using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TowerController : MonoBehaviour
{
    [Header("Tower Configuration")]
    [SerializeField] private TowerModel towerData;
    
    [Header("Visuals")]
    [SerializeField] private SpriteRenderer towerSpriteRenderer;
    [SerializeField] private GameObject rangeIndicator;
    [SerializeField] private Color rangeColor = new Color(0.2f, 0.8f, 0.2f, 0.2f);
    
    [Header("Attack")]
    [SerializeField] private Transform attackOrigin;
    [SerializeField] private GameObject projectilePrefab;
    
    // Target tracking
    private EnemyController currentTarget;
    private float attackCooldown = 0f;
    private List<EnemyController> enemiesInRange = new List<EnemyController>();
    
    // Events
    public static UnityEvent<TowerController> OnTowerUpgraded = new UnityEvent<TowerController>();
    
    // Getters
    public TowerModel TowerData => towerData;
    public bool IsSelected { get; private set; }
    
    private void Awake()
    {
        if (towerSpriteRenderer == null)
            towerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
        if (attackOrigin == null)
            attackOrigin = transform;
            
        // Create range indicator if not assigned
        if (rangeIndicator == null)
        {
            GameObject indicator = new GameObject("RangeIndicator");
            indicator.transform.SetParent(transform);
            indicator.transform.localPosition = Vector3.zero;
            
            SpriteRenderer renderer = indicator.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateCircleSprite();
            renderer.color = rangeColor;
            renderer.sortingOrder = -1; // Below the tower
            
            rangeIndicator = indicator;
        }
        
        // Hide range indicator by default
        if (rangeIndicator != null)
            rangeIndicator.SetActive(false);
            
        // Initialize tower data if not set
        if (towerData == null)
            towerData = new TowerModel();
    }
    
    private void Start()
    {
        UpdateRangeIndicator();
    }
    
    private void Update()
    {
        // Update attack cooldown
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
        
        // Find and attack target
        if (attackCooldown <= 0 && FindTarget())
        {
            Attack();
            // Reset cooldown based on attack speed (attacks per second)
            attackCooldown = 1f / towerData.attackSpeed;
        }
    }
    
    // Initialize tower with specific model
    public void Initialize(TowerModel model)
    {
        towerData = model;
        UpdateRangeIndicator();
    }
    
    // Selection state
    public void Select()
    {
        IsSelected = true;
        if (rangeIndicator != null)
            rangeIndicator.SetActive(true);
    }
    
    public void Deselect()
    {
        IsSelected = false;
        if (rangeIndicator != null)
            rangeIndicator.SetActive(false);
    }
    
    // Try to upgrade the tower
    public bool TryUpgrade(int playerGold)
    {
        if (playerGold >= towerData.upgradeCost)
        {
            towerData.Upgrade();
            UpdateRangeIndicator();
            
            // Notify listeners that tower was upgraded
            OnTowerUpgraded.Invoke(this);
            
            return true;
        }
        
        return false;
    }
    
    // Find the closest valid target within range
    private bool FindTarget()
    {
        // First check if current target is still valid
        if (IsValidTarget(currentTarget))
            return true;
        
        // Target is no longer valid, find a new one
        currentTarget = null;
        
        // Find all enemies in range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, towerData.attackRange);
        float closestDistance = float.MaxValue;
        
        foreach (Collider2D collider in colliders)
        {
            EnemyController enemy = collider.GetComponent<EnemyController>();
            if (IsValidTarget(enemy))
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    currentTarget = enemy;
                }
            }
        }
        
        return currentTarget != null;
    }
    
    // Check if an enemy is a valid target
    private bool IsValidTarget(EnemyController enemy)
    {
        return enemy != null && enemy.gameObject.activeSelf && enemy.IsActive &&
               Vector2.Distance(transform.position, enemy.transform.position) <= towerData.attackRange;
    }
    
    // Attack the current target
    protected virtual void Attack()
    {
        if (currentTarget == null) return;
        
        // For tower types that use projectiles
        if (projectilePrefab != null)
        {
            GameObject projectileObj = Instantiate(
                projectilePrefab, 
                attackOrigin.position, 
                Quaternion.identity
            );
            
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Initialize(
                    currentTarget, 
                    towerData.damage, 
                    towerData.damageType,
                    towerData.splashRadius
                );
                
                // Set slow effect for frost towers
                if (towerData.damageType == DamageType.Frost && towerData.slowPercentage > 0)
                {
                    projectile.SetSlowEffect(towerData.slowPercentage, towerData.slowDuration);
                }
            }
        }
        else
        {
            // For instant-hit towers (no projectile)
            if (towerData.splashRadius > 0)
            {
                // Area damage
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
                    currentTarget.transform.position, 
                    towerData.splashRadius
                );
                
                foreach (Collider2D hitCollider in hitColliders)
                {
                    EnemyController enemy = hitCollider.GetComponent<EnemyController>();
                    if (enemy != null && enemy.gameObject.activeSelf && enemy.IsActive)
                    {
                        enemy.TakeDamage(towerData.damage, towerData.damageType);
                        
                        // Apply slow effect for frost towers
                        if (towerData.damageType == DamageType.Frost && towerData.slowPercentage > 0)
                        {
                            enemy.ApplySlow(towerData.slowPercentage, towerData.slowDuration);
                        }
                    }
                }
            }
            else
            {
                // Single target damage
                currentTarget.TakeDamage(towerData.damage, towerData.damageType);
                
                // Apply slow effect for frost towers
                if (towerData.damageType == DamageType.Frost && towerData.slowPercentage > 0)
                {
                    currentTarget.ApplySlow(towerData.slowPercentage, towerData.slowDuration);
                }
            }
        }
    }
    
    // Update the range indicator's size based on tower range
    private void UpdateRangeIndicator()
    {
        if (rangeIndicator != null)
        {
            // Set the scale to match the attack range (diameter = 2 * range)
            float diameter = towerData.attackRange * 2f;
            rangeIndicator.transform.localScale = new Vector3(diameter, diameter, 1f);
        }
    }
    
    // Create a simple circle sprite for the range indicator
    private Sprite CreateCircleSprite()
    {
        int resolution = 100;
        Texture2D texture = new Texture2D(resolution, resolution);
        Color[] colors = new Color[resolution * resolution];
        
        float center = resolution / 2f;
        float radius = resolution / 2f;
        
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                int index = y * resolution + x;
                
                if (distance <= radius)
                {
                    colors[index] = Color.white;
                }
                else
                {
                    colors[index] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(
            texture, 
            new Rect(0, 0, resolution, resolution), 
            new Vector2(0.5f, 0.5f), 
            resolution
        );
    }
    
    // Draw the range in the editor for easy visualization
    private void OnDrawGizmosSelected()
    {
        if (towerData != null)
        {
            Gizmos.color = new Color(0.2f, 0.8f, 0.2f, 0.2f);
            Gizmos.DrawSphere(transform.position, towerData.attackRange);
        }
    }
} 