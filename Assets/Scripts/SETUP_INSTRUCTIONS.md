# Tower Defense Game Setup Guide

This guide will help you set up your tower defense game properly to avoid errors with missing tags and components.

## Step 1: Define Required Tags in Unity

You need to define the "Enemy" tag in Unity:

1. In Unity, go to Edit → Project Settings → Tags and Layers
2. Click the "+" icon under "Tags"
3. Add the tag "Enemy"
4. Click "Apply"

## Step 2: Set Up Level with Waypoints

To fix the "No waypoints found or LevelManager is missing!" error, you need to properly set up your waypoints:

1. Create an empty GameObject in your scene and name it "Level"
2. Add the "LevelSetup" component to it
3. In the Inspector, click "Create Empty Waypoints" (this will create example waypoints)
4. Arrange the waypoints to define the path enemies will follow
5. Click "Setup Level" in the Inspector (this will create LevelManager and GameManager if needed)

## Step 3: Set Up Enemy Spawner

1. Create an empty GameObject and name it "EnemySpawner"
2. Add the "EnemySpawner" component to it
3. Assign enemy prefabs in the Inspector
4. Make sure your enemy prefabs have:
   - A Rigidbody2D component (if you're using EnemyMovement script)
   - The "Enemy" tag

## Step 4: Set Up GameManager

1. Create an empty GameObject and name it "GameManager" (or use the one created by LevelSetup)
2. Add the "GameManager" component to it
3. Set up the UI references in the Inspector

## Troubleshooting

### "Tag: Enemy is not defined" Error
- Make sure you've defined the "Enemy" tag in Project Settings → Tags and Layers
- This error can also happen if you try to use the tag before it's defined in Unity

### "No waypoints found or LevelManager is missing" Error
- Make sure you've set up the LevelManager with waypoints using the LevelSetup tool
- Verify that the LevelManager has waypoints assigned in the Inspector

### "Adaptive Performance" Warning
This is just a warning about a Unity package and doesn't affect your game. You can ignore it or:
1. Go to Project Settings → Adaptive Performance
2. Configure a provider or disable the package

## Remember
- Always set up your scene hierarchy before entering Play mode
- Use the LevelSetup tool to ensure proper connections between components 