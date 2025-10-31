using UnityEngine;
using _Project.Code.Core.ServiceLocator;
using _Project.Code.Gameplay.CameraSystems;

namespace _Project.Code.Gameplay.Combat.GunSystem
{
    public class GunFollowCamera : MonoBehaviour
    {
        [Header("Follow Settings")]
        [SerializeField] private float rotationSmoothness = 10f;

        private Transform _cameraTransform;
        private Quaternion _initialLocalRotation;

        private void Start()
        {
            var cameraService = ServiceLocator.Get<CameraService>();
            if (cameraService != null && cameraService.ActiveCameraTransform != null)
            {
                _cameraTransform = cameraService.ActiveVirtualCamera.transform;
            }

            _initialLocalRotation = transform.localRotation;
        }

        private void LateUpdate()
        {
            if (_cameraTransform == null) return;

            Quaternion targetRotation = _cameraTransform.localRotation * _initialLocalRotation;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSmoothness);
        }
    }
}
