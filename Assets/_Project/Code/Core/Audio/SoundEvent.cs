
using UnityEngine;

namespace _Project.Code.Core.Audio
{
    [CreateAssetMenu(fileName = "NewSoundEvent", menuName = "Audio/Sound Event")]
    public class SoundEvent : ScriptableObject
    {
        [Header("Sound Properties")]
        public AudioClip Clip;
        [Range(0f, 1f)]
        public float Volume = 1f;
        [Range(0.1f, 3f)]
        public float Pitch = 1f;
        [Range(0f, 1f)]
        public float SpatialBlend = 1f;
        public bool Loop = false;

        [Header("Cooldown")]
        public float Cooldown = 0.1f;
    }
}
