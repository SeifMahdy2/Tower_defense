using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    [Header("Text Colors")]
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private Color goldColor = Color.yellow;
    [SerializeField] private Color healColor = Color.green;
    
    // Singleton instance
    public static EffectsManager Instance { get; private set; }
    
    private void Awake()
    {
        // Setup singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    // Show damage number text
    public void ShowDamageText(Vector3 position, int damage)
    {
        string text = "-" + damage.ToString();
        FloatingText.Create(position, text, damageColor);
    }
    
    // Show gold earned text
    public void ShowGoldText(Vector3 position, int gold)
    {
        string text = "+" + gold.ToString() + " gold";
        FloatingText.Create(position, text, goldColor);
    }
    
    // Show heal text
    public void ShowHealText(Vector3 position, int heal)
    {
        string text = "+" + heal.ToString();
        FloatingText.Create(position, text, healColor);
    }
    
    // Play a particle effect at position
    public void PlayEffect(GameObject effectPrefab, Vector3 position)
    {
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
            
            // Auto-destroy after 2 seconds to prevent clutter
            Destroy(effect, 2f);
        }
    }
} 