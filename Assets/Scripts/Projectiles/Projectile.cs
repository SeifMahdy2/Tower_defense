using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    public GameObject impactEffect;
    
    [HideInInspector]
    public Transform target;
    [HideInInspector]
    public int damage;
    
    private Vector3 lastTargetPosition;
    private bool targetWasDestroyed = false;
    
    // Use this method to set the target and damage
    public void Seek(Transform newTarget, int damageAmount)
    {
        target = newTarget;
        damage = damageAmount;
        
        if (newTarget == null)
        {
            Debug.LogError("Projectile was given a null target!");
        }
        else
        {
            // Store the target's position in case it gets destroyed
            lastTargetPosition = newTarget.position;
            Debug.Log(gameObject.name + " is seeking target " + newTarget.name + " at position " + lastTargetPosition);
        }
    }
    
    protected virtual void Update()
    {
        // Check if target exists using a safe approach
        bool targetExists = false;
        Vector3 currentTargetPosition = lastTargetPosition;
        
        try 
        {
            // This will throw an exception if target was destroyed
            if (target != null)
            {
                targetExists = true;
                currentTargetPosition = target.position;
                lastTargetPosition = currentTargetPosition;
            }
        }
        catch (System.Exception)
        {
            // Target was destroyed but reference isn't null yet
            Debug.Log(gameObject.name + ": Target was destroyed");
            targetWasDestroyed = true;
            target = null;
        }
        
        // If no target (destroyed or never set)
        if (!targetExists)
        {
            // If we have a last known position, continue moving toward it
            if (lastTargetPosition != Vector3.zero)
            {
                MoveToPosition(lastTargetPosition);
                return;
            }
            
            // No target and no last position, destroy projectile
            Debug.Log(gameObject.name + " has no target, destroying");
            Destroy(gameObject);
            return;
        }
        
        // Calculate direction to target
        Vector3 direction = currentTargetPosition - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;
        
        // Check if we've reached the target
        if (direction.magnitude <= distanceThisFrame)
        {
            // We've reached the target
            Debug.Log(gameObject.name + " hit target " + (target != null ? target.name : "unknown"));
            HitTarget();
            return;
        }
        
        // Move the projectile
        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        
        // Rotate to face the direction it's moving
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    
    // Helper method to move toward a position rather than a transform
    protected void MoveToPosition(Vector3 targetPosition)
    {
        // Calculate direction to position
        Vector3 direction = targetPosition - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;
        
        // Check if we've reached the position
        if (direction.magnitude <= distanceThisFrame)
        {
            // We've reached the position, trigger hit
            Debug.Log(gameObject.name + " reached last known target position");
            HitTarget();
            return;
        }
        
        // Move the projectile
        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        
        // Rotate to face the direction it's moving
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    
    protected virtual void HitTarget()
    {
        // Deal damage, but only if target still exists
        if (target != null && !targetWasDestroyed)
        {
            try
            {
                EnemyHealth health = target.GetComponent<EnemyHealth>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                }
            }
            catch (System.Exception)
            {
                Debug.LogWarning("Target was destroyed while trying to apply damage");
            }
        }
        
        // Create effect
        if (impactEffect != null)
        {
            GameObject effect = Instantiate(impactEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
        
        // Destroy projectile
        Destroy(gameObject);
    }
}