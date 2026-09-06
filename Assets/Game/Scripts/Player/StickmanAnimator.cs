using UnityEngine;
using StickmanTrap.Core;

namespace StickmanTrap.Player
{
    /// <summary>
    /// Procedural animation for the parts-based Stickman: idle breathing, a run cycle
    /// (legs + counter-swinging arms + body bob + forward lean) whose speed tracks the
    /// motor, tuck/spread poses in the air, squash on landing, stretch on take-off, and
    /// a knocked-over death flop. No Animator asset, no sprite sheets — fully driven by
    /// <see cref="PlayerMotor"/> velocity, which keeps it responsive and WebGL-cheap.
    /// </summary>
    [DefaultExecutionOrder(50)]
    public sealed class StickmanAnimator : MonoBehaviour
    {
        [SerializeField] private PlayerMotor _motor;
        [SerializeField] private Transform _visualRoot;
        [SerializeField] private Transform _torso;
        [SerializeField] private Transform _head;
        [SerializeField] private Transform _armL;
        [SerializeField] private Transform _armR;
        [SerializeField] private Transform _legL;
        [SerializeField] private Transform _legR;

        [Header("Run cycle")]
        [SerializeField] private float _runStrideBase = 6f;      // rad/s at min speed
        [SerializeField] private float _runStridePerSpeed = 0.9f; // extra rad/s per unit of |vx|
        [SerializeField] private float _legSwing = 48f;          // deg
        [SerializeField] private float _armSwing = 40f;          // deg
        [SerializeField] private float _bodyBob = 0.06f;         // local units
        [SerializeField] private float _runLean = 9f;            // deg forward
        [SerializeField] private float _moveThreshold = 0.6f;    // |vx| to count as running

        [Header("Air poses")]
        [SerializeField] private float _riseTuck = 34f;
        [SerializeField] private float _fallSpread = 24f;
        [SerializeField] private float _armAir = 46f;

        [Header("Idle")]
        [SerializeField] private float _breathAmp = 0.025f;
        [SerializeField] private float _breathSpeed = 2.2f;

        [Header("Juice")]
        [SerializeField] private float _landSquash = 0.20f;
        [SerializeField] private float _jumpStretch = 0.14f;
        [SerializeField] private float _squashRecover = 9f;
        [SerializeField] private float _poseLerp = 14f;
        [Tooltip("How far to drop the visual while squashing so the feet stay on the ground.")]
        [SerializeField] private float _footPlantCompensation = 0.5f;

        [Header("Death flop")]
        [SerializeField] private float _deathAngle = 84f;
        [SerializeField] private float _deathLerp = 10f;

        // captured rest pose
        private Vector3 _rootPos;
        private Quaternion _rootRot, _torsoRot, _headRot, _armLRot, _armRRot, _legLRot, _legRRot;
        private Vector3 _rootScale;

        private float _phase;
        private float _runWeight;
        private float _squash;       // >0 squash, <0 stretch
        private bool _wasGrounded = true;
        private bool _dead;
        private float _deathDir = 1f;

        private void Awake()
        {
            if (_motor == null) _motor = GetComponent<PlayerMotor>();
            if (_visualRoot == null || _legL == null || _legR == null)
            {
                Debug.LogError($"{name}: StickmanAnimator missing references.", this);
                enabled = false;
                return;
            }

            _rootPos = _visualRoot.localPosition;
            _rootRot = _visualRoot.localRotation;
            _rootScale = _visualRoot.localScale;
            _torsoRot = _torso.localRotation;
            _headRot = _head.localRotation;
            _armLRot = _armL.localRotation;
            _armRRot = _armR.localRotation;
            _legLRot = _legL.localRotation;
            _legRRot = _legR.localRotation;
        }

        private void OnEnable()
        {
            GameEvents.PlayerDied += OnDied;
            GameEvents.PlayerRespawned += OnRespawned;
            GameEvents.LevelRestarting += OnRespawned;
        }

        private void OnDisable()
        {
            GameEvents.PlayerDied -= OnDied;
            GameEvents.PlayerRespawned -= OnRespawned;
            GameEvents.LevelRestarting -= OnRespawned;
        }

        private void OnDied(DeathInfo info)
        {
            _dead = true;
            _deathDir = Random.value < 0.5f ? -1f : 1f;
        }

        private void OnRespawned()
        {
            _dead = false;
            _phase = 0f;
            _runWeight = 0f;
            _squash = 0f;
            ApplyRestPose();
        }

        private void LateUpdate()
        {
            float dt = Time.deltaTime;
            if (dt <= 0f) return;

            if (_dead)
            {
                AnimateDeath(dt);
                return;
            }

            Vector2 v = _motor.Velocity;
            bool grounded = _motor.IsGrounded;
            float speed = Mathf.Abs(v.x);

            HandleLandTakeoff(grounded, v.y);
            _wasGrounded = grounded;

            if (grounded)
                AnimateGround(speed, v.x, dt);
            else
                AnimateAir(v.y, dt);

            // squash/stretch recovery. positive _squash = shorter+wider, negative = taller+thinner
            _squash = Mathf.MoveTowards(_squash, 0f, _squashRecover * dt);
            _visualRoot.localScale = new Vector3(
                _rootScale.x * (1f - _squash * 0.6f),
                _rootScale.y * (1f + _squash),
                _rootScale.z);

            // keep the feet planted while squashing (root pivot sits above the feet)
            if (_motor.IsGrounded && !_dead)
            {
                var p = _visualRoot.localPosition;
                p.y -= _squash * _footPlantCompensation;
                _visualRoot.localPosition = p;
            }
        }

        private void AnimateGround(float speed, float vx, float dt)
        {
            bool running = speed > _moveThreshold;
            float targetWeight = running ? Mathf.Clamp01(speed / 4f) : 0f;
            _runWeight = Mathf.MoveTowards(_runWeight, targetWeight, 3f * dt);

            if (running)
                _phase += (_runStrideBase + speed * _runStridePerSpeed) * dt;
            else
                _phase += _breathSpeed * dt;

            float w = _runWeight;
            float legA = Mathf.Sin(_phase) * _legSwing * w;
            float armA = Mathf.Sin(_phase) * _armSwing * w;

            // legs & arms follow the sine directly (snappy); arms counter the legs
            SetLimbDirect(_legL, _legLRot, legA);
            SetLimbDirect(_legR, _legRRot, -legA);
            SetLimbDirect(_armL, _armLRot, -armA);
            SetLimbDirect(_armR, _armRRot, armA);

            // idle breathing on the torso when basically still
            float breathe = (1f - w) * Mathf.Sin(Time.time * _breathSpeed) * _breathAmp;

            float bob = Mathf.Sin(_phase * 2f) * _bodyBob * w;
            Vector3 targetPos = _rootPos + new Vector3(0f, bob + breathe, 0f);
            _visualRoot.localPosition = Vector3.Lerp(_visualRoot.localPosition, targetPos, _poseLerp * dt);

            float lean = -_runLean * w; // local -z leans toward facing direction (mirrors with the flip)
            Quaternion targetRot = _rootRot * Quaternion.Euler(0f, 0f, lean);
            _visualRoot.localRotation = Quaternion.Slerp(_visualRoot.localRotation, targetRot, _poseLerp * dt);

            LerpRot(_torso, _torsoRot * Quaternion.Euler(0f, 0f, breathe * 60f), dt);
            LerpRot(_head, _headRot, dt);
        }

        private void AnimateAir(float vy, float dt)
        {
            _runWeight = Mathf.MoveTowards(_runWeight, 0f, 4f * dt);

            // rising -> tuck knees forward; falling -> spread down
            float t = Mathf.Clamp(vy / 8f, -1f, 1f); // +1 rising, -1 falling
            float legFront = t > 0f ? _riseTuck * t : 0f;
            float legSpread = t < 0f ? _fallSpread * -t : 0f;

            SetLimb(_legL, _legLRot, legFront + legSpread);
            SetLimb(_legR, _legRRot, legFront - legSpread);

            float arm = _armAir * Mathf.Lerp(0.5f, 1f, Mathf.InverseLerp(-1f, 1f, t));
            SetLimb(_armL, _armLRot, arm);
            SetLimb(_armR, _armRRot, -arm);

            _visualRoot.localPosition = Vector3.Lerp(_visualRoot.localPosition, _rootPos, _poseLerp * dt);
            float airLean = -6f * t;
            LerpRootRot(_rootRot * Quaternion.Euler(0f, 0f, airLean), dt);
        }

        private void AnimateDeath(float dt)
        {
            Quaternion target = _rootRot * Quaternion.Euler(0f, 0f, _deathAngle * _deathDir);
            _visualRoot.localRotation = Quaternion.Slerp(_visualRoot.localRotation, target, _deathLerp * dt);
            _visualRoot.localPosition = Vector3.Lerp(_visualRoot.localPosition,
                _rootPos + new Vector3(0f, -0.15f, 0f), _deathLerp * dt);

            // limbs go limp toward a splayed pose
            SetLimb(_legL, _legLRot, 30f);
            SetLimb(_legR, _legRRot, -50f);
            SetLimb(_armL, _armLRot, 70f);
            SetLimb(_armR, _armRRot, -40f);
        }

        private void HandleLandTakeoff(bool grounded, float vy)
        {
            if (grounded && !_wasGrounded && vy < -3f)
                _squash = _landSquash * Mathf.Clamp01(-vy / 16f + 0.4f);
            else if (!grounded && _wasGrounded && vy > 1f)
                _squash = -_jumpStretch;
        }

        private void SetLimb(Transform limb, Quaternion rest, float deltaDeg)
            => limb.localRotation = Quaternion.Slerp(
                limb.localRotation, rest * Quaternion.Euler(0f, 0f, deltaDeg), _poseLerp * Time.deltaTime);

        private static void SetLimbDirect(Transform limb, Quaternion rest, float deltaDeg)
            => limb.localRotation = rest * Quaternion.Euler(0f, 0f, deltaDeg);

        private void LerpRot(Transform t, Quaternion target, float dt)
            => t.localRotation = Quaternion.Slerp(t.localRotation, target, _poseLerp * dt);

        private void LerpRootRot(Quaternion target, float dt)
            => _visualRoot.localRotation = Quaternion.Slerp(_visualRoot.localRotation, target, _poseLerp * dt);

        private void ApplyRestPose()
        {
            _visualRoot.localPosition = _rootPos;
            _visualRoot.localRotation = _rootRot;
            _visualRoot.localScale = _rootScale;
            _torso.localRotation = _torsoRot;
            _head.localRotation = _headRot;
            _armL.localRotation = _armLRot;
            _armR.localRotation = _armRRot;
            _legL.localRotation = _legLRot;
            _legR.localRotation = _legRRot;
        }
    }
}
