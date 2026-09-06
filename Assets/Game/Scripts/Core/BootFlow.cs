using System.Collections;
using UnityEngine;

namespace StickmanTrap.Core
{
    /// <summary>
    /// The only behaviour in Boot.unity: hold briefly (so a splash could show),
    /// wait for services, then hand off to the main menu.
    /// </summary>
    public sealed class BootFlow : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _minSplashSeconds = 0.3f;

        private IEnumerator Start()
        {
            float t = 0f;
            while (t < _minSplashSeconds || !App.Ready)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }

            App.Navigation.LoadMainMenu();
        }
    }
}
