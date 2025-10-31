using UnityEngine;
using _Project.Code.Core.Audio;
using _Project.Code.Core.Events;

namespace _Project.Code.Gameplay.Combat.GunSystem
{

    public class GunFeedbackController : MonoBehaviour
    {
        [Header("Audio Events")]
        [SerializeField] private SoundEvent fireSound;
        [SerializeField] private SoundEvent reloadSound;
        [SerializeField] private SoundEvent emptySound;

        [Header("Visuals")]
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private Animator gunAnimator;
        [SerializeField] private Transform muzzlePoint;
        //[SerializeField] private CameraShakeController cameraShake;




        public void PlayFireFeedback()
        {

            if (fireSound != null)
                EventBus.Instance.Publish(new PlaySoundEvent(fireSound, muzzlePoint.position));
            else
                Debug.LogWarning("GunFeedbackController: Missing FireClip!", this);

 
            if (muzzleFlash != null)
            {
                Instantiate(muzzleFlash, muzzlePoint.position, muzzlePoint.rotation);
            }
                muzzleFlash.Play();


            if (gunAnimator != null)
                gunAnimator.SetTrigger("Fire");

        }

        public void PlayReloadFeedback()
        {
            if (reloadSound != null)
                EventBus.Instance.Publish(new PlaySoundEvent(reloadSound, muzzlePoint.position));

            if (gunAnimator != null)
                gunAnimator.SetTrigger("Reload");
        }

        public void PlayEmptyFeedback()
        {
            if (emptySound != null)
                EventBus.Instance.Publish(new PlaySoundEvent(emptySound, muzzlePoint.position));
        }
    }
}
