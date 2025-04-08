using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [Header("References")]
    public Transform startPoint;
    public Transform[] waypoints;
    public Transform spawnPoint;

    private void Awake()
    {
        // Singleton pattern
        if (main != null && main != this)
        {
            Destroy(gameObject);
            return;
        }
        main = this;
    }

    private void Start()
    {
        // Find waypoints if not set
        if (waypoints == null || waypoints.Length == 0)
        {
            GameObject waypointParent = GameObject.Find("Waypoints");
            if (waypointParent != null)
            {
                List<Transform> waypointList = new List<Transform>();
                foreach (Transform child in waypointParent.transform)
                {
                    waypointList.Add(child);
                }
                waypoints = waypointList.ToArray();
            }
        }

        // Find start point if not set
        if (startPoint == null)
        {
            GameObject startObj = GameObject.Find("Start Point");
            if (startObj != null)
            {
                startPoint = startObj.transform;
            }
            else if (waypoints != null && waypoints.Length > 0)
            {
                startPoint = waypoints[0];
            }
        }

        // Set spawn point if not set
        if (spawnPoint == null)
        {
            if (startPoint != null)
            {
                spawnPoint = startPoint;
            }
        }
    }

    public Transform GetStartPoint()
    {
        return startPoint;
    }

    public Transform[] GetWaypoints()
    {
        return waypoints;
    }
}
