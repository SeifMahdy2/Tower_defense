using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Stats")]
    [SerializeField] private int lives = 10;
    [SerializeField] private int gold = 100;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI goldText;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void EnemyReachedEnd()
    {
        lives--;
        UpdateUI();
        
        if (lives <= 0)
        {
            GameOver();
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateUI();
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    private void UpdateUI()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives;
        }
        
        if (goldText != null)
        {
            goldText.text = "Gold: " + gold;
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        // Implement game over logic here
        // e.g., show game over screen, restart level, etc.
    }
} 