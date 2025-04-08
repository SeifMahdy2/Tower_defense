using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform archerRotationPoint;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject arrowPrefab;

    [Header("Attributes")]
    [SerializeField] private float range = 10f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private int damage = 10;

    private Transform target;
    private float attackCooldown;

    private void Start()
    {
        // Start looking for targets
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        // Rotate to look at the target
        RotateTowardsTarget();

        // Handle attacking
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0)
        {
            Attack();
            attackCooldown = 1f / attackRate;
        }
    }

    private void UpdateTarget()
    {
        // Find all enemies in the scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        // Variables to track closest enemy
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        // Loop through all enemies
        foreach (GameObject enemy in enemies)
        {
            // Calculate distance
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            
            // If this enemy is closer than the previous closest
            if (distanceToEnemy < shortestDistance && distanceToEnemy <= range)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        // Set the target to the nearest enemy (or null if none found in range)
        if (nearestEnemy != null)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    private void RotateTowardsTarget()
    {
        if (archerRotationPoint == null)
            return;
            
        // Calculate direction to target
        Vector2 direction = (target.position - transform.position).normalized;
        
        // Calculate rotation angle (in 2D)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        
        // Create rotation
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
        
        // Smoothly rotate
        archerRotationPoint.rotation = Quaternion.Slerp(
            archerRotationPoint.rotation, 
            targetRotation, 
            rotationSpeed * Time.deltaTime
        );
    }

    private void Attack()
    {
        if (arrowPrefab != null && projectileSpawnPoint != null)
        {
            GameObject arrowObj = Instantiate(arrowPrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            Projectile arrow = arrowObj.GetComponent<Projectile>();
            
            if (arrow != null)
            {
                arrow.Initialize(target, damage);
            }
        }
    }

    // Visualization for range in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
