using System;

namespace _Project.Code.Core.Events
{
    public class CameraPitchChangedEvent : IEvent
    {
        public float Pitch { get; }

        public CameraPitchChangedEvent(float pitch)
        {
            Pitch = pitch;
        }
    }
}
