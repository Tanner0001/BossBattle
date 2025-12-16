using UnityEngine;

namespace BossBattle.Gameplay.Enemies.AI
{
    public class WardenPhase3State : WardenBaseState
    {
        private float _newAbilityTimer;
        private bool _isWeakSpotExposed;
        private bool _hasReachedArenaStartPoint;

        public override void EnterState(WardenBossController warden)
        {
            base.EnterState(warden); // Set enter time for grace periods
            Debug.Log("Warden Entering Phase 3: Critical Vulnerability & Escalated Threats");

            // Ensure Warden is vulnerable
            warden.Hitbox.IsInvulnerable = false;
            warden.MaterialController?.ClearInvulnerableMaterial();

            // Initialize state
            _hasReachedArenaStartPoint = false;
            
            // Activate Phase 3 visuals
            warden.SingleGun.SetActive(false);
            warden.LeftGun.SetActive(true);
            warden.RightGun.SetActive(true);
            warden.WeakSpotTransform.gameObject.SetActive(true);
            _isWeakSpotExposed = true;
            _newAbilityTimer = warden.WardenData.Phase3NewAbilityCooldown;

            // Set destination to the Phase 3 start point
            if (warden.Phase3StartPoint != null)
            {
                warden.Agent.SetDestination(warden.Phase3StartPoint.position);
                warden.Agent.isStopped = false;
            }
            else
            {
                Debug.LogError("Phase3StartPoint is not set on WardenBossController!");
                _hasReachedArenaStartPoint = true; // Skip movement if no point is set
            }
        }

        public override void UpdateState(WardenBossController warden)
        {
            // First, check if the Warden has reached its designated start point for this phase
            if (!_hasReachedArenaStartPoint)
            {
                // Check if agent has reached the destination
                if (!warden.Agent.pathPending && warden.Agent.remainingDistance < 0.5f)
                {
                    Debug.Log("Warden has reached the Phase 3 arena start point. Engaging player.");
                    _hasReachedArenaStartPoint = true;
                    warden.Agent.isStopped = true; // Stop briefly before engaging
                }
                return; // Do nothing else until the start point is reached
            }

            // --- Combat Logic (only runs after reaching the start point) ---

            if (warden.Player == null)
            {
                warden.Agent.isStopped = true;
                return;
            }
            
            warden.transform.LookAt(warden.Player);

            // Logic for new powerful ability
            _newAbilityTimer -= Time.deltaTime;
            if (_newAbilityTimer <= 0)
            {
                PerformNewPowerfulAbility(warden);
                _newAbilityTimer = warden.WardenData.Phase3NewAbilityCooldown;
            }

            // Aggressive movement for Phase 3: relentless pursuit
            if (CanSeePlayer(warden))
            {
                warden.Agent.SetDestination(warden.GetPlayerNavMeshPosition());
                warden.Agent.isStopped = false;
                warden.Agent.stoppingDistance = 2f; // Get closer, but not on top of the player
            }
            else
            {
                 warden.Agent.SetDestination(warden.GetPlayerNavMeshPosition());
                 warden.Agent.isStopped = false;
            }

            // Placeholder for weak spot management
            if (_isWeakSpotExposed)
            {
                // Future logic...
            }
        }

        public override void OnCollisionEnter(WardenBossController warden)
        {
            // Handle collision
        }

        private void PerformNewPowerfulAbility(WardenBossController warden)
        {
            Debug.Log("Warden performs new powerful Phase 3 ability!");
            // Implement ability logic
        }
    }
}