using UnityEngine;
using _Project.Code.Core.Events;

namespace _Project.Code.Gameplay.Combat
{
    public class Hitbox : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 50f;
        [SerializeField] private Renderer targetRenderer;
        
        public float CurrentHealth { get; private set; }
        public float MaxHealth => maxHealth;

        private Color _originalColor;

        private void Start()
        {
            CurrentHealth = maxHealth;
            if (targetRenderer != null)
                _originalColor = targetRenderer.material.color;
        }

        public void ApplyDamage(float amount)
        {
            CurrentHealth -= amount;
            FlashHit();

            if (CurrentHealth <= 0)
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
