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
        [SerializeField] private AudioClip droneExplosionSFX;
        [SerializeField] private AudioClip wardenExplosionSFX;

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
                    if (droneExplosionSFX != null)
                        AudioSource.PlayClipAtPoint(droneExplosionSFX, e.Position);
                    break;
                case EnemyType.Warden:
                    if (wardenExplosionVFX != null)
                        Instantiate(wardenExplosionVFX, e.Position, Quaternion.identity);
                    if (wardenExplosionSFX != null)
                        AudioSource.PlayClipAtPoint(wardenExplosionSFX, e.Position);
                    break;
            }
        }
    }
}
