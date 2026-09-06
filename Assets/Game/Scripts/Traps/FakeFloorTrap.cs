using System.Collections;
using UnityEngine;

namespace StickmanTrap.Traps
{
    /// <summary>
    /// Looks exactly like a real platform. When the player stands on it: a short
    /// shake (the tell), then the collider drops away and the sprite falls + fades,
    /// so the player plunges through. Restores on level restart (or after
    /// <c>_rearmDelay</c> if not one-shot).
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class FakeFloorTrap : TrapBase
    {
        [Header("Fake Floor")]
        [SerializeField] private Transform _visual;
        [SerializeField] private Collider2D _solid;
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private float _shakeTime = 0.30f;
        [SerializeField] private float _shakeMagnitude = 0.05f;
        [SerializeField] private float _dropDistance = 3.5f;
        [SerializeField] private float _dropTime = 0.4f;

        private Vector3 _visualBase;

        private void Awake()
        {
            if (_visual == null) _visual = transform;
            if (_solid == null) _solid = GetComponent<Collider2D>();
            if (_sprite == null) _sprite = GetComponentInChildren<SpriteRenderer>();
            _visualBase = _visual.localPosition;
        }

        // Only collapse when the player is actually on top, not brushing a side.
        protected override bool AcceptsContact(Collider2D other)
            => _solid == null || other.bounds.min.y > _solid.bounds.center.y;

        protected override void OnActivate() => StartCoroutine(Collapse());

        private IEnumerator Collapse()
        {
            float t = 0f;
            while (t < _shakeTime)
            {
                _visual.localPosition = _visualBase + (Vector3)(Random.insideUnitCircle * _shakeMagnitude);
                t += Time.deltaTime;
                yield return null;
            }
            _visual.localPosition = _visualBase;

            if (_solid != null) _solid.enabled = false;

            Vector3 from = _visualBase;
            Vector3 to = _visualBase + Vector3.down * _dropDistance;
            t = 0f;
            while (t < _dropTime)
            {
                float k = t / _dropTime;
                _visual.localPosition = Vector3.Lerp(from, to, k * k);
                if (_sprite != null)
                {
                    Color c = _sprite.color;
                    c.a = 1f - k;
                    _sprite.color = c;
                }
                t += Time.deltaTime;
                yield return null;
            }
            _visual.localPosition = to;
        }

        protected override void OnReset()
        {
            _visual.localPosition = _visualBase;
            if (_solid != null) _solid.enabled = true;
            if (_sprite != null)
            {
                Color c = _sprite.color;
                c.a = 1f;
                _sprite.color = c;
            }
        }
    }
}
