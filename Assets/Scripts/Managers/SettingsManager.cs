using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    // Default settings
    [Header("Default Settings")]
    [SerializeField] private float defaultMusicVolume = 0.75f;
    [SerializeField] private float defaultSFXVolume = 0.75f;
    [SerializeField] private bool defaultFullscreen = true;
    [SerializeField] private int defaultQualityLevel = 2; // Medium
    
    // Current settings
    private float musicVolume;
    private float sfxVolume;
    private bool fullscreen;
    private int qualityLevel;
    
    // Singleton instance
    public static SettingsManager Instance { get; private set; }
    
    private void Awake()
    {
        // Setup singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void LoadSettings()
    {
        // Load saved settings or use defaults
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", defaultSFXVolume);
        fullscreen = PlayerPrefs.GetInt("Fullscreen", defaultFullscreen ? 1 : 0) == 1;
        qualityLevel = PlayerPrefs.GetInt("QualityLevel", defaultQualityLevel);
        
        // Apply loaded settings
        ApplySettings();
    }
    
    private void ApplySettings()
    {
        // Apply audio settings
        ApplyAudioSettings();
        
        // Apply display settings
        ApplyDisplaySettings();
        
        // Apply quality settings
        ApplyQualitySettings();
    }
    
    private void ApplyAudioSettings()
    {
        // Find audio manager to apply settings
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.SetMusicVolume(musicVolume);
            audioManager.SetSFXVolume(sfxVolume);
        }
    }
    
    private void ApplyDisplaySettings()
    {
        // Apply fullscreen
        Screen.fullScreen = fullscreen;
    }
    
    private void ApplyQualitySettings()
    {
        // Apply quality level if within range
        if (qualityLevel >= 0 && qualityLevel < QualitySettings.names.Length)
        {
            QualitySettings.SetQualityLevel(qualityLevel, true);
        }
    }
    
    // Public methods to change settings
    
    public void SetMusicVolume(float volume)
    {
        // Clamp value
        volume = Mathf.Clamp01(volume);
        
        // Set and save
        musicVolume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        
        // Apply
        ApplyAudioSettings();
    }
    
    public void SetSFXVolume(float volume)
    {
        // Clamp value
        volume = Mathf.Clamp01(volume);
        
        // Set and save
        sfxVolume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
        
        // Apply
        ApplyAudioSettings();
    }
    
    public void SetFullscreen(bool isFullscreen)
    {
        // Set and save
        fullscreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        
        // Apply
        ApplyDisplaySettings();
    }
    
    public void SetQualityLevel(int level)
    {
        // Clamp value to valid range
        level = Mathf.Clamp(level, 0, QualitySettings.names.Length - 1);
        
        // Set and save
        qualityLevel = level;
        PlayerPrefs.SetInt("QualityLevel", level);
        
        // Apply
        ApplyQualitySettings();
    }
    
    // Getters for current settings
    
    public float GetMusicVolume()
    {
        return musicVolume;
    }
    
    public float GetSFXVolume()
    {
        return sfxVolume;
    }
    
    public bool GetFullscreen()
    {
        return fullscreen;
    }
    
    public int GetQualityLevel()
    {
        return qualityLevel;
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