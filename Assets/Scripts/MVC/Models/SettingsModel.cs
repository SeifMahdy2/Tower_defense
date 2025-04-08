using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Models
{
    [System.Serializable]
    public class SettingsModel
    {
        // Audio settings
        public float MusicVolume { get; private set; }
        public float SFXVolume { get; private set; }
        
        // Display settings
        public bool Fullscreen { get; private set; }
        public int QualityLevel { get; private set; }
        
        // Default settings
        private float defaultMusicVolume = 0.75f;
        private float defaultSFXVolume = 0.75f;
        private bool defaultFullscreen = true;
        private int defaultQualityLevel = 2; // Medium
        
        // Constructor with default values
        public SettingsModel()
        {
            // Load saved settings or use defaults
            MusicVolume = PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume);
            SFXVolume = PlayerPrefs.GetFloat("SFXVolume", defaultSFXVolume);
            Fullscreen = PlayerPrefs.GetInt("Fullscreen", defaultFullscreen ? 1 : 0) == 1;
            QualityLevel = PlayerPrefs.GetInt("QualityLevel", defaultQualityLevel);
        }
        
        // Constructor with custom defaults
        public SettingsModel(float musicVolume, float sfxVolume, bool fullscreen, int qualityLevel)
        {
            defaultMusicVolume = musicVolume;
            defaultSFXVolume = sfxVolume;
            defaultFullscreen = fullscreen;
            defaultQualityLevel = qualityLevel;
            
            // Load saved settings or use provided defaults
            MusicVolume = PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume);
            SFXVolume = PlayerPrefs.GetFloat("SFXVolume", defaultSFXVolume);
            Fullscreen = PlayerPrefs.GetInt("Fullscreen", defaultFullscreen ? 1 : 0) == 1;
            QualityLevel = PlayerPrefs.GetInt("QualityLevel", defaultQualityLevel);
        }
        
        // Methods to change settings
        public void SetMusicVolume(float volume)
        {
            // Clamp value
            volume = Mathf.Clamp01(volume);
            
            // Set and save
            MusicVolume = volume;
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }
        
        public void SetSFXVolume(float volume)
        {
            // Clamp value
            volume = Mathf.Clamp01(volume);
            
            // Set and save
            SFXVolume = volume;
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }
        
        public void SetFullscreen(bool isFullscreen)
        {
            // Set and save
            Fullscreen = isFullscreen;
            PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        }
        
        public void SetQualityLevel(int level)
        {
            // Clamp value to valid range
            level = Mathf.Clamp(level, 0, QualitySettings.names.Length - 1);
            
            // Set and save
            QualityLevel = level;
            PlayerPrefs.SetInt("QualityLevel", level);
        }
        
        // Reset settings to defaults
        public void ResetToDefaults()
        {
            SetMusicVolume(defaultMusicVolume);
            SetSFXVolume(defaultSFXVolume);
            SetFullscreen(defaultFullscreen);
            SetQualityLevel(defaultQualityLevel);
        }
    }
} 