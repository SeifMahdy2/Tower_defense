using UnityEngine;
using UnityEngine.UI;

public class TowerUpgradePanelSafeArea : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        
        ApplySafeArea();
    }
    
    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        
        // Convert safe area to canvas space
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        
        if (canvas && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
        }
        
        // Keep original size and pivot
        Vector2 originalSize = rectTransform.sizeDelta;
        Vector2 originalPivot = rectTransform.pivot;
        
        // Apply safe area boundaries for positioning
        // This ensures the panel doesn't go outside the safe area
        if (rectTransform.position.x < safeArea.x + originalSize.x * originalPivot.x)
        {
            Vector3 pos = rectTransform.position;
            pos.x = safeArea.x + originalSize.x * originalPivot.x;
            rectTransform.position = pos;
        }
        
        if (rectTransform.position.x > safeArea.x + safeArea.width - originalSize.x * (1 - originalPivot.x))
        {
            Vector3 pos = rectTransform.position;
            pos.x = safeArea.x + safeArea.width - originalSize.x * (1 - originalPivot.x);
            rectTransform.position = pos;
        }
        
        if (rectTransform.position.y < safeArea.y + originalSize.y * originalPivot.y)
        {
            Vector3 pos = rectTransform.position;
            pos.y = safeArea.y + originalSize.y * originalPivot.y;
            rectTransform.position = pos;
        }
        
        if (rectTransform.position.y > safeArea.y + safeArea.height - originalSize.y * (1 - originalPivot.y))
        {
            Vector3 pos = rectTransform.position;
            pos.y = safeArea.y + safeArea.height - originalSize.y * (1 - originalPivot.y);
            rectTransform.position = pos;
        }
    }
} 