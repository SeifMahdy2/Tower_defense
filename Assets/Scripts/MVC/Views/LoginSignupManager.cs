using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class LoginSignupManager : MonoBehaviour
{
    [Header("Panel References")]
    public GameObject loginPanel;
    public GameObject signupPanel;
    
    [Header("Managers References")]
    public LoginManager loginManager;
    public SignupManager signupManager;
    
    [Header("Settings")]
    [Tooltip("Delay before switching from signup success to login panel")]
    public float panelSwitchDelay = 1.0f;
    
    [Tooltip("Scene to load after successful login")]
    public string mainMenuSceneName = "MainMenu";
    
    private void Start()
    {
        // Check references
        CheckReferences();
        
        // Initialize with login panel active and signup inactive
        if (loginPanel != null) loginPanel.SetActive(true);
        if (signupPanel != null) signupPanel.SetActive(false);
        
        // If managers aren't assigned in inspector, try to find them
        if (loginManager == null) 
        {
            loginManager = FindObjectOfType<LoginManager>();
            if (loginManager == null)
            {
                Debug.LogError("LoginManager not found in scene!");
            }
            else
            {
                Debug.Log("Found LoginManager: " + loginManager.name);
            }
        }
        
        if (signupManager == null) 
        {
            signupManager = FindObjectOfType<SignupManager>();
            if (signupManager == null)
            {
                Debug.LogError("SignupManager not found in scene!");
            }
            else
            {
                Debug.Log("Found SignupManager: " + signupManager.name);
            }
        }
        
        // Setup the buttons in LoginManager
        if (loginManager != null)
        {
            if (loginManager.loginButton != null)
            {
                // Clear existing listeners first to avoid duplicates
                loginManager.loginButton.onClick.RemoveAllListeners();
                loginManager.loginButton.onClick.AddListener(OnLoginClick);
                Debug.Log("Added listener to Login button");
            }
            
            if (loginManager.registerButton != null)
            {
                // Clear existing listeners first to avoid duplicates
                loginManager.registerButton.onClick.RemoveAllListeners();
                loginManager.registerButton.onClick.AddListener(GoToSignup);
                Debug.Log("Added listener to Go to Sign-up button");
            }
        }
        
        // Setup the buttons in SignupManager
        if (signupManager != null)
        {
            if (signupManager.signupButton != null)
            {
                // Clear existing listeners first to avoid duplicates
                signupManager.signupButton.onClick.RemoveAllListeners();
                signupManager.signupButton.onClick.AddListener(signupManager.OnSignupButtonClick);
                Debug.Log("Added listener to Signup button");
            }
            
            if (signupManager.backToLoginButton != null)
            {
                // Clear existing listeners first to avoid duplicates
                signupManager.backToLoginButton.onClick.RemoveAllListeners();
                signupManager.backToLoginButton.onClick.AddListener(GoToLogin);
                Debug.Log("Added listener to Back to Login button");
            }
            else
            {
                // Find and set up the back button if it exists
                Button backButton = signupPanel.GetComponentInChildren<Button>();
                if (backButton != null && backButton.name.ToLower().Contains("back"))
                {
                    backButton.onClick.RemoveAllListeners();
                    backButton.onClick.AddListener(GoToLogin);
                    Debug.Log("Added listener to Back to Login button");
                }
            }
        }
    }
    
    private void CheckReferences()
    {
        if (loginPanel == null) Debug.LogError("Login panel is not assigned!");
        if (signupPanel == null) Debug.LogError("Signup panel is not assigned!");
    }
    
    // Called when user clicks "Go to Signup" on login panel
    public void GoToSignup()
    {
        Debug.Log("GoToSignup called");
        if (loginManager != null) loginManager.ClearInputs();
        
        if (loginPanel != null && signupPanel != null)
        {
            loginPanel.SetActive(false);
            signupPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Cannot switch panels: one or both panels are null!");
        }
    }
    
    // Called when user clicks "Back to Login" on signup panel
    public void GoToLogin()
    {
        Debug.Log("GoToLogin called");
        if (loginPanel != null && signupPanel != null)
        {
            loginPanel.SetActive(true);
            signupPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Cannot switch panels: one or both panels are null!");
        }
    }
    
    // Called when user clicks Login button
    public void OnLoginClick()
    {
        Debug.Log("OnLoginClick called");
        if (loginManager != null)
        {
            loginManager.OnLoginButtonClicked();
        }
        else
        {
            Debug.LogError("Cannot login: LoginManager is null!");
        }
    }
    
    // Called by SignupManager after successful registration
    public void OnSignupSuccess()
    {
        Debug.Log("OnSignupSuccess called");
        // Wait briefly then switch to login panel
        StartCoroutine(SwitchToLoginAfterDelay());
    }
    
    // Delayed switch to login panel after signup success
    private IEnumerator SwitchToLoginAfterDelay()
    {
        yield return new WaitForSeconds(panelSwitchDelay);
        
        if (loginPanel != null && signupPanel != null)
        {
            loginPanel.SetActive(true);
            signupPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Cannot switch panels: one or both panels are null!");
        }
    }
    
    // Called by LoginManager after successful login
    public void OnLoginSuccess()
    {
        Debug.Log("OnLoginSuccess called");
        // Make sure scene name isn't empty
        if (string.IsNullOrEmpty(mainMenuSceneName))
        {
            Debug.LogError("Main menu scene name is not set!");
            return;
        }
        
        // Load the main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }
} 