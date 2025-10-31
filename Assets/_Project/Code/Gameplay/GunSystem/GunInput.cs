using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Code.Gameplay.Combat.GunSystem
{
    [RequireComponent(typeof(GunBase))]
    public class GunInput : MonoBehaviour
    {
        private GunBase _gun;
        private PlayerInputActions _input;
        private bool _isFiring; // 1. new flag

        private void Awake()
        {
            _gun = GetComponent<GunBase>();
            _input = new PlayerInputActions();
        }

        private void OnEnable()
        {
            _input.Enable();

            // 2. subscribe to started/canceled instead of performed
            _input.Gameplay.Attack.started += OnAttackStarted;
            _input.Gameplay.Attack.canceled += OnAttackCanceled;

            _input.Gameplay.Reload.performed += OnReload;
            _input.Gameplay.LockOn.performed += OnLockOn;
        }

        private void OnDisable()
        {
            // 5. cleanup all event handlers
            _input.Gameplay.Attack.started -= OnAttackStarted;
            _input.Gameplay.Attack.canceled -= OnAttackCanceled;
            _input.Gameplay.Reload.performed -= OnReload;
            _input.Gameplay.LockOn.performed -= OnLockOn;

            _input.Disable();
        }

        private void Update()
        {
            // 4. continuously check while holding fire
            if (_isFiring && _gun != null)
            {
                _gun.Fire(); // GunBase handles fireRate internally
            }
        }

        private void OnAttackStarted(InputAction.CallbackContext ctx)
        {
            _isFiring = true;
            Debug.Log("Fire started (holding)");
        }

        private void OnAttackCanceled(InputAction.CallbackContext ctx)
        {
            _isFiring = false;
            Debug.Log("Fire canceled (released)");
        }

        private void OnReload(InputAction.CallbackContext ctx)
        {
            Debug.Log("Reload triggered");
            _gun?.Reload();
        }

        private void OnLockOn(InputAction.CallbackContext ctx)
        {
            Debug.Log("Lock-On triggered");
        }
    }
}
