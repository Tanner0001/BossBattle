using _Project.Code.Core.StateMachine;

namespace _Project.Code.Gameplay.Enemies.AI.States
{
    public class SearchState : BaseState
    {
        private readonly DroneAIController _controller;

        public SearchState(DroneAIController controller)
        {
            _controller = controller;
        }

        public override void Enter()
        {
            if (_controller.PlayerTransform != null)
            {
                _controller.Agent.SetDestination(_controller.GetPlayerNavMeshPosition());
            }
            _controller.ResetSearchTimer();
        }
    }
}
