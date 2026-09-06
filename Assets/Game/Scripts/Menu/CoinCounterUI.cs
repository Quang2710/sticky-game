using TMPro;
using UnityEngine;
using StickmanTrap.Core;

namespace StickmanTrap.Menu
{
    /// <summary>Shows the coin balance, updating only when it actually changes.</summary>
    public sealed class CoinCounterUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;

        private void OnEnable()
        {
            if (App.Economy != null)
            {
                App.Economy.CoinsChanged += Render;
                Render(App.Economy.Coins);
            }
            else
            {
                Render(0);
            }
        }

        private void OnDisable()
        {
            if (App.Economy != null) App.Economy.CoinsChanged -= Render;
        }

        private void Render(int coins)
        {
            if (_label != null) _label.text = coins.ToString("N0");
        }
    }
}
