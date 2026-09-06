using System;
using UnityEngine;
using StickmanTrap.Audio;
using StickmanTrap.Services;

namespace StickmanTrap.Core
{
    /// <summary>
    /// Static access point for the game's global services. Populated once by
    /// <see cref="GameBootstrap"/> before the first scene loads, so any scene
    /// (Boot, MainMenu, Gameplay) can be entered directly in the editor and still
    /// find everything it needs.
    /// </summary>
    public static class App
    {
        public static ISaveService Save { get; private set; }
        public static ISettingsService Settings { get; private set; }
        public static IEconomyService Economy { get; private set; }
        public static IAudioService Audio { get; private set; }
        public static IMonetizationService Ads { get; private set; }
        public static GameNavigation Navigation { get; private set; }

        public static bool Ready { get; private set; }

        public static void Register(
            ISaveService save,
            ISettingsService settings,
            IEconomyService economy,
            IAudioService audio,
            IMonetizationService ads,
            GameNavigation navigation)
        {
            Save = save;
            Settings = settings;
            Economy = economy;
            Audio = audio;
            Ads = ads;
            Navigation = navigation;
            Ready = true;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Save = null;
            Settings = null;
            Economy = null;
            Audio = null;
            Ads = null;
            Navigation = null;
            Ready = false;
        }
    }
}
