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
    
    [SerializeField] private SpriteRenderer baseSpriteRenderer;
    
    private TD.GameManager gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<TD.GameManager>();
        
        if (baseSpriteRenderer == null)
            baseSpriteRenderer = GetComponent<SpriteRenderer>();
            
        UpdateVisuals();
    }
    
    void Update()
    {
        UpdateVisuals();
    }
    
    private void UpdateVisuals()
    {
        if (gameManager == null || baseSpriteRenderer == null)
            return;
            
        int currentHealth = gameManager.GetHealth();
        int maxHealth = gameManager.GetMaxHealth();
        float healthPercentage = (float)currentHealth / maxHealth;
        
        // Select sprite based on health percentage
        if (healthPercentage >= 1.0f && fullHealthSprite != null)
            baseSpriteRenderer.sprite = fullHealthSprite;
        else if (healthPercentage >= 0.75f && highHealthSprite != null)
            baseSpriteRenderer.sprite = highHealthSprite;
        else if (healthPercentage >= 0.5f && mediumHealthSprite != null)
            baseSpriteRenderer.sprite = mediumHealthSprite;
        else if (healthPercentage >= 0.25f && lowHealthSprite != null)
            baseSpriteRenderer.sprite = lowHealthSprite;
        else if (healthPercentage > 0 && criticalHealthSprite != null)
            baseSpriteRenderer.sprite = criticalHealthSprite;
    }
} 