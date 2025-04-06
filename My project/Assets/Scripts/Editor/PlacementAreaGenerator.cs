using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class PlacementAreaGenerator : EditorWindow
{
    private Transform pathParent;
    private Tilemap pathTilemap;
    private float placementAreaSize = 1.0f;
    private float pathOffset = 1.5f;
    private float minDistanceBetweenAreas = 2.0f;
    private int areasPerSide = 1;
    private GameObject placementAreaPrefab;
    private GameObject placementAreasParent;
    
    [MenuItem("Tower Defense/Placement Area Generator")]
    public static void ShowWindow()
    {
        GetWindow<PlacementAreaGenerator>("Placement Area Generator");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Generate Tower Placement Areas", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        pathParent = EditorGUILayout.ObjectField("Path Waypoints Parent", pathParent, typeof(Transform), true) as Transform;
        pathTilemap = EditorGUILayout.ObjectField("Path Tilemap", pathTilemap, typeof(Tilemap), true) as Tilemap;
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Placement Area Settings", EditorStyles.boldLabel);
        placementAreaSize = EditorGUILayout.FloatField("Area Size", placementAreaSize);
        pathOffset = EditorGUILayout.FloatField("Path Offset", pathOffset);
        minDistanceBetweenAreas = EditorGUILayout.FloatField("Min Distance Between Areas", minDistanceBetweenAreas);
        areasPerSide = EditorGUILayout.IntField("Areas Per Side", areasPerSide);
        
        EditorGUILayout.Space();
        
        placementAreaPrefab = EditorGUILayout.ObjectField("Placement Area Prefab", placementAreaPrefab, typeof(GameObject), false) as GameObject;
        placementAreasParent = EditorGUILayout.ObjectField("Placement Areas Parent", placementAreasParent, typeof(GameObject), true) as GameObject;
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Create Placement Areas From Path", GUILayout.Height(30)))
        {
            CreatePlacementAreasFromPath();
        }
        
        if (GUILayout.Button("Clear All Placement Areas", GUILayout.Height(30)))
        {
            ClearPlacementAreas();
        }
    }
    
    private void CreatePlacementAreasFromPath()
    {
        if (pathParent == null && pathTilemap == null)
        {
            EditorUtility.DisplayDialog("Error", "You must specify either Path Waypoints or Path Tilemap.", "OK");
            return;
        }
        
        // Create parent object for placement areas if needed
        if (placementAreasParent == null)
        {
            placementAreasParent = new GameObject("TowerPlacementAreas");
            Undo.RegisterCreatedObjectUndo(placementAreasParent, "Create Placement Areas Parent");
        }
        
        List<Vector3> pathPoints = new List<Vector3>();
        
        // Get path points from parent or tilemap
        if (pathParent != null)
        {
            // Get waypoints from parent
            for (int i = 0; i < pathParent.childCount; i++)
            {
                pathPoints.Add(pathParent.GetChild(i).position);
            }
        }
        else if (pathTilemap != null)
        {
            // Get points from tilemap
            BoundsInt bounds = pathTilemap.cellBounds;
            
            for (int x = bounds.min.x; x < bounds.max.x; x++)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y++)
                {
                    Vector3Int cellPos = new Vector3Int(x, y, 0);
                    if (pathTilemap.HasTile(cellPos))
                    {
                        pathPoints.Add(pathTilemap.GetCellCenterWorld(cellPos));
                    }
                }
            }
            
            // Sort path points (this is a simplified approach and may not work for complex paths)
            if (pathPoints.Count > 1)
            {
                List<Vector3> sortedPoints = new List<Vector3>();
                Vector3 current = pathPoints[0];
                sortedPoints.Add(current);
                pathPoints.RemoveAt(0);
                
                while (pathPoints.Count > 0)
                {
                    float minDist = float.MaxValue;
                    int nextIndex = -1;
                    
                    for (int i = 0; i < pathPoints.Count; i++)
                    {
                        float dist = Vector3.Distance(current, pathPoints[i]);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            nextIndex = i;
                        }
                    }
                    
                    if (nextIndex != -1)
                    {
                        current = pathPoints[nextIndex];
                        sortedPoints.Add(current);
                        pathPoints.RemoveAt(nextIndex);
                    }
                }
                
                pathPoints = sortedPoints;
            }
        }
        
        if (pathPoints.Count < 2)
        {
            EditorUtility.DisplayDialog("Error", "Path must have at least 2 points.", "OK");
            return;
        }
        
        // Generate placement areas along path
        List<Vector3> placementPositions = new List<Vector3>();
        
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Vector3 start = pathPoints[i];
            Vector3 end = pathPoints[i + 1];
            Vector3 direction = (end - start).normalized;
            Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0).normalized;
            
            // Calculate how many areas to place along this segment
            float segmentLength = Vector3.Distance(start, end);
            int areasPerSegment = Mathf.FloorToInt(segmentLength / minDistanceBetweenAreas);
            
            for (int j = 0; j <= areasPerSegment; j++)
            {
                // Skip first point except on first segment
                if (j == 0 && i > 0) continue;
                
                float t = j / (float)areasPerSegment;
                Vector3 pointOnPath = Vector3.Lerp(start, end, t);
                
                // Create areas on both sides of the path
                for (int side = -1; side <= 1; side += 2)
                {
                    for (int area = 0; area < areasPerSide; area++)
                    {
                        float currentOffset = pathOffset + (area * placementAreaSize * 1.1f);
                        Vector3 areaPos = pointOnPath + (perpendicular * side * currentOffset);
                        
                        // Check if too close to existing positions
                        bool tooClose = false;
                        foreach (Vector3 existingPos in placementPositions)
                        {
                            if (Vector3.Distance(areaPos, existingPos) < minDistanceBetweenAreas)
                            {
                                tooClose = true;
                                break;
                            }
                        }
                        
                        if (!tooClose)
                        {
                            placementPositions.Add(areaPos);
                            CreatePlacementArea(areaPos);
                        }
                    }
                }
            }
        }
        
        Debug.Log($"Created {placementPositions.Count} tower placement areas");
        EditorUtility.SetDirty(placementAreasParent);
    }
    
    private void CreatePlacementArea(Vector3 position)
    {
        GameObject areaObj;
        
        if (placementAreaPrefab != null)
        {
            areaObj = PrefabUtility.InstantiatePrefab(placementAreaPrefab) as GameObject;
        }
        else
        {
            areaObj = new GameObject("PlacementArea");
            areaObj.AddComponent<BoxCollider2D>();
            
            // Add TowerPlacementArea component
            areaObj.AddComponent<TowerPlacementArea>();
        }
        
        Undo.RegisterCreatedObjectUndo(areaObj, "Create Placement Area");
        
        // Set position and parent
        areaObj.transform.position = position;
        areaObj.transform.SetParent(placementAreasParent.transform);
        
        // Configure collider
        BoxCollider2D collider = areaObj.GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.size = new Vector2(placementAreaSize, placementAreaSize);
            collider.isTrigger = true;
        }
        
        // Set to PlacementArea layer if it exists
        int layerIndex = LayerMask.NameToLayer("PlacementArea");
        if (layerIndex != -1)
        {
            areaObj.layer = layerIndex;
        }
    }
    
    private void ClearPlacementAreas()
    {
        if (placementAreasParent == null) return;
        
        // Clear all children
        while (placementAreasParent.transform.childCount > 0)
        {
            Undo.DestroyObjectImmediate(placementAreasParent.transform.GetChild(0).gameObject);
        }
        
        EditorUtility.SetDirty(placementAreasParent);
    }
} 