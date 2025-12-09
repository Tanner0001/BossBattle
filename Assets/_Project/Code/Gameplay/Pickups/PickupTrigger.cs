using UnityEngine;
using _Project.Code.Core.ServiceLocator; // For ServiceLocator
using _Project.Code.Gameplay.Combat; // For Hitbox (to check if collector can receive pickup)

namespace _Project.Code.Gameplay.Pickups
{
    [RequireComponent(typeof(Collider))] // Ensure there's a collider for trigger
    public class PickupTrigger : MonoBehaviour
    {
        private IPickup _pickupEffect;
        private PickupService _pickupService;

        private void Awake()
        {
            // Get the IPickup component attached to this GameObject
            _pickupEffect = GetComponent<IPickup>();
            if (_pickupEffect == null)
            {
                Debug.LogError("PickupTrigger: No IPickup component found on this GameObject!", this);
                enabled = false; // Disable script if no pickup effect
                return;
            }

            // Ensure the collider is set as a trigger
            Collider collider = GetComponent<Collider>();
            if (collider != null && !collider.isTrigger)
            {
                collider.isTrigger = true;
                Debug.LogWarning($"PickupTrigger: Collider on {gameObject.name} was not a trigger. Setting it to trigger.", this);
            }
        }

        private void Start()
        {
            // Resolve PickupService via ServiceLocator
            if (!ServiceLocator.TryGet<PickupService>(out _pickupService))
            {
                Debug.LogError("PickupTrigger: PickupService not found in ServiceLocator!", this);
                enabled = false; // Disable script if service is missing
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"PickupTrigger: OnTriggerEnter called with {other.name}. Pickup name: {gameObject.name}", this);

            if (other.CompareTag("Player"))
            {
                Debug.Log($"PickupTrigger: {other.name} has 'Player' tag.", this);
                Hitbox collectorHitbox = other.GetComponentInChildren<Hitbox>(); // Search in children too
                if (collectorHitbox != null)
                {
                    Debug.Log($"PickupTrigger: {other.name} (or its child) has Hitbox component. Collecting pickup.", this);
                    _pickupEffect.Collect(other.gameObject); // Apply the pickup effect
                    _pickupService?.OnPickupCollected(other.gameObject, gameObject, _pickupEffect); // Notify service
                    // The individual pickup script (e.g., HealthPickup) should handle Destroy(gameObject)
                }
                else
                {
                    Debug.LogWarning($"PickupTrigger: {other.name} (Player) does NOT have a Hitbox component on itself or its children. Cannot collect pickup.", this);
                }
            }
            else
            {
                Debug.Log($"PickupTrigger: {other.name} does NOT have 'Player' tag. Not collecting.", this);
            }
        }
    }
}
