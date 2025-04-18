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
    public int upgradeLevel = 1;
    public int maxUpgradeLevel = 3;
    public int upgradeCost = 50;

    [Header("Animation")]
    [SerializeField] protected string idleAnimationName = "Idle";
    [SerializeField] protected string shootAnimationName = "Shoot";

    // Common functionality for all towers
    protected Transform target;
    protected bool hasTarget = false;
    protected float fireCountdown = 0f;
    
    // Cache for performance
    private SpriteRenderer spriteRenderer;
    
    protected virtual void Start()
    {
        // Initialize component references
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Find rotation point if not assigned
        if (rotationPoint == null)
        {
            Transform foundRotationPoint = transform.Find("RotationPoint");
            if (foundRotationPoint != null)
            {
                rotationPoint = foundRotationPoint;
                Debug.Log(gameObject.name + ": Found RotationPoint automatically");
            }
            else
            {
                Debug.LogWarning(gameObject.name + ": No RotationPoint found. Tower may not rotate correctly.");
            }
        }
        
        // Find fire point if not assigned
        if (firePoint == null && rotationPoint != null)
        {
            Transform foundFirePoint = rotationPoint.Find("FirePoint");
            if (foundFirePoint != null)
            {
                firePoint = foundFirePoint;
                Debug.Log(gameObject.name + ": Found FirePoint automatically");
            }
            else
            {
                Debug.LogWarning(gameObject.name + ": No FirePoint found. Projectiles may spawn incorrectly.");
            }
        }
        
        // Find animator if not assigned
        if (animator == null)
        {
            Animator foundAnimator = GetComponent<Animator>();
            if (foundAnimator != null)
            {
                animator = foundAnimator;
                Debug.Log(gameObject.name + ": Found Animator component automatically");
            }
            else
            {
                Debug.LogWarning(gameObject.name + ": No Animator component found. Animations will not play.");
            }
        }
        
        // Check for animator controller
        if (animator != null && animator.runtimeAnimatorController == null)
        {
            Debug.LogError(gameObject.name + ": Animator has no controller assigned!");
        }
        
        // Verify animation parameter names exist in the controller
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            bool foundIdle = false;
            bool foundShoot = false;
            
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == idleAnimationName) foundIdle = true;
                if (param.name == shootAnimationName) foundShoot = true;
            }
            
            if (!foundIdle) Debug.LogWarning(gameObject.name + ": Idle animation parameter '" + idleAnimationName + "' not found in animator controller");
            if (!foundShoot) Debug.LogWarning(gameObject.name + ": Shoot animation parameter '" + shootAnimationName + "' not found in animator controller");
        }
        
        // Fix layer targeting issues
        if (enemyLayer.value == 0)
        {
            // Try to find the "Enemy" layer
            int enemyLayerIndex = LayerMask.NameToLayer("Enemy");
            if (enemyLayerIndex != -1)
            {
                enemyLayer = 1 << enemyLayerIndex;
                Debug.Log(gameObject.name + ": Set enemyLayer to layer 'Enemy'");
            }
            else
            {
                Debug.LogError(gameObject.name + ": Enemy layer not found! Create a layer named 'Enemy'");
            }
        }
        
        // Make sure tower layer is not included in enemy layer
        int towerLayer = gameObject.layer;
        if (((1 << towerLayer) & enemyLayer.value) != 0)
        {
            // Remove tower layer from enemy mask
            enemyLayer &= ~(1 << towerLayer);
            Debug.LogWarning(gameObject.name + ": Fixed enemyLayer to exclude tower layer");
        }
        
        // Log the actual enemy layer mask for debugging
        Debug.Log(gameObject.name + ": Enemy layer mask: " + enemyLayer.value);
            
        // Start with idle animation
        PlayIdleAnimation();
        
        // Start looking for targets
        InvokeRepeating("FindTarget", 0f, 0.5f);
    }
    
    protected virtual void Update()
    {
        if (hasTarget)
        {
            // Rotate tower to face target
            RotateToTarget();

            // Check if target is still in range
            if (Vector2.Distance(transform.position, target.position) > range)
            {
                hasTarget = false;
                target = null;
                PlayIdleAnimation();
            }
            else
            {
                // Fire at target
                if (fireCountdown <= 0f)
                {
                    Shoot();
                    fireCountdown = 1f / fireRate;
                }
                
                fireCountdown -= Time.deltaTime;
            }
        }
        else
        {
            // Find new target
            FindTarget();
        }
    }
    
    protected virtual void Shoot()
    {
        // Play shooting animation
        PlayShootAnimation();
        
        if (projectilePrefab != null && firePoint != null)
        {
            // Log detailed debug information
            Debug.Log(gameObject.name + ": Firing projectile at target " + (target != null ? target.name : "null") + 
                     " from position " + firePoint.position);
                     
            GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile projectile = projectileGO.GetComponent<Projectile>();
            
            if (projectile != null)
            {
                Debug.Log(gameObject.name + ": Projectile instantiated, type: " + projectile.GetType().Name);
                projectile.Seek(target, damage);
            }
            else
            {
                Debug.LogError(gameObject.name + ": Projectile prefab does not have a Projectile component!");
            }
        }
        else
        {
            if (projectilePrefab == null)
                Debug.LogError(gameObject.name + ": Missing projectile prefab reference!");
            if (firePoint == null)
                Debug.LogError(gameObject.name + ": Missing fire point reference!");
        }
    }
    
    protected virtual void FindTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
        if (colliders.Length > 0)
        {
            // Get the closest enemy
            float shortestDistance = Mathf.Infinity;
            Transform closestEnemy = null;

            foreach (Collider2D collider in colliders)
            {
                // Skip other towers to prevent targeting each other
                if (collider.GetComponent<Tower>() != null)
                {
                    Debug.Log("Skipping targeting another tower: " + collider.name);
                    continue;
                }
                
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
            else if (hasTarget)
            {
                // No valid target found but we had one before
                hasTarget = false;
                target = null;
                PlayIdleAnimation();
            }
        }
        else if (hasTarget)
        {
            // No enemies in range but we had a target before
            hasTarget = false;
            target = null;
            PlayIdleAnimation();
            Debug.Log(gameObject.name + ": No enemies in range, returning to idle animation");
        }
    }
    
    protected virtual void RotateToTarget()
    {
        if (target == null || rotationPoint == null) return;
        
        // Calculate direction to target
        Vector3 direction = target.position - rotationPoint.position;
        
        // Calculate rotation angle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        
        // Apply rotation
        rotationPoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    
    protected virtual void PlayIdleAnimation()
    {
        if (animator != null)
        {
            // Try direct state transition to match the animation controller
            try {
                animator.Play("Idle");
            }
            catch (System.Exception e) {
                Debug.LogWarning(gameObject.name + ": Failed to play Idle animation: " + e.Message);
            }
            
            // Also try trigger method as fallback
            if (idleAnimationName != "Idle")
            {
                try {
                    animator.SetTrigger(idleAnimationName);
                }
                catch (System.Exception e) {
                    Debug.LogWarning(gameObject.name + ": Failed to set trigger " + idleAnimationName + ": " + e.Message);
                }
            }
            
            Debug.Log(gameObject.name + ": Playing idle animation state 'Idle'");
        }
        else
        {
            Debug.LogWarning(gameObject.name + ": No animator found for idle animation");
        }
    }
    
    protected virtual void PlayShootAnimation()
    {
        if (animator != null)
        {
            // Try both methods - direct state name and trigger
            // First try direct animation state transition (what's shown in the screenshot)
            animator.Play("Shooting"); // Changed from "Shoot" to "Shooting" to match the state name
            
            // Also try trigger method as fallback with correct parameter name
            if (shootAnimationName != "Shooting")
            {
                try {
                    animator.SetTrigger(shootAnimationName);
                }
                catch (System.Exception e) {
                    Debug.LogWarning(gameObject.name + ": Failed to set trigger " + shootAnimationName + ": " + e.Message);
                }
            }
            
            Debug.Log(gameObject.name + ": Playing shoot animation state 'Shooting'");
        }
        else
        {
            Debug.LogWarning(gameObject.name + ": No animator found for shoot animation");
        }
    }
    
    // Draw range gizmo for visual debugging
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
    
    // Get range for placement preview
    public virtual float GetRange()
    {
        return range;
    }

    public virtual bool CanPlaceHere()
    {
        // Default implementation - override in child classes for custom placement rules
        return true;
    }

    public virtual void Upgrade()
    {
        if (upgradeLevel < maxUpgradeLevel)
        {
            upgradeLevel++;
            // Increase stats with upgrade
            damage = Mathf.RoundToInt(damage * 1.3f);
            range *= 1.15f;
            fireRate *= 1.2f;
        }
    }

    // Add this method to validate animation state names at runtime
    protected void ValidateAnimatorStates()
    {
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            // Try to get the animator controller
            UnityEditor.Animations.AnimatorController controller = 
                animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
                
            if (controller != null)
            {
                bool foundIdle = false;
                bool foundShooting = false;
                
                // Look through all layers and states to find our animation states
                foreach (var layer in controller.layers)
                {
                    foreach (var state in layer.stateMachine.states)
                    {
                        if (state.state.name == "Idle") foundIdle = true;
                        if (state.state.name == "Shooting") foundShooting = true;
                    }
                }
                
                if (!foundIdle) Debug.LogError(gameObject.name + ": Animation state 'Idle' not found in controller!");
                if (!foundShooting) Debug.LogError(gameObject.name + ": Animation state 'Shooting' not found in controller!");
                
                Debug.Log(gameObject.name + ": Animation validation complete. Idle: " + foundIdle + ", Shooting: " + foundShooting);
            }
        }
    }
} 