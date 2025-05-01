using UnityEngine;

public class GameStartupManager : MonoBehaviour
{
    // This ensures the script runs very early in the startup process
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnGameStart()
    {
        // Create this manager if it doesn't exist yet
        if (FindObjectOfType<GameStartupManager>() == null)
        {
            GameObject managerObject = new GameObject("GameStartupManager");
            managerObject.AddComponent<GameStartupManager>();
            DontDestroyOnLoad(managerObject);
        }
    }
    
    private void Awake()
    {
        // Initialization code can go here
        Debug.Log("Game startup manager initialized");
    }
} 