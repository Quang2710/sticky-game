using System;

namespace StickmanTrap.Services
{
    /// <summary>
    /// Player-facing options. Persisted immediately on change; <see cref="Changed"/>
    /// lets audio, camera shake, etc. react without polling.
    /// </summary>
    public interface ISettingsService
    {
        bool MusicEnabled { get; set; }
        bool SfxEnabled { get; set; }
        bool ScreenShakeEnabled { get; set; }

        event Action Changed;
    }
}
