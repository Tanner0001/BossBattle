using UnityEngine;
using _Project.Code.Gameplay.Combat; // For Hitbox

namespace _Project.Code.Gameplay.Pickups
{
    public class ShieldPickup : MonoBehaviour, IPickup
    {
        [SerializeField] private float shieldAmount = 50f;

        public void Collect(GameObject collector)
        {
            if (collector.TryGetComponent<Hitbox>(out Hitbox hitbox))
            {
                // Only recharge shield if has shield and not at max shield
                if (hitbox.HasShield && hitbox.CurrentShield < hitbox.MaxShield)
                {
                    hitbox.RechargeShield(shieldAmount);
                    Debug.Log($"{collector.name} collected ShieldPickup, recharged {shieldAmount}. Current Shield: {hitbox.CurrentShield}");
                    // Optionally, you might want to flash UI or play sound here
                }
                else
                {
                    Debug.Log($"{collector.name} collected ShieldPickup but already had full shield or no shield. Current Shield: {hitbox.CurrentShield}");
                }
                Destroy(gameObject); // Destroy pickup after collection regardless of recharge amount
            } // Corrected closing brace
        }
    }
}
