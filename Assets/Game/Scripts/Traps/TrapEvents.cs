using System;
using UnityEngine;

namespace StickmanTrap.Traps
{
    /// <summary>
    /// String-keyed bus for trap-to-trap communication (chaining). Designers wire
    /// "emit" and "listen" ids in the Inspector, so no trap ever holds a direct
    /// reference to another. Kept separate from <see cref="StickmanTrap.Core.GameEvents"/>,
    /// which is for game-wide moments.
    /// </summary>
    public static class TrapEvents
    {
        /// <summary>Raised with a trap-event id when some trap fires.</summary>
        public static event Action<string> Fired;

        public static void Raise(string id)
        {
            if (!string.IsNullOrEmpty(id))
                Fired?.Invoke(id);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => Fired = null;
    }
}
