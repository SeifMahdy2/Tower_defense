using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        public bool loop = false;
        
        [HideInInspector]
        public AudioSource source;
    }
    
    [Header("Sounds")]
    [SerializeField] private Sound[] gameSounds;
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Music")]
    [SerializeField] private AudioClip[] backgroundMusic;
    
    // Singleton instance
    public static AudioManager Instance { get; private set; }
    
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
        
        // Initialize sound sources
        InitializeSounds();
        
        // Load saved volume settings
        LoadVolumeSettings();
    }
    
    private void Start()
    {
        // Start playing background music
        PlayRandomBackgroundMusic();
    }
    
    private void InitializeSounds()
    {
        // Create AudioSource components for each sound
        foreach (Sound sound in gameSounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.playOnAwake = false;
        }
    }
    
    private void LoadVolumeSettings()
    {
        // Load saved volume settings
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        
        // Apply volume settings
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }
    
    #region Public Methods
    
    public void PlaySound(string name)
    {
        // Find the requested sound
        Sound sound = System.Array.Find(gameSounds, s => s.name == name);
        
        if (sound != null)
        {
            sound.source.Play();
        }
        else
        {
            Debug.LogWarning("Sound " + name + " not found!");
        }
    }
    
    public void PlaySoundOneShot(string name)
    {
        // Find the requested sound
        Sound sound = System.Array.Find(gameSounds, s => s.name == name);
        
        if (sound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(sound.clip, sound.volume);
        }
        else
        {
            Debug.LogWarning("Sound " + name + " not found or sfxSource is null!");
        }
    }
    
    public void StopSound(string name)
    {
        // Find the requested sound
        Sound sound = System.Array.Find(gameSounds, s => s.name == name);
        
        if (sound != null)
        {
            sound.source.Stop();
        }
        else
        {
            Debug.LogWarning("Sound " + name + " not found!");
        }
    }
    
    public void PlayRandomBackgroundMusic()
    {
        if (musicSource != null && backgroundMusic.Length > 0)
        {
            // Pick a random music track
            int randomIndex = Random.Range(0, backgroundMusic.Length);
            
            // Set the music clip
            musicSource.clip = backgroundMusic[randomIndex];
            
            // Start playing
            musicSource.Play();
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        // Clamp volume between 0 and 1
        volume = Mathf.Clamp01(volume);
        
        // Apply to music source
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
        
        // Save setting
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
    
    public void SetSFXVolume(float volume)
    {
        // Clamp volume between 0 and 1
        volume = Mathf.Clamp01(volume);
        
        // Apply to SFX source
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
        
        // Apply to all sound effects
        foreach (Sound sound in gameSounds)
        {
            if (sound.source != null)
            {
                sound.source.volume = sound.volume * volume;
            }
        }
        
        // Save setting
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
    
    public void PauseAllAudio(bool pause)
    {
        if (pause)
        {
            if (musicSource != null && musicSource.isPlaying)
            {
                musicSource.Pause();
            }
            
            foreach (Sound sound in gameSounds)
            {
                if (sound.source != null && sound.source.isPlaying)
                {
                    sound.source.Pause();
                }
            }
        }
        else
        {
            if (musicSource != null)
            {
                musicSource.UnPause();
            }
            
            foreach (Sound sound in gameSounds)
            {
                if (sound.source != null)
                {
                    sound.source.UnPause();
                }
            }
        }
    }
    
    #endregion
} 