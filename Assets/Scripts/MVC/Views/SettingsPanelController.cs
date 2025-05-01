using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsPanelController : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Button logoutButton;
    
    [Tooltip("The name of the login scene to load after logout")]
    [SerializeField] private string loginSceneName = "LoginScene";
    
    private void Start()
    {
        // Initialize sliders with current values
        InitializeSliders();
        
        // Add listeners to update volume when sliders change
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }
        
        // Set up logout button
        if (logoutButton != null)
        {
            logoutButton.onClick.AddListener(LogoutUser);
        }
        else
        {
            Debug.LogWarning("[SettingsPanelController] Logout button not assigned in Inspector");
        }
    }
    
    private void OnEnable()
    {
        // Initialize sliders whenever the settings panel is shown
        InitializeSliders();
        
        // Force update volume to ensure it's active
        // This helps if the volume was muted for some reason
        if (masterVolumeSlider != null)
        {
            SetMasterVolume(masterVolumeSlider.value);
        }
    }
    
    private void InitializeSliders()
    {
        if (AudioManager.Instance == null) return;
        
        // Get the AudioMixer
        var audioMixer = AudioManager.Instance.GetAudioMixer();
        if (audioMixer == null) return;
        
        Debug.Log("[SettingsPanelController] Initializing volume slider");
        
        // Master Volume
        if (masterVolumeSlider != null)
        {
            float dbValue;
            if (audioMixer.GetFloat("MasterVolume", out dbValue))
            {
                // Convert from decibels (-80 to 0) back to slider range (0 to 1)
                // Handle the case where value is -80 (silent) by setting slider to 0
                float sliderValue;
                if (dbValue <= -80f)
                    sliderValue = 0f;
                else
                    sliderValue = Mathf.Pow(10, dbValue / 20);
                    
                masterVolumeSlider.value = sliderValue;
                Debug.Log($"[SettingsPanelController] Master slider initialized to {sliderValue} from {dbValue} dB");
                
                // Immediately apply the volume to ensure it's active
                SetMasterVolume(sliderValue);
            }
            else
            {
                // Default to 75% volume if no value can be read
                float defaultValue = 0.75f;
                masterVolumeSlider.value = defaultValue;
                SetMasterVolume(defaultValue);
                Debug.LogWarning($"[SettingsPanelController] Could not get master volume from audio mixer. Setting to {defaultValue}.");
            }
        }
    }
    
    public void SetMasterVolume(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(value);
            
            // Additional direct control of AudioListener as fallback
            // This directly affects all audio in the scene
            AudioListener.volume = value;
        }
    }
    
    public void CloseSettings()
    {
        // Play button sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSound();
        }
        
        // Hide the panel
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Logs out the current user by clearing credentials and returning to login screen
    /// </summary>
    public void LogoutUser()
    {
        Debug.Log("[SettingsPanelController] Logging out user");
        
        // Play button sound if available
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSound();
        }
        
        // Clear current user credentials
        PlayerPrefs.DeleteKey("CurrentUsername");
        PlayerPrefs.DeleteKey("CurrentPassword");
        
        // Also clear auto-login flag if you have one
        PlayerPrefs.SetInt("AutoLogin", 0);
        
        // Save the changes
        PlayerPrefs.Save();
        
        // Return to login scene
        if (string.IsNullOrEmpty(loginSceneName))
        {
            Debug.LogError("[SettingsPanelController] Login scene name not set!");
            return;
        }
        
        // Load the login scene
        SceneManager.LoadScene(loginSceneName);
    }
}