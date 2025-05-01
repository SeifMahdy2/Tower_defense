using UnityEngine;
using System.Collections;

public class PlayerProgressManager : MonoBehaviour
{
    public static PlayerProgressManager Instance;
    private ApiManager apiManager;
    private bool isConnectedToServer = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("PlayerProgressManager initialized as singleton");
        }
        else
        {
            Debug.Log("Additional PlayerProgressManager instance destroyed");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        apiManager = ApiManager.Instance;
        // Check server connection
        StartCoroutine(CheckServerConnection());
        
        // Log current progress state
        DebugLogProgress();
    }
    
    private void DebugLogProgress()
    {
        Debug.Log("======== PLAYER PROGRESS STATE ========");
        Debug.Log("Level 1 Completed: " + PlayerPrefs.GetInt("Level_1_Completed", 0));
        Debug.Log("Level 2 Completed: " + PlayerPrefs.GetInt("Level_2_Completed", 0));
        Debug.Log("======================================");
    }

    IEnumerator CheckServerConnection()
    {
        // Attempt to connect to server
        if (apiManager != null)
        {
            Debug.Log("Checking server connection...");
            // Simple ping to server
            var www = new UnityEngine.Networking.UnityWebRequest(apiManager.GetBaseUrl());
            yield return www.SendWebRequest();
            isConnectedToServer = www.result == UnityEngine.Networking.UnityWebRequest.Result.Success;
            Debug.Log("Server connection status: " + (isConnectedToServer ? "Connected" : "Not Connected"));
        }
        else
        {
            Debug.LogWarning("ApiManager not found for server connection check");
        }
    }

    // Check if a level is unlocked
    public bool IsLevelUnlocked(int levelNumber)
    {
        if (levelNumber == 1)
        {
            Debug.Log("Level 1 is always unlocked");
            return true; // Level 1 is always unlocked
        }
        
        // Check if previous level was completed
        int previousLevelCompleted = PlayerPrefs.GetInt("Level_" + (levelNumber - 1) + "_Completed", 0);
        Debug.Log("Checking if Level " + levelNumber + " is unlocked - Previous level completed: " + previousLevelCompleted);
        return previousLevelCompleted == 1;
    }

    // Save level completion status
    public void SetLevelCompleted(int levelNumber)
    {
        Debug.Log("Setting Level " + levelNumber + " as completed");
        
        // Save locally
        PlayerPrefs.SetInt("Level_" + levelNumber + "_Completed", 1);
        PlayerPrefs.Save();
        
        // Log to confirm save
        int saved = PlayerPrefs.GetInt("Level_" + levelNumber + "_Completed", 0);
        Debug.Log("Level " + levelNumber + " completion saved: " + saved);
        
        // Force save to ensure it persists
        PlayerPrefs.Save();
        
        // If connected to server, sync progress
        if (apiManager != null)
        {
            Debug.Log("Syncing progress with server");
            SyncProgressWithServer();
        }
        else
        {
            Debug.LogWarning("ApiManager not found, only saving progress locally");
            apiManager = ApiManager.Instance;
            if (apiManager != null)
            {
                SyncProgressWithServer();
            }
        }
        
        // Update debug log
        DebugLogProgress();
    }

    // Sync progress with server
    public void SyncProgressWithServer()
    {
        if (apiManager == null) 
        {
            Debug.LogWarning("ApiManager not available for sync");
            apiManager = ApiManager.Instance;
            if (apiManager == null) return;
        }
        
        string username = PlayerPrefs.GetString("CurrentUsername", "");
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogWarning("No username found for sync");
            return;
        }
        
        // Get local progress - determine how many levels are completed
        bool level1Completed = PlayerPrefs.GetInt("Level_1_Completed", 0) == 1;
        bool level2Completed = PlayerPrefs.GetInt("Level_2_Completed", 0) == 1;
        
        Debug.Log("Syncing progress with server - Level 1 completed: " + level1Completed + ", Level 2 completed: " + level2Completed);
        
        // Send to server - this updates the progress table
        apiManager.UpdateProgress(username, level1Completed, level2Completed, OnProgressUpdated);
        
        // Also update the levels_completed field in the user table
        UpdateUserLevelsCompleted(username, level1Completed, level2Completed);
    }
    
    // Update the levels_completed field in the user table
    private void UpdateUserLevelsCompleted(string username, bool level1Completed, bool level2Completed)
    {
        // Calculate levels_completed based on level completion
        int levelsCompleted = 0;
        
        if (level1Completed) levelsCompleted = 1;
        if (level2Completed) levelsCompleted = 2;
        
        Debug.Log($"Updating user record levels_completed to {levelsCompleted} for user {username}");
        
        // Make a custom web request to update the levels_completed field
        StartCoroutine(UpdateLevelsCompletedCoroutine(username, levelsCompleted));
    }
    
    private IEnumerator UpdateLevelsCompletedCoroutine(string username, int levelsCompleted)
    {
        if (apiManager == null) yield break;
        
        // Since the server expects an ID parameter but we only have username,
        // we need to modify our approach:
        
        // Option 1: Use a different endpoint in the API that accepts username
        string url = apiManager.GetBaseUrl() + "/update-levels-completed-by-username";
        
        // Create request data with username
        string jsonData = "{ \"username\": \"" + username + "\", \"levels_completed\": " + levelsCompleted + " }";
        
        // Use UnityWebRequest to update levels_completed field
        var request = new UnityEngine.Networking.UnityWebRequest(url, "PUT");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        
        yield return request.SendWebRequest();
        
        // If the custom endpoint doesn't exist or fails, try a workaround
        if (request.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("Could not update levels_completed directly with username. The server may need an API update.");
            Debug.LogWarning("Using progress table instead which should sync with the server database via existing APIs.");
            
            // Note: The existing apiManager.UpdateProgress should already handle the sync with the server
            // through the progress table, which might be sufficient depending on how the server is set up.
        }
        else
        {
            Debug.Log("Successfully updated levels_completed field in user record: " + request.downloadHandler.text);
        }
    }
    
    private void OnProgressUpdated(bool success, string response)
    {
        if (success)
        {
            Debug.Log("Successfully updated progress on server: " + response);
        }
        else
        {
            Debug.LogError("Failed to update progress on server: " + response);
        }
    }
} 