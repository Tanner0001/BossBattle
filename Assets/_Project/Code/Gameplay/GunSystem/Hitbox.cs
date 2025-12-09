using UnityEngine;
using _Project.Code.Core.Events;
using System.Collections; // Needed for Coroutines

namespace _Project.Code.Gameplay.Combat
{
    public class Hitbox : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 50f;
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private bool isPlayer = false; // Flag to identify player hitbox

        [Header("Shield Properties")]
        [SerializeField] private bool hasShield = false;
        [SerializeField] private float maxShield = 100f;
        [SerializeField] private float shieldRegenRate = 10f; // Shield points per second
        [SerializeField] private float shieldRegenDelay = 3f; // Delay before regen starts after taking damage

        public float CurrentHealth { get; private set; }
        public float MaxHealth => maxHealth;
        public float CurrentShield { get; private set; }
        public float MaxShield => maxShield;
        public bool HasShield => hasShield; // Public getter for hasShield

        private Color _originalColor;
        private float _lastDamageTime; // To track when damage was last taken for shield regen delay

        private void Start()
        {
            CurrentHealth = maxHealth;
            if (hasShield)
            {
                CurrentShield = maxShield;
            }
            _lastDamageTime = -shieldRegenDelay; // Initialize so shield starts regenerating immediately if no damage
            
            if (targetRenderer != null)
                _originalColor = targetRenderer.material.color;
        }

        private void Update()
        {
            if (hasShield && CurrentShield < maxShield && Time.time > _lastDamageTime + shieldRegenDelay)
            {
                CurrentShield += shieldRegenRate * Time.deltaTime;
                CurrentShield = Mathf.Min(CurrentShield, maxShield); // Cap shield at max
            }
        }

        public void ApplyDamage(float amount)
        {
            _lastDamageTime = Time.time; // Reset shield regen timer

            if (hasShield && CurrentShield > 0)
            {
                float damageToShield = amount;
                if (damageToShield > CurrentShield)
                {
                    damageToShield = CurrentShield;
                }

                CurrentShield -= damageToShield;
                amount -= damageToShield; // Remaining damage after shield absorption
            }

            if (amount > 0)
            {
                CurrentHealth -= amount;
                FlashHit();
            }

            if (CurrentHealth <= 0)
                Die();
        }

        public void Heal(float amount)
        {
            if (CurrentHealth < maxHealth)
            {
                CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
            }
        }

        public void RechargeShield(float amount)
        {
            if (hasShield && CurrentShield < maxShield)
            {
                CurrentShield = Mathf.Min(CurrentShield + amount, maxShield);
            }
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
            {
                targetRenderer.material.color = _originalColor;
            }
        }

        private void Die()
        {
            if (isPlayer)
            {
                EventBus.Instance.Publish(new PlayerDiedEvent());
                // Player death is typically handled by a Game Over system, not destroying the GameObject immediately
                // The PlayerHealthDisplay already calls a GameOver method.
                Debug.Log("Player Hitbox: Player Died Event Published.");
            }
            else
            {
                EventBus.Instance.Publish(new EnemyDestroyedEvent());
                Destroy(gameObject);
            }
        }
    }
}
