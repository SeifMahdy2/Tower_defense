using UnityEngine;
using System.Collections.Generic;

public class ArrowProjectile : Projectile
{
    [SerializeField] private bool canPierce = false;
    [SerializeField] private int maxPierceCount = 2;
    [SerializeField] private float pierceDamageReduction = 0.5f; // 50% damage reduction per enemy pierced
    
    private List<Transform> hitEnemies = new List<Transform>();
    private int pierceCount = 0;
    private Vector3 lastKnownTargetPosition;
    
    protected override void HitTarget()
    {
        if (target != null)
        {
            // Store last known position
            lastKnownTargetPosition = target.position;
            
            // Check if we've already hit this enemy
            if (hitEnemies.Contains(target))
                return;
                
            // Apply damage to enemy
            EnemyHealth enemy = target.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                // Calculate damage with reduction based on pierce count
                int currentDamage = Mathf.RoundToInt(damage * Mathf.Pow(1f - pierceDamageReduction, pierceCount));
                enemy.TakeDamage(currentDamage);
                
                // Add to hit list to prevent hitting same enemy twice
                hitEnemies.Add(target);
            }
            
            // Create hit effect
            if (impactEffect != null)
            {
                GameObject effect = Instantiate(impactEffect, transform.position, Quaternion.identity);
                Destroy(effect, 1f);
            }
            
            // If piercing is enabled and we haven't reached max pierce count
            if (canPierce && pierceCount < maxPierceCount)
            {
                pierceCount++;
                
                // Find next closest enemy
                FindNextTarget();
                
                // If no more targets, destroy the projectile
                if (target == null)
                    Destroy(gameObject);
                    
                return;
            }
        }
        
        // If not piercing or no more targets, destroy projectile
        Destroy(gameObject);
    }
    
    private void FindNextTarget()
    {
        // Find all enemies within a certain range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 5f, LayerMask.GetMask("Enemy"));
        
        float closestDistance = float.MaxValue;
        Transform nextTarget = null;
        
        foreach (Collider2D col in colliders)
        {
            // Skip already hit enemies
            if (hitEnemies.Contains(col.transform))
                continue;
                
            float distance = Vector3.Distance(transform.position, col.transform.position);
            
            // Find the closest enemy we haven't hit
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nextTarget = col.transform;
            }
        }
        
        // Set the new target
        if (nextTarget != null)
        {
            target = nextTarget;
            lastKnownTargetPosition = target.position;
        }
        else
        {
            target = null;
        }
    }
} 