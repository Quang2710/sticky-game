using UnityEngine;

namespace StickmanTrap.Player
{
    /// <summary>
    /// All player-feel tuning lives here so it can be balanced without touching code
    /// and shared/duplicated per skin or per chapter later. No magic numbers in the motor.
    /// </summary>
    [CreateAssetMenu(menuName = "Stickman Trap/Player Config", fileName = "PlayerConfig")]
    public sealed class PlayerConfig : ScriptableObject
    {
        [Header("Horizontal Movement")]
        [Min(0f)] public float MoveSpeed = 8f;
        [Min(0f)] public float Acceleration = 90f;
        [Min(0f)] public float Deceleration = 110f;
        [Tooltip("Multiplier applied to accel/decel while airborne (0-1 = less air control).")]
        [Range(0f, 1f)] public float AirControlMultiplier = 0.65f;

        [Header("Jump")]
        [Min(0f)] public float JumpForce = 15f;
        [Min(0f)] public float MaxFallSpeed = 22f;
        [Tooltip("Base gravity scale on the Rigidbody2D.")]
        [Min(0f)] public float BaseGravityScale = 3.5f;
        [Tooltip("Extra gravity while falling (snappier arc).")]
        [Min(1f)] public float FallMultiplier = 2.2f;
        [Tooltip("Ascent velocity is multiplied by this the moment jump is released early (variable height). 1 = fixed height.")]
        [Range(0f, 1f)] public float JumpCutMultiplier = 0.45f;

        [Header("Assist")]
        [Tooltip("Grace period to still jump just after walking off a ledge.")]
        [Min(0f)] public float CoyoteTime = 0.10f;
        [Tooltip("Window to remember a jump press made just before landing.")]
        [Min(0f)] public float JumpBufferTime = 0.12f;

        [Header("Ground Check")]
        public Vector2 GroundCheckOffset = new Vector2(0f, -0.55f);
        public Vector2 GroundCheckSize = new Vector2(0.42f, 0.12f);
    }
}
