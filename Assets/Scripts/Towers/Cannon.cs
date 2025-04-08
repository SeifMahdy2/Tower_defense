using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [Header("Tower Attributes")]
    [SerializeField] private float range = 3.5f;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private int damage = 30;
    [SerializeField] private float splashRadius = 1.2f;
    [SerializeField] private int cost = 200;
    
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject cannonballPrefab;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject rangeVisual;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    // Private variables
    private float fireCountdown = 0f;
    private Transform targetEnemy;
    private CircleCollider2D rangeCollider;
    private List<Transform> enemiesInRange = new List<Transform>();
    
    private void Awake()
    {
        // Get or create range collider
        rangeCollider = GetComponent<CircleCollider2D>();
        if (rangeCollider == null)
        {
            rangeCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        
        // Set up range collider
        rangeCollider.radius = range;
        rangeCollider.isTrigger = true;
        
        // Get sprite renderer if not assigned
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        // Create firepoint if not assigned
        if (firePoint == null)
        {
            GameObject newFirePoint = new GameObject("FirePoint");
            newFirePoint.transform.parent = transform;
            newFirePoint.transform.localPosition = new Vector3(0, 0.5f, 0);
            firePoint = newFirePoint.transform;
        }
    }
    
    private void Start()
    {
        // Update range visual if available
        if (rangeVisual != null)
        {
            rangeVisual.transform.localScale = new Vector3(range * 2, range * 2, 1);
            rangeVisual.SetActive(false); // Hide by default
        }
    }
    
    private void Update()
    {
        // If no target or target is invalid, find a new one
        if (targetEnemy == null || !IsValidTarget(targetEnemy))
        {
            FindClosestTarget();
        }
        
        // If we have a target, rotate to face it and shoot
        if (targetEnemy != null)
        {
            // Rotate to face target
            Vector3 direction = targetEnemy.position - transform.position;
            
            // Flip sprite based on target direction
            if (direction.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
            
            // Handle shooting
            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }
            
            fireCountdown -= Time.deltaTime;
        }
    }
    
    private void FindClosestTarget()
    {
        // Clear invalid enemies from the list
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            if (!IsValidTarget(enemiesInRange[i]))
            {
                enemiesInRange.RemoveAt(i);
            }
        }
        
        // Find closest enemy in range
        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;
        
        foreach (Transform enemy in enemiesInRange)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }
        
        targetEnemy = nearestEnemy;
    }
    
    private bool IsValidTarget(Transform target)
    {
        return target != null && target.gameObject.activeSelf;
    }
    
    private void Shoot()
    {
        // Create cannonball projectile
        if (cannonballPrefab != null && targetEnemy != null)
        {
            GameObject cannonballObj = Instantiate(cannonballPrefab, firePoint.position, Quaternion.identity);
            CannonProjectile cannonball = cannonballObj.GetComponent<CannonProjectile>();
            
            if (cannonball != null)
            {
                cannonball.Initialize(targetEnemy, damage, splashRadius, explosionPrefab);
            }
            else
            {
                // Fallback to regular projectile if CannonProjectile script is not found
                Projectile projectile = cannonballObj.GetComponent<Projectile>();
                if (projectile != null)
                {
                    projectile.Initialize(targetEnemy, damage);
                }
            }
        }
    }
    
    public void DealSplashDamage(Vector3 position, float radius, int damage)
    {
        // Find all enemies in splash radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, radius);
        
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }
        
        // Create explosion effect if available
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
            explosion.transform.localScale = new Vector3(radius, radius, 1);
            Destroy(explosion, 1f);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if enemy entered range
        if (collision.CompareTag("Enemy"))
        {
            enemiesInRange.Add(collision.transform);
            
            // If no current target, target this one
            if (targetEnemy == null)
            {
                targetEnemy = collision.transform;
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Remove enemy from range
        if (collision.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(collision.transform);
            
            // If this was our target, find a new one
            if (targetEnemy == collision.transform)
            {
                targetEnemy = null;
                FindClosestTarget();
            }
        }
    }
    
    public void ShowRange(bool show)
    {
        if (rangeVisual != null)
        {
            rangeVisual.SetActive(show);
        }
    }
    
    public int GetCost()
    {
        return cost;
    }
} 