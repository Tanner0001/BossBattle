using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Code.Gameplay.Combat.GunSystem
{
    [RequireComponent(typeof(GunBase))]
    public class GunInput : MonoBehaviour
    {
        private GunBase _gun;
        private PlayerInputActions _input;
        private bool _isFiring;

        private void Awake()
        {
            _gun = GetComponent<GunBase>();
            _input = new PlayerInputActions();
        }

        private void OnEnable()
        {
            _input.Enable();
            _input.Gameplay.Attack.started += OnAttackStarted;
            _input.Gameplay.Attack.canceled += OnAttackCanceled;

            _input.Gameplay.LockOn.performed += OnLockOn;
        }

        private void OnDisable()
        {
            _input.Gameplay.Attack.started -= OnAttackStarted;
            _input.Gameplay.Attack.canceled -= OnAttackCanceled;

            _input.Gameplay.LockOn.performed -= OnLockOn;
            _input.Disable();
        }

        private void Update()
        {
            if (_isFiring)
            {
                _gun.Fire();
            }
        }

        private void OnAttackStarted(InputAction.CallbackContext ctx)
        {
            Debug.Log("Test attack pressed");
            _isFiring = true;
        }

        private void OnAttackCanceled(InputAction.CallbackContext ctx)
        {
            _isFiring = false;
        }

        private void OnLockOn(InputAction.CallbackContext ctx)
        {
            Debug.Log("Lock-On triggered! (connect to targeting system later)");
        }
    }
}
