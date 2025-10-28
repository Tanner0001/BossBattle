using UnityEngine;
using _Project.Code.Core.Events;

namespace _Project.Code.Gameplay.Combat.GunSystem
{
    public class GunBase : MonoBehaviour
    {
        [Header("Gun Settings")]
        [SerializeField] private int maxAmmo = 10;
        [SerializeField] private float fireRate = 0.2f;
        [SerializeField] private float reloadTime = 1.5f;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform muzzlePoint;

        private int _currentAmmo;
        private bool _canFire = true;
        private bool _isReloading = false;

        private GunFeedbackController _feedback;

        private void Awake()
        {
            _feedback = GetComponent<GunFeedbackController>();
            _currentAmmo = maxAmmo;
        }

        public void Fire()
        {
            if (!_canFire || _isReloading) return;

            if (_currentAmmo <= 0)
            {
                _feedback?.PlayEmptyFeedback();
                EventBus.Instance.Publish(new GunEmptyEvent());
                return;
            }

            if (projectilePrefab == null)
            {
                Debug.LogWarning("GunBase: Projectile prefab not assigned!");
                return;
            }
            if (muzzlePoint == null)
            {
                Debug.LogWarning("GunBase: Muzzle point not assigned!");
                return;
            }

            GameObject projectileInstance = Instantiate(projectilePrefab, muzzlePoint.position, muzzlePoint.rotation);

            _currentAmmo--;
            _canFire = false;

            _feedback?.PlayFireFeedback();
            EventBus.Instance.Publish(new GunFiredEvent());
            EventBus.Instance.Publish(new AmmoChangedEvent(_currentAmmo, maxAmmo));

            Debug.Log("GunBase.Fire() called — attempting to play feedback", this);


            Invoke(nameof(ResetFire), fireRate);
        }

        public void Reload()
        {
            if (_isReloading || _currentAmmo == maxAmmo) return;

            _isReloading = true;
            _feedback?.PlayReloadFeedback();
            EventBus.Instance.Publish(new ReloadEvent());

            Invoke(nameof(FinishReload), reloadTime);
        }

        private void FinishReload()
        {
            _currentAmmo = maxAmmo;
            _isReloading = false;
            EventBus.Instance.Publish(new AmmoChangedEvent(_currentAmmo, maxAmmo));
        }

        private void ResetFire() => _canFire = true;
    }
}
