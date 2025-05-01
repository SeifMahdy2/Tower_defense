using UnityEngine;
using UnityEngine.UI;

public class CastleHealthDisplay : MonoBehaviour
{
    [Header("Health Bar Sprites")]
    [SerializeField] private Sprite fullHealthSprite;    // Green
    [SerializeField] private Sprite highHealthSprite;    // Yellow-green
    [SerializeField] private Sprite mediumHealthSprite;  // Yellow
    [SerializeField] private Sprite lowHealthSprite;     // Orange
    
    [Header("References")]
    [SerializeField] private Image healthBarImage;
    private TD.GameManager gameManager;
    
    void Start()
    {
        // Find references if not assigned
        if (healthBarImage == null)
            healthBarImage = GetComponentInChildren<Image>();
            
        gameManager = FindObjectOfType<TD.GameManager>();
        
        if (gameManager == null)
            Debug.LogError("Castle Health Display could not find GameManager!");
            
        UpdateHealthBar();
    }
    
    void Update()
    {
        UpdateHealthBar();
    }
    
    void UpdateHealthBar()
    {
        if (gameManager == null || healthBarImage == null) return;
        
        float healthPercent = (float)gameManager.GetHealth() / gameManager.GetMaxHealth();
        
        // Change sprite based on health percentage
        if (healthPercent > 0.75f)
            healthBarImage.sprite = fullHealthSprite;
        else if (healthPercent > 0.5f)
            healthBarImage.sprite = highHealthSprite;
        else if (healthPercent > 0.25f)
            healthBarImage.sprite = mediumHealthSprite;
        else
            healthBarImage.sprite = lowHealthSprite;
    }
}