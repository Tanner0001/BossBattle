using _Project.Code.Core.Events;
using UnityEngine;
using UnityEngine.AI;

namespace BossBattle.Gameplay.Enemies.AI
{
    public class WardenTransitionState : WardenBaseState
    {
        private enum SubState { Retreating, Advancing }
        private SubState _subState;
        
        private float _originalStoppingDistance;

        public override void EnterState(WardenBossController warden)
        {
            base.EnterState(warden);

            // Enable invulnerability and visual effect
            warden.Hitbox.IsInvulnerable = true;
            warden.MaterialController?.ApplyInvulnerableMaterial();

            // --- Start Retreat ---
            _subState = SubState.Retreating;
            if (warden.Agent != null && warden.RetreatPoint != null)
            {
                _originalStoppingDistance = warden.Agent.stoppingDistance;
                warden.Agent.stoppingDistance = 0.1f;
                warden.Agent.SetDestination(warden.RetreatPoint.position);
                warden.Agent.isStopped = false;
            }
            else
            {
                Debug.LogError("NavMeshAgent or RetreatPoint not set on WardenBossController! Aborting transition.");
                // Failsafe: Immediately try to transition to the next intended phase
                GoToNextCombatState(warden);
            }
        }

        public override void UpdateState(WardenBossController warden)
        {
            // If we are retreating, check if we've arrived
            if (_subState == SubState.Retreating)
            {
                if (!warden.Agent.pathPending && warden.Agent.remainingDistance < 0.5f)
                {
                    OnReachedRetreatPoint(warden);
                }
            }
            // If we are advancing, check if we've arrived
            else if (_subState == SubState.Advancing)
            {
                if (!warden.Agent.pathPending && warden.Agent.remainingDistance < 0.5f)
                {
                    OnReachedNextPhaseStartPoint(warden);
                }
            }
        }
        
        private void OnReachedRetreatPoint(WardenBossController warden)
        {
            // Publish event to open the door for the player
            EventBus.Instance.Publish(new WardenReachedRetreatPointEvent());

            Transform nextDestination = null;

            // Perform phase-specific action
            if (warden.nextPhaseToEnter == 2)
            {
                SpawnDrones(warden);
                nextDestination = warden.Phase2StartPoint;
            }
            else if (warden.nextPhaseToEnter == 3)
            {
                RechargeShields(warden);
                nextDestination = warden.Phase3StartPoint;
            }

            // --- Start Advancing to next phase's start point ---
            _subState = SubState.Advancing;
            if (nextDestination != null)
            {
                warden.Agent.SetDestination(nextDestination.position);
            }
            else
            {
                Debug.LogError($"Start point for Phase {warden.nextPhaseToEnter} is not set! Aborting transition.");
                GoToNextCombatState(warden);
            }
        }

        private void OnReachedNextPhaseStartPoint(WardenBossController warden)
        {
            warden.Agent.stoppingDistance = _originalStoppingDistance; // Restore original stopping distance
            GoToNextCombatState(warden);
        }

        private void GoToNextCombatState(WardenBossController warden)
        {
            if (warden.nextPhaseToEnter == 2)
                warden.TransitionToState(warden.Phase2State);
            else if (warden.nextPhaseToEnter == 3)
                warden.TransitionToState(warden.Phase3State);
            else
                warden.TransitionToState(warden.Phase1State); // Failsafe
        }

        private void SpawnDrones(WardenBossController warden)
        {
            for (int i = 0; i < warden.WardenData.DronesToSpawn; i++)
            {
                if (i < warden.DroneSpawnPoints.Length && warden.DronePrefab != null)
                {
                    Object.Instantiate(warden.DronePrefab, warden.DroneSpawnPoints[i].position, warden.DroneSpawnPoints[i].rotation);
                }
            }
        }

        private void RechargeShields(WardenBossController warden)
        {
            if (warden.Hitbox.HasShield)
            {
                warden.Hitbox.RechargeShield(warden.Hitbox.MaxShield);
            }
        }

        public override void OnCollisionEnter(WardenBossController warden) { }
    }
}
