using _Project.Code.Core.StateMachine;
using UnityEngine;

namespace _Project.Code.Gameplay.Enemies.AI.States
{
    public class CombatState : BaseState
    {
        private readonly DroneAIController _controller;

        public CombatState(DroneAIController controller)
        {
            _controller = controller;
        }

        public override void Enter()
        {
            // Stop moving when entering combat to focus on shooting
            _controller.Agent.ResetPath();
        }

        public override void Update()
        {
            if (_controller.PlayerTransform == null) return;
            
            // Aim at player
            var lookPos = _controller.PlayerTransform.position - _controller.transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            _controller.transform.rotation = Quaternion.Slerp(_controller.transform.rotation, rotation, Time.deltaTime * _controller.Agent.angularSpeed);

            // Fire weapon only if we have line of sight
            if (_controller.CanSeePlayer())
            {
                _controller.Gun.Fire();
            }
        }
    }
}
