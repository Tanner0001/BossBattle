namespace _Project.Code.Core.Events
{
    // Marker interface required by EventBus
    public interface IEvent { }

    // --- Gameplay Events ---

    public struct GunFiredEvent : IEvent { }

    public struct ReloadEvent : IEvent { }

    public struct GunEmptyEvent : IEvent { }

    public struct EnemyDestroyedEvent : IEvent { }

    public struct AmmoChangedEvent : IEvent
    {
        public int current;
        public int max;

        public AmmoChangedEvent(int current, int max)
        {
            this.current = current;
            this.max = max;
        }
    }
}
