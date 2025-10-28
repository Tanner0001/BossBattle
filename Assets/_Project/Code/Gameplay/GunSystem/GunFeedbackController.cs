using UnityEngine;

namespace _Project.Code.Gameplay.Combat.GunSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class GunFeedbackController : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioClip fireClip;
        [SerializeField] private AudioClip reloadClip;
        [SerializeField] private AudioClip emptyClip;

        [Header("Visuals")]
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private Animator gunAnimator;
       // [SerializeField] private CameraShakeController cameraShake;

        private AudioSource _audio;

        private void Awake()
        {
            _audio = GetComponent<AudioSource>();
        }

        public void PlayFireFeedback()
        {
            if (muzzleFlash != null) muzzleFlash.Play();
            if (_audio && fireClip) _audio.PlayOneShot(fireClip);
            if (gunAnimator) gunAnimator.SetTrigger("Fire");
           // if (cameraShake) cameraShake.SmallShake();
        }

        public void PlayReloadFeedback()
        {
            if (_audio && reloadClip) _audio.PlayOneShot(reloadClip);
            if (gunAnimator) gunAnimator.SetTrigger("Reload");
        }

        public void PlayEmptyFeedback()
        {
            if (_audio && emptyClip) _audio.PlayOneShot(emptyClip);
        }
    }
}
