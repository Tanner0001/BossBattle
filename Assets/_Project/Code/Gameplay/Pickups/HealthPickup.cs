using UnityEngine;
using _Project.Code.Gameplay.Combat; // For Hitbox

namespace _Project.Code.Gameplay.Pickups
{
    public class HealthPickup : MonoBehaviour, IPickup
    {
        [SerializeField] private float healthAmount = 25f;

        public void Collect(GameObject collector)
        {
            // Debug.Log("HealthPickup.Collect() method ENTERED!"); // Removed temporary log
            Hitbox hitbox = collector.GetComponentInChildren<Hitbox>(); // Search in children too
            if (hitbox != null)
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
            else
            {
                // Debug.Log("Doesnt have a collector"); // User's custom log, removed
                Debug.LogWarning($"HealthPickup: Collector {collector.name} (or its child) does NOT have a Hitbox component. Cannot heal.", this);
            }
        }
    }
}
