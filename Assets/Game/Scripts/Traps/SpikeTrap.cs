using System.Collections;
using UnityEngine;

namespace StickmanTrap.Traps
{
    /// <summary>
    /// Spikes. Two modes:
    /// - <b>Always active</b>: permanently lethal (classic static spikes).
    /// - <b>Pop-up</b>: hidden, then on trigger (+ delay/telegraph) the blades shoot
    ///   out, stay lethal for a window, and retract. One collider does both the
    ///   detection and the kill volume; lethality is gated by <see cref="IsHot"/>.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class SpikeTrap : TrapBase
    {
        [Header("Spike")]
        [SerializeField] private bool _alwaysActive = true;
        [SerializeField] private Transform _blades;
        [SerializeField] private float _hiddenLocalY = -0.6f;
        [SerializeField] private float _shownLocalY = 0f;
        [SerializeField] private float _extendTime = 0.05f;
        [SerializeField] private float _retractTime = 0.25f;
        [SerializeField] private float _lethalTime = 0.6f;

        private bool _extended;
        private Vector3 _bladesBase;

        protected override bool IsHot => _alwaysActive || _extended;

        private void Awake()
        {
            if (_blades != null) _bladesBase = _blades.localPosition;
        }

        protected override void OnActivate()
        {
            if (_alwaysActive) return;
            StartCoroutine(PopSequence());
        }

        private IEnumerator PopSequence()
        {
            yield return MoveBlades(_shownLocalY, _extendTime);
            _extended = true;
            yield return Wait(_lethalTime);
            _extended = false;
            yield return MoveBlades(_hiddenLocalY, _retractTime);
        }

        private IEnumerator MoveBlades(float toLocalY, float dur)
        {
            if (_blades == null) yield break;
            Vector3 from = _blades.localPosition;
            Vector3 to = new Vector3(_bladesBase.x, toLocalY, _bladesBase.z);
            float t = 0f;
            while (t < dur)
            {
                _blades.localPosition = Vector3.Lerp(from, to, t / dur);
                t += Time.deltaTime;
                yield return null;
            }
            _blades.localPosition = to;
        }

        protected override void OnReset()
        {
            _extended = false;
            if (_blades != null)
                _blades.localPosition = new Vector3(
                    _bladesBase.x,
                    _alwaysActive ? _shownLocalY : _hiddenLocalY,
                    _bladesBase.z);
        }
    }
}
