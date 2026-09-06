using System;
using UnityEngine;

namespace StickmanTrap.Services
{
    public sealed class LocalEconomyService : IEconomyService
    {
        private const string Key = "economy.coins";

        private readonly ISaveService _save;
        private int _coins;

        public int Coins => _coins;
        public event Action<int> CoinsChanged;

        public LocalEconomyService(ISaveService save, int startingCoins)
        {
            _save = save;
            _coins = _save.Has(Key) ? Mathf.Max(0, _save.Load(Key, 0)) : startingCoins;
        }

        public void Add(int amount)
        {
            if (amount <= 0) return;
            _coins += amount;
            Persist();
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0 || _coins < amount) return false;
            _coins -= amount;
            Persist();
            return true;
        }

        private void Persist()
        {
            _save.Save(Key, _coins);
            _save.Flush();
            CoinsChanged?.Invoke(_coins);
        }
    }
}
