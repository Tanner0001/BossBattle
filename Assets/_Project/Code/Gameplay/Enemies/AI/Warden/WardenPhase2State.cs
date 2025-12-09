using UnityEngine;
using _Project.Code.Gameplay.Combat.GunSystem; // Add this using directive if not already present

namespace BossBattle.Gameplay.Enemies.AI
{
    public class WardenPhase2State : WardenBaseState
    {
        private float _attackTimer;
        private GunBase _leftGunComponent;
        private GunBase _rightGunComponent;

        public override void EnterState(WardenBossController warden)
        {
            Debug.Log("Warden Entering Phase 2");
            warden.SingleGun.SetActive(false);
            // Ensure LeftGun and RightGun GameObjects are active/inactive as needed
            warden.LeftGun.SetActive(true);
            warden.RightGun.SetActive(true);
            _attackTimer = warden.WardenData.AttackRate;

            _leftGunComponent = warden.LeftGun.GetComponent<GunBase>();
            if (_leftGunComponent == null)
            {
                Debug.LogError("GunBase component not found on LeftGun GameObject for WardenPhase2State.");
            }

            _rightGunComponent = warden.RightGun.GetComponent<GunBase>();
            if (_rightGunComponent == null)
            {
                Debug.LogError("GunBase component not found on RightGun GameObject for WardenPhase2State.");
            }

            // Potentially move to Phase 2 start point if TransitionState didn't fully handle it or for re-engagement
            if (Vector3.Distance(warden.transform.position, warden.Phase2StartPoint.position) > 1f)
            {
                warden.Agent.SetDestination(warden.Phase2StartPoint.position);
            }
        }

        public override void UpdateState(WardenBossController warden)
        {
            // Look at player
            if (warden.Player != null)
            {
                warden.transform.LookAt(warden.Player);
            }

            // More aggressive Movement Logic for Phase 2
            if (CanSeePlayer(warden))
            {
                float distanceToPlayer = Vector3.Distance(warden.transform.position, warden.Player.position);

                if (distanceToPlayer > warden.CombatStoppingDistance + 3f) // Player too far, pursue aggressively
                {
                    warden.Agent.SetDestination(warden.GetPlayerNavMeshPosition());
                    warden.Agent.isStopped = false;
                }
                else if (distanceToPlayer < warden.CombatStoppingDistance - 3f) // Player too close, aggressive reposition/dodge
                {
                    Vector3 fleeDirection = (warden.transform.position - warden.Player.position).normalized;
                    // Try to move to a side flank position
                    Vector3 flankDirection = Quaternion.Euler(0, Random.Range(-45, 45), 0) * fleeDirection; 
                    Vector3 flankPosition = warden.transform.position + flankDirection * 7f; 
                    if (warden.Agent.destination != flankPosition)
                    {
                        warden.Agent.SetDestination(flankPosition);
                        warden.Agent.isStopped = false;
                    }
                }
                else // Optimal combat range, stop and attack
                {
                    warden.Agent.isStopped = true;
                }
            }
            else // Cannot see player, pursue last known position more actively
            {
                // More active pursuit: move towards last known player position if pathable
                if (Vector3.Distance(warden.transform.position, warden.GetPlayerNavMeshPosition()) > warden.CombatStoppingDistance)
                {
                    warden.Agent.SetDestination(warden.GetPlayerNavMeshPosition());
                    warden.Agent.isStopped = false;
                }
                else
                {
                    warden.Agent.isStopped = true; // Stop if at last known position
                }
            }


            // Attack logic for phase 2
            if (warden.Agent.isStopped && _attackTimer <= 0)
            {
                Attack(warden);
                _attackTimer = warden.WardenData.AttackRate;
            }
            _attackTimer -= Time.deltaTime;

            // Check for transition to Phase 3
            if (warden.CurrentHealth <= warden.WardenData.Phase2HealthThreshold)
            {
                warden.TransitionToState(warden.Phase3State);
            }
        }

        public override void OnCollisionEnter(WardenBossController warden)
        {
            // Handle collision
        }

        private void Attack(WardenBossController warden)
        {
            Debug.Log("Warden attacks with dual guns (burst/spread)!");
            _leftGunComponent?.Fire();
            _rightGunComponent?.Fire();
        }
    }
}
