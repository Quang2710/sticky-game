namespace StickmanTrap.Audio
{
    /// <summary>
    /// Central audio. Clips are optional everywhere — with no <c>AudioLibrary</c>
    /// assigned the game runs silently, never null-refs.
    /// </summary>
    public interface IAudioService
    {
        void PlaySfx(SfxId id);
        void PlayMusic(MusicId id);
        void StopMusic();

        /// <summary>Push the current on/off state from settings.</summary>
        void ApplySettings(bool musicEnabled, bool sfxEnabled);
    }
}
