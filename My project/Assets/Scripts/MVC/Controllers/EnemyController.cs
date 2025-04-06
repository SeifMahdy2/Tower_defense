using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [Header("Visual Feedback")]
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float hitColorDuration = 0.15f;
    
    // Events
    public static UnityEvent<EnemyController> OnEnemyReachedEnd = new UnityEvent<EnemyController>();
    public static UnityEvent<EnemyController, int> OnEnemyDeath = new UnityEvent<EnemyController, int>();
    
    // Path movement
    private WaypointPath path;
    private int currentWaypointIndex = 0;
    
    // Enemy properties
    [SerializeField] private EnemyModel enemyData;
    
    // Getters
    public EnemyModel EnemyData => enemyData;
    public bool IsActive { get; private set; }
    
    // Effects
    private float slowEffectMultiplier = 1f;
    private float slowEffectDuration = 0f;
    private Color originalColor;
    
    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
        originalColor = spriteRenderer.color;
    }
    
    private void Update()
    {
        if (!IsActive) return;
        
        // Update slow effect
        if (slowEffectDuration > 0)
        {
            slowEffectDuration -= Time.deltaTime;
            if (slowEffectDuration <= 0)
            {
                slowEffectMultiplier = 1f;
                // Reset visual indication of slowing
                spriteRenderer.color = originalColor;
            }
        }
        
        // Move towards the next waypoint
        MoveAlongPath();
    }
    
    // Initialize the enemy with a path and data
    public void Initialize(WaypointPath pathToFollow, EnemyModel data = null)
    {
        // Assign the path
        path = pathToFollow;
        
        // Reset position to the start of the path
        currentWaypointIndex = 0;
        if (path != null && path.GetWaypointCount() > 0)
        {
            transform.position = path.GetWaypointPosition(0);
        }
        
        // Set the enemy data
        if (data != null)
        {
            enemyData = data;
        }
        else if (enemyData == null)
        {
            enemyData = new EnemyModel();
        }
        else
        {
            enemyData.ResetHealth();
        }
        
        // Reset effects
        slowEffectMultiplier = 1f;
        slowEffectDuration = 0f;
        spriteRenderer.color = originalColor;
        
        // Activate the enemy
        IsActive = true;
    }
    
    // Move along the waypoint path
    private void MoveAlongPath()
    {
        if (path == null || currentWaypointIndex >= path.GetWaypointCount())
            return;
            
        // Get current target waypoint
        Vector3 targetPosition = path.GetWaypointPosition(currentWaypointIndex);
        
        // Calculate distance to move this frame with slowdown effect
        float adjustedSpeed = enemyData.moveSpeed * slowEffectMultiplier;
        float distanceToMove = adjustedSpeed * Time.deltaTime;
        
        // Move towards the waypoint
        transform.position = Vector3.MoveTowards(
            transform.position, 
            targetPosition, 
            distanceToMove
        );
        
        // Check if we reached the waypoint
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentWaypointIndex++;
            
            // If this was the last waypoint, enemy reached the end
            if (currentWaypointIndex >= path.GetWaypointCount())
            {
                ReachedEnd();
            }
        }
    }
    
    // Called when the enemy takes damage
    public void TakeDamage(float amount, DamageType damageType)
    {
        if (!IsActive) return;
        
        float damageTaken = enemyData.TakeDamage(amount, damageType);
        
        // Visual feedback for taking damage
        StartCoroutine(FlashColor());
        
        // Check if the enemy died
        if (enemyData.IsDead())
        {
            Die();
        }
    }
    
    // Apply slow effect
    public void ApplySlow(float slowPercentage, float duration)
    {
        // Calculate the slow multiplier (e.g., 30% slow = 0.7 movement speed)
        float newSlowMultiplier = 1f - Mathf.Clamp01(slowPercentage);
        
        // Only apply if this slow is stronger than current
        if (newSlowMultiplier < slowEffectMultiplier)
        {
            slowEffectMultiplier = newSlowMultiplier;
            slowEffectDuration = duration;
            
            // Visual indication of being slowed (blue tint)
            spriteRenderer.color = Color.Lerp(originalColor, Color.blue, slowPercentage);
        }
    }
    
    // When the enemy reaches the end of the path
    private void ReachedEnd()
    {
        // Trigger event that enemy reached the base
        OnEnemyReachedEnd.Invoke(this);
        
        // Deactivate the enemy
        Deactivate();
    }
    
    // When the enemy dies
    private void Die()
    {
        // Trigger death event passing the gold reward
        OnEnemyDeath.Invoke(this, enemyData.goldReward);
        
        // Deactivate the enemy
        Deactivate();
    }
    
    // Deactivate the enemy (for pooling)
    private void Deactivate()
    {
        IsActive = false;
        gameObject.SetActive(false);
    }
    
    // Visual feedback for taking damage
    private IEnumerator FlashColor()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(hitColorDuration);
        
        // If under slow effect, return to the blue tint, otherwise return to original
        if (slowEffectDuration > 0)
            spriteRenderer.color = Color.Lerp(originalColor, Color.blue, 1f - slowEffectMultiplier);
        else
            spriteRenderer.color = originalColor;
    }
} 