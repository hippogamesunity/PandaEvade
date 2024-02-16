using System;
using UnityEngine;

namespace Assets.SimpleAds.Adapters
{
    public class AdapterVungle : AdAdapter
    {
        [Header("UWP")]
        public string WindowsKey;
        public Placements WindowsPlacements;

        #if UNITY_WSA

        protected string Key => WindowsKey;
        protected Placements Placements => WindowsPlacements;

        #endif
        
        #if (UNITY_ANDROID || UNITY_IOS || UNITY_WSA) && USE_VUNGLE

        public Vungle.VungleBannerSize BannerSize = Vungle.VungleBannerSize.VungleAdSizeBanner;
        public Vungle.VungleBannerPosition BannerPosition = Vungle.VungleBannerPosition.BottomCenter;

        public override string ProviderName => "Vungle";

        public static AdapterVungle Instance;

        public void Awake()
        {
            Instance = this;
        }

        public override void Initialize()
        {
            if (Initialized) return;

            Event("Initialize");
            SubscribeEvents();
            Vungle.init(Key);
        }

        public override bool IsReadyInterstitial()
        {
            return Vungle.isAdvertAvailable(Placements.Interstitial);
        }

        public override bool IsReadyRewarded()
        {
            return Vungle.isAdvertAvailable(Placements.Rewarded);
        }

        public override bool IsReadyBanner()
        {
            return Vungle.isAdvertAvailable(Placements.Banner, BannerSize);
        }

        public override void ShowInterstitial()
        {
            Event("ShowInterstitial");
            Vungle.playAd(Placements.Interstitial);
        }

        public override void ShowRewarded(Action<string, string, float> rewardAction)
        {
            Event("ShowRewarded");
            RewardAction = rewardAction;
            Vungle.playAd(Placements.Rewarded);
        }

        public override void ShowBanner()
        {
            Event("ShowBanner");
            Vungle.showBanner(Placements.Banner);
        }

        public override void HideBanner()
        {
            Event("HideBanner");
            Vungle.closeBanner(Placements.Banner);
        }

        private void SubscribeEvents()
        {
            Vungle.onInitializeEvent += () =>
            {
                Event("OnInitialize");
                Initialized = true;
                if (Placements.LoadInterstitial && Placements.Interstitial != "") Vungle.loadAd(Placements.Interstitial);
                if (Placements.LoadRewarded && Placements.Rewarded != "") Vungle.loadAd(Placements.Rewarded);
                if (Placements.LoadBanner && Placements.Banner != "") Vungle.loadBanner(Placements.Banner, BannerSize, BannerPosition);
            };
            Vungle.adPlayableEvent += (placement, playable) => Event("AdPlayable", "Placement", placement, "Playable", playable);
            Vungle.onAdStartedEvent += placement => Event("OnAdStarted", "Placement", placement);
            Vungle.onAdEndEvent += placement =>
            {
                Event("OnAdEnd", "Placement", placement);
                Vungle.loadAd(placement);
            };
            Vungle.onAdRewardedEvent += placement =>
            {
                Event("OnAdRewarded", "Placement", placement);
                GiveReward(null, 1);
            };
            Vungle.onAdClickEvent += placement => Event("OnAdClick", "Placement", placement);
        }

        #endif
    }
}