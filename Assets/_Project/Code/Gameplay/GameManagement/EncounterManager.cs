using _Project.Code.Core.Events;
using _Project.Code.Gameplay.Combat;
using UnityEngine;

namespace _Project.Code.Gameplay.GameManagement
{
    public class EncounterManager : MonoBehaviour
    {
        private enum EncounterState { PreEncounter, Phase1, Phase2, Phase3, GameWon }
        private EncounterState _currentState;

        [Header("Warden")]
        [SerializeField] private Hitbox wardenHitbox;

        [Header("Phase Thresholds")]
        [SerializeField, Range(0, 1)] private float phase2Threshold = 0.7f;
        [SerializeField, Range(0, 1)] private float phase3Threshold = 0.3f;

        private void OnEnable()
        {
            if (wardenHitbox != null)
            {
                wardenHitbox.OnHealthChanged += OnWardenHealthChanged;
            }
            EventBus.Instance.Subscribe<WardenDiedEvent>(this, OnWardenDied);
        }

        private void OnDisable()
        {
            if (wardenHitbox != null)
            {
                wardenHitbox.OnHealthChanged -= OnWardenHealthChanged;
            }
            EventBus.Instance.Unsubscribe<WardenDiedEvent>(this);
        }

        private void Start()
        {
            // Assuming the encounter starts when this manager is enabled.
            TransitionToPhase(1);
        }

        private void OnWardenHealthChanged()
        {
            if (_currentState == EncounterState.GameWon) return;

            float healthPercentage = wardenHitbox.CurrentHealth / wardenHitbox.MaxHealth;

            if (_currentState == EncounterState.Phase1 && healthPercentage <= phase2Threshold)
            {
                TransitionToPhase(2);
            }
            else if (_currentState == EncounterState.Phase2 && healthPercentage <= phase3Threshold)
            {
                TransitionToPhase(3);
            }
        }

        private void TransitionToPhase(int phase)
        {
            switch (phase)
            {
                case 1:
                    _currentState = EncounterState.Phase1;
                    // Optional: Announce Phase 1
                    EventBus.Instance.Publish(new PhaseChangedEvent { PhaseIndex = 1, DisplayText = "Phase 1: Engage the Warden" });
                    break;
                case 2:
                    _currentState = EncounterState.Phase2;
                    EventBus.Instance.Publish(new PhaseChangedEvent { PhaseIndex = 2, DisplayText = "Phase 2: Security Overdrive" });
                    // TODO: Logic to spawn minions or enable new boss attacks would go here
                    break;
                case 3:
                    _currentState = EncounterState.Phase3;
                    EventBus.Instance.Publish(new PhaseChangedEvent { PhaseIndex = 3, DisplayText = "Phase 3: Armor Compromised" });
                    // TODO: Logic for weak-spot revelation or new boss attacks
                    break;
            }
        }

        private void OnWardenDied(WardenDiedEvent e)
        {
            if (_currentState == EncounterState.GameWon) return;

            _currentState = EncounterState.GameWon;
            EventBus.Instance.Publish(new GameWonEvent());
            // TODO: Logic for win sequence (e.g., slow-mo, play cinematic).
        }
    }
}
