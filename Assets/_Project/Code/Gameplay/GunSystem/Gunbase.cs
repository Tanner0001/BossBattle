using UnityEngine;
using _Project.Code.Core.Events;

namespace _Project.Code.Gameplay.Combat.GunSystem
{
    public class GunBase : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private GunData gunData;

        [Header("References")]
        [SerializeField] private Transform muzzlePoint;

        private int _currentAmmo;
        private bool _canFire = true;
        private bool _isReloading = false;

        private GunFeedbackController _feedback;

        private void Awake()
        {
            _feedback = GetComponent<GunFeedbackController>();
            if (gunData != null)
            {
                _currentAmmo = gunData.MaxAmmo;
            }
        }

        public void Fire()
        {
            if (!_canFire || _isReloading || gunData == null) return;

            if (_currentAmmo <= 0)
            {
                _feedback?.PlayEmptyFeedback();
                EventBus.Instance.Publish(new GunEmptyEvent());
                return;
            }

            switch (gunData.FireMode)
            {
                case FireMode.Projectile:
                    FireProjectile();
                    break;
                case FireMode.Hitscan:
                    FireHitscan();
                    break;
            }

            _currentAmmo--;
            _canFire = false;

            _feedback?.PlayFireFeedback();
            EventBus.Instance.Publish(new GunFiredEvent());
            EventBus.Instance.Publish(new AmmoChangedEvent(_currentAmmo, gunData.MaxAmmo));

            Invoke(nameof(ResetFire), gunData.FireRate);
        }

        private void FireProjectile()
        {
            if (gunData.ProjectilePrefab == null)
            {
                Debug.LogWarning("GunBase: Projectile prefab not assigned in GunData!");
                return;
            }
            if (muzzlePoint == null)
            {
                Debug.LogWarning("GunBase: Muzzle point not assigned!");
                return;
            }

            GameObject projectileInstance = Instantiate(gunData.ProjectilePrefab, muzzlePoint.position, muzzlePoint.rotation);
            if (projectileInstance.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.Damage = gunData.Damage;
            }
        }

        private void FireHitscan()
        {
            if (muzzlePoint == null)
            {
                Debug.LogWarning("GunBase: Muzzle point not assigned!");
                return;
            }

            if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out var hit, gunData.HitscanRange, gunData.HitscanLayers))
            {
                if (hit.collider.TryGetComponent<Hitbox>(out var hitbox))
                {
                    hitbox.ApplyDamage(gunData.Damage);
                }
            }
        }

        public void Reload()
        {
            if (_isReloading || gunData == null || _currentAmmo == gunData.MaxAmmo) return;

            _isReloading = true;
            _feedback?.PlayReloadFeedback();
            EventBus.Instance.Publish(new ReloadEvent());

            Invoke(nameof(FinishReload), gunData.ReloadTime);
        }

        private void FinishReload()
        {
            if (gunData != null)
            {
                _currentAmmo = gunData.MaxAmmo;
                _isReloading = false;
                EventBus.Instance.Publish(new AmmoChangedEvent(_currentAmmo, gunData.MaxAmmo));
            }
        }

        private void ResetFire() => _canFire = true;
    }
}
