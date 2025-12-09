using UnityEngine;

namespace BossBattle.Gameplay.Enemies.AI
{
    public class WardenPhase3State : WardenBaseState
    {
        private float _newAbilityTimer;
        private bool _isWeakSpotExposed;

        public override void EnterState(WardenBossController warden)
        {
            Debug.Log("Warden Entering Phase 3: Critical Vulnerability & Escalated Threats");
            // Activate Phase 3 visuals, new powerful weapon, and expose weak spot
            warden.SingleGun.SetActive(false); // Example: Turn off old guns
            warden.LeftGun.SetActive(true);
            warden.RightGun.SetActive(true);
            // Activate unique Phase 3 weapon/visuals here
            warden.WeakSpotTransform.gameObject.SetActive(true);
            _isWeakSpotExposed = true;
            _newAbilityTimer = warden.WardenData.Phase3NewAbilityCooldown;
            // Optionally spawn faster, smaller minions
            // SpawnPhase3Minions(warden);
            if (Vector3.Distance(warden.transform.position, warden.Phase3StartPoint.position) > 1f)
            {
                warden.Agent.SetDestination(warden.Phase3StartPoint.position);
            }
        }

        public override void UpdateState(WardenBossController warden)
        {
            // Look at player
            if (warden.Player != null)
            {
                warden.transform.LookAt(warden.Player);
            }

            // Logic for new powerful ability
            _newAbilityTimer -= Time.deltaTime;
            if (_newAbilityTimer <= 0)
            {
                PerformNewPowerfulAbility(warden);
                _newAbilityTimer = warden.WardenData.Phase3NewAbilityCooldown;
            }

            // Aggressive movement for Phase 3: relentless pursuit
            if (warden.Player != null && CanSeePlayer(warden))
            {
                // Constantly move towards the player, with a very small stopping distance or even none
                warden.Agent.SetDestination(warden.GetPlayerNavMeshPosition());
                warden.Agent.isStopped = false;
                warden.Agent.stoppingDistance = 0.5f; // Get very close
                
                // Could add logic for dash attacks or erratic movement here later
            }
            else if (warden.Player != null) // Cannot see player but player exists, move to last known position
            {
                 warden.Agent.SetDestination(warden.GetPlayerNavMeshPosition());
                 warden.Agent.isStopped = false;
            }
            else // No player, stop
            {
                warden.Agent.isStopped = true;
            }

            // Placeholder for weak spot management (to use _isWeakSpotExposed)
            if (_isWeakSpotExposed)
            {
                // Logic related to weak spot being exposed (e.g., visual effects, player interaction checks)
            }
        }

        public override void OnCollisionEnter(WardenBossController warden)
        {
            // Handle collision, especially for weak spot
            // Example: If collision is with player projectile and hits weak spot
            // warden.TakeDamage(damageAmount * warden.WardenData.WeakSpotMultiplier);
        }

        private void PerformNewPowerfulAbility(WardenBossController warden)
        {
            Debug.Log("Warden performs new powerful Phase 3 ability!");
            // Implement continuous laser, rapid-fire barrage, etc.
        }

        // private void SpawnPhase3Minions(WardenBossController warden)
        // {
        //     // Logic to spawn smaller, faster drones if desired
        // }
    }
}