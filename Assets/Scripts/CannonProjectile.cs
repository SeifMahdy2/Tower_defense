using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonProjectile : Projectile
{
    [SerializeField] private float splashRadius = 1.2f;
    [SerializeField] private GameObject explosionPrefab;
    
    private int damageAmount;
    
    public void Initialize(Transform target, int damage, float radius, GameObject explosion)
    {
        // Call base class Initialize method first
        base.Initialize(target, damage);
        
        // Set additional properties
        this.splashRadius = radius;
        this.damageAmount = damage;
        this.explosionPrefab = explosion;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if we hit anything
        if (collision.CompareTag("Enemy") || collision.CompareTag("Obstacle"))
        {
            // Create explosion effect if available
            Vector3 explosionPosition = transform.position;
            
            if (explosionPrefab != null)
            {
                GameObject explosion = Instantiate(explosionPrefab, explosionPosition, Quaternion.identity);
                explosion.transform.localScale = new Vector3(splashRadius, splashRadius, 1);
                Destroy(explosion, 1f);
            }
            
            // Find all enemies in splash radius
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(explosionPosition, splashRadius);
            
            foreach (Collider2D hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    Enemy enemy = hitCollider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        // Calculate damage based on distance (optional - for damage falloff)
                        float distance = Vector3.Distance(explosionPosition, hitCollider.transform.position);
                        float damagePercent = 1f - (distance / splashRadius);
                        damagePercent = Mathf.Clamp01(damagePercent);
                        
                        int finalDamage = Mathf.RoundToInt(damageAmount * damagePercent);
                        enemy.TakeDamage(finalDamage);
                    }
                }
            }
            
            // Destroy the projectile
            Destroy(gameObject);
        }
    }
    
    // Draw the splash radius in the editor for debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
} 