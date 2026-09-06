using System.Collections;
using UnityEngine;

namespace StickmanTrap.Menu
{
    /// <summary>
    /// Base for every menu screen. Owns a <see cref="CanvasGroup"/> and animates
    /// open/close as a short fade + scale (0.95 → 1). Subclasses add content via
    /// <see cref="OnShow"/> / <see cref="OnHide"/>.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class MenuPanel : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] private float _animDuration = 0.18f;
        [SerializeField, Range(0.5f, 1f)] private float _closedScale = 0.95f;

        private CanvasGroup _cg;
        private RectTransform _rt;
        private Coroutine _anim;

        public bool IsOpen { get; private set; }

        protected virtual void Awake()
        {
            _cg = GetComponent<CanvasGroup>();
            _rt = (RectTransform)transform;
            _cg.alpha = 0f;
            _cg.interactable = false;
            _cg.blocksRaycasts = false;
            _rt.localScale = Vector3.one * _closedScale;
            gameObject.SetActive(false);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            IsOpen = true;
            OnShow();
            Play(true);
        }

        public void Close()
        {
            if (!IsOpen) return;
            IsOpen = false;
            OnHide();
            Play(false);
        }

        private void Play(bool show)
        {
            if (_anim != null) StopCoroutine(_anim);
            _anim = StartCoroutine(Animate(show));
        }

        private IEnumerator Animate(bool show)
        {
            _cg.interactable = show;
            _cg.blocksRaycasts = show;

            float fromA = _cg.alpha, toA = show ? 1f : 0f;
            float fromS = _rt.localScale.x, toS = show ? 1f : _closedScale;
            float t = 0f;

            while (t < _animDuration)
            {
                float k = t / _animDuration;
                k = show ? 1f - (1f - k) * (1f - k) : k * k;
                _cg.alpha = Mathf.Lerp(fromA, toA, k);
                _rt.localScale = Vector3.one * Mathf.Lerp(fromS, toS, k);
                t += Time.unscaledDeltaTime;
                yield return null;
            }

            _cg.alpha = toA;
            _rt.localScale = Vector3.one * toS;
            if (!show) gameObject.SetActive(false);
            _anim = null;
        }

        protected virtual void OnShow() { }
        protected virtual void OnHide() { }
    }
}
