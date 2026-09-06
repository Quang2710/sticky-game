using System;

namespace StickmanTrap.Services
{
    /// <summary>
    /// Ads abstraction. NOT used by the menu yet — it only exists so gameplay /
    /// shop code written later never calls a platform SDK directly.
    /// <see cref="MockMonetizationService"/> is used in the editor and off-platform.
    /// </summary>
    public interface IMonetizationService
    {
        bool IsRewardedReady { get; }

        /// <param name="onComplete">true if the ad was watched to the end (grant reward).</param>
        void ShowRewardedAd(Action<bool> onComplete);

        void ShowMidgameAd(Action onDone);
    }
}
