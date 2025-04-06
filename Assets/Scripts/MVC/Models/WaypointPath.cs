using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    [Header("Path Settings")]
    [SerializeField] private Transform[] waypoints;
    
    // Cache the waypoints during start
    private void Awake()
    {
        // If no waypoints were manually assigned, collect them from children
        if (waypoints == null || waypoints.Length == 0)
        {
            waypoints = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                waypoints[i] = transform.GetChild(i);
            }
        }
    }
    
    // Get the position of a specific waypoint
    public Vector3 GetWaypointPosition(int waypointIndex)
    {
        if (waypointIndex < 0 || waypointIndex >= waypoints.Length)
        {
            Debug.LogWarning("Waypoint index out of range: " + waypointIndex);
            return Vector3.zero;
        }
        
        return waypoints[waypointIndex].position;
    }
    
    // Get the total number of waypoints
    public int GetWaypointCount()
    {
        return waypoints != null ? waypoints.Length : 0;
    }
    
    // Draw the path in the editor for easy visualization
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length <= 1)
            return;
        
        Gizmos.color = Color.blue;
        
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i+1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i+1].position);
                Gizmos.DrawSphere(waypoints[i].position, 0.2f);
            }
        }
        
        if (waypoints[waypoints.Length - 1] != null)
        {
            Gizmos.DrawSphere(waypoints[waypoints.Length - 1].position, 0.2f);
        }
    }
} 