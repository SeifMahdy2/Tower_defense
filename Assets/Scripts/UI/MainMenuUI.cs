using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject settingsPanel;
    
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button backToMainButton;
    [SerializeField] private Button backFromSettingsButton;
    
    [Header("Level Select")]
    [SerializeField] private Button[] levelButtons;
    
    [Header("Settings")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    
    private void Start()
    {
        // Show main panel by default
        ShowMainPanel();
        
        // Setup button listeners
        SetupButtons();
        
        // Setup settings
        LoadSettings();
    }
    
    private void SetupButtons()
    {
        // Main menu buttons
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitButtonClicked);
            
        // Level select back button
        if (backToMainButton != null)
            backToMainButton.onClick.AddListener(ShowMainPanel);
            
        // Settings back button
        if (backFromSettingsButton != null)
            backFromSettingsButton.onClick.AddListener(ShowMainPanel);
            
        // Level buttons
        SetupLevelButtons();
        
        // Settings controls
        SetupSettingsControls();
    }
    
    private void SetupLevelButtons()
    {
        if (levelButtons != null)
        {
            for (int i = 0; i < levelButtons.Length; i++)
            {
                int levelIndex = i + 1; // Level indices typically start at 1
                
                if (levelButtons[i] != null)
                {
                    levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex));
                }
            }
        }
    }
    
    private void SetupSettingsControls()
    {
        // Music volume slider
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        
        // SFX volume slider
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }
        
        // Fullscreen toggle
        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }
    }
    
    private void LoadSettings()
    {
        // Find SettingsManager
        SettingsManager settingsManager = SettingsManager.Instance;
        
        // Load values from SettingsManager if available
        if (settingsManager != null)
        {
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = settingsManager.GetMusicVolume();
            }
            
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = settingsManager.GetSFXVolume();
            }
            
            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = settingsManager.GetFullscreen();
            }
        }
        else
        {
            // Fallback to PlayerPrefs if SettingsManager not found
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
            }
            
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
            }
            
            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            }
        }
    }
    
    #region Button Handlers
    
    private void OnPlayButtonClicked()
    {
        ShowLevelSelectPanel();
    }
    
    private void OnSettingsButtonClicked()
    {
        ShowSettingsPanel();
    }
    
    private void OnQuitButtonClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    #endregion
    
    #region Panel Management
    
    private void ShowMainPanel()
    {
        // Hide all panels
        HideAllPanels();
        
        // Show main panel
        if (mainPanel != null)
        {
            mainPanel.SetActive(true);
        }
    }
    
    private void ShowLevelSelectPanel()
    {
        // Hide all panels
        HideAllPanels();
        
        // Show level select panel
        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(true);
        }
    }
    
    private void ShowSettingsPanel()
    {
        // Hide all panels
        HideAllPanels();
        
        // Show settings panel
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }
    
    private void HideAllPanels()
    {
        if (mainPanel != null)
            mainPanel.SetActive(false);
            
        if (levelSelectPanel != null)
            levelSelectPanel.SetActive(false);
            
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
    
    #endregion
    
    #region Settings
    
    private void SetMusicVolume(float volume)
    {
        // Use SettingsManager if available
        SettingsManager settingsManager = SettingsManager.Instance;
        if (settingsManager != null)
        {
            settingsManager.SetMusicVolume(volume);
        }
        else
        {
            // Fallback to PlayerPrefs
            PlayerPrefs.SetFloat("MusicVolume", volume);
            
            // Try to apply to AudioManager
            AudioManager audioManager = FindObjectOfType<AudioManager>();
            if (audioManager != null)
            {
                audioManager.SetMusicVolume(volume);
            }
        }
    }
    
    private void SetSFXVolume(float volume)
    {
        // Use SettingsManager if available
        SettingsManager settingsManager = SettingsManager.Instance;
        if (settingsManager != null)
        {
            settingsManager.SetSFXVolume(volume);
        }
        else
        {
            // Fallback to PlayerPrefs
            PlayerPrefs.SetFloat("SFXVolume", volume);
            
            // Try to apply to AudioManager
            AudioManager audioManager = FindObjectOfType<AudioManager>();
            if (audioManager != null)
            {
                audioManager.SetSFXVolume(volume);
            }
        }
    }
    
    private void SetFullscreen(bool isFullscreen)
    {
        // Use SettingsManager if available
        SettingsManager settingsManager = SettingsManager.Instance;
        if (settingsManager != null)
        {
            settingsManager.SetFullscreen(isFullscreen);
        }
        else
        {
            // Fallback to PlayerPrefs and direct application
            PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
            Screen.fullScreen = isFullscreen;
        }
    }
    
    #endregion
    
    #region Level Loading
    
    private void LoadLevel(int levelIndex)
    {
        // Load the level scene
        SceneManager.LoadScene("Level" + levelIndex);
    }
    
    #endregion
} 