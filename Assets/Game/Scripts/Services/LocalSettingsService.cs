using System;

namespace StickmanTrap.Services
{
    /// <summary>Settings stored via <see cref="ISaveService"/>. Defaults: everything on.</summary>
    public sealed class LocalSettingsService : ISettingsService
    {
        private const string KeyMusic = "settings.music";
        private const string KeySfx = "settings.sfx";
        private const string KeyShake = "settings.screenShake";

        private readonly ISaveService _save;
        private bool _music;
        private bool _sfx;
        private bool _shake;

        public event Action Changed;

        public LocalSettingsService(ISaveService save)
        {
            _save = save;
            _music = _save.Load(KeyMusic, true);
            _sfx = _save.Load(KeySfx, true);
            _shake = _save.Load(KeyShake, true);
        }

        public bool MusicEnabled
        {
            get => _music;
            set => Set(ref _music, value, KeyMusic);
        }

        public bool SfxEnabled
        {
            get => _sfx;
            set => Set(ref _sfx, value, KeySfx);
        }

        public bool ScreenShakeEnabled
        {
            get => _shake;
            set => Set(ref _shake, value, KeyShake);
        }

        private void Set(ref bool field, bool value, string key)
        {
            if (field == value) return;
            field = value;
            _save.Save(key, value);
            _save.Flush();
            Changed?.Invoke();
        }
    }
}
