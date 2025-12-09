namespace _Project.Code.Gameplay.Pickups
{
    // Interface for any collectible pickup in the game
    public interface IPickup
    {
        // Applies the pickup's effect to the collector
        void Collect(UnityEngine.GameObject collector);
    }
}
