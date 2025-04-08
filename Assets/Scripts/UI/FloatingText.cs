using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private float lifetime = 1.5f;
    
    [Header("References")]
    [SerializeField] private TextMeshProUGUI textComponent;
    
    private Vector3 moveDirection;
    private float timeSinceCreated;
    private Color originalColor;
    
    public static FloatingText Create(Vector3 position, string text, Color textColor, Transform parent = null)
    {
        // Try to find the prefab (should be placed in Resources/Prefabs/UI)
        GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/FloatingText");
        
        if (prefab == null)
        {
            Debug.LogError("FloatingText prefab not found. Make sure it's in a Resources/Prefabs/UI folder!");
            return null;
        }
        
        // Create instance
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        
        // Set parent if provided
        if (parent != null)
        {
            instance.transform.SetParent(parent);
        }
        
        // Get and setup component
        FloatingText floatingText = instance.GetComponent<FloatingText>();
        if (floatingText != null)
        {
            floatingText.SetText(text);
            floatingText.SetColor(textColor);
            floatingText.SetDirection(new Vector3(Random.Range(-0.5f, 0.5f), 1f, 0));
        }
        
        return floatingText;
    }
    
    private void Awake()
    {
        // If text component not assigned, try to get it
        if (textComponent == null)
        {
            textComponent = GetComponentInChildren<TextMeshProUGUI>();
        }
        
        // Store original color for fading
        if (textComponent != null)
        {
            originalColor = textComponent.color;
        }
        
        // Set random direction by default
        moveDirection = new Vector3(Random.Range(-0.5f, 0.5f), 1f, 0);
    }
    
    private void Update()
    {
        // Move upward
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        
        // Fade out over time
        timeSinceCreated += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, timeSinceCreated / lifetime);
        
        if (textComponent != null)
        {
            Color newColor = originalColor;
            newColor.a = alpha;
            textComponent.color = newColor;
        }
        
        // Destroy when lifetime is over
        if (timeSinceCreated >= lifetime)
        {
            Destroy(gameObject);
        }
    }
    
    public void SetText(string text)
    {
        if (textComponent != null)
        {
            textComponent.text = text;
        }
    }
    
    public void SetColor(Color color)
    {
        if (textComponent != null)
        {
            textComponent.color = color;
            originalColor = color;
        }
    }
    
    public void SetDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }
} 