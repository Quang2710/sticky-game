using UnityEngine;

namespace StickmanTrap.Player
{
    /// <summary>
    /// Cheap, deterministic ground detection via a single OverlapBox in FixedUpdate.
    /// No trigger callbacks, no per-frame allocation. The ground <see cref="LayerMask"/>
    /// is configured here in the Inspector; the box shape can be driven by PlayerConfig.
    /// </summary>
    public sealed class GroundSensor : MonoBehaviour
    {
        [SerializeField] private LayerMask _groundMask = 1;
        [SerializeField] private Vector2 _offset = new Vector2(0f, -0.55f);
        [SerializeField] private Vector2 _size = new Vector2(0.42f, 0.12f);

        public bool IsGrounded { get; private set; }

        private readonly Collider2D[] _hits = new Collider2D[4];
        private ContactFilter2D _filter;

        private void Awake() => RebuildFilter();

        private void RebuildFilter()
        {
            _filter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = _groundMask,
                useTriggers = false
            };
        }

        /// <summary>Push the probe box shape from <see cref="PlayerConfig"/>.</summary>
        public void SetShape(Vector2 offset, Vector2 size)
        {
            _offset = offset;
            _size = size;
        }

        private void FixedUpdate()
        {
            Vector2 center = (Vector2)transform.position + _offset;
            int count = Physics2D.OverlapBox(center, _size, 0f, _filter, _hits);
            IsGrounded = count > 0;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = (Application.isPlaying && IsGrounded) ? Color.green : Color.yellow;
            Gizmos.DrawWireCube((Vector2)transform.position + _offset, _size);
        }
    }
}
