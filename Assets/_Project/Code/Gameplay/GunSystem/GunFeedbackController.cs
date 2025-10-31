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
        [SerializeField] private Animator gunAnimator;
        [SerializeField] private Transform muzzlePoint; // Keep for positioning sound
        //[SerializeField] private CameraShakeController cameraShake;

        public void PlayFireFeedback()
        {
            // Publish a PlaySoundEvent for the fire sound at the muzzle's position
            if (fireSound != null)
                EventBus.Instance.Publish(new PlaySoundEvent(fireSound, muzzlePoint.position));
            else
                Debug.LogWarning("GunFeedbackController: Missing FireSound!", this);

            // Instantiate the muzzle flash and destroy it after its duration
            if (muzzleFlashPrefab != null)
            {
                var muzzleFlashInstance = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);
                if (muzzleFlashInstance.TryGetComponent<ParticleSystem>(out var particleSystem))
                {
                    Destroy(muzzleFlashInstance, particleSystem.main.duration);
                }
                else
                {
                    // If there's no particle system, destroy it after a default duration
                    Destroy(muzzleFlashInstance, 2f);
                }
            }

            if (gunAnimator != null)
                gunAnimator.SetTrigger("Fire");
        }

        public void PlayReloadFeedback()
        {
            // Publish a PlaySoundEvent for the reload sound at the gun's position
            if (reloadSound != null)
                EventBus.Instance.Publish(new PlaySoundEvent(reloadSound, transform.position));

            if (gunAnimator != null)
                gunAnimator.SetTrigger("Reload");
        }

        public void PlayEmptyFeedback()
        {
            // Publish a PlaySoundEvent for the empty sound at the gun's position
            if (emptySound != null)
                EventBus.Instance.Publish(new PlaySoundEvent(emptySound, transform.position));
        }
    }
}
