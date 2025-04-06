using UnityEngine;
using UnityEngine.Tilemaps;

public class MapInitializer : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap groundTilemap;
    public Tilemap pathTilemap;
    public Tilemap obstaclesTilemap;
    public Tilemap decorationTilemap;
    
    [Header("Waypoints")]
    public Transform waypointsParent;
    
    void Start()
    {
        // Optional: Add initialization logic here
        Debug.Log("Map initialized. Use the tile palette to build your map manually.");
    }
    
    // Draw waypoint path in the editor
    private void OnDrawGizmos()
    {
        if (waypointsParent == null || waypointsParent.childCount == 0) return;
        
        Gizmos.color = Color.yellow;
        
        // Draw connections between waypoints
        for (int i = 0; i < waypointsParent.childCount - 1; i++)
        {
            Transform current = waypointsParent.GetChild(i);
            Transform next = waypointsParent.GetChild(i + 1);
            
            Gizmos.DrawLine(current.position, next.position);
            Gizmos.DrawSphere(current.position, 0.2f);
        }
        
        // Draw the last waypoint
        if (waypointsParent.childCount > 0)
        {
            Gizmos.DrawSphere(waypointsParent.GetChild(waypointsParent.childCount - 1).position, 0.2f);
        }
    }
    
    // Helper method to clear tilemaps if needed
    public void ClearAllTilemaps()
    {
        if (groundTilemap != null) groundTilemap.ClearAllTiles();
        if (pathTilemap != null) pathTilemap.ClearAllTiles();
        if (obstaclesTilemap != null) obstaclesTilemap.ClearAllTiles();
        if (decorationTilemap != null) decorationTilemap.ClearAllTiles();
        
        Debug.Log("All tilemaps cleared");
    }
}
