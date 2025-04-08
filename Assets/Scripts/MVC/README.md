# MVC Architecture Implementation

This folder contains the Model-View-Controller (MVC) implementation for the Tower Defense game.

## Structure

The MVC pattern is implemented with the following structure:

### Models

Models store the data and business logic of the application. They are responsible for:
- Managing the data
- Implementing business rules
- Notifying observers when changes occur

Located in: `Models/`

Key model classes:
- `GameModel`: Manages game state, gold, health, and game over conditions
- `WaveModel`: Manages wave information, enemy counts, and wave progression
- `TowerModel`: Stores tower data like costs, damage, range, etc.
- `LevelModel`: Handles level data, progression, and unlocking
- `SettingsModel`: Manages game settings and preferences

### Controllers

Controllers act as an interface between Models and Views. They are responsible for:
- Receiving input from views
- Processing that input with the help of models
- Returning the results to the views

Located in: `Controllers/`

Key controller classes:
- `GameController`: Manages overall game state and resources
- `WaveController`: Controls wave spawning and enemy management
- `TowerController`: Handles tower data and placement logic
- `LevelController`: Manages level progression and completion
- `SettingsController`: Applies and saves game settings

### Views

Views display the information to the user and capture user input. They are responsible for:
- Presenting the data to the user
- Capturing user input
- Sending input to controllers for processing

Located in: `Views/`

Key view classes:
- `HUDView`: Displays game information like gold, health, and wave info
- `TowerSelectionView`: Handles tower selection UI
- `TowerPlacementView`: Manages tower placement visualization
- `TowerUpgradeView`: Displays tower upgrade options and handles upgrade interactions
- `GameOverView`: Shows game over screen
- `VictoryView`: Displays level completion screen
- `PauseMenuView`: Manages pause functionality and menu display
- `SettingsView`: Handles settings configuration interface
- `MainMenuView`: Controls main menu UI and navigation

## Communication Flow

1. **User interacts with a View**
   - Example: User clicks on a tower button

2. **View sends input to Controller**
   - Example: `TowerSelectionView` calls `TowerController.SelectTowerToBuild()`

3. **Controller updates the Model**
   - Example: `TowerController` sets the selected tower in `TowerModel`

4. **Model notifies observers of changes**
   - Example: `TowerModel` triggers `OnTowerSelected` event

5. **Views observe Model changes and update UI**
   - Example: `TowerPlacementView` shows placement indicator

## Benefits of MVC Implementation

1. **Separation of Concerns**: Each component has a specific responsibility
2. **Maintainability**: Changes to one component don't affect others
3. **Testability**: Components can be tested in isolation
4. **Reusability**: Models and Controllers can be reused across different views
5. **Extensibility**: Easy to add new features without modifying existing code

## Usage Example

To place a tower:

1. User clicks a tower button in `TowerSelectionView`
2. `TowerSelectionView` calls `TowerController.SelectTowerToBuild()`
3. `TowerController` updates the selected tower type
4. `TowerPlacementView` observes the change and shows placement indicator
5. User clicks on map to place tower
6. `TowerPlacementView` calls `TowerController.PlaceTower()`
7. `TowerController` verifies placement with `GameController` (checks gold)
8. `GameController` updates `GameModel` (reduces gold)
9. `TowerController` creates the tower object
10. `GameModel` notifies observers of the gold change
11. `HUDView` observes the change and updates the gold display 