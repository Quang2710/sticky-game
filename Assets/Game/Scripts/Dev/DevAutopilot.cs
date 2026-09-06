using UnityEngine;
using StickmanTrap.Core;
using StickmanTrap.Inputs;
using StickmanTrap.Player;

namespace StickmanTrap.Dev
{
    /// <summary>
    /// Scripted input for automated smoke-testing (editor / CI). Disabled by default.
    /// Runs the player right and jumps over gaps / obstacles detected by short raycasts.
    /// Not gameplay code — excluded from shipping builds via the editor guard.
    /// </summary>
    public sealed class DevAutopilot : MonoBehaviour
    {
        [SerializeField] private bool _enabled;
        [SerializeField] private InputService _input;
        [SerializeField] private PlayerController _player;
        [SerializeField] private LayerMask _solidMask = ~0;

        private float _jumpHoldTimer;

        public void Enable(bool on) => _enabled = on;

        private void Awake()
        {
            if (!Application.isEditor) { enabled = false; return; }
            if (_input == null) _input = FindAnyObjectByType<InputService>();
            if (_player == null) _player = FindAnyObjectByType<PlayerController>();

            GameEvents.LevelCompleted += OnComplete;
        }

        private void OnDestroy() => GameEvents.LevelCompleted -= OnComplete;

        private static void OnComplete(LevelResult r)
            => Debug.Log($"[DevAutopilot] LEVEL COMPLETE  time={r.Time:0.00}s  deaths={r.Deaths}");

        private void Update()
        {
            if (!_enabled || _input == null || _player == null) return;

            _input.UseTouch();
            var touch = _input.Touch;
            touch.SetMove(1);

            Vector2 pos = _player.transform.position;
            bool obstacleAhead = Physics2D.Raycast(pos + new Vector2(0.5f, -0.2f), Vector2.right, 0.7f, _solidMask);
            bool gapAhead = !Physics2D.Raycast(pos + new Vector2(1.3f, 0.3f), Vector2.down, 2.2f, _solidMask);

            if ((obstacleAhead || gapAhead) && _player.Motor.IsGrounded && _jumpHoldTimer <= 0f)
                _jumpHoldTimer = 0.16f;

            if (_jumpHoldTimer > 0f)
            {
                touch.PressJump();
                _jumpHoldTimer -= Time.deltaTime;
                if (_jumpHoldTimer <= 0f) touch.ReleaseJump();
            }
        }
    }
}
