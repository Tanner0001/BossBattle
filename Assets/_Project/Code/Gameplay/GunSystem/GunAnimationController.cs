using UnityEngine;
using _Project.Code.Core.Events;
using _Project.Code.Gameplay.Player.Events;

namespace _Project.Code.Gameplay.Combat.GunSystem
{
    [RequireComponent(typeof(Animator))]
    public class GunAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator _playerAnimator;
        private Animator _gunAnimator;

        // Using string hashes is more performant than using strings directly
        private readonly int _fireHash = Animator.StringToHash("Fire");
        private readonly int _reloadHash = Animator.StringToHash("Reload");

        private readonly int _speedHash = Animator.StringToHash("Speed");
        private readonly int _strafeXHash = Animator.StringToHash("StrafeX");
        private readonly int _strafeYHash = Animator.StringToHash("StrafeY");


        private void Awake()
        {
            _gunAnimator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<GunFiredEvent>(this, OnGunFired);
            EventBus.Instance.Subscribe<ReloadEvent>(this, OnReload);
        }

        private void OnDisable()
        {
            EventBus.Instance.Unsubscribe<GunFiredEvent>(this);
            EventBus.Instance.Unsubscribe<ReloadEvent>(this);
        }

        private void Update()
        {
            if (_playerAnimator == null)
                return;

            _gunAnimator.SetFloat(_speedHash, _playerAnimator.GetFloat(_speedHash));
            _gunAnimator.SetFloat(_strafeXHash, _playerAnimator.GetFloat(_strafeXHash));
            _gunAnimator.SetFloat(_strafeYHash, _playerAnimator.GetFloat(_strafeYHash));
        }

        private void OnGunFired(GunFiredEvent evt)
        {
            _gunAnimator.SetTrigger(_fireHash);
        }

        private void OnReload(ReloadEvent evt)
        {
            _gunAnimator.SetTrigger(_reloadHash);
        }
    }
}
