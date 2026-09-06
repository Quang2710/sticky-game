namespace StickmanTrap.Services
{
    /// <summary>
    /// Persistence abstraction. MVP is <see cref="LocalSaveService"/> (PlayerPrefs);
    /// a CrazyGames cloud implementation can be swapped in later without touching
    /// callers. Only stores progression-sized data, never full level/game state.
    /// </summary>
    public interface ISaveService
    {
        bool Has(string key);
        T Load<T>(string key, T fallback = default);
        void Save<T>(string key, T value);
        void Delete(string key);

        /// <summary>Persist pending writes to disk / browser storage.</summary>
        void Flush();
    }
}
