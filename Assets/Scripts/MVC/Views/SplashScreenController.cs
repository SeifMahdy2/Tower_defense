using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreenController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float displayDuration = 5f;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private string nextSceneName = "LoginScene";
    
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    
    private void Start()
    {
        // Make sure UI is fully visible at start
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
        
        // Begin splash screen sequence
        StartCoroutine(SplashScreenSequence());
    }
    
    private IEnumerator SplashScreenSequence()
    {
        // Wait for the display duration
        yield return new WaitForSeconds(displayDuration);
        
        // Fade out
        yield return StartCoroutine(FadeOut());
        
        // Load next scene
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(nextSceneName);
        }
        else
        {
            // Fallback to direct scene loading
            SceneManager.LoadScene(nextSceneName);
        }
    }
    
    private IEnumerator FadeOut()
    {
        float elapsedTime = 0;
        
        // If we don't have a canvas group, wait for the fade duration
        if (canvasGroup == null)
        {
            yield return new WaitForSeconds(fadeDuration);
            yield break;
        }
        
        // Fade out the canvas group
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Ensure we reach 0 alpha
        canvasGroup.alpha = 0f;
    }
} 