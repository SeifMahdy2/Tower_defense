using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float moveSpeed = 2f;

    private Transform target;
    private int pathIndex = 0;

    private void Start()
    {
        // Get the Rigidbody2D component if not assigned
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Check if LevelManager exists and has waypoints
        if (LevelManager.main != null && LevelManager.main.waypoints != null && LevelManager.main.waypoints.Length > 0)
        {
            target = LevelManager.main.waypoints[pathIndex];
        }
        else
        {
            Debug.LogError("No waypoints found or LevelManager is missing!");
        }
    }

    private void Update()
    {
        if (target == null) return;

        if (Vector2.Distance(target.position, transform.position) <= 0.1f)
        {
            pathIndex++;

            if (pathIndex >= LevelManager.main.waypoints.Length)
            {
                EnemySpawner.onEnemyDestroyed.Invoke();
                Destroy(gameObject);
                return;
            }
            else
            {
                target = LevelManager.main.waypoints[pathIndex];
            }
        }
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }
}
