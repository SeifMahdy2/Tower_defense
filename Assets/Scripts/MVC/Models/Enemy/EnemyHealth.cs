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
    [SerializeField] private int damageToBase = 1;
    
    [Header("UI")]
    public GameObject healthBarPrefab;
    private GameObject healthBarInstance;
    private EnemyHealthDisplay healthDisplay;
    
    [Header("Hit Effect")]
    [SerializeField] private float hitFlashDuration = 0.2f;
    [SerializeField] private Color hitColor = Color.red;
    
    private TD.GameManager gameManager;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool goldRewardGiven = false;
    
    // Public methods for health display
    public int GetCurrentHealth() { return currentHealth; }
    public int GetMaxHealth() { return maxHealth; }
    
    void Start()
    {
        SetupHealthByDifficulty();
        
        gameManager = FindObjectOfType<TD.GameManager>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
        
        CreateHealthBar();
    }
    
    private void SetupHealthByDifficulty()
    {
        switch (difficulty)
        {
            case EnemyDifficulty.Easy:
                maxHealth = 100;
                damageToBase = 10;
                break;
            case EnemyDifficulty.Medium:
                maxHealth = 350;
                damageToBase = 40;
                break;
            case EnemyDifficulty.Hard:
                maxHealth = 600;
                damageToBase = 60;
                break;
        }
        
        currentHealth = maxHealth;
    }
    
    private void CreateHealthBar()
    {
        if (healthBarPrefab == null)
            return;
            
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
            return;
            
        // Create health bar
        healthBarInstance = Instantiate(healthBarPrefab, canvas.transform);
        healthDisplay = healthBarInstance.GetComponent<EnemyHealthDisplay>();
        
        if (healthDisplay != null)
            healthDisplay.SetupHealthBar(this);
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
        
        // Check for death
        if (currentHealth <= 0)
            Die();
    }
    
    IEnumerator FlashEffect()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalColor;
    }
    
    void Die()
    {
        // Notify the EnemySpawner
        EnemySpawner.onEnemyDestroyed.Invoke();
        
        // Add gold reward
        GiveGoldReward();
        
        // Play death animation
        PlayDeathAnimation();
        
        // Disable movement
        var movement = GetComponent<EnemyMovement>();
        if (movement != null)
            movement.enabled = false;
            
        // Remove health bar
        if (healthBarInstance != null)
            Destroy(healthBarInstance);
    }
    
    private void GiveGoldReward()
    {
        if (gameManager == null || goldRewardGiven)
            return;
            
        // Calculate reward based on difficulty
        int goldReward = 15; // Default value
        switch (difficulty)
        {
            case EnemyDifficulty.Easy:   goldReward = 20; break;
            case EnemyDifficulty.Medium: goldReward = 35; break;
            case EnemyDifficulty.Hard:   goldReward = 50; break;
        }
        
        gameManager.AddGold(goldReward);
        goldRewardGiven = true;
    }
    
    private void PlayDeathAnimation()
    {
        if (animator == null)
        {
            Destroy(gameObject, 0.5f);
            return;
        }
        
        try {
            animator.Play("Death");
            animator.SetTrigger("Death");
            Destroy(gameObject, 1.0f);
        }
        catch {
            Destroy(gameObject, 0.5f);
        }
    }
    
    public int GetDamageToBase()
    {
        return damageToBase;
    }
    
    void OnDestroy()
    {
        // Clean up health bar
        if (healthBarInstance != null)
            Destroy(healthBarInstance);
    }
} 