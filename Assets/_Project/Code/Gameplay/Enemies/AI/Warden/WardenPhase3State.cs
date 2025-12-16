using UnityEngine;

namespace BossBattle.Gameplay.Enemies.AI
{
    public class WardenPhase3State : WardenBaseState
    {
        private float _newAbilityTimer;
        private bool _isWeakSpotExposed;
        private bool _hasReachedArenaStartPoint;
        private float _vulnerabilityTimer;

        public override void EnterState(WardenBossController warden)
        {
            base.EnterState(warden); // Set enter time for grace periods
            Debug.Log("Warden Entering Phase 3: Critical Vulnerability & Escalated Threats");

            // Ensure Warden is not invulnerable
            warden.Hitbox.IsInvulnerable = false;
            warden.MaterialController?.ClearInvulnerableMaterial();

            // Initialize state
            _hasReachedArenaStartPoint = false;
            _newAbilityTimer = warden.WardenData.Phase3NewAbilityCooldown;

            // Activate Phase 3 visuals
            warden.SingleGun.SetActive(false);
            warden.LeftGun.SetActive(true);
            warden.RightGun.SetActive(true);

            // Start the vulnerability cycle
            ExposeWeakSpot(warden);

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
            
            // Handle weak spot vulnerability cycle
            HandleVulnerabilityCycle(warden);

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
        }

        private void HandleVulnerabilityCycle(WardenBossController warden)
        {
            _vulnerabilityTimer -= Time.deltaTime;
            if (_vulnerabilityTimer <= 0)
            {
                if (_isWeakSpotExposed)
                {
                    HideWeakSpot(warden);
                }
                else
                {
                    ExposeWeakSpot(warden);
                }
            }
        }

        private void ExposeWeakSpot(WardenBossController warden)
        {
            Debug.Log("Warden weak spot is now EXPOSED.");
            _isWeakSpotExposed = true;
            warden.WeakSpotTransform.gameObject.SetActive(true);
            _vulnerabilityTimer = warden.WardenData.WeakSpotVulnerabilityDuration;

            if (warden.WeakSpotRenderer != null && warden.WardenData.WeakSpotExposedMaterial != null)
            {
                warden.WeakSpotRenderer.material = warden.WardenData.WeakSpotExposedMaterial;
            }

            if (warden.VulnerabilityTextElement != null)
            {
                warden.VulnerabilityTextElement.text = warden.WardenData.WeakSpotExposedText;
                warden.VulnerabilityTextElement.gameObject.SetActive(true);
            }
        }

        private void HideWeakSpot(WardenBossController warden)
        {
            Debug.Log("Warden weak spot is now PROTECTED.");
            _isWeakSpotExposed = false;
            // We can choose to disable the weakspot object entirely or just change its material
            // For this implementation, we'll just change the material.
            _vulnerabilityTimer = warden.WardenData.WeakSpotVulnerabilityCooldown;

            if (warden.WeakSpotRenderer != null && warden.WardenData.WeakSpotProtectedMaterial != null)
            {
                warden.WeakSpotRenderer.material = warden.WardenData.WeakSpotProtectedMaterial;
            }

            if (warden.VulnerabilityTextElement != null)
            {
                warden.VulnerabilityTextElement.gameObject.SetActive(false);
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