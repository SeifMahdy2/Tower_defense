using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryPanelUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI enemiesDefeatedText;
    [SerializeField] private TextMeshProUGUI goldEarnedText;
    [SerializeField] private TextMeshProUGUI timeElapsedText;
    
    private LevelManager levelManager;
    private GameManager gameManager;
    private float levelStartTime;
    
    private void Awake()
    {
        // Record level start time
        levelStartTime = Time.time;
    }
    
    private void OnEnable()
    {
        // Get references when the victory panel becomes active
        levelManager = FindObjectOfType<LevelManager>();
        gameManager = FindObjectOfType<GameManager>();
        
        // Pause game
        Time.timeScale = 0f;
        
        // Setup buttons and UI
        SetupUI();
        SetupButtons();
    }
    
    private void SetupUI()
    {
        // Show enemies defeated
        if (enemiesDefeatedText != null && gameManager != null)
        {
            enemiesDefeatedText.text = "Enemies Defeated: " + gameManager.GetEnemiesDefeated();
        }
        
        // Show gold earned (this would need to be tracked in GameManager)
        if (goldEarnedText != null && gameManager != null)
        {
            goldEarnedText.text = "Gold Earned: " + gameManager.GetTotalGoldEarned();
        }
        
        // Show time elapsed
        if (timeElapsedText != null)
        {
            float timeElapsed = Time.time - levelStartTime;
            int minutes = Mathf.FloorToInt(timeElapsed / 60f);
            int seconds = Mathf.FloorToInt(timeElapsed % 60f);
            timeElapsedText.text = "Time: " + minutes.ToString("00") + ":" + seconds.ToString("00");
        }
    }
    
    private void SetupButtons()
    {
        // Next level button
        if (nextLevelButton != null && levelManager != null)
        {
            nextLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.AddListener(() => {
                Time.timeScale = 1f;
                levelManager.LoadNextLevel();
            });
        }
        
        // Main menu button
        if (mainMenuButton != null && levelManager != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(() => {
                Time.timeScale = 1f;
                levelManager.ReturnToMainMenu();
            });
        }
    }
} 