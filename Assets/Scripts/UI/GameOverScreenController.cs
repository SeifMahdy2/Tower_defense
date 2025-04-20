using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScreenController : MonoBehaviour
{
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(RetryLevel);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }
        
        // Reset normal time scale since GameManager sets it to 0.5 on game over
        Time.timeScale = 1f;
    }

    public void RetryLevel()
    {
        PlayButtonClickSound();
        // Get the current scene name and reload it
        string currentScene = SceneManager.GetActiveScene().name;
        SceneLoader.Instance.LoadScene(currentScene);
    }

    public void GoToMainMenu()
    {
        PlayButtonClickSound();
        SceneLoader.Instance.LoadScene("Main_Menu");
    }
    
    private void PlayButtonClickSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
    }
} 