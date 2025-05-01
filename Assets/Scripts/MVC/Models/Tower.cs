using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour
{
    [Header("Base Tower Properties")]
    public string towerName;
    public int cost = 100;
    public float range = 5f;
    public float fireRate = 1f;
    public int damage = 10;
    
    [Header("References")]
    public Transform rotationPoint;
    public Transform firePoint;
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected LayerMask enemyLayer;
    [SerializeField] protected Animator animator;

    [Header("Upgrades")]
    [SerializeField] private int _upgradeLevel = 1; // Start at level 1
    public int upgradeLevel 
    { 
        get { return _upgradeLevel; } 
        protected set { _upgradeLevel = Mathf.Min(value, maxUpgradeLevel); } // Never allow higher than maxUpgradeLevel
    }
    public int maxUpgradeLevel = 2; // Only one upgrade allowed (from level 1 to 2)
    public int upgradeCost = 50;
    [SerializeField] protected Color upgradeColor = new Color(1f, 0.8f, 0.2f, 1f);

    [Header("Animation")]
    [SerializeField] protected string idleAnimationName = "Idle";
    [SerializeField] protected string shootAnimationName = "Shoot";
    
    [Header("Audio")]
    [SerializeField] protected string towerType = "";

    // Common functionality
    protected Transform target;
    protected bool hasTarget = false;
    protected float fireCountdown = 0f;
    private SpriteRenderer spriteRenderer;
    
    protected virtual void Start()
    {
        // Ensure towers always start at level 1
        upgradeLevel = 1;
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Set tower type if not defined
        if (string.IsNullOrEmpty(towerType))
        {
            string typeName = this.GetType().Name.ToLower();
            if (typeName.Contains("archer")) towerType = "archer";
            else if (typeName.Contains("mage")) towerType = "mage";
            else if (typeName.Contains("cannon")) towerType = "cannon";
            else if (typeName.Contains("frost")) towerType = "frost";
            else towerType = "default";
        }
        
        // Find required components
        if (rotationPoint == null)
            rotationPoint = transform.Find("RotationPoint");
            
        if (firePoint == null && rotationPoint != null)
            firePoint = rotationPoint.Find("FirePoint");
            
        if (animator == null)
            animator = GetComponent<Animator>();
        
        // Fix layer targeting
        if (enemyLayer.value == 0)
        {
            int enemyLayerIndex = LayerMask.NameToLayer("Enemy");
            if (enemyLayerIndex != -1)
                enemyLayer = 1 << enemyLayerIndex;
        }
        
        // Start with idle animation
        PlayIdleAnimation();
        
        // Start looking for targets
        InvokeRepeating("FindTarget", 0f, 0.5f);
    }
    
    protected virtual void Update()
    {
        if (!hasTarget || target == null)
        {
            // Reset targeting if needed
            if (hasTarget && target == null)
            {
                hasTarget = false;
                target = null;
                PlayIdleAnimation();
            }
            return;
        }
            
        // Check if target is still in range
        if (Vector2.Distance(transform.position, target.position) > range)
        {
            hasTarget = false;
            target = null;
            PlayIdleAnimation();
            return;
        }
        
        // Rotate to face target
        RotateToTarget();
        
        // Handle shooting
        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }
        
        fireCountdown -= Time.deltaTime;
    }
    
    protected virtual void Shoot()
    {
        PlayShootAnimation();
        PlayShootSound();
        
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile projectile = projectileGO.GetComponent<Projectile>();
            
            if (projectile != null)
                projectile.Seek(target, damage);
        }
    }
    
    protected virtual void PlayShootSound()
    {
        if (AudioManager.Instance != null && !string.IsNullOrEmpty(towerType))
            AudioManager.Instance.PlayTowerShootSound(towerType, transform.position);
    }
    
    protected virtual void FindTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
        if (colliders.Length == 0)
        {
            if (hasTarget)
            {
                hasTarget = false;
                target = null;
                PlayIdleAnimation();
            }
            return;
        }
        
        // Get the closest enemy
        float shortestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider2D collider in colliders)
        {
            // Skip towers
            if (collider.GetComponent<Tower>() != null)
                continue;
            
            float distance = Vector2.Distance(transform.position, collider.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestEnemy = collider.transform;
            }
        }

        if (closestEnemy != null)
        {
            target = closestEnemy;
            hasTarget = true;
        }
    }
    
    protected virtual void RotateToTarget()
    {
        if (target == null || rotationPoint == null) return;
        
        Vector3 direction = target.position - rotationPoint.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        rotationPoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    
    protected virtual void PlayIdleAnimation()
    {
        if (animator != null)
        {
            try {
                animator.Play("Idle");
            }
            catch {
                // Fallback - try using trigger
                if (idleAnimationName != "Idle")
                    animator.SetTrigger(idleAnimationName);
            }
        }
    }
    
    protected virtual void PlayShootAnimation()
    {
        if (animator != null)
        {
            try {
                animator.Play("Shooting");
            }
            catch {
                // Fallback - try using trigger
                if (shootAnimationName != "Shooting")
                    animator.SetTrigger(shootAnimationName);
            }
        }
    }
    
    // Draw range gizmo
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
    
    public virtual float GetRange()
    {
        return range;
    }

    public virtual bool CanPlaceHere()
    {
        return true;
    }

    public virtual void Upgrade()
    {
        // Prevent upgrade if already at max level
        if (upgradeLevel >= maxUpgradeLevel)
        {
            Debug.Log($"Tower already at max level ({upgradeLevel}). Cannot upgrade further.");
            return;
        }
            
        // Apply upgrade (will be capped at maxUpgradeLevel by the property)
        upgradeLevel++;
        Debug.Log($"Tower upgraded to level: {upgradeLevel}");
        
        // Improve stats
        damage = Mathf.RoundToInt(damage * 1.2f);
        range *= 1.1f;
        
        // Apply special effects
        ApplySpecialUpgradeEffect();
        
        // Visual feedback
        if (spriteRenderer != null)
            spriteRenderer.color = upgradeColor;
            
        Debug.Log($"Tower upgrade complete. Current level: {upgradeLevel}, Max allowed: {maxUpgradeLevel}");
    }
    
    // Override in subclasses for special effects
    protected virtual void ApplySpecialUpgradeEffect()
    {
        // Default implementation - improve fire rate
        fireRate *= 1.15f;
    }
} 