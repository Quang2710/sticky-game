using UnityEngine;
using UnityEngine.InputSystem;

namespace StickmanTrap.Menu
{
    /// <summary>Keyboard shortcuts for the menu. ESC = back / close panel.
    /// (Enter-to-activate is handled by the EventSystem when a button is selected.)</summary>
    public sealed class MenuInput : MonoBehaviour
    {
        [SerializeField] private MenuUIManager _ui;

        private void Awake()
        {
            if (_ui == null) _ui = FindAnyObjectByType<MenuUIManager>();
        }

        private void Update()
        {
            var kb = Keyboard.current;
            if (kb != null && kb.escapeKey.wasPressedThisFrame && _ui != null)
                _ui.GoBack();
        }
    }
}
