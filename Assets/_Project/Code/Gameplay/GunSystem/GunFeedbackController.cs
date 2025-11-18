using _Project.Code.Core.Audio;
using _Project.Code.Core.Events;
using UnityEngine;

namespace _Project.Code.Gameplay.Combat.GunSystem
{
    public class GunFeedbackController : MonoBehaviour
    {
        [Header("Audio Events")]
        [SerializeField] private SoundEvent fireSound;
        [SerializeField] private SoundEvent reloadSound;
        [SerializeField] private SoundEvent emptySound;

        [Header("Visuals")]
        [SerializeField] private GameObject muzzleFlashPrefab;
        [SerializeField] private Transform muzzlePoint;

        public void PlayFireFeedback()
        {
            if (fireSound != null)
                EventBus.Instance.Publish(new PlaySoundEvent(fireSound, muzzlePoint.position));
            else
                Debug.LogWarning("GunFeedbackController: Missing FireSound!", this);

            if (muzzleFlashPrefab != null)
            {
                var muzzleFlashInstance = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation, muzzlePoint);
                if (muzzleFlashInstance.TryGetComponent<ParticleSystem>(out var particleSystem))
                {
                    Destroy(muzzleFlashInstance, particleSystem.main.duration);
                }
                else
                {
                    Destroy(muzzleFlashInstance, 2f);
                }
            }
        }

        public void PlayReloadFeedback()
        {
            if (reloadSound != null)
                EventBus.Instance.Publish(new PlaySoundEvent(reloadSound, transform.position));
        }

        public void PlayEmptyFeedback()
        {
            if (emptySound != null)
                EventBus.Instance.Publish(new PlaySoundEvent(emptySound, transform.position));
        }
    }
}
