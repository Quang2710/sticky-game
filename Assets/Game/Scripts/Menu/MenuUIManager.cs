using System.Collections.Generic;
using UnityEngine;
using StickmanTrap.Audio;
using StickmanTrap.Core;

namespace StickmanTrap.Menu
{
    /// <summary>
    /// Owns every menu panel (serialized refs — never Find) and shows exactly one
    /// at a time with a back-stack. <see cref="MainMenuController"/> and
    /// <see cref="MenuInput"/> drive it.
    /// </summary>
    public sealed class MenuUIManager : MonoBehaviour
    {
        [SerializeField] private MainMenuPanel _mainMenu;
        [SerializeField] private LevelSelectPanel _levelSelect;
        [SerializeField] private ShopPanel _shop;
        [SerializeField] private DailyPanel _daily;
        [SerializeField] private AchievementsPanel _achievements;
        [SerializeField] private SettingsPanel _settings;

        private readonly Stack<MenuPanel> _stack = new();

        private void Start()
        {
            foreach (var p in Panels())
                if (p != null) p.gameObject.SetActive(false);

            OpenRoot(_mainMenu);
        }

        private IEnumerable<MenuPanel> Panels()
        {
            yield return _mainMenu;
            yield return _levelSelect;
            yield return _shop;
            yield return _daily;
            yield return _achievements;
            yield return _settings;
        }

        public void ShowMainMenu() => OpenRoot(_mainMenu);
        public void ShowLevelSelect() => Push(_levelSelect);
        public void ShowShop() => Push(_shop);
        public void ShowDaily() => Push(_daily);
        public void ShowAchievements() => Push(_achievements);
        public void ShowSettings() => Push(_settings);

        public void GoBack()
        {
            if (_stack.Count <= 1) return;
            _stack.Pop().Close();
            _stack.Peek().Open();
            App.Audio?.PlaySfx(SfxId.PanelClose);
        }

        private void OpenRoot(MenuPanel panel)
        {
            if (panel == null) { Warn(nameof(panel)); return; }
            while (_stack.Count > 0) _stack.Pop().Close();
            _stack.Push(panel);
            panel.Open();
        }

        private void Push(MenuPanel panel)
        {
            if (panel == null) { Warn("panel"); return; }
            if (_stack.Count > 0) _stack.Peek().Close();
            _stack.Push(panel);
            panel.Open();
            App.Audio?.PlaySfx(SfxId.PanelOpen);
        }

        private static void Warn(string what)
            => Debug.LogWarning($"[MenuUIManager] {what} is not assigned in the Inspector.");
    }
}
