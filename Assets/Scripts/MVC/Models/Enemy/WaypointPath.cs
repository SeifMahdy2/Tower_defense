using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool drawGizmos = true;
    [SerializeField] private float waypointSize = 0.7f;
    [SerializeField] private Color pathColor = Color.red;
    
    private Transform[] waypoints;
    
    private void Awake()
    {
        // Initialize the waypoints array using child transforms
        InitializeWaypoints();
    }
    
    private void InitializeWaypoints()
    {
        // Count how many child objects we have
        int waypointCount = transform.childCount;
        
        // Initialize the array
        waypoints = new Transform[waypointCount];
        
        // Fill the array with the child transforms
        for (int i = 0; i < waypointCount; i++)
        {
            waypoints[i] = transform.GetChild(i);
        }
    }
    
    // Get all waypoints
    public Transform[] GetWaypoints()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            InitializeWaypoints();
        }
        
        return waypoints;
    }
    
    // Get a specific waypoint by index
    public Transform GetWaypoint(int index)
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            InitializeWaypoints();
        }
        
        if (index >= 0 && index < waypoints.Length)
        {
            return waypoints[index];
        }
        else
        {
            Debug.LogWarning("Waypoint index out of range: " + index);
            return null;
        }
    }
    
    // Get the number of waypoints
    public int GetWaypointsCount()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            InitializeWaypoints();
        }
        
        return waypoints.Length;
    }
    
    // Get the first waypoint
    public Transform GetFirstWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            InitializeWaypoints();
        }
        
        if (waypoints.Length > 0)
        {
            return waypoints[0];
        }
        else
        {
            Debug.LogWarning("No waypoints found!");
            return null;
        }
    }
    
    // Get the last waypoint
    public Transform GetLastWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            InitializeWaypoints();
        }
        
        if (waypoints.Length > 0)
        {
            return waypoints[waypoints.Length - 1];
        }
        else
        {
            Debug.LogWarning("No waypoints found!");
            return null;
        }
    }
    
    // Draw gizmos in the editor to visualize the path
    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;
            
        // Initialize waypoints if needed
        if (waypoints == null || waypoints.Length == 0)
        {
            // Count how many child objects we have
            int waypointCount = transform.childCount;
            
            // Temporary array for gizmo drawing
            Transform[] tempWaypoints = new Transform[waypointCount];
            
            // Fill the array with the child transforms
            for (int i = 0; i < waypointCount; i++)
            {
                tempWaypoints[i] = transform.GetChild(i);
            }
            
            // Draw spheres and lines
            DrawWaypointGizmos(tempWaypoints);
        }
        else
        {
            // Draw spheres and lines using existing waypoints
            DrawWaypointGizmos(waypoints);
        }
    }
    
    private void DrawWaypointGizmos(Transform[] points)
    {
        // Set gizmo color
        Gizmos.color = pathColor;
        
        // Draw lines between waypoints
        for (int i = 0; i < points.Length - 1; i++)
        {
            if (points[i] != null && points[i+1] != null)
            {
                // Draw line from current waypoint to next waypoint
                Gizmos.DrawLine(points[i].position, points[i+1].position);
                
                // Draw sphere at waypoint position
                Gizmos.DrawSphere(points[i].position, waypointSize);
            }
        }
        
        // Draw sphere for the last waypoint
        if (points.Length > 0 && points[points.Length-1] != null)
        {
            Gizmos.DrawSphere(points[points.Length-1].position, waypointSize);
        }
    }
} 