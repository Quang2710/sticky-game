using UnityEngine;
using StickmanTrap.Core;

namespace StickmanTrap.CameraRig
{
    /// <summary>
    /// Perlin-noise positional shake on the Camera child. Listens for
    /// <see cref="GameEvents.PlayerDied"/> and can be pulsed by traps via <see cref="Shake"/>.
    /// Uses unscaled time so it still reads well during a death freeze / slow-mo.
    /// </summary>
    public sealed class CameraShake : MonoBehaviour
    {
        [SerializeField] private float _deathAmplitude = 0.32f;
        [SerializeField] private float _deathDuration = 0.22f;
        [SerializeField, Min(0f)] private float _frequency = 26f;
        [SerializeField] private float _maxAmplitude = 1.2f;

        private Vector3 _basePos;
        private float _seed;
        private float _timer;
        private float _duration;
        private float _amplitude;

        private void Awake()
        {
            _basePos = transform.localPosition;
            _seed = Random.value * 100f;
        }

        private void OnEnable() => GameEvents.PlayerDied += OnDeath;
        private void OnDisable() => GameEvents.PlayerDied -= OnDeath;

        private void OnDeath(DeathInfo _) => Shake(_deathAmplitude, _deathDuration);

        /// <summary>Request a shake. Stronger requests win; duration is replaced.</summary>
        public void Shake(float amplitude, float duration)
        {
            _amplitude = Mathf.Min(_maxAmplitude, Mathf.Max(_amplitude, amplitude));
            _duration = Mathf.Max(0.01f, duration);
            _timer = _duration;
        }

        private void LateUpdate()
        {
            if (_timer <= 0f)
            {
                transform.localPosition = _basePos;
                return;
            }

            _timer -= Time.unscaledDeltaTime;
            float falloff = Mathf.Clamp01(_timer / _duration);
            float amp = _amplitude * falloff * falloff;
            float t = Time.unscaledTime * _frequency;

            float ox = (Mathf.PerlinNoise(_seed, t) - 0.5f) * 2f;
            float oy = (Mathf.PerlinNoise(t, _seed + 10f) - 0.5f) * 2f;
            transform.localPosition = _basePos + new Vector3(ox, oy, 0f) * amp;

            if (_timer <= 0f)
            {
                transform.localPosition = _basePos;
                _amplitude = 0f;
            }
        }
    }
}
