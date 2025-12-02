using _Project.Code.Core.StateMachine;

namespace _Project.Code.Gameplay.Enemies.AI.States
{
    public class RetreatState : BaseState
    {
        private readonly DroneAIController _controller;

        public RetreatState(DroneAIController controller)
        {
            _controller = controller;
        }

        public override void Enter()
        {
            var retreatPoint = _controller.GetRetreatPoint();
            if (retreatPoint != null)
            {
                _controller.Agent.SetDestination(retreatPoint.position);
            }
        }

        public override void Update()
        {
            // Optional: Could trigger a 'Phase 2' event once the destination is reached.
            if (!_controller.Agent.pathPending && _controller.Agent.remainingDistance < 0.5f)
            {
                // Example: EventBus.Instance.Publish(new WardenRetreatedEvent());
                // For now, we can just disable the drone
                _controller.gameObject.SetActive(false);
            }
        }
    }
}
