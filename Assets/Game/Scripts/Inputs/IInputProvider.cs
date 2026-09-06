namespace StickmanTrap.Inputs
{
    /// <summary>
    /// Abstraction over "where input comes from". The player and other systems
    /// only ever read this interface — they never touch Keyboard, touch UI, or
    /// the Input System directly. Swap the concrete provider to change controls.
    /// </summary>
    public interface IInputProvider
    {
        /// <summary>-1 = left, 0 = none, +1 = right.</summary>
        float MoveX { get; }

        /// <summary>True only on the frame jump was pressed.</summary>
        bool JumpPressed { get; }

        /// <summary>True while jump is held (used for variable jump height).</summary>
        bool JumpHeld { get; }

        /// <summary>True only on the frame restart was requested.</summary>
        bool RestartPressed { get; }

        /// <summary>True only on the frame pause was toggled.</summary>
        bool PausePressed { get; }

        /// <summary>Refreshes the cached state. Called once per frame by <see cref="InputService"/>.</summary>
        void Tick();
    }
}
