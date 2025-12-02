using System.Collections.Generic;
using _Project.Code.Core.StateMachine;
using UnityEngine;

namespace _Project.Code.Gameplay.Enemies.AI.States
{
    public class PatrolState : BaseState
    {
        private readonly DroneAIController _controller;
        private IReadOnlyList<Transform> _waypoints;
        private int _waypointIndex;

        public PatrolState(DroneAIController controller)
        {
            _controller = controller;
        }

        public override void Enter()
        {
            var route = _controller.GetPatrolRoute();
            if (route != null && route.Waypoints.Count > 0)
            {
                _waypoints = route.Waypoints;
                var targetWaypoint = _waypoints[0];
                if (targetWaypoint != null)
                    _controller.Agent.SetDestination(targetWaypoint.position);
            }
        }

        public override void Update()
        {
            // Waypoint cycling
            if (_waypoints != null && _waypoints.Count > 0)
            {
                if (!_controller.Agent.pathPending && _controller.Agent.remainingDistance < 0.5f)
                {
                    _waypointIndex = (_waypointIndex + 1) % _waypoints.Count;
                    var targetWaypoint = _waypoints[_waypointIndex];
                    if (targetWaypoint != null)
                        _controller.Agent.SetDestination(targetWaypoint.position);
                }
            }
        }
    }
}
