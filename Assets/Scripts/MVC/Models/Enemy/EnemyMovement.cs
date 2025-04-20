using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 2f;
    private float originalSpeed;
    
    private Transform target;
    private int waypointIndex = 0;
    
    private WaypointPath path;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private EnemyHealth healthComponent;
    
    void Start()
    {
        // Find the path
        path = FindObjectOfType<WaypointPath>();
        
        if (path == null)
        {
            Debug.LogError("No WaypointPath found in the scene!");
            return;
        }
        
        // Set initial target
        target = path.GetWaypoint(waypointIndex);
        
        // Store original speed
        originalSpeed = speed;
        
        // Get components
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        healthComponent = GetComponent<EnemyHealth>();
        
        // Play walk animation if available
        if (animator != null)
        {
            // Check if the parameter exists first to avoid error
            try {
                animator.SetBool("IsWalking", true);
            }
            catch (System.Exception e) {
                Debug.LogWarning("Failed to set IsWalking parameter: " + e.Message);
            }
        }
    }
    
    void Update()
    {
        if (target == null || path == null)
            return;
            
        // Move towards target
        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
        
        // Flip sprite based on direction
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = dir.x < 0;
        }
        
        // Check if reached waypoint
        if (Vector3.Distance(transform.position, target.position) <= 0.2f)
        {
            // Move to next waypoint
            waypointIndex++;
            
            // Check if reached end
            if (waypointIndex >= path.GetWaypointsCount())
            {
                ReachedEnd();
                return;
            }
            
            target = path.GetWaypoint(waypointIndex);
        }
    }
    
    void ReachedEnd()
    {
        // Get damage amount from health component (based on difficulty)
        int damageAmount = 1;
        if (healthComponent != null)
        {
            damageAmount = healthComponent.GetDamageToBase();
            Debug.Log(gameObject.name + " reached end and will deal " + damageAmount + " damage to base");
        }
        
        // Damage the player castle based on enemy difficulty
        TD.GameManager gameManager = FindObjectOfType<TD.GameManager>();
        if (gameManager != null)
        {
            try
            {
                // Apply damage to the base
                gameManager.EnemyReachedEnd(damageAmount);
                Debug.Log("Applied " + damageAmount + " damage to base. Base health now: " + gameManager.GetHealth());
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to apply damage to base: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("GameManager not found! Cannot apply damage to base.");
        }
        
        // Tell the EnemySpawner this enemy is destroyed
        EnemySpawner.onEnemyDestroyed.Invoke();
        
        // Destroy the enemy
        Destroy(gameObject);
    }
    
    // Called by FrostProjectile to slow down the enemy
    public void ApplySlow(float slowAmount, float duration)
    {
        // Stop any previous slow coroutines
        StopAllCoroutines();
        
        // Apply the slow effect
        StartCoroutine(SlowEffect(slowAmount, duration));
    }
    
    IEnumerator SlowEffect(float slowAmount, float duration)
    {
        // Apply slow
        speed = originalSpeed * (1 - slowAmount);
        
        // Wait for duration
        yield return new WaitForSeconds(duration);
        
        // Return to normal speed
        speed = originalSpeed;
    }
} 