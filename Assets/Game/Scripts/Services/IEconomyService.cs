using System;

namespace StickmanTrap.Services
{
    /// <summary>
    /// Soft currency (coins). Cosmetic-only economy — never pay-to-win.
    /// <see cref="CoinsChanged"/> carries the new balance.
    /// </summary>
    public interface IEconomyService
    {
        int Coins { get; }
        event Action<int> CoinsChanged;

        void Add(int amount);
        bool TrySpend(int amount);
    }
}
