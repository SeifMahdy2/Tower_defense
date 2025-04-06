using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapHelper : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap groundTilemap;
    public Tilemap pathTilemap;
    public Tilemap obstaclesTilemap;
    public Tilemap decorationTilemap;

    [Header("Waypoints")]
    public Transform waypointsParent;
    public bool showGizmos = true;

    private void OnDrawGizmos()
    {
        if (!showGizmos || waypointsParent == null) return;

        // Draw a path connecting all waypoints
        Gizmos.color = Color.yellow;
        
        Transform[] waypoints = waypointsParent.GetComponentsInChildren<Transform>();
        
        // Skip the first one as it's the parent transform
        for (int i = 1; i < waypoints.Length - 1; i++)
        {
            // Draw a line between consecutive waypoints
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            
            // Draw a sphere at each waypoint
            Gizmos.DrawSphere(waypoints[i].position, 0.2f);
        }
        
        // Draw the last waypoint if there are any waypoints
        if (waypoints.Length > 1)
        {
            Gizmos.DrawSphere(waypoints[waypoints.Length - 1].position, 0.2f);
        }
    }
    
    // Helper method to get world position from tilemap cell position
    public Vector3 GetWorldPosition(Tilemap tilemap, Vector3Int cellPosition)
    {
        return tilemap.GetCellCenterWorld(cellPosition);
    }
    
    // Help with clearing tilemaps if needed
    public void ClearAllTilemaps()
    {
        if (groundTilemap != null) groundTilemap.ClearAllTiles();
        if (pathTilemap != null) pathTilemap.ClearAllTiles();
        if (obstaclesTilemap != null) obstaclesTilemap.ClearAllTiles();
        if (decorationTilemap != null) decorationTilemap.ClearAllTiles();
        
        Debug.Log("All tilemaps cleared.");
    }
} 