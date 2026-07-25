# Repo48-PacManGame

A classic Pac-Man game implementation built with **C#** and **Windows Forms**, featuring a complete game engine with maze navigation, multiple ghosts, power-ups, and progressive levels.

## Overview

This project is a faithful recreation of the iconic Pac-Man arcade game. The player controls Pac-Man through a maze, collecting dots and power pellets while avoiding ghosts. The game includes multiple levels with increasing difficulty and a scoring system based on collectibles and ghost encounters.

## Features

### Core Gameplay
- **Maze Navigation**: Navigate Pac-Man through dynamically generated maze layouts across multiple levels
- **Progressive Levels**: Game difficulty increases with each completed level (ghost speeds increase)
- **Lives System**: Start with 3 lives; lose a life when colliding with a ghost
- **Score Tracking**: Earn points by collecting dots (10 points) and power pellets (50 points)

### Pac-Man Mechanics
- Arrow key controls for directional movement (Up, Down, Left, Right)
- Animated mouth opening and closing while moving
- Smooth collision detection with maze walls
- Press Enter to start/restart the game

### Ghost AI
- **4 Ghosts**: Each with unique colors
- **Smart Movement**: Ghosts use weighted decision-making to hunt Pac-Man
  - 70% of the time: Chase Pac-Man using Manhattan distance
  - 30% of the time: Move randomly (creates challenge variation)
- **Avoidance Logic**: Ghosts avoid reversing direction to prevent erratic behavior
- **Adaptive Behavior**: Ghost intelligence varies based on game state

### Power-Ups
- **Power Pellets**: Temporarily makes ghosts vulnerable (8 seconds)
- **Vulnerable State**: Collect vulnerable ghosts for 200 points each
- **Color Change**: Vulnerable ghosts appear in a distinct color during activation

### Game States
- **Ready**: Display start prompt, wait for Enter key
- **Playing**: Active gameplay
- **Level Complete**: Transition between levels with brief delay
- **Game Over**: Win/lose screen with restart option

### Visual & Audio
- Grid-based rendering system with customizable cell sizes
- Color-coded maze elements (walls, dots, power pellets, ghosts)
- Dynamic window sizing that adapts to screen resolution
- Sound effects for dot collection, power-ups, ghost consumption, and death
- Overlay text system for game state messaging

## Technical Architecture

### Technology Stack
- **Language**: C# (.NET Framework 4.8)
- **UI Framework**: Windows Forms
- **Type**: Desktop Application (WinExe)

### Project Structure

```
PacmanGame/
├── Forms/
│   ├── MainForm.cs           # Main game window and engine
│   ├── MainForm.Designer.cs  # UI designer file
│   └── MainForm.resx         # Resource file
├── Models/
│   ├── AppConstants.cs       # Game configuration and constants
│   ├── CellType.cs          # Maze cell type enum
│   ├── Direction.cs         # Movement direction enum
│   ├── GameState.cs         # Game state enum
│   ├── Ghost.cs             # Ghost entity model
│   └── ThemeColors.cs       # Color scheme definitions
├── Services/
│   ├── GameEngine.cs        # Game logic coordination
│   ├── MazeBuilder.cs       # Level maze generation
│   └── SoundService.cs      # Audio management
├── Controls/
│   └── BufferedPanel.cs     # Custom rendering panel with double-buffering
└── Program.cs               # Application entry point
```

### Key Components

**MainForm.cs** (22.5 KB)
- Central game loop and state management
- Player movement and collision detection
- Ghost AI decision-making and pathfinding
- Rendering pipeline for maze, Pac-Man, and ghosts
- Input handling via arrow keys and Enter

**Models**
- Ghost: Tracks position, direction, vulnerability state, and appearance
- GameState: Enum for Ready, Playing, LevelComplete, GameOver states
- CellType: Enum for Wall, Dot, PowerPellet, Empty maze cells

**Services**
- MazeBuilder: Generates level-specific maze layouts with ghost spawn points
- SoundService: Manages audio playback for game events
- GameEngine: Coordinates game mechanics

## How to Play

1. **Launch the game** and you'll see the "PRESS ENTER TO START" prompt
2. **Press Enter** to begin playing
3. **Use Arrow Keys** to move Pac-Man in the desired direction
4. **Collect all dots** (small circles) to complete the level
5. **Avoid ghosts** - contact ends a life (unless ghosts are vulnerable)
6. **Collect power pellets** (larger circles) to temporarily make ghosts vulnerable
7. **Eat vulnerable ghosts** for bonus points
8. **Complete all levels** to win the game
9. **Press Enter** at game over screen to restart

### Controls
| Key | Action |
|-----|--------|
| ↑ Arrow Up | Move up |
| ↓ Arrow Down | Move down |
| ← Arrow Left | Move left |
| → Arrow Right | Move right |
| Enter | Start game / Restart after game over |

## Game Mechanics

### Scoring
- **Dot**: 10 points
- **Power Pellet**: 50 points
- **Ghost Eaten**: 200 points

### Difficulty Progression
- **Level 1**: Base ghost speed (220ms interval)
- **Level 2**: Faster ghosts (190ms interval)
- **Level 3+**: Progressive acceleration (minimum 90ms)

### Ghost Behavior
- Ghosts calculate valid adjacent paths, avoiding walls and back-tracking
- Smart pathfinding uses Manhattan distance calculation
- When vulnerable, ghosts prioritize escape over pursuit
- Ghosts reset to spawn points when eaten

### Collision System
- Grid-based collision detection
- Separate timers for player and ghost movement
- Simultaneous collision checks after each movement phase

## System Requirements

- **.NET Framework 4.8** or later
- **Windows** operating system (Forms-based application)
- **Display**: Minimum 1024x768 resolution recommended

## Building & Running

### Prerequisites
- Visual Studio 2015 or later
- .NET Framework 4.8 Developer Pack

### Build Steps
1. Open `PacmanGame.csproj` in Visual Studio
2. Build the project (Build > Build Solution)
3. Run the application (F5 or Debug > Start Debugging)

### Output
- Debug builds: `bin\Debug\PacmanGame.exe`
- Release builds: `bin\Release\PacmanGame.exe`

## License

This project is licensed under the **MIT License** - see the LICENSE file for details.

## Notes

- The game runs on a fixed game loop with independent timers for player and ghost movement
- Window size automatically adjusts based on screen resolution and grid configuration
- Status labels (Score, Level, Lives) reposition dynamically if window is resized
- All game values are customizable through `AppConstants.cs` (cell size, grid dimensions, etc.)
- The project includes a custom BufferedPanel component for smooth, flicker-free rendering

---

**Created with C# & Windows Forms** | Classic Arcade Game Recreation
