using UnityEngine;
using Unity.Cinemachine;
using _Project.Code.Core.Events;
using _Project.Code.Core.ServiceLocator;
using _Project.Code.Gameplay.CameraSystems.Profiles;
using _Project.Code.Gameplay.Input;
using _Project.Code.Gameplay.Player;

namespace _Project.Code.Gameplay.CameraSystems
{
    [RequireComponent(typeof(CinemachineCamera))]
    public class AimCamera : BaseCamera
    {
        [SerializeField] private AimCameraProfile _profile;

        private CinemachineCamera _vcam;
        private CameraService _cameraService;
        private PlayerService _playerService;

        private Vector3 _currentOffset;
        private float _currentFOV;
        private float _currentPitch;
        private float _currentYaw;
        private bool _isAiming;

        protected override void Awake()
        {
            base.Awake();
            _vcam = GetComponent<CinemachineCamera>();
        }

        private void Start()
        {
            _cameraService = ServiceLocator.Get<CameraService>();
            _playerService = ServiceLocator.Get<PlayerService>();

            SetupCinemachine();
            RegisterWithService();

            // Subscribe to events
            EventBus.Instance.Subscribe<PlayerRegisteredEvent>(this, OnPlayerRegistered);
            EventBus.Instance.Subscribe<LookInputEvent>(this, HandleLook);
            EventBus.Instance.Subscribe<AimStateChangedEvent>(this, HandleAimStateChanged);

            // Auto-connect to player if available
            ConnectToPlayer();

            // Initialize rotation values
            _currentPitch = 0f;
            if (_playerService.GetPlayerTransform() != null)
            {
                _currentYaw = _playerService.GetPlayerTransform().eulerAngles.y;
            }
        }

        private void SetupCinemachine()
        {
            if (_vcam == null || _profile == null) return;

            var lens = _vcam.Lens;
            lens.FieldOfView = _profile.FieldOfView;
            _vcam.Lens = lens;

            _vcam.Priority = _profile.Priority;
            _currentFOV = _profile.FieldOfView;
            _currentOffset = _profile.HipFireOffset;
        }

        private void RegisterWithService()
        {
            _cameraService.RegisterCamera(_vcam, setAsActive: true);
            Debug.Log($"AimCamera registered with CameraService");
        }

        private void ConnectToPlayer()
        {
            var player = _playerService.GetPlayerTransform();
            if (player != null)
            {
                var attachPoint = player.Find(_profile.AttachmentPoint);
                if (attachPoint == null)
                {
                    attachPoint = player;
                    Debug.LogWarning($"Attachment point '{_profile.AttachmentPoint}' not found, using player root");
                }

                transform.SetParent(attachPoint);
                transform.localPosition = _currentOffset;
                transform.localRotation = Quaternion.identity;

                Debug.Log($"AimCamera attached to: {attachPoint.name}");
            }
        }

        private void OnPlayerRegistered(PlayerRegisteredEvent evt)
        {
            if (evt.Player != null)
            {
                var attachPoint = evt.Player.Find(_profile.AttachmentPoint);
                if (attachPoint == null)
                {
                    attachPoint = evt.Player;
                    Debug.LogWarning($"Attachment point '{_profile.AttachmentPoint}' not found, using player root");
                }

                transform.SetParent(attachPoint);
                transform.localPosition = _currentOffset;
                transform.localRotation = Quaternion.identity;

                Debug.Log($"AimCamera attached to: {attachPoint.name}");
            }
        }

        private void HandleLook(LookInputEvent evt)
        {
            if (_profile == null) return;

            _currentYaw += evt.Input.x * _profile.LookSensitivityX;
            _currentPitch -= evt.Input.y * _profile.LookSensitivityY;
            _currentPitch = Mathf.Clamp(_currentPitch, _profile.MinPitch, _profile.MaxPitch);
        }

        private void HandleAimStateChanged(AimStateChangedEvent evt)
        {
            _isAiming = evt.IsAiming;
        }

        protected override void LateUpdate()
        {
            if (_profile == null) return;

            UpdateCameraState();
            ApplyRotation();

            base.LateUpdate();
        }

        private void UpdateCameraState()
        {
            // Update FOV
            var targetFOV = _isAiming ? _profile.AimFOV : _profile.FieldOfView;
            _currentFOV = Mathf.Lerp(_currentFOV, targetFOV, _profile.FOVTransitionSpeed * Time.deltaTime);
            var lens = _vcam.Lens;
            lens.FieldOfView = _currentFOV;
            _vcam.Lens = lens;

            // Update Offset
            var targetOffset = _isAiming ? _profile.AimOffset : _profile.HipFireOffset;
            _currentOffset = Vector3.Lerp(_currentOffset, targetOffset, _profile.OffsetTransitionSpeed * Time.deltaTime);
            transform.localPosition = _currentOffset;
        }

        private void ApplyRotation()
        {
            var player = _playerService.GetPlayerTransform();
            if (player != null)
            {
                var targetRotation = Quaternion.Euler(0, _currentYaw, 0);
                if (_profile.RotationDamping > 0.01f)
                {
                    player.rotation = Quaternion.Slerp(player.rotation, targetRotation,
                        (1f - _profile.RotationDamping) * 10f * Time.deltaTime);
                }
                else
                {
                    player.rotation = targetRotation;
                }
            }

            var targetPitch = Quaternion.Euler(_currentPitch, 0, 0);
            if (_profile.RotationDamping > 0.01f)
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetPitch,
                    (1f - _profile.RotationDamping) * 10f * Time.deltaTime);
            }
            else
            {
                transform.localRotation = targetPitch;
            }
        }

        private void OnDestroy()
        {
            EventBus.Instance?.Unsubscribe<PlayerRegisteredEvent>(this);
            EventBus.Instance?.Unsubscribe<LookInputEvent>(this);
            EventBus.Instance?.Unsubscribe<AimStateChangedEvent>(this);
        }
    }
}