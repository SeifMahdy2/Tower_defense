using UnityEngine;
using UnityEngine.Tilemaps;

public class ClearTilemaps : MonoBehaviour
{
    public Tilemap[] tilemaps;
    
    // Call this from the Inspector
    public void ClearAllTilemaps()
    {
        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap != null)
            {
                tilemap.ClearAllTiles();
                Debug.Log("Cleared " + tilemap.name);
            }
        }
    }
    
    // Optional: Clear all tilemaps on Start
    void Start()
    {
        // Uncomment this to clear on start
        // ClearAllTilemaps();
    }
} 