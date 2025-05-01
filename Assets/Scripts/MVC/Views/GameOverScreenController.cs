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
        
        // Stop enemy spawning and despawn any enemies
        EnsureEnemiesCleared();
    }
    
    private void OnEnable()
    {
        // Make sure enemies are cleared when this panel is enabled
        EnsureEnemiesCleared();
    }
    
    private void EnsureEnemiesCleared()
    {
        // Find the enemy spawner and stop/clear enemies
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.StopAndClearEnemies();
        }
        else
        {
            // Fallback: Directly destroy all enemies
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                Destroy(enemy);
            }
        }
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