using UnityEngine;
using StickmanTrap.Core;

namespace StickmanTrap.Menu
{
    /// <summary>
    /// Glue between menu buttons and the rest of the game. Buttons call these
    /// methods via <c>Button.onClick</c>; nothing here loads scenes directly except
    /// through <see cref="Core.App.Navigation"/>.
    /// </summary>
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private MenuUIManager _ui;

        private void Awake()
        {
            if (_ui == null) _ui = FindAnyObjectByType<MenuUIManager>();
        }

        public void OnPlayClicked()
        {
            if (App.Navigation != null) App.Navigation.LoadGameplay();
            else Debug.LogWarning("[MainMenu] Navigation service missing — is GameBootstrap running?");
        }

        public void OnLevelsClicked() => _ui.ShowLevelSelect();
        public void OnShopClicked() => _ui.ShowShop();
        public void OnDailyClicked() => _ui.ShowDaily();
        public void OnAchievementsClicked() => _ui.ShowAchievements();
        public void OnSettingsClicked() => _ui.ShowSettings();
        public void OnBackClicked() => _ui.GoBack();
    }
}
