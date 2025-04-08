using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTargeting : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private float range = 3f;
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Transform towerRotationPart; // The part of the tower that rotates (like the turret or cannon)

    [Header("Attack")]
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private bool canAttack = true;

    private Transform target;
    private float attackCooldown;

    private void Start()
    {
        // If no rotation part is assigned, use this transform
        if (towerRotationPart == null)
        {
            towerRotationPart = transform;
        }

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
        if (attackCooldown <= 0 && canAttack)
        {
            Attack();
            attackCooldown = 1f / attackRate;
        }
    }

    private void UpdateTarget()
    {
        // Find all enemies in the scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        
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
        // Calculate direction to target
        Vector2 direction = (target.position - transform.position).normalized;
        
        // Calculate rotation angle (in 2D)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        
        // Create rotation
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
        
        // Smoothly rotate
        towerRotationPart.rotation = Quaternion.Slerp(
            towerRotationPart.rotation, 
            targetRotation, 
            rotationSpeed * Time.deltaTime
        );
    }

    private void Attack()
    {
        // Implement your attack logic here
        // For example, instantiate a projectile or apply damage
        Debug.Log(gameObject.name + " is attacking " + target.name);
    }

    // Visualization for range in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
} 