using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryScreenController : MonoBehaviour
{
    [SerializeField] private Button homeButton;
    [SerializeField] private Button nextLevelButton;

    private void Start()
    {
        if (homeButton != null)
        {
            homeButton.onClick.AddListener(GoToMainMenu);
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(GoToNextLevel);
            
            // Disable next level button if we're already in Level_2
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == "Level_2")
            {
                nextLevelButton.gameObject.SetActive(false);
            }
        }
    }

    public void GoToMainMenu()
    {
        PlayButtonClickSound();
        SceneLoader.Instance.LoadScene("Main_Menu");
    }

    public void GoToNextLevel()
    {
        PlayButtonClickSound();
        string currentScene = SceneManager.GetActiveScene().name;
        
        if (currentScene == "Level_1")
        {
            SceneLoader.Instance.LoadScene("Level_2");
        }
        // No else case needed as button should be disabled for Level_2
    }
    
    private void PlayButtonClickSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
    }
} 