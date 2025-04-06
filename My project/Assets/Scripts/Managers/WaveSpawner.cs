using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveSpawner : MonoBehaviour
{
    // Singleton instance
    public static WaveSpawner Instance { get; private set; }
    
    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private WaypointPath path;
    [SerializeField] private float timeBetweenEnemies = 1f;
    
    [Header("Wave Configuration")]
    [SerializeField] private int maxWaves = 10;
    [SerializeField] private float waveScalingFactor = 1.2f;  // Increase difficulty per wave
    
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject basicEnemyPrefab;
    [SerializeField] private GameObject fastEnemyPrefab;
    [SerializeField] private GameObject tankEnemyPrefab;
    [SerializeField] private GameObject bossEnemyPrefab;
    
    // Events
    public static UnityEvent OnWaveCompleted = new UnityEvent();
    
    // Active enemies
    private List<EnemyController> activeEnemies = new List<EnemyController>();
    private int enemiesRemainingToSpawn;
    private int totalEnemiesInWave;
    
    // Wave status
    private int currentWave;
    private bool isSpawning;
    private Coroutine spawnCoroutine;
    
    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Initialize
        if (spawnPoint == null)
            spawnPoint = transform;
    }
    
    // Start a new wave
    public void StartWave(int waveNumber)
    {
        if (isSpawning) return;
        
        currentWave = waveNumber;
        
        // Calculate number of enemies for this wave
        int baseEnemyCount = 5;
        int enemyCount = Mathf.RoundToInt(baseEnemyCount * Mathf.Pow(waveScalingFactor, currentWave - 1));
        
        enemiesRemainingToSpawn = enemyCount;
        totalEnemiesInWave = enemyCount;
        
        // Start spawning enemies
        isSpawning = true;
        
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
            
        spawnCoroutine = StartCoroutine(SpawnWave());
    }
    
    // Spawn enemies for the current wave
    private IEnumerator SpawnWave()
    {
        // Wait a moment before starting to spawn
        yield return new WaitForSeconds(1f);
        
        while (enemiesRemainingToSpawn > 0)
        {
            // Spawn an enemy
            SpawnEnemy();
            enemiesRemainingToSpawn--;
            
            // Wait before spawning next enemy
            yield return new WaitForSeconds(timeBetweenEnemies);
        }
        
        // All enemies spawned for this wave
        isSpawning = false;
        
        // Check if wave is complete
        CheckWaveComplete();
    }
    
    // Spawn a single enemy
    private void SpawnEnemy()
    {
        GameObject enemyPrefab = SelectEnemyPrefab();
        if (enemyPrefab == null || path == null)
            return;
            
        GameObject enemyObject = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        
        EnemyController enemy = enemyObject.GetComponent<EnemyController>();
        if (enemy != null)
        {
            // Apply wave scaling to make enemies tougher as waves progress
            EnemyModel enemyData = new EnemyModel();
            
            // Scale health and damage based on wave number
            float healthMultiplier = 1 + ((currentWave - 1) * 0.2f);  // +20% per wave
            enemyData.maxHealth *= healthMultiplier;
            enemyData.currentHealth = enemyData.maxHealth;
            
            float rewardMultiplier = 1 + ((currentWave - 1) * 0.1f);  // +10% per wave
            enemyData.goldReward = Mathf.RoundToInt(enemyData.goldReward * rewardMultiplier);
            
            // Initialize the enemy
            enemy.Initialize(path, enemyData);
            
            // Track active enemies
            activeEnemies.Add(enemy);
            
            // Listen for enemy destruction
            StartCoroutine(WaitForEnemyDestruction(enemy));
        }
    }
    
    // Select which enemy type to spawn based on wave number
    private GameObject SelectEnemyPrefab()
    {
        if (basicEnemyPrefab == null)
            return null;
            
        // Progressively unlock new enemy types as waves advance
        List<GameObject> availablePrefabs = new List<GameObject>();
        
        // Always have basic enemies
        availablePrefabs.Add(basicEnemyPrefab);
        
        // Fast enemies from wave 2
        if (currentWave >= 2 && fastEnemyPrefab != null)
            availablePrefabs.Add(fastEnemyPrefab);
            
        // Tank enemies from wave 3
        if (currentWave >= 3 && tankEnemyPrefab != null)
            availablePrefabs.Add(tankEnemyPrefab);
            
        // Special handling for boss waves (every 5th wave)
        if (currentWave % 5 == 0 && bossEnemyPrefab != null)
        {
            // Higher chance of boss on boss waves
            float bossChance = 0.3f;
            if (Random.value < bossChance)
                return bossEnemyPrefab;
        }
        
        // Randomly select from available prefabs
        return availablePrefabs[Random.Range(0, availablePrefabs.Count)];
    }
    
    // Wait for enemy to be destroyed and update active enemies
    private IEnumerator WaitForEnemyDestruction(EnemyController enemy)
    {
        // Wait until the enemy is deactivated
        while (enemy != null && enemy.gameObject.activeSelf)
        {
            yield return null;
        }
        
        // Remove from active enemies
        activeEnemies.Remove(enemy);
        
        // Check if wave is complete
        CheckWaveComplete();
    }
    
    // Check if all enemies in the wave have been defeated
    private void CheckWaveComplete()
    {
        if (!isSpawning && activeEnemies.Count == 0)
        {
            // Wave complete
            Debug.Log("Wave " + currentWave + " complete!");
            
            // Notify listeners
            OnWaveCompleted.Invoke();
            
            // Notify game manager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndCurrentWave();
                
                // Check if game is complete (all waves defeated)
                if (currentWave >= maxWaves)
                {
                    GameManager.Instance.SetGameState(GameState.Victory);
                }
            }
        }
    }
    
    // Get the current wave progress (for UI)
    public float GetWaveProgress()
    {
        int enemiesRemaining = enemiesRemainingToSpawn + activeEnemies.Count;
        if (totalEnemiesInWave <= 0)
            return 0f;
            
        return 1f - (enemiesRemaining / (float)totalEnemiesInWave);
    }
    
    // Get enemies remaining count (for UI)
    public int GetEnemiesRemaining()
    {
        return enemiesRemainingToSpawn + activeEnemies.Count;
    }
} 