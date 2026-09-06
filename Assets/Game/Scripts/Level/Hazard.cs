using UnityEngine;
using StickmanTrap.Player;

namespace StickmanTrap.Level
{
    /// <summary>
    /// Simple "touch = die" volume or body for static hazards (pit, static spikes).
    /// Phase 2 traps get their own <c>TrapBase</c> hierarchy; this stays for the
    /// dumb-but-common case. Works as either a trigger or a solid collider.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class Hazard : MonoBehaviour
    {
        [SerializeField] private string _causeId = "hazard";
        [SerializeField] private string _playerTag = "Player";

        private void OnCollisionEnter2D(Collision2D c) => TryKill(c.collider);
        private void OnTriggerEnter2D(Collider2D c) => TryKill(c);

        private void TryKill(Collider2D col)
        {
            if (!col.CompareTag(_playerTag)) return;
            var player = col.GetComponentInParent<PlayerController>();
            if (player != null && player.IsAlive) player.Kill(_causeId);
        }
    }
}
