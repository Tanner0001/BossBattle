using _Project.Code.Core.Events;

namespace _Project.Code.Gameplay.Input
{
    public struct AimStateChangedEvent : IEvent
    {
        public bool IsAiming { get; }

        public AimStateChangedEvent(bool isAiming)
        {
            IsAiming = isAiming;
        }
    }
}