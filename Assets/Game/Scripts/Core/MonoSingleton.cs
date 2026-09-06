using UnityEngine;

namespace StickmanTrap.Core
{
    /// <summary>
    /// Minimal singleton base for the *few* genuinely global services
    /// (audio, save, monetization). Do NOT use this for gameplay objects —
    /// those are wired via serialized references or events.
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        public static T Instance { get; private set; }
        public static bool Exists => Instance != null;

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = (T)this;
            if (transform.parent == null)
                DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
