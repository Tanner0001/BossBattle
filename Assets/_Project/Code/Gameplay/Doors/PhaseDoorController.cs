using UnityEngine;
using _Project.Code.Core.Events;

namespace _Project.Code.Gameplay.Doors
{
    public class PhaseDoorController : MonoBehaviour
    {
        [Header("Phase-Locked Doors")]
        [SerializeField] private DoorController phase2ArenaDoor; // The door for the transition sequence

        [Header("End-Game Doors")]
        [SerializeField] private DoorController endGameDoor1;
        [SerializeField] private DoorController endGameDoor2;

        void OnEnable()
        {
            EventBus.Instance.Subscribe<PhaseChangedEvent>(this, OnPhaseChanged);
            EventBus.Instance.Subscribe<WardenDiedEvent>(this, OnWardenDied);
            EventBus.Instance.Subscribe<WardenReachedRetreatPointEvent>(this, OnWardenReachedRetreatPoint); // Subscribe to new event
        }

        void OnDisable()
        {
            EventBus.Instance.Unsubscribe<PhaseChangedEvent>(this);
            EventBus.Instance.Unsubscribe<WardenDiedEvent>(this);
            EventBus.Instance.Unsubscribe<WardenReachedRetreatPointEvent>(this); // Unsubscribe from new event
        }

        private void OnPhaseChanged(PhaseChangedEvent e)
        {
            switch (e.PhaseIndex)
            {
                case 1: // Fight starts - ensure door is open
                    if (phase2ArenaDoor != null)
                    {
                        phase2ArenaDoor.OpenDoor();
                        Debug.Log("PhaseDoorController: Phase 1 started, opening transition door.");
                    }
                    break;
                
                // Other cases no longer need to control this door.
                // The door is closed by WardenDoorTrigger and opened by OnWardenReachedRetreatPoint.
            }
        }

        private void OnWardenDied(WardenDiedEvent e)
        {
            Debug.Log("PhaseDoorController: Warden died, opening end-game doors.");
            if (endGameDoor1 != null)
                endGameDoor1.OpenDoor();
            if (endGameDoor2 != null)
                endGameDoor2.OpenDoor();
        }

        private void OnWardenReachedRetreatPoint(WardenReachedRetreatPointEvent e)
        {
            Debug.Log("PhaseDoorController: Warden reached retreat point, opening transition door for player.");
            if (phase2ArenaDoor != null)
            {
                phase2ArenaDoor.OpenDoor();
            }
        }
    }
}
