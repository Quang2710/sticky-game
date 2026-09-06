using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using StickmanTrap.Core;
using StickmanTrap.Player;

namespace StickmanTrap.Traps
{
    /// <summary>
    /// Shared trap machinery: trigger detection (contact / zone / timer / event / manual),
    /// an optional activation delay with a blinking telegraph, one-shot vs. re-arming,
    /// event chaining, and reset-on-restart. Subclasses only implement the actual effect
    /// (<see cref="OnActivate"/>) and how to restore themselves (<see cref="OnReset"/>).
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class TrapBase : MonoBehaviour, ITrap
    {
        [Header("Identity")]
        [SerializeField] protected string _trapId = "trap";

        [Header("Trigger")]
        [SerializeField] protected TrapTrigger _trigger = TrapTrigger.Contact;
        [Tooltip("Seconds between the trigger and the trap actually firing.")]
        [SerializeField, Min(0f)] protected float _activationDelay = 0f;
        [SerializeField] protected bool _oneShot = true;
        [Tooltip("If not one-shot: seconds before the trap re-arms.")]
        [SerializeField, Min(0f)] protected float _rearmDelay = 1.5f;
        [Tooltip("Timer trigger: seconds between activations.")]
        [SerializeField, Min(0.05f)] protected float _timerInterval = 2f;
        [Tooltip("Event trigger: fire when this trap-event id is raised.")]
        [SerializeField] protected string _listenEventId = "";
        [Tooltip("Chaining: raise this trap-event id when this trap fires.")]
        [SerializeField] protected string _emitEventId = "";

        [Header("Telegraph")]
        [Tooltip("Optional object shown (blinking) during the activation delay.")]
        [SerializeField] protected GameObject _telegraph;
        [SerializeField, Min(0f)] protected float _telegraphBlinkRate = 10f;

        [Header("Hooks (sound / VFX wired later)")]
        [SerializeField] private UnityEvent _onArmedFired;
        [SerializeField] private UnityEvent _onActivated;
        [SerializeField] private UnityEvent _onResetHook;

        protected const string PlayerTag = "Player";

        public string TrapId => _trapId;
        public bool IsArmed => _armed;

        protected bool _armed = true;
        protected bool _activated;

        private float _timer;
        private Coroutine _sequence;

        /// <summary>Subclass hook: is touching the trap lethal *right now*?</summary>
        protected virtual bool IsHot => false;

        protected virtual void Start() => ApplyResetState();

        protected virtual void OnEnable()
        {
            GameEvents.LevelRestarting += ResetTrap;
            TrapEvents.Fired += OnTrapEvent;
        }

        protected virtual void OnDisable()
        {
            GameEvents.LevelRestarting -= ResetTrap;
            TrapEvents.Fired -= OnTrapEvent;
        }

        protected virtual void Update()
        {
            if (_trigger != TrapTrigger.Timer || !_armed) return;
            _timer += Time.deltaTime;
            if (_timer >= _timerInterval)
            {
                _timer = 0f;
                TryActivate(null);
            }
        }

        private void OnTrapEvent(string id)
        {
            if (_trigger == TrapTrigger.Event && _armed && id == _listenEventId)
                TryActivate(null);
        }

        private void OnTriggerEnter2D(Collider2D other) => HandleTouch(other);
        private void OnTriggerStay2D(Collider2D other) => HandleTouch(other);
        private void OnCollisionEnter2D(Collision2D c) => HandleTouch(c.collider);
        private void OnCollisionStay2D(Collision2D c) => HandleTouch(c.collider);

        private void HandleTouch(Collider2D other)
        {
            if (!other.CompareTag(PlayerTag)) return;

            if (IsHot) KillPlayer(other);

            bool touchTrigger = _trigger == TrapTrigger.Contact || _trigger == TrapTrigger.Zone;
            if (_armed && touchTrigger && AcceptsContact(other))
                TryActivate(other);
        }

        /// <summary>Subclass filter for contact/zone triggers (e.g. "only from on top").</summary>
        protected virtual bool AcceptsContact(Collider2D other) => true;

        // ---- ITrap ----

        public void Activate() => TryActivate(null);

        public void ResetTrap()
        {
            StopAllCoroutines();
            _sequence = null;
            _timer = 0f;
            _activated = false;
            _armed = true;
            ApplyResetState();
        }

        // ---- flow ----

        protected void TryActivate(Collider2D player)
        {
            if (!_armed) return;
            _armed = false;
            _onArmedFired?.Invoke();
            _sequence = StartCoroutine(Sequence());
        }

        private IEnumerator Sequence()
        {
            if (_activationDelay > 0f)
            {
                float t = 0f;
                while (t < _activationDelay)
                {
                    if (_telegraph != null)
                        _telegraph.SetActive(Mathf.FloorToInt(t * _telegraphBlinkRate) % 2 == 0);
                    t += Time.deltaTime;
                    yield return null;
                }
                if (_telegraph != null) _telegraph.SetActive(false);
            }

            _activated = true;
            OnActivate();
            _onActivated?.Invoke();
            TrapEvents.Raise(_emitEventId);

            if (!_oneShot)
            {
                yield return Wait(_rearmDelay);
                ApplyResetState();
                _activated = false;
                _armed = true;
            }
            _sequence = null;
        }

        private void ApplyResetState()
        {
            if (_telegraph != null) _telegraph.SetActive(false);
            OnReset();
            _onResetHook?.Invoke();
        }

        protected void KillPlayer(Collider2D col)
        {
            var pc = col.GetComponentInParent<PlayerController>();
            if (pc != null && pc.IsAlive)
                pc.Kill(_trapId);
        }

        protected static IEnumerator Wait(float seconds)
        {
            float t = 0f;
            while (t < seconds) { t += Time.deltaTime; yield return null; }
        }

        /// <summary>Fire the trap's effect. May start its own coroutines.</summary>
        protected abstract void OnActivate();

        /// <summary>Restore the trap to its initial visual / physical state.</summary>
        protected abstract void OnReset();
    }
}
