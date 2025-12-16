using UnityEngine;

namespace _Project.Code.Gameplay.Doors
{
    [RequireComponent(typeof(Collider))]
    public class WardenDoorTrigger : MonoBehaviour
    {
        [SerializeField] private DoorController doorToControl;
        [SerializeField] private string wardenTag = "Warden"; // Make the tag configurable
        
        private bool _hasBeenTriggered = false;

        void Awake()
        {
            Collider col = GetComponent<Collider>();
            if (col == null)
            {
                Debug.LogError("WardenDoorTrigger requires a Collider component.", this);
                enabled = false;
                return;
            }
            col.isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (_hasBeenTriggered) return;

            if (doorToControl != null && other.CompareTag(wardenTag))
            {
                _hasBeenTriggered = true;
                Debug.Log($"Warden entered trigger, closing door: {doorToControl.gameObject.name}");
                doorToControl.CloseDoor();
                // Optional: Disable this component or the GameObject after it has served its purpose
                // enabled = false; 
            }
        }
    }
}
