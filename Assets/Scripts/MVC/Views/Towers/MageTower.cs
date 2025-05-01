using UnityEngine;

public class MageTower : Tower
{
    [Header("Mage Tower Specific")]
    [SerializeField] private GameObject magicPrefab;
    [SerializeField] private float splashRadius = 2f;
    [SerializeField] private float magicSpeed = 12f;
    [SerializeField] private bool appliesDoT = false; // Damage over time flag
    [SerializeField] private int dotDamage = 5; // Damage per tick
    [SerializeField] private float dotDuration = 3f; // Duration of DoT effect
    
    protected override void Start()
    {
        // Set unique tower properties before calling base.Start()
        towerName = "Mage Tower";
        cost = 150; // Cost set to 150 to match table values
        fireRate = 0.8f; // Mages fire slower
        damage = 20; // High damage
        towerType = "mage"; // Set tower type for audio
        
        // Make sure animation names are properly set
        idleAnimationName = "idle";
        shootAnimationName = "Shooting";
        
        // Call base to set up components and start target finding
        base.Start();
        
        // Check for projectile prefab
        if (magicPrefab == null)
        {
            Debug.LogError(gameObject.name + ": Magic projectile prefab not assigned! Mage tower won't be able to attack.");
        }
    }

    protected override void FindTarget()
    {
        base.FindTarget();
        if (hasTarget)
        {
            // Mage-specific targeting logic can be added here
            // For example, prioritize groups of enemies for splash damage
        }
    }

    protected override void Shoot()
    {
        if (target == null)
        {
            Debug.LogWarning(gameObject.name + ": Cannot shoot, no target");
            return;
        }
        
        if (magicPrefab == null)
        {
            Debug.LogError(gameObject.name + ": Magic projectile prefab is not assigned!");
            return;
        }
        
        if (firePoint == null)
        {
            Debug.LogError(gameObject.name + ": No firePoint found!");
            return;
        }
        
        // Play shooting animation - but first ensure we have an animator
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError(gameObject.name + ": No animator found for tower!");
            }
        }
        
        // Attempt to play animation
        PlayShootAnimation();
        
        // Play shooting sound
        PlayShootSound();
        
        // Create magic projectile at firePoint
        GameObject magicObj = Instantiate(magicPrefab, firePoint.position, firePoint.rotation);
        
        // Set target and damage
        MageProjectile magicProjectile = magicObj.GetComponent<MageProjectile>();
        if (magicProjectile != null)
        {
            magicProjectile.speed = magicSpeed;
            magicProjectile.splashRadius = splashRadius;
            magicProjectile.Seek(target, damage);
            
            // Set DoT effect if upgraded
            if (appliesDoT)
            {
                magicProjectile.EnableDamageOverTime(dotDamage, dotDuration);
            }
            
            Debug.Log(gameObject.name + ": Fired magic projectile at " + target.name + 
                      (appliesDoT ? " with DoT effect!" : ""));
        }
        else
        {
            Debug.LogError(gameObject.name + ": Magic prefab is missing MageProjectile component!");
        }
    }

    // Override the special upgrade effect for Mage Tower
    protected override void ApplySpecialUpgradeEffect()
    {
        // Mage upgrade: Apply damage over time with magic attacks
        appliesDoT = true;
        
        // Slightly increase area damage too
        splashRadius *= 1.2f;
        
        Debug.Log($"{gameObject.name} upgraded with Damage Over Time! DoT: {dotDamage} damage over {dotDuration} seconds, Splash radius: {splashRadius}");
    }

    protected override void OnDrawGizmosSelected()
    {
        // Call base to draw range circle
        base.OnDrawGizmosSelected();
        
        // Draw splash radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
} 