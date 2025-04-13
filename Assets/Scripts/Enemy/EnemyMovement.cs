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
        
        // Get sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        // Damage the player castle
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.EnemyReachedEnd();
        }
        
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
        
        // Change color to indicate slow
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0.5f, 0.5f, 1f);
        }
        
        // Wait for duration
        yield return new WaitForSeconds(duration);
        
        // Return to normal speed
        speed = originalSpeed;
        
        // Return to normal color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }
} 