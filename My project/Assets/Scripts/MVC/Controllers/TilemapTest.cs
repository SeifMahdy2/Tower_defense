using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapTest : MonoBehaviour
{
    public Tilemap groundTilemap;
    public Tilemap pathTilemap;
    public Tilemap obstaclesTilemap;
    
    public TileBase grassTile;
    public TileBase pathTile;
    public TileBase obstacleTile;
    
    void Start()
    {
        // Test ground tiles
        if (groundTilemap != null && grassTile != null)
        {
            PlaceTestTiles(groundTilemap, grassTile, 0, 0);
            Debug.Log("Ground tiles placed");
        }
        
        // Test path tiles
        if (pathTilemap != null && pathTile != null)
        {
            PlaceTestTiles(pathTilemap, pathTile, 6, 0);
            Debug.Log("Path tiles placed");
        }
        
        // Test obstacle tiles
        if (obstaclesTilemap != null && obstacleTile != null)
        {
            PlaceTestTiles(obstaclesTilemap, obstacleTile, 0, 6);
            Debug.Log("Obstacle tiles placed");
        }
    }
    
    private void PlaceTestTiles(Tilemap tilemap, TileBase tile, int offsetX, int offsetY)
    {
        // Place a 3x3 block of tiles
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                tilemap.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tile);
            }
        }
    }
} 