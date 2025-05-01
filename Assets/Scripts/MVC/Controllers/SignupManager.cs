using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text.RegularExpressions;
using System.Collections;

public class SignupManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public Button signupButton;
    public Button backToLoginButton;
    public TextMeshProUGUI errorText;

    // Simple email validation pattern
    private readonly string emailPattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
    
    // References to other managers
    private ApiManager apiManager;
    private LoginSignupManager loginSignupManager;
    
    // Flag to prevent multiple concurrent signup attempts
    private bool isProcessingSignup = false;
    
    // Flag to track if we've already set up button listeners
    private bool buttonListenersInitialized = false;
    
    private void Start()
    {
        // Find managers
        apiManager = ApiManager.Instance;
        loginSignupManager = FindObjectOfType<LoginSignupManager>();
        
        // Check UI references
        CheckReferences();
        
        if (apiManager == null)
        {
            Debug.LogError("ApiManager not found! Make sure it's in your scene.");
        }
        
        // Debug info
        Debug.Log("SignupManager Start - Found LoginSignupManager: " + (loginSignupManager != null));
        
        // Don't set up our own button listeners - let LoginSignupManager handle it
        // This prevents double-registration of listeners
    }
    
    private void CheckReferences()
    {
        if (usernameInput == null) Debug.LogError("Username input is not assigned!");
        if (emailInput == null) Debug.LogError("Email input is not assigned!");
        if (passwordInput == null) Debug.LogError("Password input is not assigned!");
        if (confirmPasswordInput == null) Debug.LogError("Confirm password input is not assigned!");
        if (signupButton == null) Debug.LogError("Signup button is not assigned!");
        if (errorText == null) Debug.LogError("Error text is not assigned!");
    }

    // Called when the signup button is pressed
    public void OnSignupButtonClick()
    {
        Debug.Log("[SignupManager] OnSignupButtonClick called - Current processing state: " + isProcessingSignup);
        
        // Prevent multiple concurrent signup attempts
        if (isProcessingSignup)
        {
            Debug.Log("[SignupManager] Ignoring duplicate signup request - already processing");
            return;
        }
        
        isProcessingSignup = true;
        
        // Check for null references first
        if (usernameInput == null || emailInput == null || 
            passwordInput == null || confirmPasswordInput == null)
        {
            Debug.LogError("Input fields not assigned in the inspector. Please check the references.");
            isProcessingSignup = false;
            return;
        }
        
        string username = usernameInput != null ? usernameInput.text.Trim() : "";
        string email = emailInput != null ? emailInput.text.Trim() : "";
        string password = passwordInput != null ? passwordInput.text : "";
        string confirmPassword = confirmPasswordInput != null ? confirmPasswordInput.text : "";

        Debug.Log("[SignupManager] Processing signup for username: " + username + ", email: " + email);

        // Basic validation
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowError("Username, email and password cannot be empty.");
            isProcessingSignup = false;
            return;
        }

        // Validate email format
        if (!Regex.IsMatch(email, emailPattern))
        {
            ShowError("Please enter a valid email address.");
            isProcessingSignup = false;
            return;
        }

        if (password != confirmPassword)
        {
            ShowError("Passwords do not match.");
            isProcessingSignup = false;
            return;
        }

        // Make sure API manager is available
        if (apiManager == null)
        {
            apiManager = ApiManager.Instance;
            if (apiManager == null)
            {
                Debug.LogError("ApiManager not found! Cannot register user.");
                ShowError("Registration failed: Server connection error.");
                isProcessingSignup = false;
                return;
            }
        }
        
        // Disable button during registration
        if (signupButton != null)
        {
            signupButton.interactable = false;
        }
        
        // Save username for login convenience
        PlayerPrefs.SetString("LastUsername", username);
        PlayerPrefs.Save();
        
        Debug.Log("[SignupManager] Sending registration request to server...");
        
        // Register with server
        apiManager.Register(username, email, password, (success, message, userData) => {
            Debug.Log("[SignupManager] Registration callback received - success: " + success);
            
            // Re-enable button and reset processing flag
            if (signupButton != null)
            {
                signupButton.interactable = true;
            }
            
            isProcessingSignup = false;
            
            if (success)
            {
                Debug.Log("Registration successful: " + message);
                
                // Also store password locally for login
                StoreCredentialsLocally(username, email, password);
                
                // Show success message and redirect to login
                ShowError("Registration successful! Redirecting to login...");
                
                // Notify LoginSignupManager if available
                if (loginSignupManager != null)
                {
                    loginSignupManager.OnSignupSuccess();
                }
                else
                {
                    // Fallback to the old behavior
                    StartCoroutine(NavigateToLoginAfterDelay(1.0f));
                }
            }
            else
            {
                Debug.LogError("Registration failed: " + message);
                ShowError("Registration failed: " + message);
            }
        });
    }
    
    // Store credentials in PlayerPrefs for login convenience
    private void StoreCredentialsLocally(string username, string email, string password)
    {
        PlayerPrefs.SetString("User_" + username + "_Password", password);
        PlayerPrefs.SetString("User_" + username + "_Email", email);
        PlayerPrefs.Save();
    }

    private System.Collections.IEnumerator NavigateToLoginAfterDelay(float delay)
    {
        // Wait for specified delay
        yield return new WaitForSeconds(delay);
        
        // Try to use LoginSignupManager first
        if (loginSignupManager != null)
        {
            loginSignupManager.GoToLogin();
        }
        else
        {
            // Fallback to loading the login scene directly
            Debug.Log("No LoginSignupManager found, loading LoginScene directly");
            SceneManager.LoadScene("LoginScene");
        }
    }

    public void ShowError(string message)
    {
        if (errorText != null)
        {
            errorText.text = message;
            errorText.gameObject.SetActive(true);
            
            // Log the message for debugging
            Debug.Log("SignupManager error message: " + message);
        }
        else
        {
            Debug.LogError(message);
        }
    }
    
    // Method for navigating back to login
    public void GoToLogin()
    {
        Debug.Log("SignupManager.GoToLogin called");
        
        if (loginSignupManager != null)
        {
            loginSignupManager.GoToLogin();
        }
        else
        {
            // Fallback to loading the login scene directly
            SceneManager.LoadScene("LoginScene");
        }
    }
} 