using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    [SerializeField] private Slider mainVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;

    private void Start()
    {
        // Load saved values or set default
        float mainVol = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);

        mainVolumeSlider.value = mainVol;
        musicVolumeSlider.value = musicVol;

        SetMasterVolume(mainVol);
        SetMusicVolume(musicVol);

        // Add listeners for value changes
        mainVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        
        // Add listeners for pointer up to play click sound when slider handle is released
        AddSliderSoundListeners(mainVolumeSlider);
        AddSliderSoundListeners(musicVolumeSlider);
    }

    private void AddSliderSoundListeners(Slider slider)
    {
        // Get the slider handle
        if (slider != null)
        {
            // Add event triggers to play sound when interacting with slider
            var sliderHandle = slider.transform.Find("Handle Slide Area/Handle");
            if (sliderHandle != null)
            {
                var eventTrigger = sliderHandle.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
                var pointerUpEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
                pointerUpEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
                pointerUpEntry.callback.AddListener((data) => { PlayButtonClickSound(); });
                eventTrigger.triggers.Add(pointerUpEntry);
            }
        }
    }

    public void SetMasterVolume(float value)
    {
        AudioManager.Instance.SetMasterVolume(value);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }
    
    private void PlayButtonClickSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
    }
}
