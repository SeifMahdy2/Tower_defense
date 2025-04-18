using UnityEngine;

public class ArcherTower : Tower
{
    [Header("Archer Tower Specific")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float arrowSpeed = 15f;
    
    protected override void Start()
    {
        // Set unique tower properties before calling base.Start()
        towerName = "Archer Tower";
        cost = 100;
        range = 6f; // Archers have slightly longer range than default
        
        // Make sure animation names are properly set
        idleAnimationName = "Idle"; // Make sure this matches your animation controller
        shootAnimationName = "Shoot"; // Make sure this matches your animation controller
        
        // Call base to set up components and start target finding
        base.Start();
        
        // Check for arrowPrefab
        if (arrowPrefab == null)
        {
            Debug.LogError(gameObject.name + ": Arrow prefab not assigned! Archer tower won't be able to attack.");
        }
    }
    
    protected override void Shoot()
    {
        if (target == null)
        {
            Debug.LogWarning(gameObject.name + ": Cannot shoot, no target");
            return;
        }
        
        if (arrowPrefab == null)
        {
            Debug.LogError(gameObject.name + ": Arrow prefab is not assigned!");
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
            
        // Create arrow at firePoint
        GameObject arrowObj = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
        
        // Set target and damage
        Projectile arrow = arrowObj.GetComponent<Projectile>();
        if (arrow != null)
        {
            arrow.speed = arrowSpeed;
            arrow.Seek(target, damage);
            Debug.Log(gameObject.name + ": Fired arrow at " + target.name);
        }
        else
        {
            Debug.LogError(gameObject.name + ": Arrow prefab is missing Projectile component!");
        }
    }
    
    // Optional override for range visualization with a different color
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.8f, 0.2f, 0.2f); // Slightly different red for archer
        Gizmos.DrawWireSphere(transform.position, range);
    }
}