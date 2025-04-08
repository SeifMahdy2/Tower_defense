using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Models
{
    [System.Serializable]
    public class WaveData
    {
        public string WaveName;
        public int EnemyCount;
        public GameObject EnemyPrefab;
        public float SpawnDelay;
    }
    
    [System.Serializable]
    public class WaveModel
    {
        // Wave properties
        public WaveData[] Waves { get; private set; }
        public int CurrentWaveIndex { get; private set; }
        public int EnemiesRemainingToSpawn { get; private set; }
        public int EnemiesRemainingAlive { get; private set; }
        public float Countdown { get; private set; }
        public bool FinishedSpawning { get; private set; }
        public float TimeBetweenWaves { get; private set; }
        
        // Events
        public delegate void WaveCompleted(int waveNumber);
        public event WaveCompleted OnWaveCompleted;
        
        public delegate void WaveStateChanged();
        public event WaveStateChanged OnWaveStateChanged;
        
        public delegate void AllWavesCompleted();
        public event AllWavesCompleted OnAllWavesCompleted;
        
        // Constructor
        public WaveModel(WaveData[] waves, float timeBetweenWaves = 5f)
        {
            this.Waves = waves;
            this.TimeBetweenWaves = timeBetweenWaves;
            
            // Initialize
            ResetWaves();
        }
        
        // Reset waves to initial state
        public void ResetWaves()
        {
            CurrentWaveIndex = 0;
            Countdown = 3f;
            FinishedSpawning = false;
            
            if (Waves.Length > 0)
            {
                EnemiesRemainingToSpawn = Waves[0].EnemyCount;
            }
            else
            {
                EnemiesRemainingToSpawn = 0;
                Debug.LogError("No waves defined in Wave Model!");
            }
            
            EnemiesRemainingAlive = 0;
            
            NotifyWaveStateChanged();
        }
        
        // Update countdown timer
        public void UpdateCountdown(float deltaTime)
        {
            if (AllWavesCompleted())
            {
                return;
            }
            
            // If current wave is finished and all enemies are dead
            if (FinishedSpawning && EnemiesRemainingAlive == 0)
            {
                CompleteWave();
            }
            
            // Count down to next wave if needed
            if (!FinishedSpawning && EnemiesRemainingToSpawn > 0)
            {
                if (Countdown <= 0f)
                {
                    // Ready to spawn
                    Countdown = 0;
                    return;
                }
                
                Countdown -= deltaTime;
                NotifyWaveStateChanged();
            }
        }
        
        // Start spawning the current wave
        public void StartSpawning()
        {
            FinishedSpawning = false;
            Countdown = TimeBetweenWaves;
            NotifyWaveStateChanged();
        }
        
        // Enemy was spawned
        public void EnemySpawned()
        {
            EnemiesRemainingToSpawn--;
            EnemiesRemainingAlive++;
            
            if (EnemiesRemainingToSpawn <= 0)
            {
                FinishedSpawning = true;
            }
            
            NotifyWaveStateChanged();
        }
        
        // Enemy died or reached the end
        public void EnemyRemoved()
        {
            EnemiesRemainingAlive--;
            NotifyWaveStateChanged();
        }
        
        // Complete the current wave and prepare for the next one
        private void CompleteWave()
        {
            FinishedSpawning = false;
            
            // Trigger wave completed event
            if (OnWaveCompleted != null)
            {
                OnWaveCompleted(CurrentWaveIndex);
            }
            
            // Start countdown to next wave
            if (CurrentWaveIndex < Waves.Length - 1)
            {
                CurrentWaveIndex++;
                EnemiesRemainingToSpawn = Waves[CurrentWaveIndex].EnemyCount;
                Countdown = TimeBetweenWaves;
            }
            else
            {
                // All waves completed
                if (OnAllWavesCompleted != null)
                {
                    OnAllWavesCompleted();
                }
            }
            
            NotifyWaveStateChanged();
        }
        
        // Check if all waves have been completed
        public bool AllWavesCompleted()
        {
            return CurrentWaveIndex >= Waves.Length;
        }
        
        // Get current wave data
        public WaveData GetCurrentWave()
        {
            if (CurrentWaveIndex < Waves.Length)
            {
                return Waves[CurrentWaveIndex];
            }
            
            return null;
        }
        
        // Notify subscribers of wave state change
        private void NotifyWaveStateChanged()
        {
            if (OnWaveStateChanged != null)
            {
                OnWaveStateChanged();
            }
        }
    }
} 