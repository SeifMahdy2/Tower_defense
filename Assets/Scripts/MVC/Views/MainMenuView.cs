using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TowerDefense.Controllers;

namespace TowerDefense.Views
{
    public class MainMenuView : MonoBehaviour
    {
        [Header("Menu Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject levelSelectPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject creditsPanel;
        
        [Header("Main Menu Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;
        
        [Header("Level Select")]
        [SerializeField] private Button[] levelButtons;
        [SerializeField] private Button backFromLevelSelectButton;
        
        [Header("Other Buttons")]
        [SerializeField] private Button backFromSettingsButton;
        [SerializeField] private Button backFromCreditsButton;
        
        // Controllers reference
        private LevelController levelController;
        private SettingsController settingsController;
        
        private void Start()
        {
            // Get controller references
            levelController = LevelController.Instance;
            settingsController = SettingsController.Instance;
            
            // Show only main panel initially
            ShowMainPanel();
            
            // Setup button listeners
            SetupButtons();
            
            // Setup level select buttons
            SetupLevelButtons();
        }
        
        private void SetupButtons()
        {
            // Main menu buttons
            if (playButton != null)
            {
                playButton.onClick.AddListener(() => {
                    ShowPanel(levelSelectPanel);
                });
            }
            
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(() => {
                    ShowPanel(settingsPanel);
                });
            }
            
            if (creditsButton != null)
            {
                creditsButton.onClick.AddListener(() => {
                    ShowPanel(creditsPanel);
                });
            }
            
            if (quitButton != null)
            {
                quitButton.onClick.AddListener(() => {
                    QuitGame();
                });
            }
            
            // Back buttons
            if (backFromLevelSelectButton != null)
            {
                backFromLevelSelectButton.onClick.AddListener(() => {
                    ShowMainPanel();
                });
            }
            
            if (backFromSettingsButton != null)
            {
                backFromSettingsButton.onClick.AddListener(() => {
                    ShowMainPanel();
                });
            }
            
            if (backFromCreditsButton != null)
            {
                backFromCreditsButton.onClick.AddListener(() => {
                    ShowMainPanel();
                });
            }
        }
        
        private void SetupLevelButtons()
        {
            if (levelButtons == null || levelController == null) return;
            
            // Get unlocked levels
            List<int> unlockedLevels = levelController.GetUnlockedLevels();
            
            // Setup each level button
            for (int i = 0; i < levelButtons.Length; i++)
            {
                if (levelButtons[i] == null) continue;
                
                // Final index needs to be captured for the lambda
                int levelIndex = i + 1; // Level indices usually start from 1
                
                // Check if level is unlocked
                bool isUnlocked = unlockedLevels.Contains(levelIndex);
                
                // Set button interactable state
                levelButtons[i].interactable = isUnlocked;
                
                // Setup button click
                levelButtons[i].onClick.AddListener(() => {
                    if (levelController != null)
                    {
                        levelController.LoadLevel(levelIndex);
                    }
                });
                
                // Optional: Add visual indicators for locked/unlocked levels
                // If your buttons have child objects like locks or stars, you can set them here
            }
        }
        
        private void ShowMainPanel()
        {
            ShowPanel(mainPanel);
        }
        
        private void ShowPanel(GameObject panelToShow)
        {
            // Hide all panels
            if (mainPanel != null) mainPanel.SetActive(false);
            if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (creditsPanel != null) creditsPanel.SetActive(false);
            
            // Show the requested panel
            if (panelToShow != null)
            {
                panelToShow.SetActive(true);
            }
        }
        
        private void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        // Update method if needed for dynamic content
        private void Update()
        {
            // Any dynamic updates can go here
        }
        
        // Method to refresh level buttons (call this if level unlock state changes)
        public void RefreshLevelButtons()
        {
            SetupLevelButtons();
        }
    }
} 