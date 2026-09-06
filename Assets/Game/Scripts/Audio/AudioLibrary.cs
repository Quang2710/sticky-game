using System;
using UnityEngine;

namespace StickmanTrap.Audio
{
    /// <summary>
    /// Maps ids to clips. Every entry is optional; leave it empty during greyboxing.
    /// </summary>
    [CreateAssetMenu(menuName = "Stickman Trap/Audio Library", fileName = "AudioLibrary")]
    public sealed class AudioLibrary : ScriptableObject
    {
        [Serializable]
        public struct SfxEntry
        {
            public SfxId id;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume;
        }

        [Serializable]
        public struct MusicEntry
        {
            public MusicId id;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume;
        }

        [SerializeField] private SfxEntry[] _sfx = Array.Empty<SfxEntry>();
        [SerializeField] private MusicEntry[] _music = Array.Empty<MusicEntry>();

        public bool TryGetSfx(SfxId id, out AudioClip clip, out float volume)
        {
            foreach (var e in _sfx)
            {
                if (e.id != id || e.clip == null) continue;
                clip = e.clip;
                volume = e.volume <= 0f ? 1f : e.volume;
                return true;
            }
            clip = null;
            volume = 0f;
            return false;
        }

        public bool TryGetMusic(MusicId id, out AudioClip clip, out float volume)
        {
            foreach (var e in _music)
            {
                if (e.id != id || e.clip == null) continue;
                clip = e.clip;
                volume = e.volume <= 0f ? 1f : e.volume;
                return true;
            }
            clip = null;
            volume = 0f;
            return false;
        }
    }
}
