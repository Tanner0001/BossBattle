using _Project.Code.Core.Events;
using _Project.Code.Core.ServiceLocator;
using UnityEngine;

namespace _Project.Code.Gameplay.Combat.GunSystem
{
    public class GunCameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform gunRootTransform; // The root transform of the gun model
        [SerializeField] [Range(1f, 30f)] private float followSpeed = 10f;

        private void Awake()
        {
            if (gunRootTransform == null)
            {
                gunRootTransform = transform; // Default to this GameObject's transform if not set
            }
        }

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<CameraPitchChangedEvent>(this, HandleCameraPitchChanged);
        }

        private void OnDisable()
        {
            EventBus.Instance.Unsubscribe<CameraPitchChangedEvent>(this);
        }

        private void HandleCameraPitchChanged(CameraPitchChangedEvent evt)
        {
            if (gunRootTransform == null) return;

            // Apply the camera's pitch to the gun's local rotation smoothly.
            // We only want to affect the X-axis (pitch), keeping the Y (yaw) and Z (roll) as they are,
            // or allowing other systems (like recoil) to modify them.
            Quaternion targetPitchRotation = Quaternion.Euler(evt.Pitch, gunRootTransform.localEulerAngles.y, gunRootTransform.localEulerAngles.z);
            gunRootTransform.localRotation = Quaternion.Slerp(gunRootTransform.localRotation, targetPitchRotation, followSpeed * Time.deltaTime);
        }
    }
}