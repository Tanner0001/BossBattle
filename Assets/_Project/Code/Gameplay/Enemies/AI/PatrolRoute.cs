using System.Collections.Generic;
using UnityEngine;

namespace _Project.Code.Gameplay.Enemies.AI
{
    public class PatrolRoute : MonoBehaviour
    {
        [Tooltip("The ordered list of transforms that make up the patrol route.")]
        [SerializeField] private List<Transform> waypoints = new List<Transform>();

        public IReadOnlyList<Transform> Waypoints => waypoints;

        private void OnDrawGizmos()
        {
            if (waypoints == null || waypoints.Count < 2)
                return;

            for (int i = 0; i < waypoints.Count; i++)
            {
                if (waypoints[i] == null) continue;

                Vector3 currentWaypoint = waypoints[i].position;
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(currentWaypoint, 0.3f);

                if (i > 0)
                {
                    if (waypoints[i-1] == null) continue;
                    Vector3 previousWaypoint = waypoints[i - 1].position;
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(previousWaypoint, currentWaypoint);
                }
            }
            
            // Draw a line from the last to the first waypoint to close the loop
            if (waypoints.Count > 1 && waypoints[0] != null && waypoints[waypoints.Count - 1] != null)
            {
                 Vector3 firstWaypoint = waypoints[0].position;
                 Vector3 lastWaypoint = waypoints[waypoints.Count - 1].position;
                 Gizmos.color = Color.gray;
                 Gizmos.DrawLine(lastWaypoint, firstWaypoint);
            }
        }

        public Transform GetWaypoint(int index)
        {
            if (waypoints == null || waypoints.Count == 0) return null;
            // Gracefully handle missing elements in the list
            int validIndex = index % waypoints.Count;
            return waypoints[validIndex];
        }

        public int GetWaypointCount()
        {
            return waypoints?.Count ?? 0;
        }
    }
}
