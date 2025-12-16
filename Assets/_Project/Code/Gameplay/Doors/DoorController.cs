using UnityEngine;
// using DG.Tweening; // If DOTween is used

namespace _Project.Code.Gameplay.Doors
{
    public class DoorController : MonoBehaviour
    {
        [Header("Door Movement")]
        public Vector3 openOffset = new Vector3(0, 3, 0); // Default to move up by 3 units
        public float animationDuration = 1.0f;
        
        private Vector3 _closedPosition;
        private Vector3 _openPosition;
        private bool _isOpen = false;

        void Awake()
        {
            _closedPosition = transform.localPosition; // Use local position for offset
            _openPosition = _closedPosition + openOffset;
        }

        public void OpenDoor()
        {
            if (_isOpen) return;
            _isOpen = true;
            // Example using transform.localPosition for simplicity.
            // Replace with DOTween or Animator logic if preferred.
            transform.localPosition = _openPosition;
            Debug.Log($"Door '{gameObject.name}' Opening...");
        }

        public void CloseDoor()
        {
            if (!_isOpen) return;
            _isOpen = false;
            // Example using transform.localPosition for simplicity.
            // Replace with DOTween or Animator logic if preferred.
            transform.localPosition = _closedPosition;
            Debug.Log($"Door '{gameObject.name}' Closing...");
        }

        // Optional: Method to check current state
        public bool IsOpen() => _isOpen;
        public bool IsClosed() => !_isOpen;
    }
}
