using _Project.Code.Core.Events;
using UnityEngine;

namespace _Project.Code.Gameplay.Combat.GunSystem
{
    public class GunRecoil : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform gunTransform;

        private Vector3 _initialPosition;
        private Vector3 _recoilPosition;

        private GunData _gunData;

        public void Initialize(GunData gunData)
        {
            _gunData = gunData;
            _initialPosition = gunTransform.localPosition;
            _recoilPosition = _initialPosition;
        }

        public void TriggerRecoil()
        {
            if (_gunData == null) return;

            _recoilPosition -= Vector3.forward * _gunData.RecoilAmount;

            // Publish camera recoil event
            EventBus.Instance.Publish(new CameraRecoilEvent(_gunData.RecoilAmount));
        }

        private void Update()
        {
            if (_gunData == null) return;

            // Apply recoil
            gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, _recoilPosition, Time.deltaTime * _gunData.RecoilSpeed);

            // Recover from recoil
            _recoilPosition = Vector3.Lerp(_recoilPosition, _initialPosition, Time.deltaTime * _gunData.RecoilRecoverySpeed);
        }
    }
}
