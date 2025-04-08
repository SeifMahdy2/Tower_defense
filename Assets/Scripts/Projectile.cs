using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxLifetime = 5f;
    
    private Transform target;
    private int damage;
    private Vector2 direction;

    private void Awake()
    {
        // Get components if not assigned
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void Start()
    {
        // Destroy after max lifetime to prevent memory leaks
        Destroy(gameObject, maxLifetime);
    }

    public void Initialize(Transform target, int damage)
    {
        this.target = target;
        this.damage = damage;
        
        if (target != null)
        {
            // Calculate initial direction to target
            direction = (target.position - transform.position).normalized;
            
            // Set rotation to face the target
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // If no target (shouldn't happen but just in case), destroy self
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            // Target was destroyed, continue in last known direction
            rb.velocity = direction * speed;
            return;
        }

        // Home in on target (optional - for slight tracking)
        direction = (target.position - transform.position).normalized;
        rb.velocity = direction * speed;
        
        // Update rotation to face direction of movement
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if we hit an enemy
        if (collision.CompareTag("Enemy"))
        {
            // Get the enemy component and deal damage
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            
            // Destroy the projectile
            Destroy(gameObject);
        }
    }
} 