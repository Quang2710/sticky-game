using UnityEngine;
using StickmanTrap.Audio;
using StickmanTrap.Services;

namespace StickmanTrap.Core
{
    /// <summary>
    /// Builds the global service graph exactly once, before any scene loads.
    /// Because it is a <see cref="RuntimeInitializeOnLoadMethodAttribute"/> hook,
    /// pressing Play on Boot, MainMenu or Gameplay all end up fully initialised.
    /// </summary>
    public static class GameBootstrap
    {
        private const int StartingCoins = 1250; // placeholder balance for the menu

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            if (App.Ready) return;

            var save = new LocalSaveService();
            var settings = new LocalSettingsService(save);
            var economy = new LocalEconomyService(save, StartingCoins);
            var ads = new MockMonetizationService();
            var navigation = new GameNavigation();

            AudioService audio = AudioService.Bootstrap();
            audio.ApplySettings(settings.MusicEnabled, settings.SfxEnabled);
            settings.Changed += () => audio.ApplySettings(settings.MusicEnabled, settings.SfxEnabled);

            App.Register(save, settings, economy, audio, ads, navigation);
        }
    }
}
