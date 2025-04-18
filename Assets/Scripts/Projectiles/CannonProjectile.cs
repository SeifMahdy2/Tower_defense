using UnityEngine;

public class CannonProjectile : Projectile
{
    public float explosionRadius = 2f;
    public float knockbackForce = 5f;
    
    // Fix the update method to properly override the base class method
    protected override void Update()
    {
        // Add debug message
        if (target != null)
        {
            Debug.Log(gameObject.name + ": Moving toward " + target.name + 
                     " - Distance: " + Vector2.Distance(transform.position, target.position) +
                     " - Speed: " + speed);
        }
        
        base.Update();
    }
    
    protected override void HitTarget()
    {
        Debug.Log(gameObject.name + ": Cannonball hitting target at position " + transform.position);
        
        // Create explosion effect
        if (impactEffect != null)
        {
            GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(effectIns, 2f);
        }
        
        // Apply area damage and knockback
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, LayerMask.GetMask("Enemy"));
        Debug.Log(gameObject.name + ": Found " + hitEnemies.Length + " enemies in explosion radius");
        
        foreach (Collider2D enemy in hitEnemies)
        {
            // Apply damage
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log(gameObject.name + ": Applied " + damage + " damage to " + enemy.name);
            }
            
            // Apply knockback
            Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (enemy.transform.position - transform.position).normalized;
                rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
                Debug.Log(gameObject.name + ": Applied knockback to " + enemy.name);
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