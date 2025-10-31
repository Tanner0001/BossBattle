
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
            foreach (var source in _pool)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }

            return CreateNewSource();
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
