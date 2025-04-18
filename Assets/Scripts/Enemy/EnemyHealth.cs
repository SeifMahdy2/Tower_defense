using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum EnemyDifficulty
{
    Easy,
    Medium,
    Hard
}

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;
    
    [Header("Difficulty")]
    [SerializeField] private EnemyDifficulty difficulty = EnemyDifficulty.Easy;
    [SerializeField] private int damageToBase = 1; // Base damage to castle when reaching the end
    
    [Header("UI")]
    public GameObject healthBarPrefab;
    private GameObject healthBarInstance;
    private EnemyHealthDisplay healthDisplay;
    
    [Header("Hit Effect")]
    [SerializeField] private float hitFlashDuration = 0.2f;
    [SerializeField] private Color hitColor = Color.red;
    
    // References
    private TD.GameManager gameManager;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    // Public methods for health display
    public int GetCurrentHealth() { return currentHealth; }
    public int GetMaxHealth() { return maxHealth; }
    
    void Start()
    {
        // Set starting health based on difficulty
        SetupHealthByDifficulty();
        
        // Find the game manager
        gameManager = FindObjectOfType<TD.GameManager>();
        
        // Get components
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        // Create health bar
        if (healthBarPrefab != null)
        {
            // Find the Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            
            if (canvas != null)
            {
                // Instantiate the healthbar as child of canvas
                healthBarInstance = Instantiate(healthBarPrefab, canvas.transform);
                
                // Get the health display component
                healthDisplay = healthBarInstance.GetComponent<EnemyHealthDisplay>();
                if (healthDisplay != null)
                {
                    healthDisplay.SetupHealthBar(this);
                }
                else
                {
                    Debug.LogError("Health bar prefab doesn't have EnemyHealthDisplay component!");
                }
            }
            else
            {
                Debug.LogError("No Canvas found in the scene for health bars!");
            }
        }
    }
    
    void Update()
    {
        // The position update is now handled by the EnemyHealthDisplay component
    }
    
    private void SetupHealthByDifficulty()
    {
        // Adjust health based on difficulty
        switch (difficulty)
        {
            case EnemyDifficulty.Easy:
                maxHealth = 50;
                damageToBase = 1;
                break;
            case EnemyDifficulty.Medium:
                maxHealth = 100;
                damageToBase = 2;
                break;
            case EnemyDifficulty.Hard:
                maxHealth = 200;
                damageToBase = 3;
                break;
        }
        
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // Flash red when hit
        if (spriteRenderer != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashEffect());
        }
        
        // Update the health bar - now handled by the EnemyHealthDisplay component
        // which updates automatically in its Update method
        
        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    IEnumerator FlashEffect()
    {
        // Change to hit color
        spriteRenderer.color = hitColor;
        
        // Wait for duration
        yield return new WaitForSeconds(hitFlashDuration);
        
        // Change back to original color
        spriteRenderer.color = originalColor;
    }
    
    void Die()
    {
        // Play death animation
        if (animator != null)
        {
            // Disable movement
            EnemyMovement movement = GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.enabled = false;
            }
            
            // Trigger death animation
            animator.SetTrigger("Death");
            
            // Destroy the object after animation finishes
            Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
        }
        else
        {
            // No animator, destroy immediately
            Destroy(gameObject);
        }
        
        // Add gold to the player based on difficulty
        if (gameManager != null)
        {
            int goldReward = 5; // Base reward
            
            switch (difficulty)
            {
                case EnemyDifficulty.Easy:
                    goldReward = 5;
                    break;
                case EnemyDifficulty.Medium:
                    goldReward = 10;
                    break;
                case EnemyDifficulty.Hard:
                    goldReward = 20;
                    break;
            }
            
            gameManager.AddGold(goldReward);
        }
        
        // Remove health bar
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }
    }
    
    public int GetDamageToBase()
    {
        return damageToBase;
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