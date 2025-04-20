using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthDisplay : MonoBehaviour
{
    [Header("Health Bar Sprites")]
    [Tooltip("Assign 6 sprites from empty to full health")]
    [SerializeField] public Sprite[] healthBarSprites = new Sprite[6]; // Initialize with 6 elements
    
    [Header("References")]
    [SerializeField] private Image healthBarImage;
    
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private EnemyHealth targetEnemy;
    private Camera mainCamera;
    
    void Awake()
    {
        // Get references if not assigned
        if (healthBarImage == null)
            healthBarImage = GetComponent<Image>();
            
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main;
        
        if (healthBarImage == null)
            Debug.LogError("Could not find health bar image!");
            
        // Validate health bar sprites
        if (healthBarSprites.Length != 6)
        {
            Debug.LogWarning("EnemyHealthDisplay should have exactly 6 sprites. Resizing array.");
            System.Array.Resize(ref healthBarSprites, 6);
        }
    }
    
    public void SetupHealthBar(EnemyHealth enemy)
    {
        targetEnemy = enemy;
        
        if (targetEnemy == null)
        {
            Debug.LogError("Invalid enemy health reference!");
            return;
        }
        
        UpdateHealthBar();
    }
    
    void Update()
    {
        if (targetEnemy == null)
        {
            Destroy(gameObject);
            return;
        }
        
        // Update position to follow the enemy
        UpdatePosition();
        
        // Update the health bar display
        UpdateHealthBar();
    }
    
    void UpdatePosition()
    {
        if (targetEnemy == null || mainCamera == null) return;
        
        // Convert world position to screen position
        Vector3 screenPos = mainCamera.WorldToScreenPoint(targetEnemy.transform.position + Vector3.up * 0.5f);
        
        // Convert screen position to local canvas position
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.GetComponent<RectTransform>(),
            screenPos,
            parentCanvas.worldCamera,
            out Vector2 localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }
    }
    
    public void UpdateHealthBar()
    {
        if (targetEnemy == null || healthBarImage == null) 
            return;
            
        // Check if all sprites are assigned
        bool allSpritesAssigned = true;
        foreach (Sprite sprite in healthBarSprites)
        {
            if (sprite == null)
            {
                allSpritesAssigned = false;
                break;
            }
        }
        
        if (!allSpritesAssigned)
        {
            Debug.LogWarning("Some health bar sprites are not assigned!");
            return;
        }
        
        float healthPercent = (float)targetEnemy.GetCurrentHealth() / targetEnemy.GetMaxHealth();
        
        // Calculate which sprite to use (0-5 for 6 sprites)
        int spriteIndex = Mathf.Clamp(Mathf.FloorToInt(healthPercent * healthBarSprites.Length), 0, healthBarSprites.Length - 1);
        
        // Use the right sprite based on health percentage
        healthBarImage.sprite = healthBarSprites[spriteIndex];
    }
}