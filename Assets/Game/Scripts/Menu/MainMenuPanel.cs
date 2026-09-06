using System.Collections;
using UnityEngine;

namespace StickmanTrap.Menu
{
    /// <summary>Root menu screen. Plays a short logo pop-in the first time it shows.</summary>
    public sealed class MainMenuPanel : MenuPanel
    {
        [SerializeField] private RectTransform _logo;
        [SerializeField, Min(0.1f)] private float _logoIntroDuration = 0.45f;

        private bool _introPlayed;

        protected override void OnShow()
        {
            if (_introPlayed || _logo == null) return;
            _introPlayed = true;
            StartCoroutine(LogoIntro());
        }

        private IEnumerator LogoIntro()
        {
            const float overshoot = 1.70158f;
            float t = 0f;
            while (t < _logoIntroDuration)
            {
                float k = t / _logoIntroDuration;
                float p = k - 1f;
                float eased = 1f + (overshoot + 1f) * p * p * p + overshoot * p * p; // ease-out-back
                _logo.localScale = Vector3.one * Mathf.Lerp(0.8f, 1f, eased);
                t += Time.unscaledDeltaTime;
                yield return null;
            }
            _logo.localScale = Vector3.one;
        }
    }
}
