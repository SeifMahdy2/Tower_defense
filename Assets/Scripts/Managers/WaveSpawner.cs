using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Wave
{
    public string waveName;
    public int enemyCount;
    public GameObject enemyPrefab;
    public float spawnDelay;
}

public class WaveSpawner : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private Wave[] waves;
    [SerializeField] private float timeBetweenWaves = 5f;
    
    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform waypoints;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI countdownText;
    
    // Private variables
    private int currentWaveIndex = 0;
    private int enemiesRemainingToSpawn;
    private int enemiesRemainingAlive;
    private float countdown = 3f;
    private bool finishedSpawning = false;
    
    // Events
    public delegate void WaveCompleted(int waveNumber);
    public static event WaveCompleted OnWaveCompleted;
    
    private void Start()
    {
        // Initialize for first wave
        if (waves.Length > 0)
        {
            enemiesRemainingToSpawn = waves[0].enemyCount;
            UpdateWaveText();
        }
        else
        {
            Debug.LogError("No waves defined in Wave Spawner!");
        }
    }
    
    private void Update()
    {
        // Check if game is over
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
        {
            // Stop spawning
            return;
        }
        
        // Check if we're done with all waves
        if (currentWaveIndex >= waves.Length)
        {
            // All waves complete
            if (countdownText != null)
            {
                countdownText.text = "All waves completed!";
            }
            return;
        }
        
        // Check if current wave is finished and all enemies are dead
        if (finishedSpawning && enemiesRemainingAlive == 0)
        {
            // End of wave
            finishedSpawning = false;
            
            // Start countdown to next wave
            if (currentWaveIndex < waves.Length - 1)
            {
                countdown = timeBetweenWaves;
                currentWaveIndex++;
                enemiesRemainingToSpawn = waves[currentWaveIndex].enemyCount;
                UpdateWaveText();
            }
            else
            {
                // All waves completed
                if (countdownText != null)
                {
                    countdownText.text = "All waves completed!";
                }
            }
            
            // Trigger wave completed event
            if (OnWaveCompleted != null)
            {
                OnWaveCompleted(currentWaveIndex);
            }
        }
        
        // Count down to next wave if needed
        if (!finishedSpawning && enemiesRemainingToSpawn > 0)
        {
            // Count down to next wave
            if (countdown <= 0f)
            {
                // Start spawning
                StartCoroutine(SpawnWave());
                countdown = timeBetweenWaves;
                return;
            }
            
            countdown -= Time.deltaTime;
            
            // Update countdown UI
            if (countdownText != null)
            {
                countdownText.text = "Next wave in: " + Mathf.RoundToInt(countdown);
            }
        }
    }
    
    private IEnumerator SpawnWave()
    {
        // Start spawning
        finishedSpawning = false;
        
        // Get current wave
        Wave currentWave = waves[currentWaveIndex];
        
        // Spawn all enemies in the wave
        for (int i = 0; i < currentWave.enemyCount; i++)
        {
            SpawnEnemy(currentWave.enemyPrefab);
            enemiesRemainingToSpawn--;
            enemiesRemainingAlive++;
            
            // Wait before spawning next enemy
            yield return new WaitForSeconds(currentWave.spawnDelay);
        }
        
        // All enemies spawned
        finishedSpawning = true;
    }
    
    private void SpawnEnemy(GameObject enemyPrefab)
    {
        // Create enemy
        if (spawnPoint != null && enemyPrefab != null)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            
            // Tell the enemy when it dies
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                // Listen for enemy death
                enemy.GetComponent<Enemy>().OnEnemyDeath += EnemyDied;
            }
            else
            {
                Debug.LogWarning("Enemy prefab missing Enemy component!");
            }
        }
        else
        {
            Debug.LogError("Spawn point or enemy prefab not set!");
        }
    }
    
    private void EnemyDied()
    {
        enemiesRemainingAlive--;
    }
    
    private void UpdateWaveText()
    {
        if (waveText != null)
        {
            waveText.text = "Wave: " + (currentWaveIndex + 1) + "/" + waves.Length;
        }
    }
    
    // Get the current wave index (for UI purposes)
    public int GetCurrentWaveIndex()
    {
        return currentWaveIndex;
    }
    
    // Get the total number of waves
    public int GetTotalWaves()
    {
        return waves.Length;
    }
    
    // Get the number of enemies remaining in the current wave
    public int GetRemainingEnemies()
    {
        return enemiesRemainingAlive;
    }
}

// Add this extension to the Enemy class
public static class EnemyExtensions
{
    // Event for enemy death
    public delegate void EnemyDeathHandler();
    
    // Method to invoke when enemy dies
    public static void OnEnemyDeath(this Enemy enemy, EnemyDeathHandler handler)
    {
        // Would need to implement in Enemy class
    }
} 