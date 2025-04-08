using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TowerDefense.Controllers;

namespace TowerDefense.Views
{
    public class SettingsView : MonoBehaviour
    {
        [Header("Audio Settings")]
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private TextMeshProUGUI musicVolumeText;
        [SerializeField] private TextMeshProUGUI sfxVolumeText;
        
        [Header("Display Settings")]
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private TMP_Dropdown qualityDropdown;
        
        [Header("Buttons")]
        [SerializeField] private Button resetDefaultsButton;
        [SerializeField] private Button applyButton;
        
        // Controllers reference
        private SettingsController settingsController;
        
        private void Start()
        {
            // Get controller reference
            settingsController = SettingsController.Instance;
            
            // Load current settings
            LoadSettings();
            
            // Setup controls
            SetupControls();
            
            // Subscribe to settings changed event
            if (settingsController != null)
            {
                settingsController.OnSettingsChanged += UpdateSettingsUI;
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (settingsController != null)
            {
                settingsController.OnSettingsChanged -= UpdateSettingsUI;
            }
        }
        
        private void LoadSettings()
        {
            if (settingsController == null) return;
            
            // Set slider values
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = settingsController.GetMusicVolume();
            }
            
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = settingsController.GetSFXVolume();
            }
            
            // Set toggle values
            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = settingsController.GetFullscreen();
            }
            
            // Set dropdown values
            if (qualityDropdown != null)
            {
                qualityDropdown.ClearOptions();
                qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
                qualityDropdown.value = settingsController.GetQualityLevel();
            }
            
            // Update text displays
            UpdateSettingsUI();
        }
        
        private void SetupControls()
        {
            // Music volume slider
            if (musicVolumeSlider != null && settingsController != null)
            {
                musicVolumeSlider.onValueChanged.AddListener((value) => {
                    settingsController.SetMusicVolume(value);
                    UpdateVolumeText();
                });
            }
            
            // SFX volume slider
            if (sfxVolumeSlider != null && settingsController != null)
            {
                sfxVolumeSlider.onValueChanged.AddListener((value) => {
                    settingsController.SetSFXVolume(value);
                    UpdateVolumeText();
                });
            }
            
            // Fullscreen toggle
            if (fullscreenToggle != null && settingsController != null)
            {
                fullscreenToggle.onValueChanged.AddListener((value) => {
                    settingsController.SetFullscreen(value);
                });
            }
            
            // Quality dropdown
            if (qualityDropdown != null && settingsController != null)
            {
                qualityDropdown.onValueChanged.AddListener((value) => {
                    settingsController.SetQualityLevel(value);
                });
            }
            
            // Reset defaults button
            if (resetDefaultsButton != null && settingsController != null)
            {
                resetDefaultsButton.onClick.AddListener(() => {
                    settingsController.ResetToDefaults();
                    LoadSettings(); // Reload UI with default values
                });
            }
        }
        
        private void UpdateSettingsUI()
        {
            // Update volume text displays
            UpdateVolumeText();
        }
        
        private void UpdateVolumeText()
        {
            if (musicVolumeText != null && musicVolumeSlider != null)
            {
                int percentage = Mathf.RoundToInt(musicVolumeSlider.value * 100);
                musicVolumeText.text = percentage + "%";
            }
            
            if (sfxVolumeText != null && sfxVolumeSlider != null)
            {
                int percentage = Mathf.RoundToInt(sfxVolumeSlider.value * 100);
                sfxVolumeText.text = percentage + "%";
            }
        }
    }
} 