using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TowerPlacementArea : MonoBehaviour
{
    [Header("Placement Settings")]
    [SerializeField] private bool isOccupied = false;
    [SerializeField] private Color availableColor = new Color(0.2f, 0.8f, 0.2f, 0.3f);
    [SerializeField] private Color occupiedColor = new Color(0.8f, 0.2f, 0.2f, 0.3f);
    [SerializeField] private Color highlightColor = new Color(0.3f, 0.9f, 0.3f, 0.5f);
    
    // Visual indicator
    private SpriteRenderer visualIndicator;
    private BoxCollider2D boxCollider;
    
    // Reference to the tower placed here
    private TowerController placedTower;
    
    // State
    private bool isHighlighted = false;
    
    // Properties
    public bool IsOccupied => isOccupied;
    public TowerController PlacedTower => placedTower;
    
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        
        // Ensure the collider is a trigger
        boxCollider.isTrigger = true;
        
        // Create visual indicator
        CreateVisualIndicator();
        
        // Ensure this object is on the PlacementArea layer
        // If the layer doesn't exist, it will stay on its current layer
        int placementLayer = LayerMask.NameToLayer("PlacementArea");
        if (placementLayer != -1)
        {
            gameObject.layer = placementLayer;
        }
        else
        {
            Debug.LogWarning("PlacementArea layer not found. Please create it in the Tags & Layers settings.");
        }
    }
    
    private void Update()
    {
        // Update the visual indicator color based on state
        UpdateVisualColor();
    }
    
    // Create a simple visual indicator to show the placement area
    private void CreateVisualIndicator()
    {
        if (visualIndicator == null)
        {
            GameObject indicator = new GameObject("PlacementIndicator");
            indicator.transform.SetParent(transform);
            indicator.transform.localPosition = Vector3.zero;
            
            visualIndicator = indicator.AddComponent<SpriteRenderer>();
            visualIndicator.sprite = CreateSquareSprite();
            visualIndicator.color = availableColor;
            visualIndicator.sortingOrder = -2; // Below towers and path
            
            // Set the size based on the box collider
            Vector2 size = boxCollider.size;
            indicator.transform.localScale = new Vector3(size.x, size.y, 1f);
        }
    }
    
    // Create a simple square sprite for the placement indicator
    private Sprite CreateSquareSprite()
    {
        int resolution = 100;
        Texture2D texture = new Texture2D(resolution, resolution);
        Color[] colors = new Color[resolution * resolution];
        
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.white;
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(
            texture, 
            new Rect(0, 0, resolution, resolution), 
            new Vector2(0.5f, 0.5f),
            resolution
        );
    }
    
    // Update the visual color based on state
    private void UpdateVisualColor()
    {
        if (visualIndicator == null) return;
        
        if (isHighlighted)
        {
            visualIndicator.color = highlightColor;
        }
        else if (isOccupied)
        {
            visualIndicator.color = occupiedColor;
        }
        else
        {
            visualIndicator.color = availableColor;
        }
    }
    
    // Place a tower on this area
    public bool PlaceTower(TowerController tower)
    {
        if (isOccupied) return false;
        
        // Set as occupied
        isOccupied = true;
        placedTower = tower;
        
        // Position the tower properly
        if (tower != null)
        {
            tower.transform.position = transform.position;
        }
        
        return true;
    }
    
    // Remove the tower from this area
    public void RemoveTower()
    {
        isOccupied = false;
        placedTower = null;
    }
    
    // Highlight this area (for tower placement preview)
    public void Highlight(bool highlight)
    {
        isHighlighted = highlight;
    }
    
    // Toggle visibility of the placement indicator
    public void SetIndicatorVisible(bool visible)
    {
        if (visualIndicator != null)
        {
            visualIndicator.enabled = visible;
        }
    }
} 