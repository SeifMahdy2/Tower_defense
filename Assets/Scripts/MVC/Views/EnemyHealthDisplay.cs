using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthDisplay : MonoBehaviour
{
    [Header("Health Bar")]
    [Tooltip("Assign 6 sprites from empty to full health")]
    [SerializeField] public Sprite[] healthBarSprites = new Sprite[6];
    [SerializeField] private Image healthBarImage;
    
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private EnemyHealth targetEnemy;
    private Camera mainCamera;
    
    void Awake()
    {
        // Get required components
        if (healthBarImage == null)
            healthBarImage = GetComponent<Image>();
            
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main;
        
        // Ensure correct sprite array size
        if (healthBarSprites.Length != 6)
            System.Array.Resize(ref healthBarSprites, 6);
    }
    
    public void SetupHealthBar(EnemyHealth enemy)
    {
        targetEnemy = enemy;
        if (targetEnemy != null)
            UpdateHealthBar();
    }
    
    void Update()
    {
        // If target is destroyed, destroy this health bar too
        if (targetEnemy == null)
        {
            Destroy(gameObject);
            return;
        }
        
        UpdatePosition();
        UpdateHealthBar();
    }
    
    void UpdatePosition()
    {
        if (targetEnemy == null || mainCamera == null) 
            return;
        
        // Get screen position of enemy (with slight upward offset)
        Vector3 screenPos = mainCamera.WorldToScreenPoint(targetEnemy.transform.position + Vector3.up * 0.5f);
        
        // Convert to canvas space
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
            
        // Check for missing sprites
        foreach (Sprite sprite in healthBarSprites)
        {
            if (sprite == null)
                return;
        }
        
        // Calculate health percentage
        float healthPercent = (float)targetEnemy.GetCurrentHealth() / targetEnemy.GetMaxHealth();
        
        // Select appropriate sprite based on health percentage
        int spriteIndex = Mathf.Clamp(Mathf.FloorToInt(healthPercent * healthBarSprites.Length), 0, healthBarSprites.Length - 1);
        healthBarImage.sprite = healthBarSprites[spriteIndex];
    }
}