using UnityEngine;
using _Project.Code.Core.Events;
using System.Collections; // Needed for Coroutines
using System;

namespace _Project.Code.Gameplay.Combat
{
    public class Hitbox : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 50f;
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private bool isPlayer = false; // Flag to identify player hitbox
        [SerializeField] private EnemyType enemyType; // Type of enemy for event publishing

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
        public bool IsInvulnerable { get; set; } // Property to control damage immunity

        public event Action OnHealthChanged; // Event to notify subscribers of health/shield changes

        private Color _originalColor;
        private float _lastDamageTime; // To track when damage was last taken for shield regen delay

        // New method to initialize max health programmatically
        public void Initialize(float newMaxHealth)
        {
            maxHealth = newMaxHealth;
            CurrentHealth = maxHealth;
            if (hasShield)
            {
                CurrentShield = maxShield;
            }
            _lastDamageTime = -shieldRegenDelay; // Initialize so shield starts regenerating immediately if no damage
            OnHealthChanged?.Invoke(); // Notify after initialization
        }

        private void Start()
        {
            // If not initialized externally, initialize with inspector values
            if (CurrentHealth == 0 && maxHealth > 0) 
            {
                Initialize(maxHealth);
            }
            
            if (targetRenderer != null)
                _originalColor = targetRenderer.material.color;
        }

        private void Update()
        {
            if (hasShield && CurrentShield < maxShield && Time.time > _lastDamageTime + shieldRegenDelay)
            {
                CurrentShield += shieldRegenRate * Time.deltaTime;
                CurrentShield = Mathf.Min(CurrentShield, maxShield); // Cap shield at max
                OnHealthChanged?.Invoke();
            }
        }

        public void ApplyDamage(float amount)
        {
            if (IsInvulnerable) return; // Ignore damage if invulnerable

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
            OnHealthChanged?.Invoke();

            if (CurrentHealth <= 0)
                Die();
        }

        public void Heal(float amount)
        {
            if (CurrentHealth < maxHealth)
            {
                CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
                OnHealthChanged?.Invoke();
            }
        }

        public void RechargeShield(float amount)
        {
            if (hasShield && CurrentShield < maxShield)
            {
                CurrentShield = Mathf.Min(CurrentShield + amount, maxShield);
                OnHealthChanged?.Invoke();
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
                // Broadcast death event with data for feedback systems.
                EventBus.Instance.Publish(new EnemyDiedEvent { Type = this.enemyType, Position = transform.position });

                if (enemyType == EnemyType.Warden)
                {
                    EventBus.Instance.Publish(new WardenDiedEvent());
                }

                // Disable or pool the object. Do not destroy immediately if effects need its position.
                gameObject.SetActive(false);
            }
        }
    }
}
