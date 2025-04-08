using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefense.Models;

namespace TowerDefense.Controllers
{
    public class SettingsController : MonoBehaviour
    {
        [Header("Default Settings")]
        [SerializeField] private float defaultMusicVolume = 0.75f;
        [SerializeField] private float defaultSFXVolume = 0.75f;
        [SerializeField] private bool defaultFullscreen = true;
        [SerializeField] private int defaultQualityLevel = 2; // Medium
        
        // Models
        private SettingsModel settingsModel;
        
        // Singleton instance
        public static SettingsController Instance { get; private set; }
        
        // Events
        public delegate void SettingsChanged();
        public event SettingsChanged OnSettingsChanged;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            // Initialize model
            settingsModel = new SettingsModel(defaultMusicVolume, defaultSFXVolume, defaultFullscreen, defaultQualityLevel);
            
            // Apply settings on startup
            ApplySettings();
        }
        
        // Apply all settings
        private void ApplySettings()
        {
            // Apply audio settings
            ApplyAudioSettings();
            
            // Apply display settings
            ApplyDisplaySettings();
            
            // Apply quality settings
            ApplyQualitySettings();
            
            // Notify listeners
            if (OnSettingsChanged != null)
            {
                OnSettingsChanged();
            }
        }
        
        private void ApplyAudioSettings()
        {
            // Find audio manager to apply settings
            AudioManager audioManager = FindObjectOfType<AudioManager>();
            if (audioManager != null)
            {
                audioManager.SetMusicVolume(settingsModel.MusicVolume);
                audioManager.SetSFXVolume(settingsModel.SFXVolume);
            }
        }
        
        private void ApplyDisplaySettings()
        {
            // Apply fullscreen
            Screen.fullScreen = settingsModel.Fullscreen;
        }
        
        private void ApplyQualitySettings()
        {
            // Apply quality level if within range
            int qualityLevel = settingsModel.QualityLevel;
            if (qualityLevel >= 0 && qualityLevel < QualitySettings.names.Length)
            {
                QualitySettings.SetQualityLevel(qualityLevel, true);
            }
        }
        
        // Public methods to change settings
        
        public void SetMusicVolume(float volume)
        {
            settingsModel.SetMusicVolume(volume);
            ApplyAudioSettings();
            
            if (OnSettingsChanged != null)
            {
                OnSettingsChanged();
            }
        }
        
        public void SetSFXVolume(float volume)
        {
            settingsModel.SetSFXVolume(volume);
            ApplyAudioSettings();
            
            if (OnSettingsChanged != null)
            {
                OnSettingsChanged();
            }
        }
        
        public void SetFullscreen(bool isFullscreen)
        {
            settingsModel.SetFullscreen(isFullscreen);
            ApplyDisplaySettings();
            
            if (OnSettingsChanged != null)
            {
                OnSettingsChanged();
            }
        }
        
        public void SetQualityLevel(int level)
        {
            settingsModel.SetQualityLevel(level);
            ApplyQualitySettings();
            
            if (OnSettingsChanged != null)
            {
                OnSettingsChanged();
            }
        }
        
        // Reset settings to defaults
        public void ResetToDefaults()
        {
            settingsModel.ResetToDefaults();
            ApplySettings();
        }
        
        // Getters for model properties
        
        public float GetMusicVolume()
        {
            return settingsModel.MusicVolume;
        }
        
        public float GetSFXVolume()
        {
            return settingsModel.SFXVolume;
        }
        
        public bool GetFullscreen()
        {
            return settingsModel.Fullscreen;
        }
        
        public int GetQualityLevel()
        {
            return settingsModel.QualityLevel;
        }
    }
} 