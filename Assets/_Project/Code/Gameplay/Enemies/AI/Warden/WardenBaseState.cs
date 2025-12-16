using UnityEngine;

namespace BossBattle.Gameplay.Enemies.AI
{
    public abstract class WardenBaseState
    {
        protected float timeEnteredState;

        public virtual void EnterState(WardenBossController warden)
        {
            timeEnteredState = Time.time;
        }
        public abstract void UpdateState(WardenBossController warden);
        public abstract void OnCollisionEnter(WardenBossController warden);

        // Utility method to check if the warden can see the player, adapted from DroneAIController
        protected bool CanSeePlayer(WardenBossController warden)
        {
            if (warden.Player == null) return false;

            Collider[] targetsInViewRadius = Physics.OverlapSphere(warden.ModelRoot.position, warden.viewRadius, warden.playerMask);

            foreach (var target in targetsInViewRadius)
            {
                Transform targetTransform = target.transform;
                Vector3 dirToTarget = (targetTransform.position - warden.ModelRoot.position).normalized;

                if (Vector3.Angle(warden.ModelRoot.forward, dirToTarget) < warden.viewAngle / 2)
                {
                    float distToTarget = Vector3.Distance(warden.ModelRoot.position, targetTransform.position);
                    if (!Physics.Raycast(warden.ModelRoot.position, dirToTarget, distToTarget, warden.obstacleMask))
                    {
                        return true; // Player is visible
                    }
                }
            }
            return false;
        }
        
        // Helper method from DroneAIController for Gizmos or advanced sight calculations if needed
        protected Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal, WardenBossController warden)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += warden.ModelRoot.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}

