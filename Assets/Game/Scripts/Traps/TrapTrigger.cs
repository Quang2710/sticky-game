namespace StickmanTrap.Traps
{
    /// <summary>How a trap decides to fire.</summary>
    public enum TrapTrigger
    {
        /// <summary>Player touches one of the trap's colliders.</summary>
        Contact,
        /// <summary>Player enters the trap's trigger zone.</summary>
        Zone,
        /// <summary>Fires on a repeating timer.</summary>
        Timer,
        /// <summary>Fires when a matching <see cref="TrapEvents"/> id is raised (chaining).</summary>
        Event,
        /// <summary>Only fires via <see cref="ITrap.Activate"/> from code / another system.</summary>
        Manual
    }
}
