namespace StickmanTrap.Traps
{
    /// <summary>Common surface every trap exposes to the rest of the game.</summary>
    public interface ITrap
    {
        string TrapId { get; }
        bool IsArmed { get; }

        /// <summary>Fire the trap now (used by Manual trigger, buttons, other traps).</summary>
        void Activate();

        /// <summary>Return the trap to its initial state (called on level restart).</summary>
        void ResetTrap();
    }
}
