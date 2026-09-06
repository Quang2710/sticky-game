using UnityEngine;
using StickmanTrap.Core;

namespace StickmanTrap.Audio
{
    /// <summary>
    /// Persistent audio host: one looping music source + one SFX source (PlayOneShot).
    /// Bootstrapped from <c>Resources/AudioService</c> so it exists from any entry scene.
    /// </summary>
    public sealed class AudioService : MonoSingleton<AudioService>, IAudioService
    {
        [SerializeField] private AudioLibrary _library;
        [SerializeField, Range(0f, 1f)] private float _masterMusic = 0.6f;
        [SerializeField, Range(0f, 1f)] private float _masterSfx = 0.9f;

        private AudioSource _music;
        private AudioSource _sfx;
        private bool _musicEnabled = true;
        private bool _sfxEnabled = true;
        private MusicId _currentMusic = MusicId.None;

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;

            _music = gameObject.AddComponent<AudioSource>();
            _music.loop = true;
            _music.playOnAwake = false;

            _sfx = gameObject.AddComponent<AudioSource>();
            _sfx.loop = false;
            _sfx.playOnAwake = false;
        }

        /// <summary>Ensure a single instance exists (called by <see cref="GameBootstrap"/>).</summary>
        public static AudioService Bootstrap()
        {
            if (Instance != null) return Instance;

            var prefab = Resources.Load<GameObject>("AudioService");
            GameObject go = prefab != null ? Instantiate(prefab) : new GameObject("AudioService");
            go.name = "AudioService";
            return go.GetComponent<AudioService>() ?? go.AddComponent<AudioService>();
        }

        public void ApplySettings(bool musicEnabled, bool sfxEnabled)
        {
            _musicEnabled = musicEnabled;
            _sfxEnabled = sfxEnabled;

            if (!_musicEnabled) _music.Stop();
            else if (_currentMusic != MusicId.None && !_music.isPlaying) PlayMusic(_currentMusic);
        }

        public void PlaySfx(SfxId id)
        {
            if (!_sfxEnabled || _library == null || id == SfxId.None) return;
            if (_library.TryGetSfx(id, out var clip, out var vol))
                _sfx.PlayOneShot(clip, vol * _masterSfx);
        }

        public void PlayMusic(MusicId id)
        {
            _currentMusic = id;
            if (!_musicEnabled || _library == null || id == MusicId.None)
            {
                _music.Stop();
                return;
            }

            if (_library.TryGetMusic(id, out var clip, out var vol))
            {
                _music.clip = clip;
                _music.volume = vol * _masterMusic;
                _music.Play();
            }
            else
            {
                _music.Stop();
            }
        }

        public void StopMusic()
        {
            _currentMusic = MusicId.None;
            _music.Stop();
        }
    }
}
