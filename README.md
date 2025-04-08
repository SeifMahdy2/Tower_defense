# Tower Defense Game

A simple but complete tower defense game built in Unity.

## Game Features

- Four unique tower types:
  - **Archer Tower**: Fast firing rate, moderate damage
  - **Mage Tower**: High damage, slower firing rate
  - **Frost Tower**: Slows enemies in range
  - **Cannon Tower**: Area damage to multiple enemies

- Wave-based enemy spawning system with configurable waves
- Gold economy for tower purchases and upgrades
- Health system for tracking player lives
- Complete UI system including:
  - Main menu
  - Level selection
  - Settings menu
  - Pause menu
  - Game over screen
  - Victory screen
  - In-game HUD

- Game speed controls (1x, 2x, 3x)
- Audio system with music and sound effects
- Settings management with persistent preferences
- Visual effects for damage, gold, and tower attacks

## How to Play

1. Start the game and select a level from the level selection screen
2. Use your starting gold to place towers along the enemy path
3. Defend against waves of enemies
4. Earn gold for defeating enemies, which can be used to build more towers
5. Complete all waves to win the level and unlock the next one

## Controls

- **Left Mouse Button**: Select and place towers
- **Escape**: Pause game
- **UI Buttons**: Interact with game menus and tower selection

## Tower Types

### Archer Tower
- Basic tower with good rate of fire
- Effective against single targets
- Cost: 100 gold

### Mage Tower
- High damage but slower fire rate
- Great for taking out tougher enemies
- Cost: 150 gold

### Frost Tower
- Slows enemies in range
- Useful for strategic choke points
- Cost: 125 gold

### Cannon Tower
- Area damage to multiple enemies
- Perfect for groups of enemies
- Cost: 200 gold

## Development

This game was created using Unity and C#. The project structure is organized as follows:

- **Scripts/**
  - **Managers/**: Game state and system management scripts
  - **UI/**: User interface components
  - **Towers/**: Tower behavior scripts
  - **Utility/**: Helper functions and utilities

- **Prefabs/**: Reusable game objects
- **Scenes/**: Game levels
- **Resources/**: Game assets (sprites, audio, etc.)

## Credits

This tower defense game was created as a learning project.

## License

This project is for educational purposes. 