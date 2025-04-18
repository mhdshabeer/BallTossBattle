# AR Ball Toss Game

A simple 2D ball toss game built in Unity with WebGL export for browser play. The game features both standard 2D gameplay and AR functionality through WebXR.

## Features

- Classic ball toss gameplay with targets and scoring
- WebXR-based augmented reality mode
- Dual control schemes for both desktop and mobile devices
- Real-time physics for ball throwing and bouncing
- Visual effects for impacts and collisions
- Audio feedback for game events
- High score tracking

## Requirements

### For Development
- Unity 2021.3 or newer
- WebGL build support module

### For Playing
- Any modern web browser with WebGL support
- For AR mode: WebXR-compatible browser (Chrome, Edge, or Firefox) on Android with ARCore support

## How to Play

### 2D Mode
- **Aim**: Right-click and move the mouse to aim
- **Throw**: Left-click and hold to charge throw, release to throw
- **Objective**: Hit as many targets as possible with your limited number of balls

### AR Mode
- **Place Targets**: Tap on a surface in AR view to place a target
- **Aim**: Touch and drag horizontally to adjust aim
- **Throw**: Touch and hold to charge throw, release to throw
- **Objective**: Place targets in your real environment and hit them with virtual balls

## Build and Run Instructions

1. Open the project in Unity
2. Go to File > Build Settings
3. Select WebGL as the platform
4. Click "Switch Platform" if not already selected
5. In Player Settings, select "ARTemplate" as the WebGL template
6. Click "Build" and select a destination folder
7. Host the built files on a web server that supports HTTPS (required for WebXR)
8. Access the game through a compatible browser

## Project Structure

- **Assets/Scripts**: Core game scripts
  - **GameManager.cs**: Main game logic and state management
  - **PlayerController.cs**: Handles player input and ball throwing
  - **BallController.cs**: Physics and collision for balls
  - **TargetController.cs**: Target behavior and scoring
  - **UIManager.cs**: UI management and transitions
  - **ARManager.cs**: AR functionality control
  - **WebGLARBridge.cs**: Unity to JavaScript bridge for WebXR
  - **AudioManager.cs**: Sound effects and music

- **Assets/Prefabs**: Game object prefabs
  - **Ball.prefab**: The throwable ball
  - **Target.prefab**: Targets to hit

- **Assets/Plugins/WebGL**: WebGL-specific plugins
  - **WebGLARBridge.jslib**: JavaScript implementation of WebXR functionality

- **Assets/WebGLTemplates**: Custom WebGL templates
  - **ARTemplate**: Custom template with WebXR support

## Credits

- Developed by [Your Name]
- Built with Unity
- AR functionality using WebXR API