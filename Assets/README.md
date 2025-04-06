# Tower Defense Game - Setup Guide

## 🧩 Step-by-Step Setup Process

### Step 1: Import Your Assets
1. Copy your `@enviroment_sprites.png` into the Assets folder
2. Import your tower and enemy sprites to use in the game

### Step 2: Set Up Your Map
1. Open the MainScene in the Scenes folder
2. Use the Tile Palette to manually create your map:
   - Select a Tilemap (Ground, Path, Obstacles, Decoration)
   - Use the Tile Palette window to paint tiles directly onto the scene
   - Start with the ground layer, then add paths, obstacles, and decorations

### Step 3: Set Up Tower Placement Areas
1. In Unity, go to menu Tower Defense → Placement Area Generator
2. Set the Path Waypoints Parent to the PathWaypoints object created by MapBuilder
3. Configure the placement area settings (size, offset from path, etc.)
4. Click "Create Placement Areas From Path" to generate tower placement areas along the path

### Step 4: Create Game Managers
1. Create a new empty GameObject and name it "GameManager"
2. Add the `GameManager` component to it
3. Create another empty GameObject and name it "WaveSpawner"
4. Add the `WaveSpawner` component to it
5. Assign the PathWaypoints to the WaveSpawner's path field

### Step 5: Create Tower Prefabs
1. Create a new prefab for each tower type (Archer, Mage, Frost, Cannon)
2. Add a sprite renderer to each and assign the appropriate sprite
3. Add a circle collider to each
4. Add the `TowerController` component to each
5. Configure each tower's settings (range, damage, attack speed, etc.)

### Step 6: Create Enemy Prefabs
1. Create a new prefab for each enemy type (Basic, Fast, Tank, Boss)
2. Add a sprite renderer to each and assign the appropriate sprite
3. Add a circle collider to each
4. Add the `EnemyController` component to each
5. Configure each enemy's settings (health, speed, etc.)

### Step 7: Create the UI
1. Create a Canvas in your scene
2. Add UI elements for:
   - Gold display
   - Lives display
   - Wave information
   - Tower selection buttons
   - Start wave button
3. Add the `UIController` component to the Canvas
4. Configure the UI references in the UIController inspector

### Step 8: Test Your Game
1. Make sure all components are properly connected
2. Press Play and start placing towers!
3. Start a wave and watch your towers defend against the enemies

## 🛠️ Available Tools

- **MapBuilder**: Creates your game map using tilemaps
- **PlacementAreaGenerator**: Creates tower placement areas along the path
- **TowerPlacementController**: Handles tower placement and upgrading
- **WaveSpawner**: Manages enemy wave spawning
- **GameManager**: Manages overall game state

## 📋 Game Mechanics

- Place towers on designated areas along the path
- Towers attack enemies automatically when they're in range
- Different tower types have different abilities:
  - Archer Tower: Fast firing, low damage
  - Mage Tower: Area damage, medium speed
  - Frost Tower: Slows enemies
  - Cannon Tower: High damage, slow rate
- Earn gold by defeating enemies
- Use gold to place and upgrade towers
- Defend your base from waves of increasingly difficult enemies
- Win by surviving all waves or lose if too many enemies reach your base

## 🧪 Customization

- Modify tower properties in their respective Model classes
- Adjust enemy difficulty and rewards in the WaveSpawner
- Change the path layout in the MapBuilder
- Add more tower types or enemy types by extending the base classes

## Scripts
- **TileMapHelper/MapInitializer**: Visualizes waypoints and provides tilemap utility functions

Enjoy building your tower defense game! 