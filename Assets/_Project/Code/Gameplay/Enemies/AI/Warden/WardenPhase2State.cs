using UnityEngine;
using _Project.Code.Gameplay.Combat.GunSystem;

namespace BossBattle.Gameplay.Enemies.AI
{
    public class WardenPhase2State : WardenBaseState
    {
        private float _attackTimer;
        private GunBase _leftGunComponent;
        private GunBase _rightGunComponent;
        private float _strafeTimer;
        private Vector3 _strafeDirection;

        public override void EnterState(WardenBossController warden)
        {
            base.EnterState(warden);
            Debug.Log("Warden Entering Phase 2");

            // Ensure Warden is vulnerable
            warden.Hitbox.IsInvulnerable = false;
            warden.MaterialController?.ClearInvulnerableMaterial();

            // Configure guns
            warden.SingleGun.SetActive(false);
            warden.LeftGun.SetActive(true);
            warden.RightGun.SetActive(true);
            
            // Set phase-specific agent properties
            warden.Agent.stoppingDistance = warden.Phase2StoppingDistance;

            // Get gun components
            _leftGunComponent = warden.LeftGun.GetComponent<GunBase>();
            if (_leftGunComponent == null)
                Debug.LogError("GunBase component not found on LeftGun for WardenPhase2State.");

            _rightGunComponent = warden.RightGun.GetComponent<GunBase>();
            if (_rightGunComponent == null)
                Debug.LogError("GunBase component not found on RightGun for WardenPhase2State.");

            // Reset timers
            _attackTimer = warden.WardenData.AttackRate;
            _strafeTimer = 0f;
        }

        public override void UpdateState(WardenBossController warden)
        {
            if (warden.Player == null)
            {
                warden.Agent.isStopped = true;
                return;
            }
            
            warden.transform.LookAt(warden.Player);

            HandleMovement(warden);
            HandleAttack(warden);

            // Check for transition to Phase 3, with a grace period
            if (Time.time > timeEnteredState + 1.0f && warden.CurrentHealth <= warden.WardenData.Phase2HealthThreshold)
            {
                warden.nextPhaseToEnter = 3; // Set the next phase
                warden.TransitionToState(warden.TransitionState); // Go to transition state first
            }
        }

        private void HandleMovement(WardenBossController warden)
        {
            float distanceToPlayer = Vector3.Distance(warden.transform.position, warden.Player.position);

            // Disengage if player is too far
            if (distanceToPlayer > warden.Phase2EngagementRange)
            {
                warden.Agent.isStopped = false;
                warden.Agent.SetDestination(warden.Phase2StartPoint.position);
                return;
            }

            // Advance if player is outside stopping distance
            if (distanceToPlayer > warden.Agent.stoppingDistance)
            {
                warden.Agent.isStopped = false;
                warden.Agent.SetDestination(warden.GetPlayerNavMeshPosition());
            }
            // Strafe and reposition if player is within stopping distance
            else
            {
                warden.Agent.isStopped = true; // Stop forward movement
                
                _strafeTimer -= Time.deltaTime;
                if (_strafeTimer <= 0)
                {
                    // Pick a new strafe direction (left or right) and duration
                    _strafeDirection = Random.value > 0.5f ? warden.transform.right : -warden.transform.right;
                    _strafeTimer = Random.Range(1f, 2.5f); // Strafe for 1-2.5 seconds
                }
                
                // Use Move instead of SetDestination for smoother short-distance strafing
                warden.Agent.Move(_strafeDirection * (warden.Agent.speed / 2) * Time.deltaTime);
            }
        }

        private void HandleAttack(WardenBossController warden)
        {
            _attackTimer -= Time.deltaTime;
            // Attack if player is visible and within engagement range
            if (_attackTimer <= 0 && CanSeePlayer(warden))
            {
                Debug.Log("Warden attacks with dual guns!");
                _leftGunComponent?.Fire();
                _rightGunComponent?.Fire();
                _attackTimer = warden.WardenData.AttackRate;
            }
        }
        
        public override void OnCollisionEnter(WardenBossController warden)
        {
            // Handle collision
        }
    }
}
