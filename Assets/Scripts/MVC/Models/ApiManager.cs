using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ApiManager : MonoBehaviour
{
    private const string API_URL = "http://localhost:3000";
    public static ApiManager Instance;

    private string currentUsername = "";
    
    // Track ongoing registration requests to prevent duplicates
    private Dictionary<string, bool> ongoingRegistrations = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("ApiManager initialized as singleton");
        }
        else
        {
            Debug.Log("Additional ApiManager instance destroyed");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Load any saved username
        currentUsername = PlayerPrefs.GetString("CurrentUsername", "");
    }

    public string GetBaseUrl()
    {
        return API_URL;
    }

    // Login user
    public void Login(string username, string password, Action<bool, string, UserData> callback)
    {
        StartCoroutine(LoginCoroutine(username, password, callback));
    }

    private IEnumerator LoginCoroutine(string username, string password, Action<bool, string, UserData> callback)
    {
        // Create request body
        LoginRequest request = new LoginRequest
        {
            username = username, 
            password = password 
        };
        
        string jsonData = JsonUtility.ToJson(request);

        using (UnityWebRequest www = new UnityWebRequest(API_URL + "/login", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();
        
            if (www.result == UnityWebRequest.Result.Success)
        {
                try
                {
                    LoginResponse response = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);
                    
                    if (response.success)
                    {
                        // Save username for later use
                        currentUsername = username;
                        PlayerPrefs.SetString("CurrentUsername", username);
                        PlayerPrefs.Save();
                        
                        // Update local progress based on server data
                        if (response.user != null && response.user.levels != null)
                        {
                            bool level1Unlocked = response.user.levels.level1;
                            bool level2Unlocked = response.user.levels.level2;
                            
                            // Update local player prefs to match server
                            PlayerPrefs.SetInt("Level_1_Completed", level2Unlocked ? 1 : 0);
                            PlayerPrefs.SetInt("Level_2_Completed", 0); // Initialize level 2 completion
                            PlayerPrefs.Save();
                            
                            Debug.Log($"Updated local progress: Level 1 unlocked: {level1Unlocked}, Level 2 unlocked: {level2Unlocked}");
                        }
                        
                        callback(true, "Login successful", response.user);
    }
                    else
                    {
                        callback(false, response.error ?? "Unknown error", null);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to parse login response: " + e.Message);
                    callback(false, "Failed to parse server response", null);
                }
            }
            else
            {
                string errorMessage = "Login failed: " + www.error;
                
                if (!string.IsNullOrEmpty(www.downloadHandler.text))
    {
                    try
                    {
                        BaseResponse errorResponse = JsonUtility.FromJson<BaseResponse>(www.downloadHandler.text);
                        if (!string.IsNullOrEmpty(errorResponse.error))
                        {
                            errorMessage = errorResponse.error;
                        }
                    }
                    catch
                    {
                        // Use the default error message
                    }
                }
                
                callback(false, errorMessage, null);
    }
        }
    }

    // Register user
    public void Register(string username, string email, string password, Action<bool, string, UserData> callback)
    {
        // Create a unique key to identify this registration request
        string registrationKey = $"{username}_{email}";
        
        // Check if we already have an ongoing registration with the same details
        if (ongoingRegistrations.ContainsKey(registrationKey) && ongoingRegistrations[registrationKey])
        {
            Debug.LogWarning($"Ignoring duplicate registration request for {username} / {email} - request already in progress");
            return;
        }
        
        Debug.Log($"Register request received: {{ username: '{username}', email: '{email}', password: '{password.Length} chars' }}");
        
        // Mark this registration as in progress
        ongoingRegistrations[registrationKey] = true;
        
        StartCoroutine(RegisterCoroutine(username, email, password, (success, message, userData) => {
            // Mark registration as complete
            ongoingRegistrations[registrationKey] = false;
            
            // Forward the callback
            callback(success, message, userData);
        }));
    }

    private IEnumerator RegisterCoroutine(string username, string email, string password, Action<bool, string, UserData> callback)
    {
        // Create request body
        RegistrationRequest request = new RegistrationRequest
        {
            username = username,
            email = email,
            password = password
        };
        
        string jsonData = JsonUtility.ToJson(request);
        Debug.Log($"Register request received: {jsonData}");
        
        using (UnityWebRequest www = new UnityWebRequest(API_URL + "/register", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("Sending registration web request...");
            yield return www.SendWebRequest();
            Debug.Log("Registration web request complete: " + www.result);

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Save username for later use
                currentUsername = username;
                PlayerPrefs.SetString("CurrentUsername", username);
                PlayerPrefs.Save();
                
                try
                {
                    UserData userData = JsonUtility.FromJson<UserData>(www.downloadHandler.text);
                    callback(true, "Registration successful", userData);
                }
                catch
                {
                    // If can't parse as UserData, just report success
                    callback(true, "Registration successful", null);
                }
            }
            else
            {
                string errorMessage = "Registration failed: " + www.error;
                if (!string.IsNullOrEmpty(www.downloadHandler.text))
                {
                    try
                    {
                        BaseResponse errorResponse = JsonUtility.FromJson<BaseResponse>(www.downloadHandler.text);
                        if (!string.IsNullOrEmpty(errorResponse.error))
                        {
                            errorMessage = errorResponse.error;
    }
                    }
                    catch
                    {
                        // Use the default error message
                    }
                }
                callback(false, errorMessage, null);
            }
        }
    }

    // Update progress
    public void UpdateProgress(string username, bool level1Completed, bool level2Completed, Action<bool, string> callback)
    {
        StartCoroutine(UpdateProgressCoroutine(username, level1Completed, level2Completed, callback));
    }
    
    private IEnumerator UpdateProgressCoroutine(string username, bool level1Completed, bool level2Completed, Action<bool, string> callback)
    {
        // Create request body
        ProgressUpdateRequest request = new ProgressUpdateRequest
        {
            username = username, 
            level1 = level1Completed,
            level2 = level2Completed
        };
        
        string jsonData = JsonUtility.ToJson(request);
        Debug.Log($"Sending progress update: {jsonData}");

        using (UnityWebRequest www = new UnityWebRequest(API_URL + "/update-progress", "POST"))
        {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                callback(true, "Progress updated successfully");
            }
            else
            {
                string errorMessage = "Progress update failed: " + www.error;
                if (!string.IsNullOrEmpty(www.downloadHandler.text))
                {
                    try
                    {
                        BaseResponse errorResponse = JsonUtility.FromJson<BaseResponse>(www.downloadHandler.text);
                        if (!string.IsNullOrEmpty(errorResponse.message))
                        {
                            errorMessage = errorResponse.message;
}
                    }
                    catch
                    {
                        // Use the default error message
                    }
                }
                callback(false, errorMessage);
            }
        }
    }

    // Get user's progress from server
    public void GetProgress(string username, Action<bool, bool, bool> callback)
    {
        StartCoroutine(GetProgressCoroutine(username, callback));
}

    private IEnumerator GetProgressCoroutine(string username, Action<bool, bool, bool> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(API_URL + "/progress/" + username))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    ProgressResponse response = JsonUtility.FromJson<ProgressResponse>(www.downloadHandler.text);
                    
                    if (response.success && response.progress != null)
                    {
                        bool level1 = response.progress.level1;
                        bool level2 = response.progress.level2;
                        callback(true, level1, level2);
}
                    else
                    {
                        callback(false, false, false);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to parse progress response: " + e.Message);
                    callback(false, false, false);
                }
            }
            else
            {
                Debug.LogError("Failed to get progress: " + www.error);
                callback(false, false, false);
            }
        }
    }
} 