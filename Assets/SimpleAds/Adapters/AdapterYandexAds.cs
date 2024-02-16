using System;
using UnityEngine;

#if (UNITY_ANDROID || UNITY_IOS) && USE_YANDEXADS

using YandexMobileAds;
using YandexMobileAds.Base;

#endif

namespace Assets.SimpleAds.Adapters
{
    public class AdapterYandexAds : AdAdapter
    {
        #if (UNITY_ANDROID || UNITY_IOS) && USE_YANDEXADS
		
		public AdPosition BannerPosition = AdPosition.BottomCenter;

        public override string ProviderName => "YandexAds";

        public Interstitial Interstitial;
        public RewardedAd RewardedAd;
        public Banner Banner;
        
        public static bool BannerLoaded { get; private set; }

        private InterstitialAdLoader _interstitialAdLoader;
        private RewardedAdLoader _rewardedAdLoader;

        public static AdapterYandexAds Instance;

        public void Awake()
        {
            Instance = this;
        }

        public override void Initialize()
        {
            if (Initialized) return;

            Event("Initialize");

            if (Placements.LoadInterstitial && Placements.Interstitial != "") RequestInterstitial();
            if (Placements.LoadRewarded && Placements.Rewarded != "") RequestRewardedAd();
            if (Placements.LoadBanner && Placements.Banner != "") RequestBanner();

            Initialized = true;
        }

        public override bool IsReadyInterstitial()
        {
            return Interstitial != null;
        }

        public override bool IsReadyRewarded()
        {
            return RewardedAd != null;
        }

        public override bool IsReadyBanner()
        {
            return BannerLoaded;
        }

        public override void ShowInterstitial()
        {
            Event("ShowInterstitial");
            Interstitial?.Show();
        }

        public override void ShowRewarded(Action<string, string, float> rewardAction)
        {
            Event("ShowRewarded");
            RewardAction = rewardAction;
            RewardedAd?.Show();
        }

        public override void ShowBanner()
        {
            Event("ShowBanner");
            Banner?.Show();
        }

        public override void HideBanner()
        {
            Event("HideBanner");
            Banner?.Hide();
            Banner?.Destroy();
            RequestBanner();
        }

        public void OnDestroy()
        {
            Banner?.Destroy();
        }

        public void RequestInterstitial()
        {
            if (_interstitialAdLoader == null)
            {
                _interstitialAdLoader = new InterstitialAdLoader();
                _interstitialAdLoader.OnAdLoaded += (_, args) =>
                {
                    Event("InterstitialAdLoader.OnAdLoaded");
                    Interstitial = args.Interstitial;
                    Interstitial.OnAdClicked += (_, _) => Event("Interstitial.OnAdClicked");
                    Interstitial.OnAdShown += (_, _) => { Event("Interstitial.OnAdShown"); RequestInterstitial(); };
                    Interstitial.OnAdFailedToShow += (_, error) => Event("Interstitial.OnAdFailedToShow", "Error", error.Message);
                    Interstitial.OnAdImpression += (_, _) => Event("Interstitial.OnAdImpression");
                    Interstitial.OnAdDismissed += (_, _) => Event("Interstitial.OnAdDismissed");
                };
                _interstitialAdLoader.OnAdFailedToLoad += (_, error) => { Event("InterstitialAdLoader.OnAdFailedToLoad", "Error", error.Message); };
            }

            var request = new AdRequestConfiguration.Builder(Placements.Interstitial).Build();

            _interstitialAdLoader.LoadAd(request);
        }

        public void RequestRewardedAd()
        {
            if (RewardedAd == null)
            {
                _rewardedAdLoader = new RewardedAdLoader();
                _rewardedAdLoader.OnAdLoaded += (_, args) =>
                {
                    Event("RewardedAdLoader.OnAdLoaded");
                    RewardedAd = args.RewardedAd;
                    RewardedAd.OnAdClicked += (_, _) => Event("RewardedAd.OnAdClicked");
                    RewardedAd.OnAdShown += (_, _) => { Event("RewardedAd.OnAdShown"); RequestRewardedAd(); };
                    RewardedAd.OnAdFailedToShow += (_, error) => Event("RewardedAd.OnAdFailedToShow", "Error", error.Message);
                    RewardedAd.OnAdImpression += (_, _) => Event("RewardedAd.OnAdImpression");
                    RewardedAd.OnAdDismissed += (_, _) => Event("RewardedAd.OnAdDismissed");
                    RewardedAd.OnRewarded += (_, reward) => { Event("RewardedAd.OnRewarded ", "Reward", reward.type, "Amount", reward.amount); GiveReward(reward.type, reward.amount); };
                };
                _rewardedAdLoader.OnAdFailedToLoad += (_, error) => { Event("RewardedAdLoader.OnAdFailedToLoad", "Error", error.Message); };
            }

            var request = new AdRequestConfiguration.Builder(Placements.Rewarded).Build();

            _rewardedAdLoader.LoadAd(request);
        }

        public void RequestBanner()
        {
            var size = BannerAdSize.InlineSize(ScreenUtils.ConvertPixelsToDp((int) Screen.safeArea.width), 100);

            Banner = new Banner(Placements.Banner, size, BannerPosition);
            Banner.OnAdLoaded += (_, _) => { Event("Banner.OnAdLoaded"); BannerLoaded = true; };
            Banner.OnAdFailedToLoad += (_, args) => { Event("Banner.OnAdFailedToLoad", "Error", args.Message); };
            Banner.OnReturnedToApplication += (_, _) => { Event("Banner.OnReturnedToApplication"); };
            Banner.OnLeftApplication += (_, _) => { Event("Banner.OnLeftApplication"); };
            Banner.OnAdClicked += (_, _) => { Event("Banner.OnAdClicked"); };
            Banner.OnImpression += (_, _) => { Event("Banner.OnImpression"); };

            var request = new AdRequest.Builder().Build();

            Banner.LoadAd(request);
            Banner.Hide();
        }

        #endif
    }
}