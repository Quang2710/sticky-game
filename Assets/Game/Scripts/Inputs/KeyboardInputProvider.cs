using UnityEngine;
using UnityEngine.InputSystem;

namespace StickmanTrap.Inputs
{
    /// <summary>Desktop / WebGL keyboard input via the Input System (polling API).</summary>
    public sealed class KeyboardInputProvider : IInputProvider
    {
        public float MoveX { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool JumpHeld { get; private set; }
        public bool RestartPressed { get; private set; }
        public bool PausePressed { get; private set; }

        public void Tick()
        {
            Keyboard kb = Keyboard.current;
            if (kb == null)
            {
                MoveX = 0f;
                JumpPressed = JumpHeld = RestartPressed = PausePressed = false;
                return;
            }

            float x = 0f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) x -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x += 1f;
            MoveX = x;

            bool jumpKey = kb.spaceKey.wasPressedThisFrame
                           || kb.wKey.wasPressedThisFrame
                           || kb.upArrowKey.wasPressedThisFrame;
            JumpPressed = jumpKey;
            JumpHeld = kb.spaceKey.isPressed || kb.wKey.isPressed || kb.upArrowKey.isPressed;

            RestartPressed = kb.rKey.wasPressedThisFrame;
            PausePressed = kb.escapeKey.wasPressedThisFrame;
        }
    }
}
