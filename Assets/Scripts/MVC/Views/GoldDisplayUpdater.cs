using UnityEngine;
using TMPro;
using TD;

// Attach this script directly to the Gold_Overall text object
public class GoldDisplayUpdater : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    private GameManager gameManager;
    private int lastKnownGold = -1;
    
    void Start()
    {
        // Get the text component
        textComponent = GetComponent<TextMeshProUGUI>();
        if (textComponent == null)
        {
            Debug.LogError("GoldDisplayUpdater: No TextMeshProUGUI component found on this object!");
            enabled = false;
            return;
        }
        
        // Find the GameManager
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GoldDisplayUpdater: No GameManager found in the scene!");
            enabled = false;
            return;
        }
        
        Debug.Log("GoldDisplayUpdater initialized on " + gameObject.name);
        
        // Force initial update
        UpdateText();
    }
    
    void Update()
    {
        UpdateText();
    }
    
    void LateUpdate()
    {
        // Make absolutely sure the text is updated in LateUpdate too
        UpdateText();
    }
    
    void UpdateText()
    {
        if (textComponent != null && gameManager != null)
        {
            int currentGold = gameManager.GetCurrentGold();
            
            // Only update if the gold amount has changed
            if (currentGold != lastKnownGold)
            {
                textComponent.text = currentGold.ToString();
                // Force the mesh to update
                textComponent.ForceMeshUpdate();
                
                lastKnownGold = currentGold;
                Debug.Log("GoldDisplayUpdater: Updated gold display to " + currentGold);
            }
        }
    }
} 