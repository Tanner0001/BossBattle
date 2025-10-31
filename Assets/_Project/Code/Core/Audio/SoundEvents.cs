
using _Project.Code.Core.Events;
using UnityEngine;

namespace _Project.Code.Core.Audio
{
    public struct PlaySoundEvent : IEvent
    {
        public SoundEvent SoundEvent { get; }
        public Vector3 Position { get; }

        public PlaySoundEvent(SoundEvent soundEvent, Vector3 position)
        {
            SoundEvent = soundEvent;
            Position = position;
        }
    }
}
