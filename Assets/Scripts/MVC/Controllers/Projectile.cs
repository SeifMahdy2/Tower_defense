using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float maxLifetime = 5f;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private float rotationSpeed = 200f;
    
    // Target and damage info
    private EnemyController target;
    private float damage;
    private DamageType damageType;
    private float splashRadius;
    
    // Slow effect (for frost towers)
    private float slowPercentage;
    private float slowDuration;
    
    // Current lifetime
    private float lifetime;
    
    private void Update()
    {
        // Check if target is valid
        if (target == null || !target.gameObject.activeSelf || !target.IsActive)
        {
            Destroy(gameObject);
            return;
        }
        
        // Move towards target
        Vector3 direction = target.transform.position - transform.position;
        float distanceThisFrame = moveSpeed * Time.deltaTime;
        
        // If we will reach the target this frame
        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }
        
        // Rotate projectile to face movement direction
        if (rotationSpeed > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        
        // Move projectile
        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        
        // Check lifetime
        lifetime += Time.deltaTime;
        if (lifetime >= maxLifetime)
        {
            Destroy(gameObject);
        }
    }
    
    // Initialize the projectile with target and damage info
    public void Initialize(EnemyController targetEnemy, float damageAmount, DamageType type, float splashDamageRadius = 0f)
    {
        target = targetEnemy;
        damage = damageAmount;
        damageType = type;
        splashRadius = splashDamageRadius;
        lifetime = 0f;
        
        // Rotate to initially face the target
        if (target != null)
        {
            Vector3 direction = target.transform.position - transform.position;
            transform.up = direction.normalized;
        }
    }
    
    // Set slow effect for frost towers
    public void SetSlowEffect(float percentage, float duration)
    {
        slowPercentage = percentage;
        slowDuration = duration;
    }
    
    // When projectile hits the target
    private void HitTarget()
    {
        // Create impact effect if assigned
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }
        
        // Apply area damage if splash radius is set
        if (splashRadius > 0)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, splashRadius);
            
            foreach (Collider2D hitCollider in hitColliders)
            {
                EnemyController enemy = hitCollider.GetComponent<EnemyController>();
                if (enemy != null && enemy.gameObject.activeSelf && enemy.IsActive)
                {
                    // Apply damage
                    enemy.TakeDamage(damage, damageType);
                    
                    // Apply slow effect for frost towers
                    if (damageType == DamageType.Frost && slowPercentage > 0)
                    {
                        enemy.ApplySlow(slowPercentage, slowDuration);
                    }
                }
            }
        }
        else
        {
            // Apply single target damage
            if (target != null && target.gameObject.activeSelf && target.IsActive)
            {
                // Apply damage
                target.TakeDamage(damage, damageType);
                
                // Apply slow effect for frost towers
                if (damageType == DamageType.Frost && slowPercentage > 0)
                {
                    target.ApplySlow(slowPercentage, slowDuration);
                }
            }
        }
        
        // Destroy the projectile
        Destroy(gameObject);
    }
    
    // Draw splash radius in editor
    private void OnDrawGizmosSelected()
    {
        if (splashRadius > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, splashRadius);
        }
    }
} 