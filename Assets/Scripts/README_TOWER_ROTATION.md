# Tower Rotation Implementation Guide

This guide walks you through implementing tower rotation to make them face enemies, similar to the reference screenshot.

## Step 1: Prepare Your Tower Prefabs

1. Open your tower prefab in the Unity Editor
2. Split your tower into two parts:
   - Base (static part that doesn't rotate)
   - Turret/Top (part that rotates to face enemies)

Example hierarchy:
```
Tower (parent object)
├── Base (doesn't rotate)
└── Turret (rotates to face enemies)
```

## Step 2: Add the TowerTargeting Script

1. Select your tower GameObject in the hierarchy
2. Add the TowerTargeting component (Component → Scripts → TowerTargeting)
3. Configure the parameters:
   - **Range**: How far the tower can detect enemies (visualized by a red circle in the Scene view)
   - **Enemy Tag**: Set to "Enemy" (this is set automatically)
   - **Rotation Speed**: How quickly the tower rotates (5 is a good starting value)
   - **Tower Rotation Part**: Drag the turret/top part that should rotate into this field

## Step 3: Tag Your Enemies

Ensure all enemy prefabs have:
1. The tag "Enemy" applied
2. The Enemy component attached

Note: Our updated EnemySpawner now does this automatically.

## Step 4: Test in Play Mode

1. Enter Play Mode
2. Spawn some enemies
3. Watch as your towers automatically detect and rotate to face the nearest enemy

## Troubleshooting

If towers aren't rotating:
- Check the Enemy tag is correctly applied
- Verify the tower range is large enough to detect enemies
- Make sure LevelManager has waypoints set up correctly
- Check the console for any error messages

## Advanced Features

To enhance your tower rotation:
- Add a projectile system so towers can fire at enemies
- Add different rotation speeds for different tower types 
- Implement tower upgrades to increase range or rotation speed

## Customizing the Rotation

If you need to adjust the rotation angle:
- In `TowerTargeting.cs`, find the `RotateTowardsTarget()` method
- Modify this line to adjust the angle offset: `float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;`
- The `-90f` value rotates the sprite to point upward. Change this value to match your sprite's default orientation.

## Visual Polish

For the best visual effect:
- Add a small delay or smoothing to the rotation to make it feel more mechanical
- Add rotation limits for certain tower types
- Add visual feedback when a tower acquires a target 