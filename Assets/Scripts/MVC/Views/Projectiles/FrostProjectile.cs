using UnityEngine;

public class FrostProjectile : Projectile
{
    public float slowAmount = 0.5f;
    public float slowDuration = 2f;
    
    // Fix warning by adding override keyword
    protected override void Update()
    {
        base.Update();
    }
    
    protected override void HitTarget()
    {
        // Create frost impact effect
        if (impactEffect != null)
        {
            GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(effectIns, 2f);
        }
        
        // Only try to access target if it still exists
        if (target != null)
        {
            // Apply damage
            EnemyHealth enemy = target.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            
            // Apply slow effect
            EnemyMovement movement = target.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.ApplySlow(slowAmount, slowDuration);
            }
        }
        else
        {
            Debug.Log("Frost projectile hit position but target was already destroyed");
        }
        
        Destroy(gameObject);
    }
} 