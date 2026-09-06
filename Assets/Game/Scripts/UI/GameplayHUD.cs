using UnityEngine;
using UnityEngine.UI;
using StickmanTrap.Core;
using StickmanTrap.Level;

namespace StickmanTrap.UI
{
    /// <summary>
    /// Minimal MVP HUD: live timer + death counter, a "YOU DIED" flash and a
    /// "LEVEL COMPLETE" panel. Real screens (LevelComplete with stars, etc.) come
    /// in Phase 4 — this is deliberately tiny and event-driven.
    /// </summary>
    public sealed class GameplayHUD : MonoBehaviour
    {
        [SerializeField] private LevelController _level;

        [Header("Live stats")]
        [SerializeField] private Text _timerText;
        [SerializeField] private Text _deathText;

        [Header("Panels")]
        [SerializeField] private GameObject _deathPanel;
        [SerializeField] private Text _deathPanelText;
        [SerializeField] private GameObject _completePanel;
        [SerializeField] private Text _completePanelText;

        private void Awake()
        {
            if (_level == null) _level = FindAnyObjectByType<LevelController>();
            if (_deathPanel != null) _deathPanel.SetActive(false);
            if (_completePanel != null) _completePanel.SetActive(false);
        }

        private void OnEnable()
        {
            GameEvents.PlayerDied += OnDied;
            GameEvents.PlayerRespawned += OnRespawned;
            GameEvents.LevelCompleted += OnCompleted;
        }

        private void OnDisable()
        {
            GameEvents.PlayerDied -= OnDied;
            GameEvents.PlayerRespawned -= OnRespawned;
            GameEvents.LevelCompleted -= OnCompleted;
        }

        private void Update()
        {
            if (_level == null) return;
            if (_timerText != null) _timerText.text = _level.ElapsedTime.ToString("00.00") + "s";
            if (_deathText != null) _deathText.text = "Deaths: " + _level.Deaths;
        }

        private void OnDied(DeathInfo info)
        {
            if (_deathPanel != null) _deathPanel.SetActive(true);
            if (_deathPanelText != null)
                _deathPanelText.text = $"YOU DIED\nDeaths: {_level.Deaths}\npress  R  to retry";
        }

        private void OnRespawned()
        {
            if (_deathPanel != null) _deathPanel.SetActive(false);
        }

        private void OnCompleted(LevelResult r)
        {
            if (_deathPanel != null) _deathPanel.SetActive(false);
            if (_completePanel != null) _completePanel.SetActive(true);
            if (_completePanelText != null)
                _completePanelText.text =
                    $"LEVEL COMPLETE!\nTime: {r.Time:00.00}s   Deaths: {r.Deaths}\npress  R  to replay";
        }

        // Wire to on-screen buttons.
        public void OnRetryButton() => _level.RestartLevel();
        public void OnReloadButton() => _level.ReloadScene();
    }
}
