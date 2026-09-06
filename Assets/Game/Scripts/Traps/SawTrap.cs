using UnityEngine;

namespace StickmanTrap.Traps
{
    public enum SawMotion { Static, Horizontal, Vertical, Waypoints }

    /// <summary>
    /// Spinning blade — always lethal on contact. Optionally patrols horizontally,
    /// vertically (ping-pong) or along a waypoint path. Moved via a kinematic
    /// Rigidbody2D so collisions/triggers stay reliable.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class SawTrap : TrapBase
    {
        [Header("Saw")]
        [SerializeField] private Transform _blade;
        [SerializeField] private float _spinSpeed = 720f;      // deg/sec
        [SerializeField] private SawMotion _motion = SawMotion.Static;
        [Tooltip("Peak-to-peak travel for Horizontal (x) / Vertical (y).")]
        [SerializeField] private float _travel = 4f;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private Transform[] _waypoints;

        protected override bool IsHot => true;

        private Rigidbody2D _rb;
        private Vector2 _origin;
        private float _phase;
        private int _wp;
        private int _wpDir = 1;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            _origin = _rb.position;
        }

        protected override void Update()
        {
            base.Update();
            if (_blade != null)
                _blade.Rotate(0f, 0f, -_spinSpeed * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            Vector2 target = _origin;

            switch (_motion)
            {
                case SawMotion.Horizontal:
                    _phase += Time.fixedDeltaTime * _moveSpeed;
                    target = _origin + Vector2.right * (Mathf.PingPong(_phase, _travel) - _travel * 0.5f);
                    break;
                case SawMotion.Vertical:
                    _phase += Time.fixedDeltaTime * _moveSpeed;
                    target = _origin + Vector2.up * (Mathf.PingPong(_phase, _travel) - _travel * 0.5f);
                    break;
                case SawMotion.Waypoints:
                    target = NextWaypointStep();
                    break;
            }

            _rb.MovePosition(target);
        }

        private Vector2 NextWaypointStep()
        {
            if (_waypoints == null || _waypoints.Length < 2) return _rb.position;

            Vector2 goal = _waypoints[_wp].position;
            Vector2 step = Vector2.MoveTowards(_rb.position, goal, _moveSpeed * Time.fixedDeltaTime);
            if (((Vector2)goal - step).sqrMagnitude < 0.0004f)
            {
                _wp += _wpDir;
                if (_wp >= _waypoints.Length) { _wp = _waypoints.Length - 2; _wpDir = -1; }
                else if (_wp < 0) { _wp = 1; _wpDir = 1; }
            }
            return step;
        }

        protected override void OnActivate() { /* saw is passive; contact handled by IsHot */ }

        protected override void OnReset()
        {
            _phase = 0f;
            _wp = 0;
            _wpDir = 1;
            if (_rb != null) _rb.position = _origin;
            else transform.position = _origin;
        }
    }
}
