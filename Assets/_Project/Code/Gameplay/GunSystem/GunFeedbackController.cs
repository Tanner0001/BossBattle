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
        [SerializeField] private Transform muzzlePoint;
        //[SerializeField] private CameraShakeController cameraShake;

        private AudioSource _audio;

        private void Awake()
        {
            _audio = GetComponent<AudioSource>();
            _audio.playOnAwake = false;
            _audio.spatialBlend = 0f; 
            _audio.volume = 1f;
        }

        private void Start()
        {
            Debug.Log("Testing AudioSource playback manually...");
            if (fireClip != null)
                _audio.PlayOneShot(fireClip);
        }


        public void PlayFireFeedback()
        {

            if (fireClip != null)
                _audio.PlayOneShot(fireClip);
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
            if (reloadClip != null)
                _audio.PlayOneShot(reloadClip);

            if (gunAnimator != null)
                gunAnimator.SetTrigger("Reload");
        }

        public void PlayEmptyFeedback()
        {
            if (emptyClip != null)
                _audio.PlayOneShot(emptyClip);
        }
    }
}
