using UnityEngine;
using _Project.Code.Core.Events;

namespace _Project.Code.Gameplay.Combat
{
    public class Hitbox : MonoBehaviour
    {
        [SerializeField] private float health = 50f;
        [SerializeField] private Renderer targetRenderer;
        private Color _originalColor;

        private void Start()
        {
            if (targetRenderer != null)
                _originalColor = targetRenderer.material.color;
        }

        public void ApplyDamage(float amount)
        {
            health -= amount;
            FlashHit();

            if (health <= 0)
                Die();
        }

        private void FlashHit()
        {
            if (targetRenderer != null)
            {
                targetRenderer.material.color = Color.red;
                Invoke(nameof(ResetColor), 0.1f);
            }
        }

        private void ResetColor()
        {
            if (targetRenderer != null)
                targetRenderer.material.color = _originalColor;
        }

        private void Die()
        {
            EventBus.Instance.Publish(new EnemyDestroyedEvent());
            Destroy(gameObject);
        }
    }
}
