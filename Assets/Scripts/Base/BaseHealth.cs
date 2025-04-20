using UnityEngine;
using UnityEngine.UI;

public class BaseHealth : MonoBehaviour
{
    [Header("Health Sprites")]
    [SerializeField] private Sprite fullHealthSprite;
    [SerializeField] private Sprite highHealthSprite;  // 75% - 99%
    [SerializeField] private Sprite mediumHealthSprite; // 50% - 74%
    [SerializeField] private Sprite lowHealthSprite;    // 25% - 49%
    [SerializeField] private Sprite criticalHealthSprite; // 1% - 24%
    
    [Header("References")]
    [SerializeField] private SpriteRenderer baseSpriteRenderer;
    
    private TD.GameManager gameManager;
    
    void Start()
    {
        // Find the GameManager if not assigned
        gameManager = FindObjectOfType<TD.GameManager>();
        
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found! Base health visuals won't work.");
            return;
        }
        
        // Find sprite renderer if not assigned
        if (baseSpriteRenderer == null)
        {
            baseSpriteRenderer = GetComponent<SpriteRenderer>();
            
            if (baseSpriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer not found on base object! Base health visuals won't work.");
                return;
            }
        }
        
        // Set initial sprite
        UpdateVisuals();
    }
    
    void Update()
    {
        // Continuously update visuals in case health changes
        UpdateVisuals();
    }
    
    private void UpdateVisuals()
    {
        if (gameManager == null || baseSpriteRenderer == null)
            return;
            
        int currentHealth = gameManager.GetHealth();
        int maxHealth = gameManager.GetMaxHealth();
        
        // Calculate health percentage
        float healthPercentage = (float)currentHealth / maxHealth;
        
        // Update sprite based on health percentage
        if (healthPercentage >= 1.0f && fullHealthSprite != null)
        {
            baseSpriteRenderer.sprite = fullHealthSprite;
        }
        else if (healthPercentage >= 0.75f && highHealthSprite != null)
        {
            baseSpriteRenderer.sprite = highHealthSprite;
        }
        else if (healthPercentage >= 0.5f && mediumHealthSprite != null)
        {
            baseSpriteRenderer.sprite = mediumHealthSprite;
        }
        else if (healthPercentage >= 0.25f && lowHealthSprite != null)
        {
            baseSpriteRenderer.sprite = lowHealthSprite;
        }
        else if (healthPercentage > 0 && criticalHealthSprite != null)
        {
            baseSpriteRenderer.sprite = criticalHealthSprite;
        }
        
        // Log health status for debugging
        Debug.Log("Base Health: " + currentHealth + "/" + maxHealth + " (" + (healthPercentage * 100) + "%)");
    }
} 