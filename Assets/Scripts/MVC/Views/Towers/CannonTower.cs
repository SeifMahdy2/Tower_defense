using UnityEngine;

public class CannonTower : Tower
{
    [Header("Cannon Tower Specific")]
    [SerializeField] private GameObject cannonballPrefab;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float cannonballSpeed = 12f;
    
    protected override void Start()
    {
        // Set unique tower properties before calling base.Start()
        towerName = "Cannon Tower";
        cost = 175;
        fireRate = 0.5f; // Cannon fires slower
        damage = 25; // High damage
        towerType = "cannon"; // Set tower type for audio
        
        // Make sure animation names are properly set
        idleAnimationName = "Idle";
        shootAnimationName = "Shooting";
        
        // Call base to set up components and start target finding
        base.Start();
        
        // Check for projectile prefab
        if (cannonballPrefab == null)
        {
            Debug.LogError(gameObject.name + ": Cannonball prefab not assigned! Cannon tower won't be able to attack.");
        }
    }

    protected override void Shoot()
    {
        if (target == null)
        {
            Debug.LogWarning(gameObject.name + ": Cannot shoot, no target");
            return;
        }
        
        if (cannonballPrefab == null)
        {
            Debug.LogError(gameObject.name + ": Cannonball prefab is not assigned!");
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
        
        // Create cannonball at firePoint
        GameObject cannonballObj = Instantiate(cannonballPrefab, firePoint.position, firePoint.rotation);
        
        // Set target and damage
        CannonProjectile cannonProjectile = cannonballObj.GetComponent<CannonProjectile>();
        if (cannonProjectile != null)
        {
            cannonProjectile.speed = cannonballSpeed;
            cannonProjectile.explosionRadius = explosionRadius;
            cannonProjectile.knockbackForce = knockbackForce;
            cannonProjectile.Seek(target, damage);
            Debug.Log(gameObject.name + ": Fired cannonball at " + target.name + 
                     " - Target position: " + target.position + 
                     " - Projectile position: " + cannonballObj.transform.position +
                     " - Distance: " + Vector2.Distance(cannonballObj.transform.position, target.position));
        }
        else
        {
            Debug.LogError(gameObject.name + ": Cannonball prefab is missing CannonProjectile component!");
        }
    }
    
    // Override the special upgrade effect for Cannon Tower
    protected override void ApplySpecialUpgradeEffect()
    {
        // Cannon upgrade: Increased explosion radius and knockback force
        explosionRadius *= 1.5f; // 50% larger explosion radius
        knockbackForce *= 1.3f; // 30% stronger knockback
        
        Debug.Log($"{gameObject.name} upgraded with Enhanced Explosions! New explosion radius: {explosionRadius}, Knockback force: {knockbackForce}");
    }
    
    protected override void OnDrawGizmosSelected()
    {
        // Call base to draw range circle
        base.OnDrawGizmosSelected();
        
        // Draw explosion radius with different color
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
} 