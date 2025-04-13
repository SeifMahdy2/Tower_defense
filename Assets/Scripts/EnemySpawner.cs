using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Attributes")]
    [SerializeField] private int baseEnemies = 8;
    [SerializeField] private float spawnRate = 0.5f;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float difficultyScalingFactor = 0.75f;
    [SerializeField] private int maxWaves = 3; // Limit to 3 waves

    [Header("Events")]
    public static UnityEvent onEnemyDestroyed = new UnityEvent();

    [Header("Debug")]
    [SerializeField] private int currentWave = 0; // Start at 0 so first wave becomes 1
    [SerializeField] private float timeSinceLastSpawn;
    [SerializeField] private int enemiesLeftToSpawn;
    [SerializeField] private int enemiesAlive;
    [SerializeField] private bool isSpawning = false; // Start as false to wait for first wave
    [SerializeField] private bool allWavesCompleted = false;

    private void Awake() {
        // Clear any previous listeners to prevent duplicates
        if (onEnemyDestroyed != null) {
            onEnemyDestroyed.RemoveAllListeners();
        }
        onEnemyDestroyed = new UnityEvent();
        onEnemyDestroyed.AddListener(EnemyDestroyed);
    }

    private void Start() {
        // Reset values to make sure we're starting fresh
        currentWave = 0;
        enemiesLeftToSpawn = 0;
        enemiesAlive = 0;
        isSpawning = false;
        allWavesCompleted = false;
        
        Debug.Log("Game starting! First wave in " + timeBetweenWaves + " seconds...");
        StartCoroutine(StartNextWave());
    }

    private void Update() {
        // If all waves are completed, do nothing
        if (allWavesCompleted) {
            return;
        }

        // If currently spawning, handle enemy spawning
        if (isSpawning && enemiesLeftToSpawn > 0) {
            timeSinceLastSpawn += Time.deltaTime;
            if (timeSinceLastSpawn >= 1f / spawnRate) {
                SpawnEnemy();
                timeSinceLastSpawn = 0f;
            }
        }
        
        // Check if wave is complete (all enemies spawned and all enemies defeated)
        if (isSpawning && enemiesLeftToSpawn <= 0 && enemiesAlive <= 0) {
            EndWave();
        }
    }

    private void SpawnEnemy() {
        if (enemiesLeftToSpawn <= 0) {
            return; // Safety check
        }
        
        if (enemyPrefabs.Length > 0) {
            GameObject prefabToSpawn = enemyPrefabs[0];
            GameObject enemy = Instantiate(prefabToSpawn, LevelManager.main.startPoint.position, Quaternion.identity);
            enemy.transform.parent = transform; // Parent to spawner for organization
            enemiesLeftToSpawn--;
            enemiesAlive++;
            Debug.Log("Spawning enemy. Enemies left to spawn: " + enemiesLeftToSpawn + ", Alive: " + enemiesAlive);
        } else {
            Debug.LogError("No enemy prefabs assigned to spawner!");
        }
    }

    private void EnemyDestroyed() {
        enemiesAlive--;
        Debug.Log("Enemy destroyed. Enemies Alive: " + enemiesAlive);
    }

    private IEnumerator StartNextWave() {
        // Wait before starting the wave
        yield return new WaitForSeconds(timeBetweenWaves);
        
        // Increment wave count before starting new wave
        currentWave++;
        
        // Check if we've exceeded max waves
        if (currentWave > maxWaves) {
            allWavesCompleted = true;
            Debug.Log("All " + maxWaves + " waves completed! You win!");
            yield break;
        }
        
        // Start the wave
        Debug.Log("Starting Wave " + currentWave + " of " + maxWaves);
        enemiesLeftToSpawn = EnemiesPerWave();
        isSpawning = true;
    }

    private void EndWave() {
        Debug.Log("Wave " + currentWave + " completed!");
        isSpawning = false;
        timeSinceLastSpawn = 0f;
        
        // Start next wave using coroutine
        StartCoroutine(StartNextWave());
    }

    private int EnemiesPerWave() {
        return Mathf.RoundToInt(baseEnemies * Mathf.Pow(currentWave, difficultyScalingFactor));
    }
}
