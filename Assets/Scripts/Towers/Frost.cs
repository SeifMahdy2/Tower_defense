using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frost : MonoBehaviour
{
    [Header("Tower Attributes")]
    [SerializeField] private float range = 2.5f;
    [SerializeField] private int cost = 150;
    [SerializeField] private float slowAmount = 0.3f; // 30% speed reduction
    [SerializeField] private float slowDuration = 0.5f; // How long the effect lasts after leaving range
    
    [Header("References")]
    [SerializeField] private GameObject rangeVisual;
    [SerializeField] private GameObject slowEffect;
    
    // Private variables
    private CircleCollider2D rangeCollider;
    private List<Enemy> enemiesInRange = new List<Enemy>();
    
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
    }
    
    private void Start()
    {
        // Update range visual if available
        if (rangeVisual != null)
        {
            rangeVisual.transform.localScale = new Vector3(range * 2, range * 2, 1);
            rangeVisual.SetActive(false); // Hide by default
        }
        
        // Apply frost effect
        if (slowEffect != null)
        {
            slowEffect.transform.localScale = new Vector3(range * 2, range * 2, 1);
        }
    }
    
    private void Update()
    {
        // Apply slow effect to all enemies in range
        ApplySlowToEnemiesInRange();
        
        // Clean up destroyed enemies from the list
        CleanupEnemyList();
    }
    
    private void ApplySlowToEnemiesInRange()
    {
        foreach (Enemy enemy in enemiesInRange)
        {
            if (enemy != null)
            {
                enemy.ApplySlow(slowAmount, slowDuration);
            }
        }
    }
    
    private void CleanupEnemyList()
    {
        // Remove any destroyed enemies from our list
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            if (enemiesInRange[i] == null)
            {
                enemiesInRange.RemoveAt(i);
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if enemy entered range
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
                
                // Immediately apply slow effect
                enemy.ApplySlow(slowAmount, slowDuration);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Remove enemy from range
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemiesInRange.Remove(enemy);
                
                // The slow effect will wear off based on the slowDuration
                // This is handled in the Enemy class
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