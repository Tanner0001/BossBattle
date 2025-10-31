
using System.Collections.Generic;
using _Project.Code.Core.Events;
using _Project.Code.Core.ServiceLocator;
using UnityEngine;

namespace _Project.Code.Core.Audio
{
    public class SoundManager : MonoBehaviourService
    {
        private AudioSourcePool _audioSourcePool;
        private readonly Dictionary<SoundEvent, float> _cooldowns = new();

        private void Awake()
        {
            _audioSourcePool = new AudioSourcePool(transform);
        }

        public override void Initialize()
        {
            EventBus.Instance.Subscribe<PlaySoundEvent>(this, OnPlaySound);
        }

        private void OnPlaySound(PlaySoundEvent evt)
        {
            if (evt.SoundEvent == null) return;

            if (_cooldowns.TryGetValue(evt.SoundEvent, out var lastPlayedTime))
            {
                if (Time.time - lastPlayedTime < evt.SoundEvent.Cooldown)
                {
                    return;
                }
            }

            var source = _audioSourcePool.Get();
            source.transform.position = evt.Position;
            source.clip = evt.SoundEvent.Clip;
            source.volume = evt.SoundEvent.Volume;
            source.pitch = evt.SoundEvent.Pitch;
            source.spatialBlend = evt.SoundEvent.SpatialBlend;
            source.loop = evt.SoundEvent.Loop;
            source.Play();

            _cooldowns[evt.SoundEvent] = Time.time;
        }

        private void OnDestroy()
        {
            EventBus.Instance.Unsubscribe<PlaySoundEvent>(this);
        }
    }
}
