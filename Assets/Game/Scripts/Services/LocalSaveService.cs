using System;
using System.Globalization;
using UnityEngine;

namespace StickmanTrap.Services
{
    /// <summary>
    /// PlayerPrefs-backed save (on WebGL this is browser IndexedDB). Primitives are
    /// stored directly; anything else is round-tripped through <see cref="JsonUtility"/>.
    /// </summary>
    public sealed class LocalSaveService : ISaveService
    {
        private bool _dirty;

        public bool Has(string key) => PlayerPrefs.HasKey(key);

        public T Load<T>(string key, T fallback = default)
        {
            if (!PlayerPrefs.HasKey(key)) return fallback;

            string raw = PlayerPrefs.GetString(key, string.Empty);
            if (string.IsNullOrEmpty(raw)) return fallback;

            Type t = typeof(T);
            try
            {
                if (t == typeof(int)) return (T)(object)int.Parse(raw, CultureInfo.InvariantCulture);
                if (t == typeof(bool)) return (T)(object)(raw == "1" || raw == "true");
                if (t == typeof(float)) return (T)(object)float.Parse(raw, CultureInfo.InvariantCulture);
                if (t == typeof(string)) return (T)(object)raw;
                return JsonUtility.FromJson<T>(raw);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Save] Failed to read '{key}': {e.Message}");
                return fallback;
            }
        }

        public void Save<T>(string key, T value)
        {
            Type t = typeof(T);
            string raw;
            if (t == typeof(int)) raw = ((int)(object)value).ToString(CultureInfo.InvariantCulture);
            else if (t == typeof(bool)) raw = ((bool)(object)value) ? "1" : "0";
            else if (t == typeof(float)) raw = ((float)(object)value).ToString(CultureInfo.InvariantCulture);
            else if (t == typeof(string)) raw = (string)(object)value ?? string.Empty;
            else raw = JsonUtility.ToJson(value);

            PlayerPrefs.SetString(key, raw);
            _dirty = true;
        }

        public void Delete(string key)
        {
            if (!PlayerPrefs.HasKey(key)) return;
            PlayerPrefs.DeleteKey(key);
            _dirty = true;
        }

        public void Flush()
        {
            if (!_dirty) return;
            PlayerPrefs.Save();
            _dirty = false;
        }
    }
}
