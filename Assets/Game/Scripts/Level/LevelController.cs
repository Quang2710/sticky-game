using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using StickmanTrap.Core;
using StickmanTrap.Inputs;
using StickmanTrap.Player;

namespace StickmanTrap.Level
{
    /// <summary>
    /// Per-level orchestrator: owns the <see cref="GamePhase"/>, run timer and death
    /// count, and drives the fast death → respawn loop (in-place, no scene reload —
    /// Phase 2 traps will listen to <see cref="GameEvents.LevelRestarting"/> to reset).
    /// </summary>
    public sealed class LevelController : MonoBehaviour
    {
        [Header("Refs (auto-found if left empty)")]
        [SerializeField] private PlayerController _player;
        [SerializeField] private PlayerSpawn _spawn;
        [SerializeField] private InputService _input;

        [Header("Death / Respawn")]
        [Tooltip("Beat before anything happens, so the death registers.")]
        [SerializeField, Min(0f)] private float _deathFreeze = 0.12f;
        [Tooltip("If auto-respawn is on, wait this long (or until Restart pressed).")]
        [SerializeField, Min(0f)] private float _autoRespawnDelay = 0.55f;
        [SerializeField] private bool _autoRespawn = true;

        public GamePhase Phase { get; private set; } = GamePhase.Loading;
        public int Deaths { get; private set; }
        public float ElapsedTime { get; private set; }

        private Coroutine _deathRoutine;

        private void Awake()
        {
            if (_player == null) _player = FindAnyObjectByType<PlayerController>();
            if (_spawn == null) _spawn = FindAnyObjectByType<PlayerSpawn>();
            if (_input == null) _input = FindAnyObjectByType<InputService>();
        }

        private void OnEnable()
        {
            GameEvents.PlayerDied += OnPlayerDied;
            GameEvents.GoalReached += OnGoalReached;
        }

        private void OnDisable()
        {
            GameEvents.PlayerDied -= OnPlayerDied;
            GameEvents.GoalReached -= OnGoalReached;
        }

        private IEnumerator Start()
        {
            if (_spawn != null && _player != null)
                _player.TeleportTo(_spawn.Position);

            Phase = GamePhase.Ready;
            GameEvents.RaiseLevelLoaded();

            yield return null; // let everything subscribe / settle one frame

            Phase = GamePhase.Playing;
            GameEvents.RaiseLevelStarted();
        }

        private void Update()
        {
            if (Phase == GamePhase.Playing)
                ElapsedTime += Time.deltaTime;

            // Manual restart is always available (except once the level is won).
            if (Phase != GamePhase.Completed && RestartPressed())
                Respawn();
        }

        private bool RestartPressed()
            => _input != null && _input.Provider != null && _input.Provider.RestartPressed;

        private void OnPlayerDied(DeathInfo info)
        {
            if (Phase != GamePhase.Playing) return;

            Phase = GamePhase.Dead;
            Deaths++;

            if (_deathRoutine != null) StopCoroutine(_deathRoutine);
            _deathRoutine = StartCoroutine(DeathSequence());
        }

        private IEnumerator DeathSequence()
        {
            yield return WaitUnscaled(_deathFreeze);

            if (!_autoRespawn) yield break;

            float wait = 0f;
            while (wait < _autoRespawnDelay)
            {
                if (RestartPressed()) break;
                wait += Time.unscaledDeltaTime;
                yield return null;
            }

            if (Phase == GamePhase.Dead)
                Respawn();
        }

        /// <summary>Fast in-place restart. Also the target for the Retry button.</summary>
        public void Respawn()
        {
            if (Phase == GamePhase.Completed) return;

            if (_deathRoutine != null)
            {
                StopCoroutine(_deathRoutine);
                _deathRoutine = null;
            }

            GameEvents.RaiseLevelRestarting();

            if (_spawn != null && _player != null)
                _player.TeleportTo(_spawn.Position);

            GameEvents.RaisePlayerRespawned();
            Phase = GamePhase.Playing;
        }

        public void RestartLevel() => Respawn();

        /// <summary>Full scene reload — kept as a fallback / "reset everything" path.</summary>
        public void ReloadScene()
            => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        private void OnGoalReached()
        {
            if (Phase == GamePhase.Completed) return;

            Phase = GamePhase.Completed;
            var result = new LevelResult(ElapsedTime, Deaths, 0);
            GameEvents.RaiseLevelCompleted(result);
        }

        private static IEnumerator WaitUnscaled(float seconds)
        {
            float t = 0f;
            while (t < seconds)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }
        }
    }
}
