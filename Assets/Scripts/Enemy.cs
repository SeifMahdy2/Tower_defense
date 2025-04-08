using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int goldReward = 25;
    [SerializeField] private int damage = 10;
    [SerializeField] private float moveSpeed = 2f;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject deathEffect;

    // Private variables
    private int currentHealth;
    private Transform target;
    private int waypointIndex = 0;
    private Transform[] waypoints;
    private bool isSlowed = false;
    private float slowTimer = 0f;
    private float originalSpeed;
    private GameManager gameManager;
    
    // Enemy death event
    public delegate void EnemyDeathHandler();
    public event EnemyDeathHandler OnEnemyDeath;

    private void Awake()
    {
        // Get references if not assigned
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
        originalSpeed = moveSpeed;
        
        // Find game manager
        gameManager = FindObjectOfType<GameManager>();
        
        // Find waypoints
        if (GameObject.Find("Path"))
        {
            Transform pathTransform = GameObject.Find("Path").transform;
            waypoints = new Transform[pathTransform.childCount];
            for (int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = pathTransform.GetChild(i);
            }
            
            // Set first waypoint as target
            if (waypoints.Length > 0)
                target = waypoints[0];
        }
        else
        {
            Debug.LogError("No path found for enemy to follow!");
        }
    }

    private void Update()
    {
        if (target == null) return;
        
        // Move towards current waypoint
        Vector3 direction = target.position - transform.position;
        transform.Translate(direction.normalized * moveSpeed * Time.deltaTime, Space.World);
        
        // Flip sprite based on movement direction
        if (direction.x < 0)
            spriteRenderer.flipX = true;
        else if (direction.x > 0)
            spriteRenderer.flipX = false;
        
        // Check if waypoint reached
        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            GetNextWaypoint();
        }
        
        // Handle slow effect timer
        if (isSlowed)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f)
            {
                // Reset speed
                moveSpeed = originalSpeed;
                isSlowed = false;
            }
        }
    }

    private void GetNextWaypoint()
    {
        // Check if we reached the end
        if (waypointIndex >= waypoints.Length - 1)
        {
            ReachedEnd();
            return;
        }
        
        // Move to next waypoint
        waypointIndex++;
        target = waypoints[waypointIndex];
    }

    private void ReachedEnd()
    {
        // Damage player base
        if (gameManager != null)
        {
            gameManager.EnemyReachedEnd(damage);
        }
        
        // Trigger death event without giving gold
        if (OnEnemyDeath != null)
        {
            OnEnemyDeath();
        }
        
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // Check if dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ApplySlow(float slowAmount, float duration)
    {
        if (!isSlowed || slowTimer < duration)
        {
            isSlowed = true;
            slowTimer = duration;
            moveSpeed = originalSpeed * (1f - slowAmount);
        }
    }

    private void Die()
    {
        // Give gold reward
        if (gameManager != null)
        {
            gameManager.AddGold(goldReward);
            gameManager.IncrementEnemiesDefeated();
        }
        
        // Trigger death event
        if (OnEnemyDeath != null)
        {
            OnEnemyDeath();
        }
        
        // Spawn death effect if available
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        
        // Destroy enemy
        Destroy(gameObject);
    }
} 