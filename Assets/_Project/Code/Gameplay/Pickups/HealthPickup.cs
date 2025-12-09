using UnityEngine;
using _Project.Code.Gameplay.Combat; // For Hitbox

namespace _Project.Code.Gameplay.Pickups
{
    public class HealthPickup : MonoBehaviour, IPickup
    {
        [SerializeField] private float healthAmount = 25f;

        public void Collect(GameObject collector)
        {
            if (collector.TryGetComponent<Hitbox>(out Hitbox hitbox))
            {
                // Only heal if not at max health
                if (hitbox.CurrentHealth < hitbox.MaxHealth)
                {
                    hitbox.Heal(healthAmount);
                    Debug.Log($"{collector.name} collected HealthPickup, healed {healthAmount}. Current Health: {hitbox.CurrentHealth}");
                    // Optionally, you might want to flash UI or play sound here
                }
                else
                {
                    Debug.Log($"{collector.name} collected HealthPickup but was already at full health. Current Health: {hitbox.CurrentHealth}");
                }
                Destroy(gameObject); // Destroy pickup after collection regardless of healing amount
            } // Corrected closing brace
        }
    }
}
