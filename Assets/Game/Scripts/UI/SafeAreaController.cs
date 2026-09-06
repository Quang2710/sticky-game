using UnityEngine;

namespace StickmanTrap.UI
{
    /// <summary>
    /// Insets a RectTransform to <see cref="Screen.safeArea"/> (notches, rounded
    /// corners on mobile browsers). Put it on a full-screen child of the Canvas
    /// that holds the top bar and panels.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaController : MonoBehaviour
    {
        private RectTransform _rt;
        private Rect _lastSafe;
        private int _lastW, _lastH;

        private void Awake()
        {
            _rt = (RectTransform)transform;
            Apply();
        }

        private void Update()
        {
            if (Screen.safeArea != _lastSafe || Screen.width != _lastW || Screen.height != _lastH)
                Apply();
        }

        private void Apply()
        {
            _lastSafe = Screen.safeArea;
            _lastW = Screen.width;
            _lastH = Screen.height;

            if (_lastW <= 0 || _lastH <= 0) return;

            Vector2 min = _lastSafe.position;
            Vector2 max = _lastSafe.position + _lastSafe.size;
            min.x /= _lastW; min.y /= _lastH;
            max.x /= _lastW; max.y /= _lastH;

            _rt.anchorMin = min;
            _rt.anchorMax = max;
            _rt.offsetMin = Vector2.zero;
            _rt.offsetMax = Vector2.zero;
        }
    }
}
