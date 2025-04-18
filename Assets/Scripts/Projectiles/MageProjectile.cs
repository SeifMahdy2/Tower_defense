using UnityEngine;

public class MageProjectile : Projectile
{
    public float splashRadius = 1.5f;
    
    // Add Update method to ensure proper movement
    void Update()
    {
        base.Update();
    }
    
    protected override void HitTarget()
    {
        // Create impact effect
        if (impactEffect != null)
        {
            GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(effectIns, 2f);
        }
        
        // Splash damage
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, splashRadius, LayerMask.GetMask("Enemy"));
        
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
        
        Destroy(gameObject);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
} 