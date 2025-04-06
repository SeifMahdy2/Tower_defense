# 🏰 Tower Defense Game Implementation Guide

## 📋 Overview
This is a 2D Tower Defense game built in Unity. Players defend a castle gate from waves of enemies by strategically placing and upgrading towers along an enemy path.

## 🚀 Setup Instructions

### Required Components

1. **Enemy Path**:
   - Create an empty GameObject named "Path"
   - Add multiple child objects with transforms to define waypoints
   - Add the `WaypointPath` component to the path object

2. **Enemy Prefabs**:
   - Create basic sprites for each enemy type (Basic, Fast, Tank, Boss)
   - Add SpriteRenderer, Rigidbody2D (Kinematic), CircleCollider2D
   - Add the `EnemyController` component
   - Tag them as "Enemy"

3. **Tower Prefabs**:
   - Create sprites for each tower type (Archer, Mage, Frost, Cannon)
   - Add SpriteRenderer and CircleCollider2D
   - Add the `TowerController` component
   - Tag them as "Tower"

4. **Projectile Prefabs** (for towers):
   - Create a basic projectile sprite
   - Add the `Projectile` component
   - Configure speed and visual properties

5. **Placement Areas**:
   - Create a grid or specific areas where towers can be placed
   - Add BoxCollider2D components
   - Set them to a separate "PlacementArea" layer

6. **Game Managers**:
   - Create empty GameObjects for "GameManager" and "WaveSpawner"
   - Add the respective script components

7. **UI Elements**:
   - Create Canvas with proper UI elements as defined in UIController
   - Panels: Main Menu, HUD, Pause, Game Over, Victory
   - Add TextMeshPro elements for displaying gold, lives, waves
   - Add buttons for tower selection and wave control

## 📐 Implementation Steps

1. **Set up the Scene**:
   - Create the enemy path with waypoints
   - Add placement areas for towers
   - Design a background/terrain

2. **Configure Game Managers**:
   - Add GameManager and WaveSpawner to your scene
   - Link the path to the WaveSpawner
   - Set up spawn points

3. **Create Prefabs**:
   - Design enemy prefabs with appropriate sprites
   - Design tower prefabs with range indicators
   - Create projectile prefabs

4. **Set up UI**:
   - Implement main menu, HUD, and other UI panels
   - Link all UI elements to the UIController
   - Configure button actions

5. **Test and Balance**:
   - Adjust tower costs, damages, and ranges
   - Balance enemy health and speed
   - Tune wave progression difficulty

## 🧩 Architecture

This project follows the Model-View-Controller (MVC) pattern:

- **Models**: Data structures like EnemyModel and TowerModel
- **Views**: UI elements and visuals
- **Controllers**: Game logic components like EnemyController and TowerController

## 🔄 Game Loop

1. Player starts a wave
2. Enemies spawn and follow the path
3. Towers attack enemies within range
4. Player earns gold by killing enemies
5. Player can place/upgrade towers between waves
6. Game continues until player loses all lives or completes all waves

## 📝 Notes

- The scripts use Unity events for communication, creating loose coupling
- The game supports different tower types with unique behaviors
- Enemy difficulty scales with wave number
- Tower placement is restricted to designated areas 