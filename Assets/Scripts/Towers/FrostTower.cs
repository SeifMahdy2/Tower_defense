using UnityEngine;

public class FrostTower : Tower
{
    [Header("Frost Tower Specific")]
    [SerializeField] private GameObject frostPrefab;
    [SerializeField] private float slowAmount = 0.5f; // 50% slow
    [SerializeField] private float slowDuration = 3f;
    [SerializeField] private float frostSpeed = 10f;
    
    protected override void Start()
    {
        // Set unique tower properties before calling base.Start()
        towerName = "Frost Tower";
        cost = 125;
        fireRate = 1.2f; // Frost towers fire at medium rate
        damage = 8; // Lower damage but with slowing effect
        
        // Make sure animation names are properly set
        idleAnimationName = "Idle";
        shootAnimationName = "Shooting";
        
        // Call base to set up components and start target finding
        base.Start();
        
        // Check for projectile prefab
        if (frostPrefab == null)
        {
            Debug.LogError(gameObject.name + ": Frost projectile prefab not assigned! Frost tower won't be able to attack.");
        }
    }

    protected override void Shoot()
    {
        if (target == null)
        {
            Debug.LogWarning(gameObject.name + ": Cannot shoot, no target");
            return;
        }
        
        if (frostPrefab == null)
        {
            Debug.LogError(gameObject.name + ": Frost projectile prefab is not assigned!");
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
        
        // Create frost projectile at firePoint
        GameObject frostObj = Instantiate(frostPrefab, firePoint.position, firePoint.rotation);
        
        // Set target and damage
        FrostProjectile frostProjectile = frostObj.GetComponent<FrostProjectile>();
        if (frostProjectile != null)
        {
            frostProjectile.speed = frostSpeed;
            frostProjectile.slowAmount = slowAmount;
            frostProjectile.slowDuration = slowDuration;
            frostProjectile.Seek(target, damage);
            Debug.Log(gameObject.name + ": Fired frost projectile at " + target.name);
        }
        else
        {
            Debug.LogError(gameObject.name + ": Frost prefab is missing FrostProjectile component!");
        }
    }
    
    protected override void OnDrawGizmosSelected()
    {
        // Call base to draw range circle
        base.OnDrawGizmosSelected();
        
        // Draw frost effect radius with different color
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
} 