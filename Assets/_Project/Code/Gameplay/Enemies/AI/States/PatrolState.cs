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
            _controller.CurrentPatrolScanOffset = 0; // Reset scan offset
            _controller.Agent.updateRotation = false; // Take control of rotation
            _controller.PatrolScanTimeOffset = Random.Range(0f, 100f); // Initialize random offset for desynchronized scanning

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
            // Update scan offset for visual rotation
            float offset = _controller.ScanAngleRange * Mathf.Sin((Time.time + _controller.PatrolScanTimeOffset) * _controller.ScanSpeed);
            _controller.CurrentPatrolScanOffset = offset;
            
            // Apply scan rotation to the model root
            // The base rotation is the agent's current forward direction
            _controller.ModelRoot.rotation = Quaternion.Slerp(_controller.ModelRoot.rotation, _controller.transform.rotation * Quaternion.Euler(0, offset, 0), Time.deltaTime * 5f);


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

        public override void Exit()
        {
            _controller.Agent.updateRotation = true; // Give control of rotation back to the agent
            _controller.CurrentPatrolScanOffset = 0; // Reset offset on exit
        }
    }
}
