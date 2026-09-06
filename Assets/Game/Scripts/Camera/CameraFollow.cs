using UnityEngine;

namespace StickmanTrap.CameraRig
{
    /// <summary>
    /// Smooth 2D follow with a dead zone and orthographic level bounds clamping.
    /// Put this on the "CameraRig" parent; the actual Camera is a child so
    /// <see cref="CameraShake"/> can perturb it independently.
    /// </summary>
    public sealed class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector2 _offset = new Vector2(0f, 1f);

        [Header("Smoothing")]
        [SerializeField, Min(0f)] private float _smoothTime = 0.16f;

        [Header("Dead Zone (world units, half-extents)")]
        [SerializeField] private Vector2 _deadZone = new Vector2(1.4f, 0.9f);

        [Header("Bounds")]
        [SerializeField] private bool _useBounds = true;
        [SerializeField] private Bounds _worldBounds = new Bounds(Vector3.zero, new Vector3(40f, 20f, 0f));

        private Camera _cam;
        private Vector3 _vel;

        public void SetTarget(Transform t) => _target = t;

        public void SetBounds(Bounds b)
        {
            _worldBounds = b;
            _useBounds = true;
        }

        private void Awake()
        {
            _cam = GetComponentInChildren<Camera>();
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 desired = transform.position;
            Vector2 focus = (Vector2)_target.position + _offset;
            Vector2 delta = focus - (Vector2)transform.position;

            if (Mathf.Abs(delta.x) > _deadZone.x)
                desired.x = focus.x - Mathf.Sign(delta.x) * _deadZone.x;
            if (Mathf.Abs(delta.y) > _deadZone.y)
                desired.y = focus.y - Mathf.Sign(delta.y) * _deadZone.y;

            Vector3 next = Vector3.SmoothDamp(transform.position, desired, ref _vel, _smoothTime);
            next.z = transform.position.z;

            if (_useBounds && _cam != null && _cam.orthographic)
            {
                float vExt = _cam.orthographicSize;
                float hExt = vExt * _cam.aspect;
                if (_worldBounds.size.x > hExt * 2f)
                    next.x = Mathf.Clamp(next.x, _worldBounds.min.x + hExt, _worldBounds.max.x - hExt);
                else
                    next.x = _worldBounds.center.x;
                if (_worldBounds.size.y > vExt * 2f)
                    next.y = Mathf.Clamp(next.y, _worldBounds.min.y + vExt, _worldBounds.max.y - vExt);
                else
                    next.y = _worldBounds.center.y;
            }

            transform.position = next;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(_worldBounds.center, _worldBounds.size);

            if (_target != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube((Vector2)_target.position + _offset, _deadZone * 2f);
            }
        }
    }
}
