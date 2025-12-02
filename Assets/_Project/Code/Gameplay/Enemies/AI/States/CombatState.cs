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
            // Take control of rotation
            _controller.Agent.updateRotation = false;
            // Set the agent to stop at a specific distance for combat
            _controller.Agent.stoppingDistance = _controller.CombatStoppingDistance;
        }

        public override void Update()
        {
            if (_controller.PlayerTransform == null) return;

            // Continuously update destination to chase the player
            _controller.Agent.SetDestination(_controller.PlayerTransform.position);
            
            // Aim at player
            var lookPos = _controller.PlayerTransform.position - _controller.transform.position;
            lookPos.y = 0;

            // Safety check to prevent look rotation error if vector is zero
            if (lookPos.sqrMagnitude > 0.001f)
            {
                var rotation = Quaternion.LookRotation(lookPos);
                // Use a dedicated rotation speed for responsiveness
                _controller.transform.rotation = Quaternion.Slerp(_controller.transform.rotation, rotation, Time.deltaTime * 10f);
            }

            // Check for ammo before firing
            if (_controller.Gun.CurrentAmmo <= 0)
            {
                _controller.StateMachine.TransitionTo<ReloadState>();
                return;
            }
            
            // Fire weapon only if we have line of sight
            if (_controller.CanSeePlayer())
            {
                _controller.Gun.Fire();
            }
        }

        public override void Exit()
        {
            // Give control of rotation back to the agent
            _controller.Agent.updateRotation = true;
            // Reset stopping distance to its default value
            _controller.Agent.stoppingDistance = _controller.DefaultStoppingDistance;
        }
    }
}
