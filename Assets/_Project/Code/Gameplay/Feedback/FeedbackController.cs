using _Project.Code.Core.Audio;
using _Project.Code.Core.Events;
using UnityEngine;

namespace _Project.Code.Gameplay.Feedback
{
    public class FeedbackController : MonoBehaviour
    {
        [Header("Explosion VFX")]
        [SerializeField] private GameObject droneExplosionVFX;
        [SerializeField] private GameObject wardenExplosionVFX;

        [Header("Explosion SFX")]
        [SerializeField] private SoundEvent droneExplosionSound; // Changed from AudioClip to SoundEvent
        [SerializeField] private SoundEvent wardenExplosionSound; // Changed from AudioClip to SoundEvent

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<EnemyDiedEvent>(this, OnEnemyDied);
        }

        private void OnDisable()
        {
            EventBus.Instance.Unsubscribe<EnemyDiedEvent>(this);
        }

        private void OnEnemyDied(EnemyDiedEvent e)
        {
            switch (e.Type)
            {
                case EnemyType.Drone:
                    if (droneExplosionVFX != null)
                        Instantiate(droneExplosionVFX, e.Position, Quaternion.identity);
                    if (droneExplosionSound != null) // Check SoundEvent existence
                        EventBus.Instance.Publish(new PlaySoundEvent(droneExplosionSound, e.Position, null)); // Publish PlaySoundEvent
                    break;
                case EnemyType.Warden:
                    if (wardenExplosionVFX != null)
                        Instantiate(wardenExplosionVFX, e.Position, Quaternion.identity);
                    if (wardenExplosionSound != null) // Check SoundEvent existence
                        EventBus.Instance.Publish(new PlaySoundEvent(wardenExplosionSound, e.Position, null)); // Publish PlaySoundEvent
                    break;
            }
        }
    }
}
