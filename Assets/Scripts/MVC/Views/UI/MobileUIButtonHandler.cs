using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TD
{
    /// <summary>
    /// Handles touch input for UI buttons on mobile platforms with improved touch response
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class MobileUIButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private float scaleOnPress = 0.95f;
        [SerializeField] private float scaleDuration = 0.1f;
        
        private Button button;
        private Vector3 originalScale;
        private bool isScaling = false;
        private float scaleProgress = 0f;
        private bool scaleDown = true;
        
        private void Awake()
        {
            button = GetComponent<Button>();
            originalScale = transform.localScale;
        }
        
        private void Update()
        {
            if (isScaling)
            {
                scaleProgress += Time.deltaTime / scaleDuration;
                
                if (scaleProgress >= 1f)
                {
                    scaleProgress = 1f;
                    isScaling = false;
                }
                
                float scaleMultiplier = scaleDown ? 
                    Mathf.Lerp(1f, scaleOnPress, scaleProgress) : 
                    Mathf.Lerp(scaleOnPress, 1f, scaleProgress);
                
                transform.localScale = originalScale * scaleMultiplier;
            }
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!button.interactable)
                return;
                
            // Start scaling down
            isScaling = true;
            scaleDown = true;
            scaleProgress = 0f;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!button.interactable)
                return;
                
            // Start scaling up
            isScaling = true;
            scaleDown = false;
            scaleProgress = 0f;
        }
    }
} 