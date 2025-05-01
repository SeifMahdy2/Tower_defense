using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryScreenController : MonoBehaviour
{
    [SerializeField] private Button homeButton;
    [SerializeField] private Button nextLevelButton;

    private PlayerProgressManager progressManager;
    private string currentScene;
    private int currentLevel;

    private void Start()
    {
        progressManager = PlayerProgressManager.Instance;
        currentScene = SceneManager.GetActiveScene().name;
        
        Debug.Log("Victory Screen Started for scene: " + currentScene);
        
        // Extract level number from scene name
        if (currentScene.Contains("Level_"))
        {
            int.TryParse(currentScene.Substring(currentScene.IndexOf("_") + 1), out currentLevel);
            
            Debug.Log("Current Level: " + currentLevel);
            
            // Save level completion when victory screen appears
            SaveLevelCompletion(currentLevel);
        }

        if (homeButton != null)
        {
            homeButton.onClick.AddListener(GoToMainMenu);
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(GoToNextLevel);
            
            // Disable next level button if we're already in Level_2
            if (currentScene == "Level_2")
            {
                nextLevelButton.gameObject.SetActive(false);
            }
        }
        
        // Stop enemy spawning and despawn any enemies
        EnsureEnemiesCleared();
    }
    
    private void SaveLevelCompletion(int levelNumber)
    {
        if (progressManager != null)
        {
            Debug.Log("Saving completion for Level " + levelNumber + " via PlayerProgressManager");
            progressManager.SetLevelCompleted(levelNumber);
        }
        else
        {
            // Direct PlayerPrefs fallback
            Debug.Log("Saving completion for Level " + levelNumber + " via PlayerPrefs directly");
            PlayerPrefs.SetInt("Level_" + levelNumber + "_Completed", 1);
            PlayerPrefs.Save();
            
            // Extra verification for debugging
            int verifyValue = PlayerPrefs.GetInt("Level_" + levelNumber + "_Completed", 0);
            Debug.Log("Verified Level " + levelNumber + " completion status: " + verifyValue);
        }
        
        // Additional verification check
        Debug.Log("Verify Level " + levelNumber + " completion status in PlayerPrefs: " + 
            PlayerPrefs.GetInt("Level_" + levelNumber + "_Completed", 0));
            
        // Check if level 2 is now unlocked
        if (levelNumber == 1)
        {
            bool level2Unlocked = PlayerPrefs.GetInt("Level_1_Completed", 0) == 1;
            Debug.Log("After saving, Level 2 should be " + (level2Unlocked ? "UNLOCKED" : "still LOCKED"));
        }
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

    public void GoToMainMenu()
    {
        PlayButtonClickSound();
        
        // Reset time scale in case it was modified
        Time.timeScale = 1.0f;
        
        // Double-check that completion was saved
        if (currentLevel > 0)
        {
            SaveLevelCompletion(currentLevel);
        }
        
        // Pre-clean memory before loading
        Resources.UnloadUnusedAssets();
        
        // Load main menu scene
        LoadScene("Main_Menu");
    }

    public void GoToNextLevel()
    {
        PlayButtonClickSound();
        
        // Reset time scale in case it was modified
        Time.timeScale = 1.0f;
        
        if (currentScene == "Level_1")
        {
            // Double-check that level completion was saved
            int levelCompletionStatus = PlayerPrefs.GetInt("Level_1_Completed", 0);
            Debug.Log("Before loading Level 2, checking Level 1 completion: " + levelCompletionStatus);
            
            // Force save again if needed
            if (levelCompletionStatus != 1)
            {
                Debug.Log("Level 1 completion not found, saving again...");
                SaveLevelCompletion(1);
            }
            
            Debug.Log("Loading Level 2...");
            
            // Pre-clean memory before loading next level
            PreloadCleanup();
            
            // Level 2 should be unlocked at this point since we've won Level 1
            LoadScene("Level_2");
        }
    }
    
    private void LoadScene(string sceneName)
    {
        // First try with SceneLoader if available
        if (SceneLoader.Instance != null)
        {
            Debug.Log($"Loading {sceneName} via SceneLoader");
            SceneLoader.Instance.LoadScene(sceneName);
        }
        else
        {
            // Fallback to direct scene loading
            Debug.Log($"Loading {sceneName} via direct SceneManager");
            SceneManager.LoadScene(sceneName);
        }
    }
    
    private void PreloadCleanup()
    {
        // Clean up any remaining enemies
        EnsureEnemiesCleared();
        
        // Remove the tower cleanup code since the Tower tag doesn't exist
        // Towers will be destroyed when the scene is unloaded anyway
        
        // Trigger garbage collection
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        
        Debug.Log("Scene cleanup completed, ready to load next level");
    }
    
    private void PlayButtonClickSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
    }
} 