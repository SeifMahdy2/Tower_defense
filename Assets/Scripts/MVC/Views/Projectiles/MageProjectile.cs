using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageProjectile : Projectile
{
    public float splashRadius = 2f;
    [SerializeField] private LayerMask enemyLayer;
    
    // Damage over time properties
    private bool hasDotEffect = false;
    private int dotDamage = 0;
    private float dotDuration = 0f;
    
    // Enable DoT effect on this projectile
    public void EnableDamageOverTime(int dotAmount, float duration)
    {
        hasDotEffect = true;
        dotDamage = dotAmount;
        dotDuration = duration;
    }
    
    private void Start()
    {
        // Set enemy layer if not set
        if (enemyLayer.value == 0)
        {
            int enemyLayerIndex = LayerMask.NameToLayer("Enemy");
            if (enemyLayerIndex != -1)
            {
                enemyLayer = 1 << enemyLayerIndex;
                Debug.Log(gameObject.name + ": Set enemyLayer to layer 'Enemy'");
            }
            else
            {
                Debug.LogError(gameObject.name + ": Enemy layer not found! Create a layer named 'Enemy'");
            }
        }
    }
    
    // Fix warning by adding override keyword
    protected override void Update()
    {
        base.Update();
    }
    
    protected override void HitTarget()
    {
        // Create impact effect (if available)
        if (impactEffect != null)
        {
            GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(effectIns, 2f);
        }
        
        // Apply splash damage
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, splashRadius, enemyLayer);
        
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            // Get the enemy's health component
            EnemyHealth enemyHealth = enemyCollider.GetComponent<EnemyHealth>();
            
            if (enemyHealth != null)
            {
                // Apply instant damage
                enemyHealth.TakeDamage(damage);
                
                // Apply damage over time if upgraded
                if (hasDotEffect)
                {
                    ApplyDamageOverTime(enemyHealth);
                }
            }
        }
        
        // Destroy the projectile
        Destroy(gameObject);
    }
    
    private void ApplyDamageOverTime(EnemyHealth enemy)
    {
        // Add DoT component to the enemy
        DamageOverTime dot = enemy.gameObject.GetComponent<DamageOverTime>();
        
        // If there's no DoT component, add one
        if (dot == null)
        {
            dot = enemy.gameObject.AddComponent<DamageOverTime>();
        }
        
        // Set or refresh the DoT effect
        dot.ApplyDotEffect(dotDamage, dotDuration);
    }
    
    // Draw the splash radius in the editor for visualization
    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
} 