using UnityEngine;
using _Project.Code.Gameplay.Combat.GunSystem;

namespace BossBattle.Gameplay.Enemies.AI
{
    public class WardenPhase1State : WardenBaseState
    {
        private float _attackTimer;
        private GunBase _singleGunComponent;

        public override void EnterState(WardenBossController warden)
        {
            base.EnterState(warden);
            Debug.Log("Warden Entering Phase 1");

            // Ensure Warden is vulnerable
            warden.Hitbox.IsInvulnerable = false;
            warden.MaterialController?.ClearInvulnerableMaterial();

            warden.SingleGun.SetActive(true);
            warden.LeftGun.SetActive(false);
            warden.RightGun.SetActive(false);
            _attackTimer = warden.WardenData.AttackRate;

            _singleGunComponent = warden.SingleGun.GetComponent<GunBase>();
            if (_singleGunComponent == null)
            {
                Debug.LogError("GunBase component not found on SingleGun GameObject for WardenPhase1State.");
            }
        }

        public override void UpdateState(WardenBossController warden)
        {
            // Look at player
            if (warden.Player != null)
            {
                warden.transform.LookAt(warden.Player);
            }

            // Movement Logic
            if (CanSeePlayer(warden))
            {
                float distanceToPlayer = Vector3.Distance(warden.transform.position, warden.Player.position);

                if (distanceToPlayer > warden.Agent.stoppingDistance + 2f) // Player too far, move closer
                {
                    warden.Agent.SetDestination(warden.GetPlayerNavMeshPosition());
                    warden.Agent.isStopped = false;
                }
                else if (distanceToPlayer < warden.Agent.stoppingDistance - 2f) // Player too close, move away
                {
                    Vector3 fleeDirection = (warden.transform.position - warden.Player.position).normalized;
                    Vector3 fleePosition = warden.transform.position + fleeDirection * 5f; // Move 5 units away
                    if (warden.Agent.destination != fleePosition) // Only set destination if it's new
                    {
                        warden.Agent.SetDestination(fleePosition);
                        warden.Agent.isStopped = false;
                    }
                }
                else // Optimal range, stop and attack
                {
                    warden.Agent.isStopped = true;
                }
            }
            else // Cannot see player, patrol or move to last known position
            {
                // Simple placeholder: if cannot see, stop for now. Could implement patrol/search logic here.
                warden.Agent.isStopped = true;
            }

            // Attack logic for phase 1
            if (warden.Agent.isStopped && _attackTimer <= 0) // Only attack if stopped (at optimal range)
            {
                Attack(warden);
                _attackTimer = warden.WardenData.AttackRate;
            }
            _attackTimer -= Time.deltaTime; // Decrement timer always, even when moving

            // Check for transition
            if (warden.CurrentHealth <= warden.WardenData.Phase1HealthThreshold)
            {
                warden.nextPhaseToEnter = 2; // Set the next phase
                warden.TransitionToState(warden.TransitionState);
            }
        }

        public override void OnCollisionEnter(WardenBossController warden)
        {
            // Handle collision
        }

        private void Attack(WardenBossController warden)
        {
            Debug.Log("Warden attacks with single gun!");
            _singleGunComponent?.Fire();
        }
    }
}
