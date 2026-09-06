using UnityEngine;
using StickmanTrap.CameraRig;
using StickmanTrap.Player;

namespace StickmanTrap.Level
{
    /// <summary>
    /// Defines the playable rectangle. Feeds the camera its clamp bounds and kills
    /// the player if they fall out of the world (below the box by a margin).
    /// </summary>
    public sealed class LevelBounds : MonoBehaviour
    {
        [SerializeField] private Bounds _bounds = new Bounds(Vector3.zero, new Vector3(40f, 20f, 0f));
        [SerializeField, Min(0f)] private float _killMargin = 3f;

        public Bounds Bounds => _bounds;

        private PlayerController _player;

        private void Start()
        {
            _player = FindAnyObjectByType<PlayerController>();

            var follow = FindAnyObjectByType<CameraFollow>();
            if (follow != null) follow.SetBounds(_bounds);
        }

        private void Update()
        {
            if (_player == null || !_player.IsAlive) return;

            if (_player.transform.position.y < _bounds.min.y - _killMargin)
                _player.Kill("void");
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0.55f, 0.1f, 1f);
            Gizmos.DrawWireCube(_bounds.center, _bounds.size);
        }
    }
}
