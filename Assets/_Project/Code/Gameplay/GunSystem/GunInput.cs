using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Code.Gameplay.Combat.GunSystem
{
    [RequireComponent(typeof(GunBase))]
    public class GunInput : MonoBehaviour
    {
        private GunBase _gun;
        private PlayerInputActions _input;

        private void Awake()
        {
            _gun = GetComponent<GunBase>();
            _input = new PlayerInputActions();
        }

        private void OnEnable()
        {
            _input.Enable();
            _input.Gameplay.Attack.performed += OnAttack;
            _input.Gameplay.Reload.performed += OnReload;
            _input.Gameplay.LockOn.performed += OnLockOn;
        }

        private void OnDisable()
        {
            _input.Gameplay.Attack.performed -= OnAttack;
            _input.Gameplay.Reload.performed -= OnReload;
            _input.Gameplay.LockOn.performed -= OnLockOn;
            _input.Disable();
        }

        private void OnAttack(InputAction.CallbackContext ctx)
        {
            if (_gun == null) return;
            Debug.Log("Test attack pressed");
            _gun.Fire();
        }

        private void OnReload(InputAction.CallbackContext ctx)
        {
            Debug.Log("Test reload pressed");
            _gun.Reload();
        }


        private void OnLockOn(InputAction.CallbackContext ctx)
        {
            Debug.Log("Lock-On triggered");
        }
    }
}
