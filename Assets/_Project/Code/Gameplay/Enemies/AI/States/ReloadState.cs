using _Project.Code.Core.StateMachine;
using UnityEngine;

namespace _Project.Code.Gameplay.Enemies.AI.States
{
    public class ReloadState : BaseState
    {
        private readonly DroneAIController _controller;

        public ReloadState(DroneAIController controller)
        {
            _controller = controller;
        }

        public override void Enter()
        {
            // Stop moving to perform the reload
            _controller.Agent.ResetPath();
            _controller.Gun.Reload();
        }

        public override void Update()
        {
            // Wait for the gun to finish its reload routine
            if (!_controller.Gun.IsReloading)
            {
                // Once reloaded, go back to searching for the player
                _controller.StateMachine.TransitionTo<SearchState>();
            }
        }
    }
}
