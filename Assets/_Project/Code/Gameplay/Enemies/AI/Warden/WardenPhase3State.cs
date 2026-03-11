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

            warden.Hitbox.IsInvulnerable = false;
            warden.MaterialController?.ClearInvulnerableMaterial();

            _hasReachedArenaStartPoint = false;
            _newAbilityTimer = warden.WardenData.Phase3NewAbilityCooldown;

            warden.SingleGun.SetActive(false);
            warden.LeftGun.SetActive(true);
            warden.RightGun.SetActive(true);

            ExposeWeakSpot(warden);

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
            if (!_hasReachedArenaStartPoint)
            {
                if (!warden.Agent.pathPending && warden.Agent.remainingDistance < 0.5f)
                {
                    _hasReachedArenaStartPoint = true;
                    warden.Agent.isStopped = true;
                }
                return; 
            }

            // --- Combat Logic (only runs after reaching the start point) ---

            if (warden.Player == null)
            {
                warden.Agent.isStopped = true;
                return;
            }

            warden.transform.LookAt(warden.Player);

            _newAbilityTimer -= Time.deltaTime;
            if (_newAbilityTimer <= 0)
            {
                PerformNewPowerfulAbility(warden);
                _newAbilityTimer = warden.WardenData.Phase3NewAbilityCooldown;
            }
            
            HandleVulnerabilityCycle(warden);

            if (CanSeePlayer(warden))
            {
                warden.Agent.SetDestination(warden.GetPlayerNavMeshPosition());
                warden.Agent.isStopped = false;
                warden.Agent.stoppingDistance = 2f;
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
            _isWeakSpotExposed = false;
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
        }

        private void PerformNewPowerfulAbility(WardenBossController warden)
        {
        }
    }
}