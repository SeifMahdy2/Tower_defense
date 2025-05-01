using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTime : MonoBehaviour
{
    private bool isActive = false;
    private int damagePerTick = 0;
    private float duration = 0f;
    private float tickInterval = 0.5f; // Apply damage every half second
    private float timeSinceLastTick = 0f;
    private float totalTimeActive = 0f;
    
    private EnemyHealth enemyHealth;
    private ParticleSystem dotEffectParticles;
    
    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        
        // Create a simple particle effect for the DoT
        CreateDotVisualEffect();
    }
    
    private void CreateDotVisualEffect()
    {
        // Check if we already have a particle system
        dotEffectParticles = GetComponentInChildren<ParticleSystem>();
        
        if (dotEffectParticles == null)
        {
            // Create a new game object for the particles
            GameObject particleObj = new GameObject("DotEffect");
            particleObj.transform.SetParent(transform);
            particleObj.transform.localPosition = Vector3.zero;
            
            // Add a particle system
            dotEffectParticles = particleObj.AddComponent<ParticleSystem>();
            
            // Configure a simple purple particle effect for the DoT
            var main = dotEffectParticles.main;
            main.startColor = new Color(0.8f, 0.3f, 0.8f); // Purple color
            main.startSize = 0.3f;
            main.startLifetime = 0.5f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            
            var emission = dotEffectParticles.emission;
            emission.rateOverTime = 10f;
            
            // Start with particles disabled
            dotEffectParticles.Stop();
        }
    }
    
    public void ApplyDotEffect(int damage, float effectDuration)
    {
        // Set or refresh the effect
        damagePerTick = damage;
        duration = effectDuration;
        isActive = true;
        totalTimeActive = 0f;
        
        // Show the effect
        if (dotEffectParticles != null && !dotEffectParticles.isPlaying)
        {
            dotEffectParticles.Play();
        }
    }
    
    private void Update()
    {
        if (!isActive || enemyHealth == null) return;
        
        // Add to our timers
        timeSinceLastTick += Time.deltaTime;
        totalTimeActive += Time.deltaTime;
        
        // Apply damage at regular intervals
        if (timeSinceLastTick >= tickInterval)
        {
            ApplyDamageTick();
            timeSinceLastTick = 0f;
        }
        
        // Check if effect has expired
        if (totalTimeActive >= duration)
        {
            StopDotEffect();
        }
    }
    
    private void ApplyDamageTick()
    {
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damagePerTick);
            Debug.Log($"DoT applied {damagePerTick} damage to {gameObject.name}");
        }
    }
    
    private void StopDotEffect()
    {
        isActive = false;
        
        // Stop particles
        if (dotEffectParticles != null)
        {
            dotEffectParticles.Stop();
        }
    }
    
    private void OnDisable()
    {
        StopDotEffect();
    }
} 