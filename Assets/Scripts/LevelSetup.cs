using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelSetup : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform waypointsParent;
    public Transform startPoint;
    
    [Header("Setup")]
    public bool setupLevel = false;
    
    private void OnValidate()
    {
        if (setupLevel)
        {
            setupLevel = false;
            SetupLevel();
        }
    }
    
    public void SetupLevel()
    {
        // Find or create LevelManager
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            GameObject levelManagerObj = new GameObject("LevelManager");
            levelManager = levelManagerObj.AddComponent<LevelManager>();
            Debug.Log("Created LevelManager");
        }
        
        // Set up waypoints
        if (waypointsParent != null)
        {
            List<Transform> waypoints = new List<Transform>();
            foreach (Transform child in waypointsParent)
            {
                waypoints.Add(child);
            }
            
            levelManager.waypoints = waypoints.ToArray();
            Debug.Log("Set up " + waypoints.Count + " waypoints");
        }
        else
        {
            Debug.LogError("No waypoints parent assigned!");
        }
        
        // Set up start point
        if (startPoint != null)
        {
            levelManager.startPoint = startPoint;
            levelManager.spawnPoint = startPoint;
            Debug.Log("Set up start point");
        }
        else if (levelManager.waypoints != null && levelManager.waypoints.Length > 0)
        {
            levelManager.startPoint = levelManager.waypoints[0];
            levelManager.spawnPoint = levelManager.waypoints[0];
            Debug.Log("Using first waypoint as start point");
        }
        
        // Create GameManager if needed
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            GameObject gameManagerObj = new GameObject("GameManager");
            gameManagerObj.AddComponent<GameManager>();
            Debug.Log("Created GameManager");
        }
    }
    
    // Editor helper function to create empty waypoints
    [ContextMenu("Create Empty Waypoints")]
    public void CreateEmptyWaypoints()
    {
        // Create parent if needed
        if (waypointsParent == null)
        {
            GameObject waypointsObj = new GameObject("Waypoints");
            waypointsParent = waypointsObj.transform;
            waypointsObj.transform.SetParent(transform);
        }
        
        // Create 5 waypoints
        for (int i = 0; i < 5; i++)
        {
            GameObject waypoint = new GameObject("Waypoint_" + i);
            waypoint.transform.SetParent(waypointsParent);
            waypoint.transform.position = transform.position + new Vector3(i * 2, 0, 0);
        }
        
        // Set start point to first waypoint
        if (waypointsParent.childCount > 0)
        {
            startPoint = waypointsParent.GetChild(0);
        }
        
        Debug.Log("Created empty waypoints");
    }
} 