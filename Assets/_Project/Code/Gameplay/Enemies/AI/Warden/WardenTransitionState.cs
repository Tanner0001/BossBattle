using UnityEngine;
using UnityEngine.AI;

namespace BossBattle.Gameplay.Enemies.AI
{
    public class WardenTransitionState : WardenBaseState
    {
        private bool _hasReachedRetreatPoint;

        public override void EnterState(WardenBossController warden)
        {
            Debug.Log("Warden Entering Transition State");
            _hasReachedRetreatPoint = false;
            if (warden.Agent != null && warden.RetreatPoint != null)
            {
                warden.Agent.SetDestination(warden.RetreatPoint.position);
            }
            else
            {
                Debug.LogError("NavMeshAgent or RetreatPoint not set on WardenBossController.");
            }
        }

        public override void UpdateState(WardenBossController warden)
        {
            if (!_hasReachedRetreatPoint)
            {
                if (!warden.Agent.pathPending && warden.Agent.remainingDistance < 0.5f && warden.Agent.velocity.sqrMagnitude < 0.1f)
                {
                    _hasReachedRetreatPoint = true;
                    SpawnDrones(warden);
                    // Now, move to the Phase 2 start point (instead of teleporting)
                    warden.Agent.SetDestination(warden.Phase2StartPoint.position);
                    Debug.Log("Warden reached retreat point, spawning drones and moving to Phase 2 start.");
                }
            }
            else // Drones spawned, now moving to Phase 2 start point
            {
                if (!warden.Agent.pathPending && warden.Agent.remainingDistance < 0.5f && warden.Agent.velocity.sqrMagnitude < 0.1f)
                {
                    Debug.Log("Warden reached Phase 2 start point, transitioning to Phase 2.");
                    warden.TransitionToState(warden.Phase2State);
                }
            }
        }

        public override void OnCollisionEnter(WardenBossController warden)
        {
            // Handle collision
        }

        private void SpawnDrones(WardenBossController warden)
        {
            Debug.Log("Spawning Drones...");
            for (int i = 0; i < warden.WardenData.DronesToSpawn; i++)
            {
                if (i < warden.DroneSpawnPoints.Length && warden.DronePrefab != null)
                {
                    Object.Instantiate(warden.DronePrefab, warden.DroneSpawnPoints[i].position, warden.DroneSpawnPoints[i].rotation);
                }
            }
        }
    }
}
