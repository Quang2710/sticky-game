using UnityEngine;

namespace StickmanTrap.Inputs
{
    public enum InputScheme { Auto, Keyboard, Touch }

    /// <summary>
    /// Owns the active <see cref="IInputProvider"/> and ticks it once per frame
    /// (early, via execution order). Consumers read <see cref="Provider"/>.
    /// One instance per gameplay scene; referenced through serialized fields, not a singleton.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public sealed class InputService : MonoBehaviour
    {
        [SerializeField] private InputScheme _scheme = InputScheme.Auto;

        public IInputProvider Provider { get; private set; }
        public TouchInputProvider Touch { get; private set; }

        private KeyboardInputProvider _keyboard;

        private void Awake()
        {
            _keyboard = new KeyboardInputProvider();
            Touch = new TouchInputProvider();
            Provider = ResolveScheme() == InputScheme.Touch ? Touch : _keyboard;
        }

        private InputScheme ResolveScheme()
        {
            if (_scheme != InputScheme.Auto) return _scheme;
#if UNITY_ANDROID || UNITY_IOS
            return InputScheme.Touch;
#else
            return Application.isMobilePlatform ? InputScheme.Touch : InputScheme.Keyboard;
#endif
        }

        private void Update() => Provider?.Tick();

        public void UseTouch() => Provider = Touch;
        public void UseKeyboard() => Provider = _keyboard;
    }
}
