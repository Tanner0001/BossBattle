using UnityEngine;
using _Project.Code.Core.ServiceLocator;
using _Project.Code.Core.Events; // For custom events

namespace _Project.Code.Gameplay.Pickups
{
    // Event published when a pickup is collected
    public class PickupCollectedEvent : IEvent
    {
        public GameObject Collector { get; }
        public GameObject PickupGameObject { get; }
        public IPickup CollectedPickup { get; }

        public PickupCollectedEvent(GameObject collector, GameObject pickupGameObject, IPickup collectedPickup)
        {
            Collector = collector;
            PickupGameObject = pickupGameObject;
            CollectedPickup = collectedPickup;
        }
    }

    public class PickupService : MonoBehaviourService
    {
        // Public method to be called when a pickup is collected
        public void OnPickupCollected(GameObject collector, GameObject pickupGameObject, IPickup collectedPickup)
        {
            Debug.Log($"Pickup collected by {collector.name}: {pickupGameObject.name}");
            EventBus.Instance.Publish(new PickupCollectedEvent(collector, pickupGameObject, collectedPickup));
        }
    }
}
