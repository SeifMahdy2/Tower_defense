using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    public GameObject loadingScreen;
    public Slider progressBar;

    void Awake()
    {
        // Enforce singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: keep across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadAsynchronously(() => SceneManager.LoadSceneAsync(sceneName)));
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(() => SceneManager.LoadSceneAsync(sceneIndex)));
    }

    private IEnumerator LoadAsynchronously(Func<AsyncOperation> loadOperationFunc)
    {
        loadingScreen.SetActive(true);

        AsyncOperation operation = loadOperationFunc();

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;
            yield return null;
        }

        loadingScreen.SetActive(false);

    }

}
