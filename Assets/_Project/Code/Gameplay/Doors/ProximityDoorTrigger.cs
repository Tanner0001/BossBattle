using UnityEngine;
using System.Collections.Generic; // Needed for List
using System.Linq; // Needed for Any()

namespace _Project.Code.Gameplay.Doors
{
    [RequireComponent(typeof(Collider))]
    public class ProximityDoorTrigger : MonoBehaviour
    {
        [SerializeField] private DoorController doorToControl;
        [Tooltip("Any object with one of these tags will trigger the door.")]
        [SerializeField] private List<string> triggerTags = new List<string> { "Player" };
        [SerializeField] private bool closeOnExit = true;

        void Awake()
        {
            Collider col = GetComponent<Collider>();
            if (col == null)
            {
                Debug.LogError("ProximityDoorTrigger requires a Collider component.", this);
                enabled = false;
                return;
            }
            col.isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (doorToControl != null && triggerTags.Any(other.CompareTag))
            {
                doorToControl.OpenDoor();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (closeOnExit && doorToControl != null && triggerTags.Any(other.CompareTag))
            {
                doorToControl.CloseDoor();
            }
        }
    }
}
