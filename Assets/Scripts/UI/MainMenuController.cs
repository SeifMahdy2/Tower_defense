using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    public void OnClickPlayButton()
    {
        PlayButtonClickSound();
        SceneLoader.Instance.LoadScene("Level_1");
    }

    public void OnClickQuitButton()
    {
        PlayButtonClickSound();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OnClickSettingsButton()
    {
        PlayButtonClickSound();
        settingsPanel.SetActive(true);
    }

    public void OnClickCloseSettings()
    {
        PlayButtonClickSound();
        settingsPanel.SetActive(false);
    }
    
    private void PlayButtonClickSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
    }
}
