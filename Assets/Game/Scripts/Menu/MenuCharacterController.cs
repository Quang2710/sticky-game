using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StickmanTrap.Menu
{
    /// <summary>
    /// The living Stickman on the menu. Pure decoration: idle breathing plus
    /// random looks / hops, an occasional "rock on the head" troll gag, and a
    /// speech bubble that nags the player if they sit idle. Never blocks input,
    /// never touches gameplay systems. Everything is toggleable via the config.
    /// </summary>
    public sealed class MenuCharacterController : MonoBehaviour
    {
        [SerializeField] private MenuCharacterConfig _config;
        [SerializeField] private Transform _visualRoot;
        [SerializeField] private Transform _head;
        [SerializeField] private Transform _armL;
        [SerializeField] private Transform _armR;
        [SerializeField] private Transform _legL;
        [SerializeField] private Transform _legR;

        [Header("Troll gag")]
        [SerializeField] private Transform _rock;

        [Header("Speech bubble")]
        [SerializeField] private GameObject _speechBubble;
        [SerializeField] private TMP_Text _speechText;

        private Vector3 _rootPos, _rootScale;
        private Quaternion _rootRot, _headRot, _armLRot, _armRRot, _legLRot, _legRRot;
        private Vector3 _rockHome;

        private float _actionTimer;
        private float _idleTimer;
        private bool _busy;
        private bool _bubbleShown;
        private Vector2 _lastMousePos;

        private void Awake()
        {
            if (_visualRoot == null) _visualRoot = transform;
            _rootPos = _visualRoot.localPosition;
            _rootRot = _visualRoot.localRotation;
            _rootScale = _visualRoot.localScale;
            if (_head) _headRot = _head.localRotation;
            if (_armL) _armLRot = _armL.localRotation;
            if (_armR) _armRRot = _armR.localRotation;
            if (_legL) _legLRot = _legL.localRotation;
            if (_legR) _legRRot = _legR.localRotation;
            if (_rock) { _rockHome = _rock.localPosition; _rock.gameObject.SetActive(false); }
            if (_speechBubble) _speechBubble.SetActive(false);

            if (_config == null)
            {
                Debug.LogWarning($"{name}: MenuCharacterController has no MenuCharacterConfig — it will just stand still.", this);
                enabled = false;
                return;
            }

            _actionTimer = NextActionDelay();
            if (Mouse.current != null) _lastMousePos = Mouse.current.position.ReadValue();
        }

        private void Update()
        {
            if (_config == null) return;

            Breathe();
            TrackIdle();

            if (_busy || !_config.EnableRandomActions) return;

            _actionTimer -= Time.deltaTime;
            if (_actionTimer <= 0f)
            {
                _actionTimer = NextActionDelay();
                StartCoroutine(RandomAction());
            }
        }

        private float NextActionDelay()
            => Random.Range(_config.ActionInterval.x, _config.ActionInterval.y);

        // ---------- idle animation ----------

        private void Breathe()
        {
            float t = Time.time;
            float b = Mathf.Sin(t * _config.BreathSpeed) * _config.BreathAmplitude;
            if (!_busy)
                _visualRoot.localPosition = _rootPos + new Vector3(0f, b, 0f);

            float sway = Mathf.Sin(t * _config.BreathSpeed * 0.5f) * _config.LimbSwayDegrees;
            if (_armL) _armL.localRotation = _armLRot * Quaternion.Euler(0f, 0f, sway);
            if (_armR) _armR.localRotation = _armRRot * Quaternion.Euler(0f, 0f, -sway);
        }

        // ---------- random actions ----------

        private IEnumerator RandomAction()
        {
            _busy = true;

            if (_config.EnableTrollGag && _rock != null && Random.value < _config.TrollGagChance)
                yield return TrollGag();
            else
            {
                int pick = Random.Range(0, 3);
                if (pick == 0) yield return Look(1f);
                else if (pick == 1) yield return Look(-1f);
                else yield return Hop();
            }

            RestPose();
            _busy = false;
        }

        private IEnumerator Look(float dir)
        {
            float t = 0f;
            Quaternion target = _headRot * Quaternion.Euler(0f, 0f, -14f * dir);
            while (t < 0.25f) { _head.localRotation = Quaternion.Slerp(_head.localRotation, target, 12f * Time.deltaTime); t += Time.deltaTime; yield return null; }
            yield return new WaitForSeconds(1.3f);
            t = 0f;
            while (t < 0.25f) { _head.localRotation = Quaternion.Slerp(_head.localRotation, _headRot, 12f * Time.deltaTime); t += Time.deltaTime; yield return null; }
        }

        private IEnumerator Hop()
        {
            float t = 0f;
            while (t < 0.5f)
            {
                float k = t / 0.5f;
                float y = Mathf.Sin(k * Mathf.PI) * 0.5f;
                _visualRoot.localPosition = _rootPos + new Vector3(0f, y, 0f);
                float squash = k < 0.15f ? -0.12f : 0f;
                _visualRoot.localScale = new Vector3(_rootScale.x * (1f - squash), _rootScale.y * (1f + squash), _rootScale.z);
                t += Time.deltaTime;
                yield return null;
            }
            _visualRoot.localScale = _rootScale;
        }

        private IEnumerator TrollGag()
        {
            if (_rock == null) { yield return Hop(); yield break; }

            _rock.gameObject.SetActive(true);
            _rock.localPosition = _rockHome + new Vector3(0f, 6f, 0f);

            float t = 0f;
            Vector3 from = _rock.localPosition;
            while (t < 0.35f)
            {
                _rock.localPosition = Vector3.Lerp(from, _rockHome, (t / 0.35f) * (t / 0.35f));
                t += Time.deltaTime;
                yield return null;
            }
            _rock.localPosition = _rockHome;

            // fall over
            Quaternion downRot = _rootRot * Quaternion.Euler(0f, 0f, 78f);
            t = 0f;
            while (t < 0.25f)
            {
                _visualRoot.localRotation = Quaternion.Slerp(_visualRoot.localRotation, downRot, 14f * Time.deltaTime);
                if (_legL) _legL.localRotation = _legLRot * Quaternion.Euler(0f, 0f, 40f);
                if (_legR) _legR.localRotation = _legRRot * Quaternion.Euler(0f, 0f, -55f);
                t += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            _rock.gameObject.SetActive(false);
            t = 0f;
            while (t < 0.35f)
            {
                _visualRoot.localRotation = Quaternion.Slerp(_visualRoot.localRotation, _rootRot, 12f * Time.deltaTime);
                t += Time.deltaTime;
                yield return null;
            }
        }

        private void RestPose()
        {
            _visualRoot.localPosition = _rootPos;
            _visualRoot.localRotation = _rootRot;
            _visualRoot.localScale = _rootScale;
            if (_head) _head.localRotation = _headRot;
            if (_legL) _legL.localRotation = _legLRot;
            if (_legR) _legR.localRotation = _legRRot;
        }

        // ---------- idle jokes ----------

        private void TrackIdle()
        {
            if (!_config.EnableIdleJokes) return;

            bool activity = false;
            var mouse = Mouse.current;
            if (mouse != null)
            {
                Vector2 p = mouse.position.ReadValue();
                if ((p - _lastMousePos).sqrMagnitude > 4f || mouse.leftButton.wasPressedThisFrame) activity = true;
                _lastMousePos = p;
            }
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) activity = true;

            if (activity)
            {
                _idleTimer = 0f;
                if (_bubbleShown) HideBubble();
                return;
            }

            _idleTimer += Time.deltaTime;

            if (!_bubbleShown && _idleTimer >= _config.IdleJokeMinTime)
                ShowBubble(_config.IdleLineOne);
            else if (_bubbleShown && _idleTimer >= _config.IdleJokeSecondLine)
                _speechText?.SetText(_config.IdleLineTwo);
        }

        private void ShowBubble(string text)
        {
            _bubbleShown = true;
            if (_speechText != null) _speechText.text = text;
            if (_speechBubble != null) _speechBubble.SetActive(true);
            CancelInvoke(nameof(HideBubble));
            Invoke(nameof(HideBubble), _config.IdleJokeVisibleTime);
        }

        private void HideBubble()
        {
            _bubbleShown = false;
            if (_speechBubble != null) _speechBubble.SetActive(false);
        }
    }
}
