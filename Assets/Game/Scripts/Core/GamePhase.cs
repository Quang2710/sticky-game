namespace StickmanTrap.Core
{
    /// <summary>High-level state of the active level.</summary>
    public enum GamePhase
    {
        Loading,
        Ready,
        Playing,
        Dead,
        Completed,
        Paused
    }
}
