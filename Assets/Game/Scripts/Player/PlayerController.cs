using UnityEngine;
using StickmanTrap.Core;
using StickmanTrap.Inputs;

namespace StickmanTrap.Player
{
    /// <summary>
    /// Thin orchestrator: pulls resolved input from <see cref="InputService"/>,
    /// feeds <see cref="PlayerMotor"/>, and owns the alive/dead state.
    /// Traps and hazards call <see cref="Kill"/>; the level system respawns via events.
    /// </summary>
    [RequireComponent(typeof(PlayerMotor))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputService _input;
        [SerializeField] private PlayerMotor _motor;

        public bool IsAlive { get; private set; } = true;
        public PlayerMotor Motor => _motor;

        private void Reset() => _motor = GetComponent<PlayerMotor>();

        private void Awake()
        {
            if (_motor == null) _motor = GetComponent<PlayerMotor>();
            if (_input == null) _input = FindAnyObjectByType<InputService>();
        }

        private void OnEnable() => GameEvents.PlayerRespawned += HandleRespawned;
        private void OnDisable() => GameEvents.PlayerRespawned -= HandleRespawned;

        private void Update()
        {
            if (!IsAlive || _input == null || _input.Provider == null) return;

            IInputProvider p = _input.Provider;
            _motor.SetInput(p.MoveX, p.JumpPressed, p.JumpHeld);
        }

        /// <summary>Kill the player. Safe to call multiple times per death.</summary>
        public void Kill(string causeId)
        {
            if (!IsAlive) return;
            IsAlive = false;
            _motor.ControlEnabled = false;
            _motor.ResetState();
            GameEvents.RaisePlayerDied(new DeathInfo(causeId, transform.position));
        }

        /// <summary>Hard-place the player (spawn / checkpoint / teleport trap).</summary>
        public void TeleportTo(Vector3 position)
        {
            transform.position = position;
            _motor.ResetState();
        }

        private void HandleRespawned()
        {
            IsAlive = true;
            _motor.ControlEnabled = true;
            _motor.ResetState();
        }
    }
}
