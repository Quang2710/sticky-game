using UnityEngine;
using UnityEngine.EventSystems;
using StickmanTrap.Audio;
using StickmanTrap.Core;

namespace StickmanTrap.UI
{
    /// <summary>
    /// Drop-in hover/press feel for any UI element: scale toward 1.05 on hover,
    /// 0.95 on press, back to 1 on release; plays hover/click SFX via the audio
    /// service. Add next to (or instead of) a Button.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class UIButtonFX : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField, Range(1f, 1.3f)] private float _hoverScale = 1.05f;
        [SerializeField, Range(0.7f, 1f)] private float _pressScale = 0.95f;
        [SerializeField, Min(1f)] private float _speed = 14f;
        [SerializeField] private bool _playSound = true;

        private RectTransform _rt;
        private Vector3 _baseScale;
        private float _target = 1f;
        private bool _hovered;

        private void Awake()
        {
            _rt = (RectTransform)transform;
            _baseScale = _rt.localScale;
        }

        private void OnEnable()
        {
            _target = 1f;
            _hovered = false;
            if (_rt != null) _rt.localScale = _baseScale;
        }

        private void Update()
        {
            Vector3 want = _baseScale * _target;
            if ((_rt.localScale - want).sqrMagnitude < 0.000001f) return;
            _rt.localScale = Vector3.Lerp(_rt.localScale, want, _speed * Time.unscaledDeltaTime);
        }

        public void OnPointerEnter(PointerEventData e)
        {
            _hovered = true;
            _target = _hoverScale;
            if (_playSound) App.Audio?.PlaySfx(SfxId.ButtonHover);
        }

        public void OnPointerExit(PointerEventData e)
        {
            _hovered = false;
            _target = 1f;
        }

        public void OnPointerDown(PointerEventData e) => _target = _pressScale;

        public void OnPointerUp(PointerEventData e)
        {
            _target = _hovered ? _hoverScale : 1f;
            if (_playSound) App.Audio?.PlaySfx(SfxId.ButtonClick);
        }
    }
}
