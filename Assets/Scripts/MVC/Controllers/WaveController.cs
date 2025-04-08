using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefense.Models;

namespace TowerDefense.Controllers
{
    public class WaveController : MonoBehaviour
    {
        [Header("Wave Settings")]
        [SerializeField] private Wave[] waveData;
        [SerializeField] private float timeBetweenWaves = 5f;
        
        [Header("Spawn Settings")]
        [SerializeField] private Transform spawnPoint;
        
        // Models
        private WaveModel waveModel;
        
        // Events relayed from model
        public delegate void WaveCompleted(int waveNumber);
        public static event WaveCompleted OnWaveCompleted;
        
        public delegate void WaveStateChanged();
        public event WaveStateChanged OnWaveStateChanged;
        
        public delegate void AllWavesCompleted();
        public event AllWavesCompleted OnAllWavesCompleted;
        
        private void Awake()
        {
            // Convert legacy Wave array to WaveData array
            WaveData[] convertedWaves = ConvertWaves(waveData);
            
            // Initialize model
            waveModel = new WaveModel(convertedWaves, timeBetweenWaves);
            
            // Subscribe to model events
            waveModel.OnWaveCompleted += HandleWaveCompleted;
            waveModel.OnWaveStateChanged += HandleWaveStateChanged;
            waveModel.OnAllWavesCompleted += HandleAllWavesCompleted;
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from model events
            if (waveModel != null)
            {
                waveModel.OnWaveCompleted -= HandleWaveCompleted;
                waveModel.OnWaveStateChanged -= HandleWaveStateChanged;
                waveModel.OnAllWavesCompleted -= HandleAllWavesCompleted;
            }
        }
        
        private void Update()
        {
            // Check if game is over
            if (GameController.Instance != null && GameController.Instance.IsGameOver())
            {
                // Stop spawning
                return;
            }
            
            // Update model with time
            waveModel.UpdateCountdown(Time.deltaTime);
            
            // Start spawning if countdown reaches zero
            if (waveModel.Countdown <= 0 && !waveModel.FinishedSpawning && waveModel.EnemiesRemainingToSpawn > 0)
            {
                StartCoroutine(SpawnWave());
            }
        }
        
        // Convert legacy Wave array to WaveData array
        private WaveData[] ConvertWaves(Wave[] legacyWaves)
        {
            if (legacyWaves == null)
            {
                return new WaveData[0];
            }
            
            WaveData[] newWaves = new WaveData[legacyWaves.Length];
            
            for (int i = 0; i < legacyWaves.Length; i++)
            {
                newWaves[i] = new WaveData
                {
                    WaveName = legacyWaves[i].waveName,
                    EnemyCount = legacyWaves[i].enemyCount,
                    EnemyPrefab = legacyWaves[i].enemyPrefab,
                    SpawnDelay = legacyWaves[i].spawnDelay
                };
            }
            
            return newWaves;
        }
        
        // Relay wave completed event
        private void HandleWaveCompleted(int waveNumber)
        {
            if (OnWaveCompleted != null)
            {
                OnWaveCompleted(waveNumber);
            }
        }
        
        // Relay wave state changed event
        private void HandleWaveStateChanged()
        {
            if (OnWaveStateChanged != null)
            {
                OnWaveStateChanged();
            }
        }
        
        // Relay all waves completed event
        private void HandleAllWavesCompleted()
        {
            if (OnAllWavesCompleted != null)
            {
                OnAllWavesCompleted();
            }
        }
        
        // Start spawning the current wave
        private IEnumerator SpawnWave()
        {
            // Start spawning
            waveModel.StartSpawning();
            
            // Get current wave
            WaveData currentWave = waveModel.GetCurrentWave();
            
            if (currentWave == null)
            {
                yield break;
            }
            
            // Spawn all enemies in the wave
            for (int i = 0; i < currentWave.EnemyCount; i++)
            {
                SpawnEnemy(currentWave.EnemyPrefab);
                waveModel.EnemySpawned();
                
                // Wait before spawning next enemy
                yield return new WaitForSeconds(currentWave.SpawnDelay);
            }
        }
        
        // Spawn an enemy
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
                    enemyComponent.OnEnemyDeath += EnemyDied;
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
        
        // Called when an enemy dies
        private void EnemyDied()
        {
            waveModel.EnemyRemoved();
        }
        
        // Getters for model properties
        public int GetCurrentWaveIndex()
        {
            return waveModel.CurrentWaveIndex;
        }
        
        public int GetTotalWaves()
        {
            return waveModel.Waves.Length;
        }
        
        public int GetRemainingEnemies()
        {
            return waveModel.EnemiesRemainingAlive;
        }
    }
} 