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
            return;
        
        // Initialize
        target = path.GetWaypoint(waypointIndex);
        originalSpeed = speed;
        
        // Get components
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        healthComponent = GetComponent<EnemyHealth>();
        
        // Start walking animation
        if (animator != null)
        {
            try {
                animator.SetBool("IsWalking", true);
            }
            catch {
                // Animation parameter might not exist
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
            spriteRenderer.flipX = dir.x < 0;
        
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
        // Get damage amount from health component
        int damageAmount = healthComponent != null ? healthComponent.GetDamageToBase() : 1;
        
        // Damage the player castle
        TD.GameManager gameManager = FindObjectOfType<TD.GameManager>();
        if (gameManager != null)
            gameManager.EnemyReachedEnd(damageAmount);
        
        // Tell the spawner this enemy is destroyed
        EnemySpawner.onEnemyDestroyed.Invoke();
        
        // Destroy the enemy
        Destroy(gameObject);
    }
    
    // Called by frost towers to slow down the enemy
    public void ApplySlow(float slowAmount, float duration)
    {
        StopAllCoroutines();
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