using UnityEngine;

namespace StickmanTrap.Inputs
{
    /// <summary>
    /// Input fed by on-screen UI buttons (wired in a later phase). It exists now so
    /// <see cref="StickmanTrap.Player.PlayerController"/> never has to change when
    /// mobile / touch support is switched on.
    /// </summary>
    public sealed class TouchInputProvider : IInputProvider
    {
        private int _moveDir;
        private bool _jumpHeld;
        private bool _jumpPressedLatch;

        public float MoveX { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool JumpHeld { get; private set; }
        public bool RestartPressed { get; private set; }
        public bool PausePressed { get; private set; }

        // Called by UI button EventTriggers (press/release).
        public void SetMove(int dir) => _moveDir = Mathf.Clamp(dir, -1, 1);
        public void PressJump() { _jumpPressedLatch = true; _jumpHeld = true; }
        public void ReleaseJump() => _jumpHeld = false;

        public void Tick()
        {
            MoveX = _moveDir;
            JumpPressed = _jumpPressedLatch;
            JumpHeld = _jumpHeld;
            _jumpPressedLatch = false;
            RestartPressed = false;
            PausePressed = false;
        }
    }
}
