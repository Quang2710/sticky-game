using System;
using UnityEngine;

namespace StickmanTrap.Services
{
    /// <summary>Editor / off-platform stand-in: "ads" always succeed instantly.</summary>
    public sealed class MockMonetizationService : IMonetizationService
    {
        public bool IsRewardedReady => true;

        public void ShowRewardedAd(Action<bool> onComplete)
        {
            Debug.Log("[Ads] (mock) rewarded ad -> granting reward");
            onComplete?.Invoke(true);
        }

        public void ShowMidgameAd(Action onDone)
        {
            Debug.Log("[Ads] (mock) midgame ad -> done");
            onDone?.Invoke();
        }
    }
}
