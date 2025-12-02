
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Code.Core.Audio
{
    public class AudioSourcePool
    {
        private readonly List<AudioSource> _pool = new();
        private readonly Transform _parent;

        public AudioSourcePool(Transform parent)
        {
            _parent = parent;
        }

        public AudioSource Get()
        {
            // Iterate in reverse to safely remove elements during iteration
            for (int i = _pool.Count - 1; i >= 0; i--)
            {
                var source = _pool[i];
                if (source == null) // If the source was destroyed externally
                {
                    _pool.RemoveAt(i);
                    continue;
                }

                if (!source.isPlaying && !source.gameObject.activeInHierarchy)
                {
                    source.gameObject.SetActive(true); // Reactivate before returning
                    return source;
                }
            }

            return CreateNewSource();
        }

        public void Return(AudioSource source)
        {
            source.Stop();
            source.clip = null; // Clear clip
            source.transform.SetParent(_parent); // Re-parent to pool root
            source.transform.localPosition = Vector3.zero; // Reset position
            source.gameObject.SetActive(false); // Deactivate for reuse
        }

        private AudioSource CreateNewSource()
        {
            var go = new GameObject("AudioSource");
            go.transform.SetParent(_parent);
            var source = go.AddComponent<AudioSource>();
            _pool.Add(source);
            return source;
        }
    }
}
