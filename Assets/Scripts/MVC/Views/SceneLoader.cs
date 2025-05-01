using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;
    
    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider progressBar;
    [SerializeField] private bool useLoadingDelay = false;
    [SerializeField] private float loadingDelay = 0.2f;
    
    [Header("Optimization")]
    [SerializeField] private bool unloadPreviousScene = true;
    
    public static SceneLoader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SceneLoader>();
                
                if (instance == null)
                {
                    GameObject go = new GameObject("SceneLoader");
                    instance = go.AddComponent<SceneLoader>();
                }
            }
            
            return instance;
        }
    }
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void LoadScene(string sceneName)
    {
        Debug.Log($"Loading scene: {sceneName}");
        StartCoroutine(LoadAsynchronously(() => SceneManager.LoadSceneAsync(sceneName)));
    }
    
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(() => SceneManager.LoadSceneAsync(sceneIndex)));
    }
    
    private IEnumerator LoadAsynchronously(Func<AsyncOperation> loadOperationFunc)
    {
        // First, trigger garbage collection to free up memory
        Resources.UnloadUnusedAssets();
        GC.Collect();
        
        // Activate loading screen
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
            
            // Reset progress bar
            if (progressBar != null)
                progressBar.value = 0f;
        }
            
        // Start the load operation
        AsyncOperation operation = loadOperationFunc();
        
        // Allow immediate activation for faster loading
        operation.allowSceneActivation = true;
        
        // Report loading progress
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress);
            
            // Update the progress bar if available
            if (progressBar != null)
                progressBar.value = progress;
                
            // Wait for the next frame
            yield return null;
        }
        
        // Short delay after loading is complete (optional)
        if (useLoadingDelay)
            yield return new WaitForSeconds(loadingDelay);
        
        // Hide loading screen when done
        if (loadingScreen != null)
            loadingScreen.SetActive(false);
            
        // Final garbage collection after loading
        Resources.UnloadUnusedAssets();
    }
}
