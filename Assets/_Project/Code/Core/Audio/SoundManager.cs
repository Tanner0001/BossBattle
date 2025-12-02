
using System.Collections.Generic;
using _Project.Code.Core.Events;
using _Project.Code.Core.ServiceLocator;
using UnityEngine;
using System.Collections; // Added for Coroutines

namespace _Project.Code.Core.Audio
{
    public class SoundManager : MonoBehaviourService
    {
        private AudioSourcePool _audioSourcePool;
        private readonly Dictionary<SoundEvent, float> _cooldowns = new();

        private void Awake()
        {
            _audioSourcePool = new AudioSourcePool(transform);
            DontDestroyOnLoad(this.gameObject);
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
            source.transform.parent = evt.ParentTransform; // Set parent here
            source.clip = evt.SoundEvent.Clip;
            source.volume = evt.SoundEvent.Volume;
            source.pitch = evt.SoundEvent.Pitch;
            source.spatialBlend = evt.SoundEvent.SpatialBlend;
            source.loop = evt.SoundEvent.Loop;
            source.Play();

            if (!source.loop) // Only return to pool if not looping
            {
                StartCoroutine(PlayAndReturnToPool(source, evt.SoundEvent.Clip.length));
            }

            _cooldowns[evt.SoundEvent] = Time.time;
        }

        private IEnumerator PlayAndReturnToPool(AudioSource source, float clipLength)
        {
            yield return new WaitForSeconds(clipLength);
            if (source == null) yield break; // Check if the source was destroyed prematurely
            _audioSourcePool.Return(source);
        }

        private void OnDestroy()
        {
            EventBus.Instance.Unsubscribe<PlaySoundEvent>(this);
        }
    }
}
