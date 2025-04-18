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

    // References
    private TD.GameManager gameManager;
    private WaypointPath path;
    
    // Events
    public UnityEvent onWaveCompleted = new UnityEvent();
    public UnityEvent onAllWavesCompleted = new UnityEvent();
    public static UnityEvent onEnemyDestroyed = new UnityEvent();

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
        isSpawning = false;
        allWavesCompleted = false;
        
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
        if (!isSpawning && !allWavesCompleted && currentWave < waves.Length)
        {
            StopAllCoroutines();
            StartCoroutine(StartNextWave(0f)); // Start immediately
        }
    }

    private IEnumerator StartNextWave(float delay = -1f) 
    {
        // Use the specified delay or default to timeBetweenWaves
        float waitTime = delay >= 0f ? delay : timeBetweenWaves;
        
        // Wait before starting the wave
        yield return new WaitForSeconds(waitTime);
        
        // Increment wave count
        currentWave++;
        
        // Check if we've exceeded max waves
        if (currentWave > waves.Length) 
        {
            allWavesCompleted = true;
            Debug.Log("All " + waves.Length + " waves completed!");
            
            // Invoke event
            onAllWavesCompleted.Invoke();
            
            // Notify GameManager
            if (gameManager != null)
            {
                gameManager.WaveCompleted();
            }
            
            yield break;
        }
        
        // Get current wave
        Wave wave = waves[currentWave - 1];
        
        // Start the wave
        Debug.Log("Starting Wave " + currentWave + " of " + waves.Length + ": " + wave.waveName);
        
        // Calculate total enemies
        enemiesLeftToSpawn = wave.easyEnemyCount + wave.mediumEnemyCount + wave.hardEnemyCount;
        isSpawning = true;
        
        // Spawn enemies in this wave
        yield return StartCoroutine(SpawnWave(wave));
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        // Spawn easy enemies
        for (int i = 0; i < wave.easyEnemyCount; i++)
        {
            SpawnEnemy(easyEnemyPrefab);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
        
        // Wait between groups
        if (wave.easyEnemyCount > 0 && (wave.mediumEnemyCount > 0 || wave.hardEnemyCount > 0))
            yield return new WaitForSeconds(wave.timeBetweenGroups);
        
        // Spawn medium enemies
        for (int i = 0; i < wave.mediumEnemyCount; i++)
        {
            SpawnEnemy(mediumEnemyPrefab);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
        
        // Wait between groups
        if (wave.mediumEnemyCount > 0 && wave.hardEnemyCount > 0)
            yield return new WaitForSeconds(wave.timeBetweenGroups);
        
        // Spawn hard enemies
        for (int i = 0; i < wave.hardEnemyCount; i++)
        {
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
        enemy.transform.parent = transform; // Parent to spawner for organization
        enemiesLeftToSpawn--;
        enemiesAlive++;
    }

    private void EnemyDestroyed() 
    {
        enemiesAlive--;
        
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
        
        // Notify GameManager
        if (gameManager != null)
        {
            gameManager.WaveCompleted();
        }
        
        // Start next wave automatically if enabled
        if (autoStartNextWave && currentWave < waves.Length)
        {
            StartCoroutine(StartNextWave());
        }
    }
    
    private void UpdateUI()
    {
        if (waveText != null)
        {
            if (allWavesCompleted)
                waveText.text = "All Waves Complete!";
            else
                waveText.text = "Wave: " + currentWave + " / " + waves.Length;
        }
        
        if (enemyCountText != null)
        {
            enemyCountText.text = "Enemies: " + enemiesAlive;
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
        return enemiesLeftToSpawn + enemiesAlive;
    }
    
    public bool IsWaveInProgress()
    {
        return isSpawning || enemiesAlive > 0;
    }
}
