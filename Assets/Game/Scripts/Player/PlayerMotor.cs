using UnityEngine;

namespace StickmanTrap.Player
{
    /// <summary>
    /// Pure movement physics: acceleration-based horizontal motion, variable-height
    /// jump, coyote time, jump buffering, fall gravity and terminal velocity.
    /// Reads nothing about input sources — <see cref="PlayerController"/> feeds it.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        [SerializeField] private PlayerConfig _config;
        [SerializeField] private GroundSensor _ground;

        public bool FacingRight { get; private set; } = true;
        public bool IsGrounded => _ground != null && _ground.IsGrounded;
        public Vector2 Velocity => _rb.linearVelocity;

        /// <summary>When false the motor ignores input and eases to a stop.</summary>
        public bool ControlEnabled { get; set; } = true;

        private Rigidbody2D _rb;
        private float _moveInput;
        private bool _jumpHeld;
        private bool _jumpQueued;
        private float _coyoteCounter;
        private float _jumpBufferCounter;
        private bool _jumpCutQueued;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();

            if (_config == null)
            {
                Debug.LogError($"{name}: PlayerMotor has no PlayerConfig assigned.", this);
                enabled = false;
                return;
            }

            if (_ground == null) _ground = GetComponentInChildren<GroundSensor>();
            if (_ground != null)
                _ground.SetShape(_config.GroundCheckOffset, _config.GroundCheckSize);

            _rb.gravityScale = _config.BaseGravityScale;
            _rb.freezeRotation = true;
        }

        /// <summary>Feed resolved input for this frame (called from Update).</summary>
        public void SetInput(float moveX, bool jumpPressed, bool jumpHeld)
        {
            _moveInput = moveX;

            // Releasing jump while still rising cuts the ascent once (variable height).
            if (_jumpHeld && !jumpHeld) _jumpCutQueued = true;
            _jumpHeld = jumpHeld;

            if (jumpPressed) _jumpBufferCounter = _config.JumpBufferTime;
        }

        /// <summary>Zero out all motion &amp; buffers (used on respawn / death).</summary>
        public void ResetState()
        {
            _rb.linearVelocity = Vector2.zero;
            _moveInput = 0f;
            _jumpHeld = false;
            _jumpQueued = false;
            _jumpCutQueued = false;
            _coyoteCounter = 0f;
            _jumpBufferCounter = 0f;
            _rb.gravityScale = _config.BaseGravityScale;
        }

        private void Update()
        {
            if (!ControlEnabled) return;

            _coyoteCounter = IsGrounded ? _config.CoyoteTime : _coyoteCounter - Time.deltaTime;
            if (_jumpBufferCounter > 0f) _jumpBufferCounter -= Time.deltaTime;

            if (_jumpBufferCounter > 0f && _coyoteCounter > 0f)
            {
                _jumpQueued = true;
                _jumpBufferCounter = 0f;
                _coyoteCounter = 0f;
            }

            UpdateFacing();
        }

        private void FixedUpdate()
        {
            if (!ControlEnabled)
            {
                float eased = Mathf.MoveTowards(_rb.linearVelocity.x, 0f,
                    _config.Deceleration * Time.fixedDeltaTime);
                _rb.linearVelocity = new Vector2(eased, _rb.linearVelocity.y);
                return;
            }

            ApplyHorizontal();
            ApplyJump();
            ApplyJumpCut();
            ApplyGravityModifiers();
            ClampFallSpeed();
        }

        private void ApplyHorizontal()
        {
            float target = _moveInput * _config.MoveSpeed;
            float diff = target - _rb.linearVelocity.x;

            bool accelerating = Mathf.Abs(target) > 0.01f
                                && Mathf.Sign(target) == Mathf.Sign(diff);
            float rate = accelerating ? _config.Acceleration : _config.Deceleration;
            if (!IsGrounded) rate *= _config.AirControlMultiplier;

            float step = Mathf.Clamp(diff, -rate * Time.fixedDeltaTime, rate * Time.fixedDeltaTime);
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x + step, _rb.linearVelocity.y);
        }

        private void ApplyJump()
        {
            if (!_jumpQueued) return;
            _jumpQueued = false;
            _jumpCutQueued = false;
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _config.JumpForce);
        }

        private void ApplyJumpCut()
        {
            if (!_jumpCutQueued) return;
            _jumpCutQueued = false;
            if (_rb.linearVelocity.y > 0f)
                _rb.linearVelocity = new Vector2(
                    _rb.linearVelocity.x,
                    _rb.linearVelocity.y * _config.JumpCutMultiplier);
        }

        private void ApplyGravityModifiers()
        {
            float g = _config.BaseGravityScale;
            if (_rb.linearVelocity.y < -0.01f)
                g *= _config.FallMultiplier;
            _rb.gravityScale = g;
        }

        private void ClampFallSpeed()
        {
            if (_rb.linearVelocity.y < -_config.MaxFallSpeed)
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, -_config.MaxFallSpeed);
        }

        private void UpdateFacing()
        {
            if (_moveInput > 0.05f && !FacingRight) Flip();
            else if (_moveInput < -0.05f && FacingRight) Flip();
        }

        private void Flip()
        {
            FacingRight = !FacingRight;
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (FacingRight ? 1f : -1f);
            transform.localScale = s;
        }
    }
}
