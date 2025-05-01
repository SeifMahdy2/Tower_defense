using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[System.Serializable]
public class Wave
{
    public string waveName;
    public int easyEnemyCount;
    public int mediumEnemyCount;
    public int hardEnemyCount;
    public float spawnInterval = 0.5f;
    public float timeBetweenGroups = 2f;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject easyEnemyPrefab;
    [SerializeField] private GameObject mediumEnemyPrefab;
    [SerializeField] private GameObject hardEnemyPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI enemyCountText;

    [Header("Wave Settings")]
    [SerializeField] private Wave[] waves;
    [SerializeField] private float timeBetweenWaves = 10f;
    [SerializeField] private bool autoStartNextWave = true;
    
    [Header("Debug")]
    [SerializeField] private int currentWave = 0;
    [SerializeField] private int enemiesLeftToSpawn;
    [SerializeField] private int enemiesAlive;
    [SerializeField] private bool isSpawning = false;
    [SerializeField] private bool allWavesCompleted = false;
    [SerializeField] private int enemiesSpawned = 0; // Track number of enemies spawned in current wave
    [SerializeField] private int totalEnemiesInWave = 0; // Total enemies in current wave

    // References
    private TD.GameManager gameManager;
    private WaypointPath path;
    
    // Events
    public UnityEvent onWaveCompleted = new UnityEvent();
    public UnityEvent onAllWavesCompleted = new UnityEvent();
    public static UnityEvent onEnemyDestroyed = new UnityEvent();
    
    // Track which waves have been completed to avoid duplicate rewards
    private bool[] waveCompletionRewardGiven;
    
    // Flag to track if spawning is paused (e.g. when game over panel is active)
    private bool isPaused = false;

    private void Awake() 
    {
        // Clear any previous listeners to prevent duplicates
        if (onEnemyDestroyed != null) 
        {
            onEnemyDestroyed.RemoveAllListeners();
        }
        onEnemyDestroyed = new UnityEvent();
        onEnemyDestroyed.AddListener(EnemyDestroyed);
        
        // Find the game manager
        gameManager = FindObjectOfType<TD.GameManager>();
        
        // Find the path
        path = FindObjectOfType<WaypointPath>();
        
        // If spawn point is not assigned, try to find it
        if (spawnPoint == null)
        {
            // First try to find by name
            GameObject startPointObj = GameObject.Find("Start Point");
            if (startPointObj != null)
            {
                spawnPoint = startPointObj.transform;
                Debug.Log("Found Start Point by name");
            }
            // Then check if path exists and has waypoints
            else if (path != null && path.GetWaypointsCount() > 0)
            {
                spawnPoint = path.GetFirstWaypoint();
                Debug.Log("Using first waypoint as spawn point");
            }
            // Finally, use this transform as last resort
            else
            {
                spawnPoint = transform;
                Debug.Log("Using EnemySpawner transform as spawn point");
            }
        }
    }

    private void Start() 
    {
        // Reset values to make sure we're starting fresh
        currentWave = 0;
        enemiesLeftToSpawn = 0;
        enemiesAlive = 0;
        enemiesSpawned = 0;
        totalEnemiesInWave = 0;
        isSpawning = false;
        allWavesCompleted = false;
        isPaused = false;
        
        // Initialize wave completion tracking array
        waveCompletionRewardGiven = new bool[waves.Length];
        for (int i = 0; i < waves.Length; i++)
        {
            waveCompletionRewardGiven[i] = false;
        }
        
        // Check that path exists
        if (path == null)
        {
            Debug.LogError("No WaypointPath found in the scene! Enemies need a path to follow.");
            return;
        }
        
        // Update UI
        UpdateUI();
        
        // Start first wave if there are waves defined
        if (waves.Length > 0)
        {
            Debug.Log("Game starting! First wave in " + timeBetweenWaves + " seconds...");
            StartCoroutine(StartNextWave());
        }
        else
        {
            Debug.LogWarning("No waves defined in the EnemySpawner!");
        }
    }

    private void Update() 
    {
        // Update UI
        UpdateUI();
    }
    
    public void StartWave()
    {
        if (!isSpawning && !allWavesCompleted && currentWave < waves.Length && enemiesAlive <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(StartNextWave(0f)); // Start immediately
        }
    }
    
    // Method to pause/resume enemy spawning
    public void SetPaused(bool pause)
    {
        isPaused = pause;
        
        // If pausing, stop any active spawning coroutine
        if (pause && isSpawning)
        {
            StopAllCoroutines();
            isSpawning = false;
        }
        // If unpausing and we're in the middle of a wave, restart spawning
        else if (!pause && currentWave > 0 && currentWave <= waves.Length && enemiesLeftToSpawn > 0)
        {
            StartCoroutine(ResumeSpawning());
        }
    }
    
    private IEnumerator ResumeSpawning()
    {
        if (currentWave > 0 && currentWave <= waves.Length)
        {
            Wave currentWaveData = waves[currentWave - 1];
            isSpawning = true;
            yield return StartCoroutine(SpawnWave(currentWaveData));
        }
    }

    private IEnumerator StartNextWave(float delay = -1f) 
    {
        // Make sure we don't start a new wave if there are still enemies alive
        if (enemiesAlive > 0)
        {
            Debug.LogWarning("Cannot start next wave - there are still " + enemiesAlive + " enemies alive!");
            yield break;
        }
        
        // Don't start if we're paused
        if (isPaused)
        {
            Debug.Log("Cannot start next wave - spawning is paused!");
            yield break;
        }
        
        // Use the specified delay or default to timeBetweenWaves
        float waitTime = delay >= 0f ? delay : timeBetweenWaves;
        
        Debug.Log("Waiting " + waitTime + " seconds before starting the next wave...");
        
        // Wait before starting the wave
        yield return new WaitForSeconds(waitTime);
        
        Debug.Log("Wait completed. Starting wave now if not paused.");
        
        // Check if we're paused after waiting
        if (isPaused)
        {
            Debug.Log("Cannot start next wave - spawning is paused!");
            yield break;
        }
        
        // Check if we've already completed all waves
        if (currentWave >= waves.Length)
        {
            Debug.Log("All " + waves.Length + " waves already completed!");
            allWavesCompleted = true;
            
            // Invoke event only if it wasn't invoked before
            if (!allWavesCompleted)
            {
                onAllWavesCompleted.Invoke();
            }
            
            yield break;
        }
        
        // Increment wave count
        currentWave++;
        
        // Get current wave
        Wave wave = waves[currentWave - 1];
        
        // Start the wave
        Debug.Log("Starting Wave " + currentWave + " of " + waves.Length + ": " + wave.waveName);
        
        // Reset spawn counter for new wave
        enemiesSpawned = 0;
        
        // Calculate total enemies
        totalEnemiesInWave = wave.easyEnemyCount + wave.mediumEnemyCount + wave.hardEnemyCount;
        enemiesLeftToSpawn = totalEnemiesInWave;
        isSpawning = true;
        
        // Spawn enemies in this wave
        yield return StartCoroutine(SpawnWave(wave));
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        // Spawn easy enemies
        for (int i = 0; i < wave.easyEnemyCount; i++)
        {
            // Check if we're paused before each spawn
            if (isPaused)
            {
                isSpawning = false;
                yield break;
            }
            
            SpawnEnemy(easyEnemyPrefab);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
        
        // Wait between groups
        if (!isPaused && wave.easyEnemyCount > 0 && (wave.mediumEnemyCount > 0 || wave.hardEnemyCount > 0))
            yield return new WaitForSeconds(wave.timeBetweenGroups);
        
        // Check if we're paused after waiting
        if (isPaused)
        {
            isSpawning = false;
            yield break;
        }
        
        // Spawn medium enemies
        for (int i = 0; i < wave.mediumEnemyCount; i++)
        {
            // Check if we're paused before each spawn
            if (isPaused)
            {
                isSpawning = false;
                yield break;
            }
            
            SpawnEnemy(mediumEnemyPrefab);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
        
        // Wait between groups
        if (!isPaused && wave.mediumEnemyCount > 0 && wave.hardEnemyCount > 0)
            yield return new WaitForSeconds(wave.timeBetweenGroups);
        
        // Check if we're paused after waiting
        if (isPaused)
        {
            isSpawning = false;
            yield break;
        }
        
        // Spawn hard enemies
        for (int i = 0; i < wave.hardEnemyCount; i++)
        {
            // Check if we're paused before each spawn
            if (isPaused)
            {
                isSpawning = false;
                yield break;
            }
            
            SpawnEnemy(hardEnemyPrefab);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
        
        // All enemies spawned
        isSpawning = false;
    }

    private void SpawnEnemy(GameObject enemyPrefab) 
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is null!");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point is null!");
            return;
        }
        
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        
        // Ensure the enemy has the "Enemy" tag
        if (enemy.tag != "Enemy")
        {
            enemy.tag = "Enemy";
        }
        
        enemy.transform.parent = transform; // Parent to spawner for organization
        enemiesLeftToSpawn--;
        enemiesAlive++;
        enemiesSpawned++;
    }

    private void EnemyDestroyed() 
    {
        // Make sure we don't go below zero
        if (enemiesAlive > 0)
        {
            enemiesAlive--;
        }
        else
        {
            Debug.LogWarning("Enemy counter tried to go negative! Reset to 0.");
            enemiesAlive = 0;
        }
        
        Debug.Log("Enemy destroyed. Enemies alive: " + enemiesAlive);
        
        // Check if wave is complete
        if (enemiesLeftToSpawn <= 0 && enemiesAlive <= 0 && !isSpawning)
        {
            EndWave();
        }
    }

    private void EndWave() 
    {
        Debug.Log("Wave " + currentWave + " completed!");
        
        // Invoke event
        onWaveCompleted.Invoke();
        
        // Check if this was the last wave (wave 3 in a 3-wave game)
        if (currentWave == waves.Length)
        {
            allWavesCompleted = true;
            Debug.Log("All " + waves.Length + " waves completed!");
            onAllWavesCompleted.Invoke();
            
            // Notify GameManager - only once per wave
            if (gameManager != null && !waveCompletionRewardGiven[currentWave - 1])
            {
                gameManager.WaveCompleted();
                waveCompletionRewardGiven[currentWave - 1] = true;
                Debug.Log("Wave " + currentWave + " completion reward given");
            }
        }
        else
        {
            // Not the last wave, just normal wave completion
            // Notify GameManager
            if (gameManager != null && !waveCompletionRewardGiven[currentWave - 1])
            {
                gameManager.WaveCompleted();
                waveCompletionRewardGiven[currentWave - 1] = true;
                Debug.Log("Wave " + currentWave + " completion reward given");
            }
            
            // Wait a moment before starting the next coroutine to ensure clean separation
            CancelInvoke("TriggerNextWave");
            Debug.Log("Scheduling next wave trigger in 2.0 seconds. Wave " + (currentWave + 1) + " will start after that with " + timeBetweenWaves + " seconds delay");
            Invoke("TriggerNextWave", 2.0f);
        }
    }
    
    private void TriggerNextWave()
    {
        // Start next wave automatically if enabled and not paused
        if (autoStartNextWave && currentWave < waves.Length && !isPaused)
        {
            // Make sure no enemies are still alive before starting
            if (enemiesAlive <= 0)
            {
                Debug.Log("Starting next wave with delay of " + timeBetweenWaves + " seconds");
                StopAllCoroutines(); // Stop any existing wave coroutines
                StartCoroutine(StartNextWave(-1f)); // Explicitly use the timeBetweenWaves value
            }
            else
            {
                // If enemies are still alive, try again later
                Debug.Log("Enemies still alive, delaying next wave check");
                Invoke("TriggerNextWave", 2.0f);
            }
        }
    }
    
    private void UpdateUI()
    {
        if (waveText != null)
        {
            // Always show the current wave number, even if all waves are completed
            waveText.text = "Wave: " + currentWave + " / " + waves.Length;
        }
        
        if (enemyCountText != null)
        {
            // Show spawned enemies vs total enemies
            enemyCountText.text = "Enemies: " + enemiesSpawned + " / " + totalEnemiesInWave;
        }
    }
    
    // Public methods for external control
    
    public int GetCurrentWave()
    {
        return currentWave;
    }
    
    public int GetTotalWaves()
    {
        return waves.Length;
    }
    
    public int GetRemainingEnemies()
    {
        return totalEnemiesInWave - enemiesSpawned + enemiesAlive;
    }
    
    public bool IsWaveInProgress()
    {
        return isSpawning || enemiesAlive > 0;
    }
    
    // Method to stop all spawning and clear all enemies
    public void StopAndClearEnemies()
    {
        StopAllCoroutines();
        SetPaused(true);
        
        // Find and destroy all enemies with "Enemy" tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        
        // Reset counters
        enemiesAlive = 0;
        isSpawning = false;
        
        Debug.Log("Stopped spawning and cleared " + enemies.Length + " enemies");
    }
}
