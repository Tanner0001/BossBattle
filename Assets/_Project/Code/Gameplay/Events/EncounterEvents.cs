using _Project.Code.Core.Events;
using UnityEngine;

public enum EnemyType
{
    Drone,
    Warden
}

// Fired by Health.cs when any enemy dies.
public struct EnemyDiedEvent : IEvent
{
    public EnemyType Type;
    public Vector3 Position;
}

// Fired by Health.cs only when the Warden dies.
public struct WardenDiedEvent : IEvent { }

// Fired by EncounterManager.cs on phase transitions.
public struct PhaseChangedEvent : IEvent
{
    public int PhaseIndex;
    public string DisplayText;
}

// Fired by EncounterManager.cs when the win condition is met.
public struct GameWonEvent : IEvent { }

// Fired by WardenTransitionState when the Warden reaches its retreat point.
public struct WardenReachedRetreatPointEvent : IEvent { }
