
# Technical Design Document: Core Systems Rework

## 1. Introduction

### 1.1. Project
BossBattle

### 1.2. Document Purpose
This document outlines a complete rework of the core systems for the project, focusing on the Sound System, FPS Controller, and Shooting System. The goal is to create a more modular, reusable, and performant architecture that is easier to maintain and extend.

### 1.3. Goals
*   **Modularity:** Each system should be self-contained and have a single responsibility.
*   **Reusability:** Systems should be designed to be reusable in other projects with minimal changes.
*   **Performance:** The new architecture should be more performant than the current one, especially in the shooting and sound systems.
*   **Future-Proofing:** The new architecture should be flexible enough to accommodate future changes and new features.

## 2. High-Level Architecture

### 2.1. Overview
The new architecture will be based on a data-driven, event-based design. ScriptableObjects will be used to define game data (weapons, player stats, sound events, etc.), and a global event bus will be used for communication between systems. This will create a loosely coupled architecture where systems can be added, removed, or modified without affecting other systems.

### 2.2. Core Principles

*   **Data-Driven Design:** Using ScriptableObjects to define game data. This allows for easy tweaking of game balance and behavior without changing any code.
*   **Event-Driven Communication:** Using a global event bus for communication between systems. This decouples the systems and makes the architecture more flexible.
*   **Separation of Concerns:** Each system has a single responsibility. For example, the `PlayerController` is responsible for player movement, while the `GunBase` is responsible for weapon logic.

### 2.3. System Interaction Diagram

```
+-----------------+      +-----------------+      +-----------------+
|   Input System  |----->| PlayerController|----->|   Event Bus     |
+-----------------+      +-----------------+      +-----------------+
        |                      |                      ^
        |                      |                      |
        v                      v                      |
+-----------------+      +-----------------+      +-----------------+
|  Camera System  |      |  Player Stats   |      |  Sound System   |
+-----------------+      +-----------------+      +-----------------+
        ^                      ^                      |
        |                      |                      |
        |                      |                      v
+-----------------+      +-----------------+      +-----------------+
| Shooting System |<-----|   Gun Data      |<-----|  UI System      |
+-----------------+      +-----------------+      +-----------------+
```

## 3. System Redesigns

### 3.1. Sound System

#### 3.1.1. Architecture

*   **`SoundManager`:** A singleton service that manages all sound playback. It will have a pool of `AudioSource` components to avoid the overhead of creating and destroying them at runtime.
*   **`SoundEvent`:** A ScriptableObject that defines a sound event. It will contain information about the sound to play (AudioClip, volume, pitch, etc.), as well as any additional properties (looping, spatial blend, etc.).
*   **`AudioSourcePool`:** A simple object pool for `AudioSource` components.
*   **`ReverbZone`:** A trigger volume that applies a reverb effect to all sounds played within it.

#### 3.1.2. Features

*   **Event-driven SFX and music:** Sounds are played by raising a `SoundEvent` on the event bus.
*   **Dynamic music layering:** The `SoundManager` can be extended to support dynamic music layering based on game state.
*   **Reverb zones:** The `SoundManager` will automatically apply reverb effects to sounds played within a `ReverbZone`.
*   **Mixing and mastering:** The `SoundManager` will expose a set of mixer groups that can be used to control the volume of different sound types (SFX, music, UI, etc.).

#### 3.1.3. Pseudocode

**Playing a sound event:**
```csharp
// Somewhere in the game code (e.g., in the GunFeedbackController)
EventBus.Instance.Raise(new PlaySoundEvent(gunshotSoundEvent));

// In the SoundManager
public void OnPlaySoundEvent(PlaySoundEvent evt)
{
    AudioSource source = audioSourcePool.Get();
    source.clip = evt.SoundEvent.Clip;
    source.volume = evt.SoundEvent.Volume;
    // ... set other properties
    source.Play();
}
```

### 3.2. FPS Controller

#### 3.2.1. Architecture

*   **`PlayerController`:** The main MonoBehaviour that manages the player. It will contain a state machine for player states.
*   **`PlayerMovement`:** A class that handles all player movement logic (walking, sprinting, crouching, etc.).
*   **`PlayerCamera`:** A class that handles all camera logic (looking around, ADS, etc.).
*   **`PlayerInput`:** A class that handles all player input using the new Input System.
*   **`PlayerStats`:** A ScriptableObject that defines player stats (health, speed, etc.).

#### 3.2.2. Features

*   **State machine for player states:** The `PlayerController` will use a state machine to manage player states (idle, walking, sprinting, crouching, ADS).
*   **Modular camera system:** The `PlayerCamera` will be designed to be easily extended with new camera behaviors.
*   **Data-driven movement parameters:** All movement parameters (speed, jump height, etc.) will be defined in the `PlayerStats` ScriptableObject.

#### 3.2.3. Pseudocode

**Player movement loop:**
```csharp
// In the PlayerMovement class
public void UpdateMovement(Vector2 moveInput, bool isSprinting)
{
    float speed = isSprinting ? playerStats.SprintSpeed : playerStats.WalkSpeed;
    Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
    characterController.Move(moveDirection * speed * Time.deltaTime);
}
```

### 3.3. Shooting System

#### 3.3.1. Architecture

*   **`GunBase`:** The weapon itself. It will be responsible for firing, reloading, and managing ammo.
*   **`GunData`:** A ScriptableObject that defines weapon stats (damage, fire rate, ammo capacity, etc.).
*   **`GunFeedbackController`:** A class that handles all weapon feedback (VFX, recoil, muzzle flash, shell ejection, audio events).
*   **`Projectile` / `Raycast` system:** The `GunBase` will use a projectile or raycast system to determine what was hit.

#### 3.3.2. Features

*   **Data-driven weapon stats:** All weapon stats will be defined in the `GunData` ScriptableObject.
*   **Decoupled feedback system:** The `GunFeedbackController` will be completely decoupled from the `GunBase`. This will make it easy to create new weapons with different feedback effects.
*   **Support for different weapon types:** The `GunBase` will be designed to support different weapon types (hitscan, projectile, etc.).

#### 3.3.3. Pseudocode

**Weapon fire logic:**
```csharp
// In the GunBase class
public void Fire()
{
    if (CanFire())
    {
        // ... fire logic (raycast or instantiate projectile)
        currentAmmo--;
        lastFireTime = Time.time;
        EventBus.Instance.Raise(new GunFiredEvent(this));
    }
}

// In the GunFeedbackController
public void OnGunFired(GunFiredEvent evt)
{
    if (evt.Gun == myGun)
    {
        // ... play muzzle flash, recoil, etc.
        EventBus.Instance.Raise(new PlaySoundEvent(gunshotSoundEvent));
    }
}
```

## 4. Optimization & Cleanup Plan

### 4.1. Code to Remove
*   All player controllers that are not the new `PlayerController`.
*   Any legacy input handling code.
*   Redundant or unused prefabs.
*   The static `ServiceLocator` should be replaced with a proper DI solution.

### 4.2. Code to Merge/Rewrite
*   The existing `GunInput` and `GunFeedbackController` should be merged into the new `GunFeedbackController`.
*   The existing state machines should be refactored to use a more lightweight implementation.
*   The `GameTimeDisplay` should be refactored to use the new UI system.

### 4.3. Asset Cleanup
*   Review all prefabs and remove any that are no longer used.
*   Review all materials and textures and remove any that are no longer used.
*   Organize all assets into a more logical folder structure.

## 5. Unity 6 Best Practices

### 5.1. New Input System
The new Input System will be used for all player input. This will make it easy to support different input devices and to create custom input bindings.

### 5.2. Event-Based Architecture
The `EventBus` will be used to decouple systems and to make the architecture more flexible.

### 5.3. ScriptableObject-Based Data
`ScriptableObjects` will be used to create a data-driven architecture. This will make it easy to tweak game balance and behavior without changing any code.

### 5.4. `async/await`
`async/await` will be used for asynchronous operations, such as loading scenes or assets, to improve performance and responsiveness.
