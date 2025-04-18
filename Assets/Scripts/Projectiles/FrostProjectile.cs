using UnityEngine;

public class FrostProjectile : Projectile
{
    public float slowAmount = 0.5f;
    public float slowDuration = 2f;
    
    // Add Update method to ensure proper movement
    void Update()
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
        
        Destroy(gameObject);
    }
} 