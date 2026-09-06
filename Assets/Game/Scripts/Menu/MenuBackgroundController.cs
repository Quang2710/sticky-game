using System.Collections;
using UnityEngine;

namespace StickmanTrap.Menu
{
    /// <summary>
    /// Light animated backdrop: clouds drifting left (wrapping around) and a
    /// decorative saw that slides across every so often. Sprites only — no
    /// physics, no trap system. WebGL-cheap.
    /// </summary>
    public sealed class MenuBackgroundController : MonoBehaviour
    {
        [Header("Clouds")]
        [SerializeField] private Transform[] _clouds;
        [SerializeField] private float _cloudSpeed = 0.35f;
        [SerializeField] private float _leftEdge = -12f;
        [SerializeField] private float _rightEdge = 12f;

        [Header("Decorative saw")]
        [SerializeField] private Transform _decoSaw;
        [SerializeField] private float _sawSpeed = 6f;
        [SerializeField] private float _sawSpin = 540f;
        [SerializeField] private Vector2 _sawInterval = new Vector2(7f, 14f);
        [SerializeField] private float _sawY = -3.2f;

        private float _spanX;

        private void Awake()
        {
            _spanX = _rightEdge - _leftEdge;
            if (_decoSaw != null)
            {
                _decoSaw.position = new Vector3(_rightEdge + 3f, _sawY, _decoSaw.position.z);
                StartCoroutine(SawLoop());
            }
        }

        private void Update()
        {
            if (_clouds == null) return;

            for (int i = 0; i < _clouds.Length; i++)
            {
                var c = _clouds[i];
                if (c == null) continue;
                Vector3 p = c.position;
                p.x -= _cloudSpeed * (0.6f + 0.4f * (i % 3)) * Time.deltaTime;
                if (p.x < _leftEdge - 2f) p.x += _spanX + 4f;
                c.position = p;
            }
        }

        private IEnumerator SawLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_sawInterval.x, _sawInterval.y));

                float x = _rightEdge + 3f;
                while (x > _leftEdge - 3f)
                {
                    x -= _sawSpeed * Time.deltaTime;
                    _decoSaw.position = new Vector3(x, _sawY, _decoSaw.position.z);
                    _decoSaw.Rotate(0f, 0f, -_sawSpin * Time.deltaTime);
                    yield return null;
                }
            }
        }
    }
}
