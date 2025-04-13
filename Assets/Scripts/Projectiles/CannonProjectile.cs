using UnityEngine;

public class CannonProjectile : Projectile
{
    public float explosionRadius = 2f;
    public float knockbackForce = 5f;
    
    protected override void HitTarget()
    {
        // Create explosion effect
        if (impactEffect != null)
        {
            GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(effectIns, 2f);
        }
        
        // Apply area damage and knockback
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, LayerMask.GetMask("Enemy"));
        
        foreach (Collider2D enemy in hitEnemies)
        {
            // Apply damage
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            
            // Apply knockback
            Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (enemy.transform.position - transform.position).normalized;
                rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
            }
        }
        
        Destroy(gameObject);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
} 