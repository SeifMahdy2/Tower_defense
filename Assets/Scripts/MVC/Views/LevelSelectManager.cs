using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Level References")]
    [SerializeField] private GameObject[] levelButtons;
    [SerializeField] private GameObject[] lockedPanels;
    
    [Header("Level Names")]
    [SerializeField] private string[] levelSceneNames;
    
    private PlayerProgressManager progressManager;
    private ApiManager apiManager;
    
    private void OnEnable()
    {
        // Refresh UI every time the panel becomes active
        StartCoroutine(RefreshLevelStatusWithServer());
    }
    
    private void Start()
    {
        // Find required managers
        progressManager = PlayerProgressManager.Instance;
        apiManager = ApiManager.Instance;
        
        // Refresh UI on startup with server data
        StartCoroutine(RefreshLevelStatusWithServer());
        
        // Debug the current status
        DebugLevelStatus();
    }
    
    /// <summary>
    /// Check player progress and update UI accordingly, including server data
    /// </summary>
    private IEnumerator RefreshLevelStatusWithServer()
    {
        Debug.Log("[LevelSelectManager] Refreshing level unlock status with server data");
        
        // Always enable level 1
        if (levelButtons.Length > 0 && levelButtons[0] != null)
        {
            levelButtons[0].GetComponent<Button>().interactable = true;
        }
        
        // First use local data
        RefreshLevelStatus();
        
        // Then try to get server data if available
        if (apiManager != null)
        {
            string username = PlayerPrefs.GetString("CurrentUsername", "");
            if (!string.IsNullOrEmpty(username))
            {
                bool requestComplete = false;
                bool level1Unlocked = true; // Level 1 always unlocked
                bool level2Unlocked = false;
                
                // Get progress from server
                apiManager.GetProgress(username, (success, level1Completed, level2Completed) => {
                    if (success)
                    {
                        Debug.Log($"[LevelSelectManager] Got server progress - Level1: {level1Completed}, Level2: {level2Completed}");
                        
                        // Update UI based on server data
                        level2Unlocked = level1Completed; // Unlock level 2 if level 1 is completed
                        
                        // Update level 2 button and locked panel
                        if (levelButtons.Length > 1 && levelButtons[1] != null)
                        {
                            levelButtons[1].GetComponent<Button>().interactable = level2Unlocked;
                        }
                        
                        if (lockedPanels.Length > 0 && lockedPanels[0] != null)
                        {
                            lockedPanels[0].SetActive(!level2Unlocked);
                            Debug.Log($"[LevelSelectManager] Updated Level 2 locked panel from server: {!level2Unlocked}");
                        }
                        
                        // Also update local PlayerPrefs to match server
                        if (level1Completed && PlayerPrefs.GetInt("Level_1_Completed", 0) == 0)
                        {
                            PlayerPrefs.SetInt("Level_1_Completed", 1);
                            PlayerPrefs.Save();
                            Debug.Log("[LevelSelectManager] Updated local Level_1_Completed to match server");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("[LevelSelectManager] Failed to get progress from server, using local data only");
                    }
                    
                    requestComplete = true;
                });
                
                // Wait for the server request to complete (up to 3 seconds)
                float timeout = 3.0f;
                float elapsed = 0f;
                while (!requestComplete && elapsed < timeout)
                {
                    elapsed += 0.1f;
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
        
        yield return null;
    }
    
    /// <summary>
    /// Check player progress using local data and update UI
    /// </summary>
    public void RefreshLevelStatus()
    {
        Debug.Log("[LevelSelectManager] Refreshing level unlock status from local data");
        
        // Always enable level 1
        if (levelButtons.Length > 0 && levelButtons[0] != null)
        {
            levelButtons[0].GetComponent<Button>().interactable = true;
        }
        
        // For each level (starting at index 1 for level 2)
        for (int i = 1; i < levelButtons.Length; i++)
        {
            int levelNumber = i + 1; // Level 2, 3, etc.
            bool isUnlocked = IsLevelUnlocked(levelNumber);
            
            Debug.Log($"[LevelSelectManager] Level {levelNumber} unlock status: {isUnlocked}");
            
            // Set button interactable based on unlock status
            if (levelButtons[i] != null)
            {
                levelButtons[i].GetComponent<Button>().interactable = isUnlocked;
            }
            
            // Show/hide locked panel
            if (i - 1 < lockedPanels.Length && lockedPanels[i - 1] != null)
            {
                lockedPanels[i - 1].SetActive(!isUnlocked);
                Debug.Log($"[LevelSelectManager] Level {levelNumber} locked panel visibility: {!isUnlocked}");
            }
        }
    }
    
    /// <summary>
    /// Check if a given level is unlocked using local PlayerPrefs
    /// </summary>
    private bool IsLevelUnlocked(int levelNumber)
    {
        // Level 1 is always unlocked
        if (levelNumber == 1)
            return true;
        
        // Check if the previous level was completed
        int previousLevelCompleted = PlayerPrefs.GetInt($"Level_{levelNumber-1}_Completed", 0);
        Debug.Log($"[LevelSelectManager] Previous level ({levelNumber-1}) completion status: {previousLevelCompleted}");
        
        return previousLevelCompleted == 1;
    }
    
    /// <summary>
    /// Load the selected level scene
    /// </summary>
    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelSceneNames.Length)
        {
            Debug.LogError($"[LevelSelectManager] Level index {levelIndex} is out of range!");
            return;
        }
        
        Debug.Log($"[LevelSelectManager] Loading level: {levelSceneNames[levelIndex]}");
        SceneManager.LoadScene(levelSceneNames[levelIndex]);
    }
    
    /// <summary>
    /// Log current level status to debug console
    /// </summary>
    private void DebugLevelStatus()
    {
        Debug.Log("======== LEVEL SELECT STATUS ========");
        for (int i = 1; i <= 2; i++) // Show for Level 1 and Level 2
        {
            int completed = PlayerPrefs.GetInt($"Level_{i}_Completed", 0);
            Debug.Log($"Level {i} Completed: {completed}");
        }
        Debug.Log("====================================");
    }
    
    /// <summary>
    /// Force a level unlock (for testing or admin purposes)
    /// </summary>
    public void ForceUnlockLevel(int levelNumber)
    {
        Debug.Log($"[LevelSelectManager] Force unlocking level {levelNumber}");
        
        // Set local unlock status
        if (levelNumber > 1)
        {
            PlayerPrefs.SetInt($"Level_{levelNumber-1}_Completed", 1);
            PlayerPrefs.Save();
        }
        
        // Update server if we have API manager and username
        if (apiManager != null)
        {
            string username = PlayerPrefs.GetString("CurrentUsername", "");
            if (!string.IsNullOrEmpty(username))
            {
                bool level1Completed = levelNumber >= 2;
                bool level2Completed = levelNumber >= 3;
                
                apiManager.UpdateProgress(username, level1Completed, level2Completed, (success, message) => {
                    if (success)
                    {
                        Debug.Log($"[LevelSelectManager] Successfully updated server with force unlocked level {levelNumber}");
                    }
                    else
                    {
                        Debug.LogError($"[LevelSelectManager] Failed to update server with force unlocked level: {message}");
                    }
                });
            }
        }
        
        // Refresh UI
        RefreshLevelStatus();
    }
} 