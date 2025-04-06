# Manual Map Creation Guide

This guide will help you set up your tower defense map manually using Unity's Tile Palette tool.

## Step 1: Understanding the Tilemaps

Your scene should contain a Grid GameObject with several child Tilemaps:
- **GroundTilemap**: For the base terrain (grass)
- **PathTilemap**: For the path enemies will follow
- **ObstaclesTilemap**: For trees, rocks, and other obstacles
- **DecorationsTilemap**: For flowers and decorative elements

## Step 2: Open the Tile Palette

1. Select any Tilemap in the Hierarchy
2. Click "Window → 2D → Tile Palette" in the top menu
3. You should see the Tile Palette window open, showing your available tiles

## Step 3: Create the Ground Layer

1. Select the GroundTilemap in the Hierarchy
2. In the Tile Palette, select your grass tile
3. Paint the entire play area with grass tiles

## Step 4: Create the Path

1. Select the PathTilemap in the Hierarchy
2. In the Tile Palette, select your path tile
3. Paint a winding path from one edge of the map to another
4. Make the path 2-3 tiles wide for better visibility
5. Consider creating interesting turns and bends for strategic tower placement

## Step 5: Add Obstacles and Decorations

1. Select the ObstaclesTilemap in the Hierarchy
2. Paint trees, rocks and other obstacles around your map (not on the path)
3. Switch to the DecorationsTilemap and add flowers and other details

## Step 6: Create Waypoints

1. Create an empty GameObject named "Waypoints" in your Hierarchy
2. Create child empty GameObjects for each turning point along your path
3. Position them at the center of your path, following its course
4. Name them sequentially (e.g., Waypoint1, Waypoint2, etc.)
5. Assign the Waypoints parent to your MapInitializer component

## Step 7: Finishing Up

1. Make sure your Camera is positioned to view the entire map
2. Test by entering Play mode - your map should be visible
3. If using the WaypointFollower component for enemies, they should follow your waypoints

## Tips for Good Map Design

- Create a path that gives towers enough time to attack enemies
- Place obstacles strategically to create natural tower placement areas
- Balance open areas and chokepoints for strategic gameplay
- Use decorations to make the map visually appealing
- Make sure the path is visually distinct from the ground 