using UnityEngine;
using _Project.Code.Core.Events;
using System.Collections;

namespace _Project.Code.Gameplay.Combat.GunSystem
{
    [RequireComponent(typeof(GunRecoil))]
    public class GunBase : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private GunData gunData;

        [Header("References")]
        [SerializeField] private Transform muzzlePoint;

        private int _currentAmmo;
        private bool _canFire = true;
        private bool _isReloading = false;

        public bool IsReloading => _isReloading;
        public int CurrentAmmo => _currentAmmo;

        private GunFeedbackController _feedback;
        private GunRecoil _gunRecoil;

        private void Awake()
        {
            _feedback = GetComponent<GunFeedbackController>();
            _gunRecoil = GetComponent<GunRecoil>();

            if (gunData != null)
            {
                _currentAmmo = gunData.MaxAmmo;
                _gunRecoil.Initialize(gunData);
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
            _gunRecoil.TriggerRecoil();
            EventBus.Instance.Publish(new GunFiredEvent());
            EventBus.Instance.Publish(new AmmoChangedEvent(_currentAmmo, gunData.MaxAmmo));

            StartCoroutine(ResetFireCoroutine(gunData.FireRate));
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

            Vector3 startPoint = muzzlePoint.position;
            Vector3 direction = muzzlePoint.forward;
            Vector3 endPoint;

            if (Physics.Raycast(startPoint, direction, out var hit, gunData.HitscanRange, gunData.HitscanLayers))
            {
                endPoint = hit.point;
                if (hit.collider.TryGetComponent<Hitbox>(out var hitbox))
                {
                    hitbox.ApplyDamage(gunData.Damage);
                }
            }
            else
            {
                endPoint = startPoint + direction * gunData.HitscanRange;
            }

            // Use the projectile script for the visual tracer
            if (gunData.HitscanVisualPrefab != null)
            {
                var visualInstance = Instantiate(gunData.HitscanVisualPrefab, startPoint, Quaternion.LookRotation(direction));
                if (visualInstance.TryGetComponent<Projectile>(out var projectileTracer))
                {
                    projectileTracer.InitializeAsVisualTracer(endPoint);
                }
                else
                {
                    // Fallback for prefabs without the projectile script
                    Debug.LogWarning($"Prefab {gunData.HitscanVisualPrefab.name} is missing Projectile script for visual tracer. Destroying after 2 seconds.");
                    Destroy(visualInstance, 2f);
                }
            }
        }

        public void Reload()
        {
            if (_isReloading || gunData == null || _currentAmmo == gunData.MaxAmmo) return;

            _isReloading = true;
            _feedback?.PlayReloadFeedback();
            EventBus.Instance.Publish(new ReloadEvent());

            StartCoroutine(ReloadCoroutine(gunData.ReloadTime));
        }

        private IEnumerator ReloadCoroutine(float reloadTime)
        {
            yield return new WaitForSeconds(reloadTime);
            if (gunData != null)
            {
                _currentAmmo = gunData.MaxAmmo;
                _isReloading = false;
                EventBus.Instance.Publish(new AmmoChangedEvent(_currentAmmo, gunData.MaxAmmo));
            }
        }

        private IEnumerator ResetFireCoroutine(float fireRate)
        {
            yield return new WaitForSeconds(fireRate);
            _canFire = true;
        }
    }
}
