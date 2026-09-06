using UnityEngine;
using StickmanTrap.Core;
using StickmanTrap.Player;

namespace StickmanTrap.Level
{
    /// <summary>
    /// The level exit. Raises <see cref="GameEvents.GoalReached"/> once when a live
    /// player enters. (A "fake goal" trap in a later phase will be a different component.)
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class Goal : MonoBehaviour
    {
        [SerializeField] private string _playerTag = "Player";
        private bool _reached;

        private void Reset()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        private void OnEnable() => GameEvents.LevelRestarting += ResetState;
        private void OnDisable() => GameEvents.LevelRestarting -= ResetState;

        private void ResetState() => _reached = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_reached || !other.CompareTag(_playerTag)) return;

            var player = other.GetComponentInParent<PlayerController>();
            if (player == null || !player.IsAlive) return;

            _reached = true;
            player.Motor.ControlEnabled = false;
            GameEvents.RaiseGoalReached();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.35f);
            var col = GetComponent<Collider2D>();
            if (col != null) Gizmos.DrawCube(col.bounds.center, col.bounds.size);
        }
    }
}
