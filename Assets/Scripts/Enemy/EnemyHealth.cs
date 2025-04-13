using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;
    
    [Header("UI")]
    public GameObject healthBarPrefab;
    private GameObject healthBarInstance;
    private Slider healthSlider;
    
    [Header("Effects")]
    public GameObject deathEffect;
    
    // References
    private GameManager gameManager;
    
    void Start()
    {
        // Set starting health
        currentHealth = maxHealth;
        
        // Find the game manager
        gameManager = FindObjectOfType<GameManager>();
        
        // Create health bar
        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
            healthBarInstance.transform.SetParent(GameObject.Find("Canvas").transform);
            healthSlider = healthBarInstance.GetComponentInChildren<Slider>();
            UpdateHealthBar();
        }
    }
    
    void Update()
    {
        // Update health bar position
        if (healthBarInstance != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 0.5f);
            healthBarInstance.transform.position = screenPos;
        }
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // Update the health bar
        UpdateHealthBar();
        
        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth;
        }
    }
    
    void Die()
    {
        // Create death effect
        if (deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
        
        // Add gold to the player
        if (gameManager != null)
        {
            gameManager.AddGold(10);
        }
        
        // Remove health bar
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }
        
        // Destroy the enemy
        Destroy(gameObject);
    }
    
    void OnDestroy()
    {
        // Clean up health bar if it exists
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }
    }
} 