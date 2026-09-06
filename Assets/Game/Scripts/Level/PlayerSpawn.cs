using UnityEngine;

namespace StickmanTrap.Level
{
    /// <summary>Marker for where the player starts / respawns in a level.</summary>
    public sealed class PlayerSpawn : MonoBehaviour
    {
        public Vector3 Position => transform.position;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.4f);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 1.2f);
        }
    }
}
