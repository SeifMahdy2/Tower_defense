using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    [Header("Login UI")]
    public GameObject loginPanel;
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Button registerButton;
    public TextMeshProUGUI statusText;
    
    [Header("Settings")]
    public bool autoLogin = true;
    public float statusMessageDuration = 3f;
    
    private ApiManager apiManager;
    private PlayerProgressManager progressManager;
    private LoginSignupManager loginSignupManager;
    
    private void Start()
    {
        apiManager = ApiManager.Instance;
        progressManager = PlayerProgressManager.Instance;
        loginSignupManager = FindObjectOfType<LoginSignupManager>();
        
        Debug.Log("LoginManager Start - Buttons setup will be handled by LoginSignupManager");
        
        // Clear any previous status messages
        if (statusText != null)
            statusText.text = "";
            
        // Try auto-login if enabled
        if (autoLogin)
            StartCoroutine(TryAutoLogin());
    }
    
    // Make this public so it can be called directly from LoginSignupManager
    public void OnLoginButtonClicked()
    {
        Debug.Log("LoginManager.OnLoginButtonClicked called");
        
        if (apiManager == null)
        {
            Debug.LogError("ApiManager not found!");
            ShowStatus("Error: Server connection not available", true);
            return;
        }
        
        string username = usernameInput != null ? usernameInput.text : "";
        string password = passwordInput != null ? passwordInput.text : "";
        
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowStatus("Please enter both username and password", true);
            return;
        }
        
        // Disable login button while request is processing
        if (loginButton != null)
            loginButton.interactable = false;
            
        ShowStatus("Logging in...", false);
        
        apiManager.Login(username, password, (success, message, userData) => {
            if (success)
            {
                // Save credentials for auto-login
                PlayerPrefs.SetString("CurrentUsername", username);
                PlayerPrefs.SetString("CurrentPassword", password); // Note: This is not secure
                PlayerPrefs.Save();
                
                ShowStatus("Login successful! Redirecting...", false);
                
                // Refresh the player progress
                if (progressManager != null)
                    progressManager.SyncProgressWithServer();
                    
                // Notify the LoginSignupManager of successful login
                if (loginSignupManager != null)
                {
                    loginSignupManager.OnLoginSuccess();
                }
                else
                {
                    // Fallback if no LoginSignupManager
                    Debug.LogWarning("LoginSignupManager not found - can't redirect after login");
                    // Hide login panel after successful login
                    StartCoroutine(HideLoginPanelAfterDelay(1f));
                }
            }
            else
            {
                Debug.LogError("Login failed: " + message);
                ShowStatus("Login failed: " + message, true);
                
                // Re-enable login button
                if (loginButton != null)
                    loginButton.interactable = true;
            }
        });
    }
    
    private IEnumerator TryAutoLogin()
    {
        yield return new WaitForSeconds(0.5f); // Wait for other systems to initialize
        
        string savedUsername = PlayerPrefs.GetString("CurrentUsername", "");
        string savedPassword = PlayerPrefs.GetString("CurrentPassword", "");
        
        if (!string.IsNullOrEmpty(savedUsername) && !string.IsNullOrEmpty(savedPassword))
        {
            Debug.Log("Attempting auto-login with saved credentials");
            
            // Populate login fields
            if (usernameInput != null)
                usernameInput.text = savedUsername;
                
            if (passwordInput != null)
                passwordInput.text = savedPassword;
                
            // Trigger login
            OnLoginButtonClicked();
        }
        else
        {
            Debug.Log("No saved credentials found for auto-login");
        }
    }
    
    private IEnumerator HideLoginPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (loginPanel != null)
            loginPanel.SetActive(false);
            
        // Trigger event for successful login (game can start)
        Debug.Log("Login complete - game can start now");
    }
    
    public void ShowStatus(string message, bool isError)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = isError ? Color.red : Color.green;
            
            // Clear message after delay
            StartCoroutine(ClearStatusAfterDelay(statusText, statusMessageDuration));
        }
    }
    
    private IEnumerator ClearStatusAfterDelay(TextMeshProUGUI textField, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (textField != null)
            textField.text = "";
    }
    
    // Clear all input fields
    public void ClearInputs()
    {
        // Clear login inputs
        if (usernameInput != null)
            usernameInput.text = "";
            
        if (passwordInput != null)
            passwordInput.text = "";
            
        // Clear status messages
        if (statusText != null)
            statusText.text = "";
    }
} 