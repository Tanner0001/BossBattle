# Unity Player Controller System

A modular player controller framework for Unity 6 demonstrating architecture patterns and state machine design.

## Tested & Working Controllers

The following controllers are fully implemented and tested:

| Controller | Type | Features |
|------------|------|----------|
| **ThirdPerson** | Action | Lock-on targeting, sprint, jump, orbital camera |
| **PlayerAiming** | First-Person | Strafe movement, first-person aiming, head bob |
| **SideScroller** | Platformer | Double jump, air dash, wall slide, variable jump height |
| **TopDown** | Twin-Stick | 8-direction movement, twin-stick aiming |
| **Tank** | Vehicle | Forward/backward + turn in place controls |

## Requirements

- Unity 6000.0.50f1
- Input System (1.14.0+)
- Cinemachine (3.x)
- URP (17.0.4)

## Getting Started

### Example Scenes

Pre-configured example scenes are located in two places:

**Controller Demos** (`Assets/_Project/Scenes/Demos/`):
- `BaseScene-ThirdPerson.unity` - Third-person action controller
- `BaseScene-FPS.unity` - First-person aiming controller
- `BaseScene-SideScroll.unity` - Side-scrolling platformer
- `BaseScene-TopDown.unity` - Top-down twin-stick shooter
- `BaseScene-Tank.unity` - Tank/vehicle controller

**System Examples** (`Assets/Scenes/Example/`):
- `[Main_Menu_Example].unity` - Example main menu setup
- `[Gameplay_Example].unity` - Example gameplay scene
- `[End_Example].unity` - Example end screen

**Bootstrap Scene** (`Assets/Scenes/_Bootstrap.unity`):
- Contains the `[GameInitializer]` prefab that sets up all persistent services
- Must be the first scene loaded (Build Settings index 0 recommended)
- Services persist across all scene transitions via `DontDestroyOnLoad`

### Setting Up a New Scene

To create a new gameplay scene from scratch:

1. **Create the Scene**
   - Create a new scene in Unity
   - Add basic geometry (ground plane, walls, etc.)

2. **Add Player**
   - Drag a player prefab from `Assets/_Project/Prefabs/Players/` into your scene
   - Available prefabs: `ThirdPersonPlayer`, `FPSPlayer`, `SideScrollPlayer`, `TopDownPlayer`, `TankPlayer`
   - Each prefab is pre-configured with the appropriate controller, motor, ground check, and movement profile

3. **Add Camera**
   - Drag the matching camera prefab from `Assets/_Project/Prefabs/Cameras/` into your scene
   - Match camera to player type: `ThirdPersonCamera` → `ThirdPersonPlayer`, etc.
   - Camera prefabs are pre-configured with CinemachineCamera and camera profiles
   - No manual wiring needed - cameras auto-connect to players via `PlayerService`

4. **Set Game State**
   - Add `SceneGameState` component to any GameObject in your scene (or use the prefab from `Assets/_Project/Prefabs/Services/[SceneGameState].prefab`)
   - Set the "Initial State" dropdown to:
     - **Gameplay** - For playable game scenes
     - **Menu** - For main menu, pause menu, or UI-focused scenes
     - **Paused** - Rarely used (mainly for testing pause state)
   - This tells the `GameManagementService` what state to start in when the scene loads

5. **Ensure Bootstrap Runs First**
   - The `_Bootstrap` scene must load before your scene to initialize services
   - In **Build Settings**, put `_Bootstrap` at index 0
   - Your gameplay scenes can be loaded additively or directly (services persist either way)

6. **Play**
   - Hit Play in Unity Editor
   - If you see errors about missing services, ensure `_Bootstrap` scene has loaded first
   - The `[GameInitializer]` prefab in `_Bootstrap` creates all necessary services on startup

### Prefabs Overview

**Player Prefabs** (`Assets/_Project/Prefabs/Players/`):
- Pre-configured with controller, motor, ground check, and movement profile
- Ready to drop into any scene

**Camera Prefabs** (`Assets/_Project/Prefabs/Cameras/`):
- Pre-configured with CinemachineCamera, camera script, and camera profile
- Auto-connect to player via `PlayerService` (no manual linking required)

**Service Prefabs** (`Assets/_Project/Prefabs/Services/`):
- `[InputService]` - Handles input events
- `[CameraService]` - Manages active camera
- `[PlayerService]` - Tracks active player
- `[GameManagementService]` - Game state machine (Gameplay/Paused/Menu)
- `[SceneService]` - Scene loading and transitions
- These are instantiated by `[GameInitializer]` in the `_Bootstrap` scene

**GameInitializer** (`Assets/_Project/Prefabs/[GameInitializer].prefab`):
- Place in `_Bootstrap` scene
- Instantiates all service prefabs on startup
- Registers services with `ServiceLocator`
- Persists across scene transitions

### ScriptableObject Profiles

**Movement Profiles** (`Assets/_Project/ScriptableObjects/GameData/PlayerMovement/`):
- `ThirdPersonMovementProfile` - Walk/run/sprint speeds, jump settings, lock-on behavior
- `PlayerAimingProfile` - FPS movement speeds, strafe settings
- `JumpingMovementProfile` - Used by SideScroller (jump height, air control, double jump, dash)
- `TopDownMovementProfile` - Movement speed, rotation speed
- `TankMovementProfile` - Forward/backward speed, turn speed

**Camera Profiles** (`Assets/_Project/ScriptableObjects/GameData/Camera/`):
- `ThirdPersonCameraProfile` - Follow distance, orbit speed, lock-on camera behavior
- `AimCameraProfile` - FOV, sensitivity, head bob settings
- `SideScrollerCameraProfile` - Follow smoothing, look-ahead distance
- `TopDownCameraProfile` - Height, angle, follow smoothing
- `TankCameraProfile` - Follow distance, rotation damping

To customize a controller's behavior:
1. Locate the existing profile in `ScriptableObjects/GameData/`
2. Either modify it directly OR duplicate it (Right-click → Duplicate)
3. Assign the profile to the controller component in the Inspector

## Input Configuration

Input is configured in `PlayerInputActions.inputactions` with the Unity Input System.

**Keyboard/Mouse:**
- WASD = Move
- Mouse = Look/Aim
- Space = Jump
- Left Shift = Sprint
- Left Ctrl = Dodge
- E = Interact
- Left Mouse = Attack
- Right Mouse = Lock On

**Gamepad:**
- Left Stick = Move
- Right Stick = Look/Aim
- A Button (South) = Jump
- Right Trigger = Sprint/Attack
- B Button (East) = Dodge
- X Button (West) = Interact
- Right Stick Press = Lock On

## Controller-Specific Features

### ThirdPerson
- **Movement:** Walk, run, sprint with smooth speed transitions
- **Combat:** Lock-on targeting system with strafe movement
- **Jumping:** Coyote time and jump buffering for responsive controls
- **Camera:** Orbital camera that adjusts based on lock-on state
- **States:** Idle, Walking, Running, Sprinting, Jumping, Falling, LockOn, LockOnJumping

### PlayerAiming (First-Person)
- **Movement:** Walk and sprint with strafe capability
- **Camera:** First-person with optional head bob effect
- **Aiming:** Full 360-degree look control
- **States:** Idle, Walking, Running, Jumping, Falling

### SideScroller
- **Jumping:** Variable jump height (hold for higher jumps), optional double jump
- **Air Movement:** Optional air dash with cooldown, apex hang time, fast fall
- **Wall Mechanics:** Optional wall slide and wall jump (configurable in profile)
- **Acceleration:** Smooth acceleration and deceleration on ground
- **States:** Idle, Walking, Running, Jumping, Falling, WallSlide, AirDash

### TopDown
- **Movement:** 8-direction movement with smooth acceleration
- **Aiming:** Twin-stick aiming (independent movement and aim directions)
- **Rotation:** Smooth rotation towards aim direction
- **Camera:** Fixed top-down view that follows player
- **States:** Idle, Moving, Falling

### Tank
- **Movement:** Forward/backward with physics-based momentum
- **Rotation:** Turn in place controls (no strafing)
- **Camera:** Follows vehicle orientation with smoothing
- **States:** Idle, Moving, Falling

## Code Architecture

This framework is built on a foundation of established architecture patterns to keep code modular, testable, and maintainable.

### Core Patterns

Located in `Assets/_Project/Code/Core/`, these patterns are used throughout the project:

#### 1. Service Locator Pattern
**Location:** `Assets/_Project/Code/Core/ServiceLocator/`

Centralized dependency injection for global services.

```csharp
// Register a service (done automatically by GameInitializer)
ServiceLocator.Register<InputService>(inputService);

// Retrieve a service
var inputService = ServiceLocator.Get<InputService>();
```

**Services in this project:**
- `InputService` - Publishes input events from Unity Input System
- `CameraService` - Tracks and manages the active camera
- `PlayerService` - Tracks the active player transform
- `GameManagementService` - Game state FSM (Gameplay/Paused/Menu)
- `SceneService` - Scene loading and transitions

**Key classes:**
- `IService` - Interface all services must implement
- `MonoBehaviourService` - Base class for MonoBehaviour-based services
- `GameInitializer` - Instantiates service prefabs and registers them on startup

#### 2. Finite State Machine Pattern
**Location:** `Assets/_Project/Code/Core/StateMachine/`

Type-safe state machine for managing controller behavior.

```csharp
// Create state machine with initial state
var stateMachine = new FiniteStateMachine<IState>(idleState);

// Add additional states
stateMachine.AddState(walkingState);
stateMachine.AddState(runningState);

// Transition to a new state
stateMachine.TransitionTo<WalkingState>();

// Update current state
stateMachine.Update(); // or FixedUpdate()
```

**Key classes:**
- `IState` - Interface all states must implement
- `BaseState` - Base implementation with Enter/Exit/Update/FixedUpdate
- `FiniteStateMachine<T>` - Generic state machine

**Why FSM?** Isolates behavior into discrete states. When jumping, only jump code runs. No complex if/else chains or flag checking.

#### 3. Event Bus Pattern
**Location:** `Assets/_Project/Code/Core/Events/`

Weakly-referenced event system for decoupled messaging.

```csharp
// Define an event
public struct JumpInputEvent : IEvent
{
    public bool IsPressed;
}

// Publish an event
EventBus.Instance.Publish(new JumpInputEvent { IsPressed = true });

// Subscribe to an event
EventBus.Instance.Subscribe<JumpInputEvent>(this, OnJumpInput);

// Unsubscribe
EventBus.Instance.Unsubscribe<JumpInputEvent>(this);
```

**Key classes:**
- `IEvent` - Marker interface for events
- `EventBus` - Singleton event dispatcher using WeakReferences
- `EventBusSubscriber` - MonoBehaviour base class with automatic cleanup

**Why EventBus?** Decouples input handling from controllers. States subscribe only to events they care about.

#### 4. Strategy Pattern
**Location:** `Assets/_Project/Code/Core/Strategy/`

Encapsulates interchangeable algorithms (used for camera effects).

```csharp
public interface IStrategy<TContext>
{
    void Execute(TContext context);
}

// ScriptableObject-based strategies for designer control
[CreateAssetMenu(menuName = "Strategies/MyStrategy")]
public class MyStrategy : ScriptableStrategy<MyContext>
{
    public override void Execute(MyContext context)
    {
        // Strategy implementation
    }
}
```

**Used in:** Camera effects (head bob, camera shake, etc.)

#### 5. Object Pooling
**Location:** `Assets/_Project/Code/Core/ObjectPool/`

High-performance object reuse for frequently spawned objects.

```csharp
var pool = new ObjectPool<Projectile>(
    createFunc: () => Instantiate(projectilePrefab),
    defaultCapacity: 20,
    maxSize: 100
);

Projectile obj = pool.Get();
pool.Release(obj);
```

**Key classes:**
- `ObjectPool<T>` - Generic pooling system
- `IPoolable` - Optional interface for spawn/despawn callbacks
- `PooledFactory<T>` - Combines factory and pooling patterns

#### 6. MVC Pattern
**Location:** `Assets/_Project/Code/Core/MVC/`

Model-View-Controller separation for UI and game systems.

```csharp
// Model - Data storage
public class PlayerStatsModel : BaseModel
{
    private int _health;
    public int Health
    {
        get => _health;
        set { _health = value; NotifyDataChanged(); }
    }
}

// View - Display logic
public class PlayerStatsView : BaseView
{
    [SerializeField] private TextMeshProUGUI _healthText;

    public override void UpdateDisplay(IModel model)
    {
        var stats = model as PlayerStatsModel;
        _healthText.text = $"HP: {stats.Health}";
    }
}

// Controller - Logic layer
public class PlayerStatsController : BaseController<PlayerStatsModel, PlayerStatsView>
{
    // Automatically wires model changes to view updates
}
```

**Key classes:**
- `IModel`, `IView`, `IController<TModel, TView>` - Core interfaces
- `BaseModel`, `BaseView`, `BaseController<TModel, TView>` - Base implementations

### How Controllers Work

Each player controller follows a consistent structure:

#### Controller Component
**Example:** `ThirdPersonController.cs`, `TankController.cs`

- Extends `BasePlayerController`
- Holds references to components (motor, ground check, movement profile)
- Stores shared state (IsGrounded, MoveInput, IsSprinting)
- Creates and initializes the state machine
- Handles controller-level logic (jump buffering, sprint toggling)

#### Base State
**Example:** `TPBaseState.cs`, `TankBaseState.cs`

- Extends `BaseState` from Core
- Provides access to controller reference
- Provides access to state machine reference
- Can contain shared logic for all states of that controller

#### Individual States
**Example:** `TPWalkingState.cs`, `TankMovingState.cs`

- Extends the controller's base state
- **Enter():** Subscribe to input events, set up animations
- **Update():** Apply movement logic, check for transitions
- **Exit():** Unsubscribe from events, clean up
- **FixedUpdate():** Physics-based movement (optional)

#### Motor
**Example:** `CharacterControllerMotor.cs`, `RigidbodyMotor.cs`

- Implements `IMotor` interface
- Handles actual Unity physics (CharacterController, Rigidbody)
- Provides movement methods: `Move()`, `Jump()`, `ApplyGravity()`, `SetVelocity()`
- Controllers call motor methods, never touch physics components directly

**Why Motor?** Abstracts physics implementation. Can swap CharacterController for Rigidbody without changing state code.

#### Camera
**Example:** `ThirdPersonCamera.cs`, `TankCamera.cs`

- Manages Cinemachine camera behavior
- Auto-connects to player via `PlayerService.GetPlayerTransform()`
- Configured via ScriptableObject camera profile
- Can apply effects using Strategy pattern (head bob, camera shake)

### Key Design Principles

#### States Own Their Input

Input subscriptions happen in `Enter()`, unsubscriptions in `Exit()`. This isolates input handling.

```csharp
public override void Enter()
{
    EventBus.Instance.Subscribe<MoveInputEvent>(this, HandleMove);
}

public override void Exit()
{
    EventBus.Instance.Unsubscribe<MoveInputEvent>(this);
}
```

**Why?** Only active states receive input. Idle state doesn't process movement input.

#### Gravity Always Applies

All grounded states apply gravity every frame to ensure smooth ledge detection.

```csharp
public override void Update()
{
    // Apply gravity even when idle/walking
    _controller.Motor.ApplyGravity(_controller.Profile.Gravity);

    // Movement logic...
}
```

**Why?** Without constant gravity, walking off a ledge doesn't trigger falling until the next input. Character appears to float.

#### Momentum Preservation

Controllers preserve velocity during state transitions. Jumping from a sprint maintains that speed.

```csharp
public override void Enter()
{
    // Preserve horizontal velocity, set vertical velocity for jump
    Vector3 velocity = _controller.Motor.Velocity;
    velocity.y = _controller.Profile.JumpForce;
    _controller.Motor.SetVelocity(velocity);
}
```

**Why?** Jumping should feel responsive. Resetting velocity on transition feels "sticky" and unnatural.

### Extending the Framework

#### Creating a New Controller

1. **Create Controller Script**
   - Extend `BasePlayerController`
   - Add controller-specific fields (movement profile, etc.)
   - Override `Awake()` to initialize state machine

2. **Create Movement Profile**
   - Extend `MovementProfile` ScriptableObject
   - Add controller-specific settings (speeds, physics values)
   - Create `[CreateAssetMenu]` for designer access

3. **Create Base State**
   - Extend `BaseState`
   - Add controller and state machine references
   - Add shared helper methods

4. **Create Individual States**
   - Extend your base state
   - Implement Enter/Update/Exit/FixedUpdate
   - Handle input events and transitions

5. **Create Camera**
   - Extend `MonoBehaviour`
   - Get player reference from `PlayerService`
   - Control CinemachineCamera component
   - Create camera profile ScriptableObject for settings

6. **Create Prefabs**
   - Player prefab with controller component and profile
   - Camera prefab with camera script and profile

#### Adding a New State to Existing Controller

1. Create new state script extending the controller's base state
2. Implement `Enter()`, `Update()`, `Exit()`, and `FixedUpdate()` (if needed)
3. Subscribe to required input events in `Enter()`
4. Add transition logic to/from existing states
5. Add the state to the state machine in controller's `Awake()`

```csharp
protected override void Awake()
{
    base.Awake();

    var idleState = new TPIdleState(this, _stateMachine);
    _stateMachine = new FiniteStateMachine<IState>(idleState);
    _stateMachine.AddState(new TPWalkingState(this, _stateMachine));
    _stateMachine.AddState(new TPRunningState(this, _stateMachine));
    _stateMachine.AddState(new TPMyNewState(this, _stateMachine)); // New state
}
```

#### Adding a New Input

1. **Add to Input Actions:** Open `PlayerInputActions.inputactions` in Unity, add new action
2. **Create Event:** Add event struct in `InputEvents.cs` implementing `IEvent`
3. **Handle in InputService:** Subscribe to Input System action, publish your event
4. **Subscribe in States:** States that need the input subscribe to your event

```csharp
// InputEvents.cs
public struct MyNewInputEvent : IEvent
{
    public bool IsPressed;
}

// InputService.cs
_inputActions.Gameplay.MyNewAction.performed += HandleMyNewAction;

private void HandleMyNewAction(InputAction.CallbackContext context)
{
    EventBus.Instance.Publish(new MyNewInputEvent { IsPressed = true });
}

// MyState.cs
public override void Enter()
{
    EventBus.Instance.Subscribe<MyNewInputEvent>(this, OnMyNewInput);
}

private void OnMyNewInput(MyNewInputEvent evt)
{
    // Handle input
}
```

## Project Structure

```
Assets/
├── Scenes/
│   ├── _Bootstrap.unity              Bootstrap scene with GameInitializer
│   └── Example/                      Example scenes (menu, gameplay, end)
│
├── _Project/
│   ├── Code/
│   │   ├── Core/                     Reusable architecture patterns
│   │   │   ├── ServiceLocator/       Dependency injection
│   │   │   ├── StateMachine/         FSM implementation
│   │   │   ├── Events/               Event bus system
│   │   │   ├── MVC/                  Model-View-Controller
│   │   │   ├── ObjectPool/           Object pooling
│   │   │   ├── Strategy/             Strategy pattern
│   │   │   └── Factory/              Factory patterns
│   │   │
│   │   └── Gameplay/
│   │       ├── PlayerControllers/    Controller implementations
│   │       │   ├── Base/             Shared motor, ground check, base controller
│   │       │   ├── ThirdPerson/      Third-person action controller
│   │       │   ├── PlayerAiming/     First-person controller
│   │       │   ├── SideScroller/     Platformer controller
│   │       │   ├── TopDown/          Twin-stick controller
│   │       │   ├── Tank/             Vehicle controller
│   │       │   └── Profiles/         Movement profiles
│   │       │
│   │       ├── CameraSystems/        Camera behaviors and profiles
│   │       ├── Input/                Input service and events
│   │       ├── Animation/            Animation controller
│   │       └── Scenes/               Scene service and game state management
│   │
│   ├── Prefabs/
│   │   ├── Players/                  Player prefabs (pre-configured)
│   │   ├── Cameras/                  Camera prefabs (pre-configured)
│   │   ├── Services/                 Service prefabs
│   │   └── [GameInitializer].prefab  Initializes all services
│   │
│   ├── Scenes/Demos/                 Demo scenes for each controller
│   │
│   ├── ScriptableObjects/GameData/
│   │   ├── PlayerMovement/           Movement profiles
│   │   └── Camera/                   Camera profiles
│   │
│   ├── Animations/                   Animation clips and controllers
│   └── Art/                          Models, materials, textures
```

---

**Unity Version:** 6000.0.50f1
**Last Updated:** October 2025
