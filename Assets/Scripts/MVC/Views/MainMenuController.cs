using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TD;
using TMPro;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject levelSelectPanel;
    
    [Header("Level Selection")]
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject lockedPanel;
    
    private PlayerProgressManager progressManager;

    private void Start()
    {
        Debug.Log("MainMenuController Start");
        progressManager = PlayerProgressManager.Instance;
        
        // Initialize panels (ensure level select panel is inactive at start)
        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(false);
        }
        
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
        
        SetupButtonHandlers();
        SetupLevelSelectButtons();
        
        // Force a refresh with slight delay to ensure everything is loaded
        StartCoroutine(DelayedRefresh());
    }
    
    private IEnumerator DelayedRefresh()
    {
        yield return new WaitForSeconds(0.5f);
        RefreshLevelUnlocks();
    }
    
    // Called every time the scene becomes active
    private void OnEnable()
    {
        Debug.Log("MainMenuController OnEnable - Refreshing level unlocks");
        // Refresh level unlocks when returning to this scene
        RefreshLevelUnlocks();
    }

    private void SetupButtonHandlers()
    {
        // Add touch handlers to all buttons
        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            if (button.GetComponent<MobileUIButtonHandler>() == null)
                button.gameObject.AddComponent<MobileUIButtonHandler>();
        }
    }
    
    private void SetupLevelSelectButtons()
    {
        if (level1Button != null)
        {
            level1Button.onClick.AddListener(() => LoadLevel(1));
        }
        
        if (level2Button != null)
        {
            level2Button.onClick.AddListener(() => LoadLevel(2));
        }
        
        if (backButton != null)
        {
            backButton.onClick.AddListener(BackToMainMenu);
        }
        
        // Initialize level locks
        RefreshLevelUnlocks();
    }
    
    private void RefreshLevelUnlocks()
    {
        if (level2Button != null && lockedPanel != null)
        {
            // Check if level 2 is unlocked
            bool isLevel2Unlocked = CheckIfLevel2Unlocked();
            
            // Direct check for debugging
            int level1Completed = PlayerPrefs.GetInt("Level_1_Completed", 0);
            Debug.Log("Level 1 completion direct check: " + level1Completed);
            
            Debug.Log("Level 2 Unlocked: " + isLevel2Unlocked);
            Debug.Log("LockedPanel state before: " + (lockedPanel.activeSelf ? "Active" : "Inactive"));
            
            // Update button interactable state
            level2Button.interactable = isLevel2Unlocked;
            
            // IMPORTANT: Make sure to explicitly set active state
            lockedPanel.SetActive(!isLevel2Unlocked);
            
            Debug.Log("LockedPanel state after: " + (lockedPanel.activeSelf ? "Active" : "Inactive"));
            
            // Force canvas update to ensure UI changes take effect
            Canvas.ForceUpdateCanvases();
        }
        else
        {
            Debug.LogWarning("Missing references for level unlock UI: " + 
                            "Level2Button=" + (level2Button != null) + 
                            ", LockedPanel=" + (lockedPanel != null));
        }
    }
    
    private bool CheckIfLevel2Unlocked()
    {
        // Always check PlayerPrefs first for most accurate information
        int completed = PlayerPrefs.GetInt("Level_1_Completed", 0);
        Debug.Log("Level 1 completion status via PlayerPrefs directly: " + completed);
        
        // Check via PlayerProgressManager if available
        if (progressManager != null)
        {
            bool unlocked = progressManager.IsLevelUnlocked(2);
            Debug.Log("Level 2 unlock status via PlayerProgressManager: " + unlocked);
            
            // If there's a mismatch, debug it
            if ((completed == 1) != unlocked)
            {
                Debug.LogWarning("Mismatch between PlayerPrefs (" + completed + 
                                ") and PlayerProgressManager (" + unlocked + ") for Level 2 unlock!");
            }
            
            return unlocked;
        }
        
        Debug.Log("Level 2 should be " + (completed == 1 ? "UNLOCKED" : "LOCKED"));
        return completed == 1;
    }

    public void OnClickPlayButton()
    {
        PlayButtonClickSound();
        ProvideTouchFeedback();
        
        // Refresh level unlocks when opening the level select screen
        RefreshLevelUnlocks();
        
        // Show level select panel instead of loading level directly
        if (mainMenuPanel != null && levelSelectPanel != null)
        {
            mainMenuPanel.SetActive(false);
            levelSelectPanel.SetActive(true);
            
            // Force a refresh after panel becomes active
            StartCoroutine(DelayedPanelRefresh());
        }
    }
    
    private IEnumerator DelayedPanelRefresh()
    {
        yield return null; // Wait one frame
        RefreshLevelUnlocks();
    }
    
    private void LoadLevel(int levelNumber)
    {
        PlayButtonClickSound();
        ProvideTouchFeedback();
        
        // Check if level is unlocked (always allow level 1)
        if (levelNumber == 1 || (levelNumber == 2 && CheckIfLevel2Unlocked()))
        {
            // For debugging
            Debug.Log("Loading Level " + levelNumber);
            
            SceneLoader.Instance.LoadScene("Level_" + levelNumber);
        }
        else
        {
            Debug.Log("Cannot load Level " + levelNumber + " - Level is locked");
        }
    }
    
    private void BackToMainMenu()
    {
        PlayButtonClickSound();
        ProvideTouchFeedback();
        
        if (mainMenuPanel != null && levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
    }

    public void OnClickQuitButton()
    {
        PlayButtonClickSound();
        ProvideTouchFeedback();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OnClickSettingsButton()
    {
        PlayButtonClickSound();
        ProvideTouchFeedback();
        settingsPanel.SetActive(true);
    }

    public void OnClickCloseSettings()
    {
        PlayButtonClickSound();
        ProvideTouchFeedback();
        settingsPanel.SetActive(false);
    }
    
    private void PlayButtonClickSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClickSound();
    }
    
    private void ProvideTouchFeedback()
    {
        #if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
        #endif
    }
}
