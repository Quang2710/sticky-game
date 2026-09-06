using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using StickmanTrap.Core;

namespace StickmanTrap.Menu
{
    /// <summary>Binds three toggles to <see cref="Core.App.Settings"/> (two-way).</summary>
    public sealed class SettingsPanel : MenuPanel
    {
        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private Toggle _sfxToggle;
        [SerializeField] private Toggle _screenShakeToggle;

        protected override void OnShow()
        {
            var s = App.Settings;
            if (s == null)
            {
                Debug.LogWarning("[SettingsPanel] Settings service not available.");
                return;
            }

            Bind(_musicToggle, s.MusicEnabled, OnMusicChanged);
            Bind(_sfxToggle, s.SfxEnabled, OnSfxChanged);
            Bind(_screenShakeToggle, s.ScreenShakeEnabled, OnScreenShakeChanged);
        }

        private static void Bind(Toggle toggle, bool value, UnityAction<bool> handler)
        {
            if (toggle == null) return;
            toggle.onValueChanged.RemoveListener(handler);
            toggle.SetIsOnWithoutNotify(value);
            toggle.onValueChanged.AddListener(handler);
        }

        private void OnMusicChanged(bool v) { if (App.Settings != null) App.Settings.MusicEnabled = v; }
        private void OnSfxChanged(bool v) { if (App.Settings != null) App.Settings.SfxEnabled = v; }
        private void OnScreenShakeChanged(bool v) { if (App.Settings != null) App.Settings.ScreenShakeEnabled = v; }
    }
}
