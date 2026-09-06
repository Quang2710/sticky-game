using System;
using UnityEngine;

namespace StickmanTrap.Core
{
    /// <summary>Context passed when the player dies. Keep it lightweight (struct).</summary>
    public readonly struct DeathInfo
    {
        public readonly string CauseId;
        public readonly Vector2 Position;

        public DeathInfo(string causeId, Vector2 position)
        {
            CauseId = causeId;
            Position = position;
        }
    }

    /// <summary>Summary of a completed level run.</summary>
    public readonly struct LevelResult
    {
        public readonly float Time;
        public readonly int Deaths;
        public readonly int CoinsCollected;
        public readonly bool NoDeath;

        public LevelResult(float time, int deaths, int coinsCollected)
        {
            Time = time;
            Deaths = deaths;
            CoinsCollected = coinsCollected;
            NoDeath = deaths == 0;
        }
    }

    /// <summary>
    /// Central gameplay event hub. Systems communicate through here instead of
    /// holding direct references to each other. Keep this list small and intentional —
    /// it is the public "API surface" between subsystems.
    /// </summary>
    public static class GameEvents
    {
        public static event Action LevelLoaded;
        public static event Action LevelStarted;
        public static event Action GoalReached;
        public static event Action<DeathInfo> PlayerDied;
        public static event Action PlayerRespawned;
        public static event Action LevelRestarting;
        public static event Action<LevelResult> LevelCompleted;

        public static void RaiseLevelLoaded() => LevelLoaded?.Invoke();
        public static void RaiseLevelStarted() => LevelStarted?.Invoke();
        public static void RaiseGoalReached() => GoalReached?.Invoke();
        public static void RaisePlayerDied(DeathInfo info) => PlayerDied?.Invoke(info);
        public static void RaisePlayerRespawned() => PlayerRespawned?.Invoke();
        public static void RaiseLevelRestarting() => LevelRestarting?.Invoke();
        public static void RaiseLevelCompleted(LevelResult result) => LevelCompleted?.Invoke(result);

        /// <summary>
        /// Clears every subscriber. Runs automatically on each Play session so stale
        /// listeners never survive "Enter Play Mode (no domain reload)".
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            LevelLoaded = null;
            LevelStarted = null;
            GoalReached = null;
            PlayerDied = null;
            PlayerRespawned = null;
            LevelRestarting = null;
            LevelCompleted = null;
        }
    }
}
