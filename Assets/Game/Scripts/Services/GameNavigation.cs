using UnityEngine.SceneManagement;
using StickmanTrap.Core;

namespace StickmanTrap.Services
{
    /// <summary>
    /// The one place scene loads happen. UI calls <c>App.Navigation.LoadGameplay()</c>
    /// rather than touching <see cref="SceneManager"/> or hard-coding scene names.
    /// </summary>
    public sealed class GameNavigation
    {
        public void LoadBoot() => SceneManager.LoadScene(SceneNames.Boot);
        public void LoadMainMenu() => SceneManager.LoadScene(SceneNames.MainMenu);
        public void LoadGameplay() => SceneManager.LoadScene(SceneNames.Gameplay);
    }
}
