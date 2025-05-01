using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [SerializeField] private AudioMixer audioMixer;
    
    [Header("Audio Sources")]
    private AudioSource musicAudioSource;
    private AudioSource sfxAudioSource;
    private AudioSource uiAudioSource;
    
    [Header("Audio Clips")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip buttonSound;
    
    [Header("Tower Sounds")]
    [SerializeField] private AudioClip archerSound;
    [SerializeField] private AudioClip mageSound;
    [SerializeField] private AudioClip cannonSound;
    [SerializeField] private AudioClip frostSound;
    [SerializeField] private AudioClip upgradeSound;
    
    private const string MASTER_VOLUME = "MasterVolume";
    
    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudio();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void SetupAudio()
    {
        // Create audio sources with different default settings
        SetupAudioSources();
        
        // Connect sources to mixer
        ConnectAudioSourcesToMixer();
        
        // Set volumes
        LoadSavedVolumes();
        
        // Force AudioListener to max
        AudioListener.volume = 1f;
    }
    
    private void SetupAudioSources()
    {
        // Music source - looping, lower volume
        musicAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource.playOnAwake = false;
        musicAudioSource.loop = true;
        musicAudioSource.volume = 0.7f;
        
        // Sound effects - non-looping, higher volume
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        sfxAudioSource.playOnAwake = false;
        sfxAudioSource.loop = false;
        sfxAudioSource.volume = 0.8f;
        
        // UI sounds - non-looping, medium volume
        uiAudioSource = gameObject.AddComponent<AudioSource>();
        uiAudioSource.playOnAwake = false;
        uiAudioSource.loop = false;
        uiAudioSource.volume = 0.7f;
    }
    
    private void ConnectAudioSourcesToMixer()
    {
        if (audioMixer == null)
            return;
            
        var masterGroup = audioMixer.FindMatchingGroups("Master");
        if (masterGroup.Length > 0)
        {
            var group = masterGroup[0];
            musicAudioSource.outputAudioMixerGroup = group;
            sfxAudioSource.outputAudioMixerGroup = group;
            uiAudioSource.outputAudioMixerGroup = group;
        }
    }
    
    private void LoadSavedVolumes()
    {
        // Default volume is 75% (-2.5dB)
        float defaultVolume = -2.5f;
        
        // Get saved volume or use default
        float masterVolume = PlayerPrefs.HasKey(MASTER_VOLUME) ? 
                             PlayerPrefs.GetFloat(MASTER_VOLUME) : 
                             defaultVolume;
        
        // Save default if not exists
        if (!PlayerPrefs.HasKey(MASTER_VOLUME))
            PlayerPrefs.SetFloat(MASTER_VOLUME, defaultVolume);
        
        // Apply volume
        audioMixer.SetFloat(MASTER_VOLUME, masterVolume);
        PlayerPrefs.Save();
    }
    
    private float ConvertToDecibel(float sliderValue)
    {
        // Convert 0-1 range to logarithmic scale for better volume control
        if (sliderValue <= 0.001f)
            return -80f; // Complete mute
            
        // Logarithmic curve for more natural volume scaling
        return Mathf.Log10(Mathf.Lerp(0.001f, 1f, sliderValue)) * 20f;
    }
    
    public void SetMasterVolume(float sliderValue)
    {
        // Convert slider value (0-1) to decibels (-80 to 0)
        float dbValue = ConvertToDecibel(sliderValue);
        
        // Apply to mixer
        audioMixer.SetFloat(MASTER_VOLUME, dbValue);
        
        // Save setting
        PlayerPrefs.SetFloat(MASTER_VOLUME, dbValue);
        PlayerPrefs.Save();
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Play appropriate music based on scene
        if (scene.name == "Main_Menu")
            PlayMusic(mainMenuMusic);
        else if (scene.name.Contains("Level_") || scene.name.Contains("Game"))
            PlayMusic(gameplayMusic);
    }
    
    public void PlayMusic(AudioClip music)
    {
        if (music == null) 
            return;
        
        // Don't restart the same music
        if (musicAudioSource.clip == music && musicAudioSource.isPlaying)
            return;
            
        musicAudioSource.clip = music;
        musicAudioSource.Play();
    }
    
    public void PlayButtonSound()
    {
        if (buttonSound != null)
            uiAudioSource.PlayOneShot(buttonSound);
    }
    
    public void PlayButtonClickSound()
    {
        PlayButtonSound();
    }
    
    public void PlayTowerSound(string towerType)
    {
        AudioClip sound = GetTowerSound(towerType);
        if (sound != null)
            sfxAudioSource.PlayOneShot(sound);
    }
    
    public void PlayTowerShootSound(string towerType, Vector3 position)
    {
        AudioClip sound = GetTowerSound(towerType);
        if (sound != null)
            AudioSource.PlayClipAtPoint(sound, position, 0.9f);
    }
    
    public void PlayTowerUpgradeSound(Vector3 position)
    {
        if (upgradeSound != null && sfxAudioSource != null)
            AudioSource.PlayClipAtPoint(upgradeSound, position, sfxAudioSource.volume);
    }
    
    private AudioClip GetTowerSound(string towerType)
    {
        if (string.IsNullOrEmpty(towerType))
            return null;
        
        switch (towerType.ToLower())
        {
            case "archer": return archerSound;
            case "mage": return mageSound;
            case "cannon": return cannonSound;
            case "frost": return frostSound;
            default: return null;
        }
    }
    
    public AudioMixer GetAudioMixer()
    {
        return audioMixer;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
#if UNITY_EDITOR
    // Testing methods only available in editor
    public void TestAudioSystem()
    {
        // Test volume levels
        for (float vol = 0f; vol <= 1f; vol += 0.25f)
        {
            SetMasterVolume(vol);
            if (buttonSound != null)
                uiAudioSource.PlayOneShot(buttonSound);
        }
        
        // Reset to mid-level
        SetMasterVolume(0.75f);
        
        // Test tower sounds
        if (archerSound) AudioSource.PlayClipAtPoint(archerSound, Vector3.zero);
        if (mageSound) AudioSource.PlayClipAtPoint(mageSound, Vector3.zero);
        if (cannonSound) AudioSource.PlayClipAtPoint(cannonSound, Vector3.zero);
        if (frostSound) AudioSource.PlayClipAtPoint(frostSound, Vector3.zero);
    }
    
    public void ResetAudioPrefs()
    {
        float defaultVolume = -2.5f;
        
        PlayerPrefs.DeleteKey(MASTER_VOLUME);
        PlayerPrefs.Save();
        
        audioMixer.SetFloat(MASTER_VOLUME, defaultVolume);
        PlayerPrefs.SetFloat(MASTER_VOLUME, defaultVolume);
        
        AudioListener.volume = 1.0f;
    }
#endif
}