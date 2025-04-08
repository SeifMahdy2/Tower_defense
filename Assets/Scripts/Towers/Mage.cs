using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : MonoBehaviour
{
    [Header("Tower Attributes")]
    [SerializeField] private float range = 4f;
    [SerializeField] private float fireRate = 0.8f; // Slower fire rate
    [SerializeField] private int damage = 50; // Higher damage
    [SerializeField] private int cost = 175;
    
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject magicPrefab;
    [SerializeField] private GameObject rangeVisual;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject chargingEffect;
    
    // Private variables
    private float fireCountdown = 0f;
    private Transform targetEnemy;
    private CircleCollider2D rangeCollider;
    private List<Transform> enemiesInRange = new List<Transform>();
    private bool isCharging = false;
    
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
        
        // Initialize charging effect
        if (chargingEffect != null)
        {
            chargingEffect.SetActive(false);
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
            
            // Hide charging effect if target lost
            if (targetEnemy == null && isCharging && chargingEffect != null)
            {
                chargingEffect.SetActive(false);
                isCharging = false;
            }
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
                StartCoroutine(ChargeAndShoot());
                fireCountdown = 1f / fireRate;
            }
            
            fireCountdown -= Time.deltaTime;
        }
    }
    
    private IEnumerator ChargeAndShoot()
    {
        // Start charging effect
        if (chargingEffect != null)
        {
            chargingEffect.SetActive(true);
            isCharging = true;
        }
        
        // Wait for charge time (0.3 seconds)
        yield return new WaitForSeconds(0.3f);
        
        // Check if target still exists
        if (targetEnemy != null && IsValidTarget(targetEnemy))
        {
            Shoot();
        }
        
        // Turn off charging effect
        if (chargingEffect != null)
        {
            chargingEffect.SetActive(false);
            isCharging = false;
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
        // Create magic projectile
        if (magicPrefab != null && targetEnemy != null)
        {
            GameObject magicObj = Instantiate(magicPrefab, firePoint.position, Quaternion.identity);
            Projectile magic = magicObj.GetComponent<Projectile>();
            
            if (magic != null)
            {
                magic.Initialize(targetEnemy, damage);
            }
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