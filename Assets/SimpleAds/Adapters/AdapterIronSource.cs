using System;

namespace Assets.SimpleAds.Adapters
{
    public class AdapterIronSource : AdAdapter
    {
        #if (UNITY_ANDROID || UNITY_IOS) && USE_IRONSOURCE

        public IronSourceBannerSize BannerSize = IronSourceBannerSize.SMART;
        public IronSourceBannerPosition BannerPosition = IronSourceBannerPosition.BOTTOM;

        public static bool BannerLoaded { get; private set; }

        public override string ProviderName => "IronSource";

        public static AdapterIronSource Instance;

        public void Awake()
        {
            Instance = this;
        }

        public override void Initialize()
        {
            if (Initialized) return;

            Event("Initialize");
            SubscribeEvents();
            IronSource.Agent.validateIntegration();
            IronSource.Agent.init(Key);
        }

        public override bool IsReadyInterstitial()
        {
            return IronSource.Agent.isInterstitialReady();
        }

        public override bool IsReadyRewarded()
        {
            return IronSource.Agent.isRewardedVideoAvailable();
        }

        public override bool IsReadyBanner()
        {
            return BannerLoaded;
        }

        public override void ShowInterstitial()
        {
            Event("ShowInterstitial");
            IronSource.Agent.showInterstitial();
        }

        public override void ShowRewarded(Action<string, string, float> rewardAction)
        {
            Event("ShowRewarded");
            RewardAction = rewardAction;
            IronSource.Agent.showRewardedVideo();
        }

        public override void ShowBanner()
        {
            Event("ShowBanner");
            IronSource.Agent.displayBanner();
        }

        public override void HideBanner()
        {
            Event("HideBanner");
            IronSource.Agent.hideBanner();
        }

        private void OnApplicationPause(bool pause)
        {
            IronSource.Agent.onApplicationPause(pause);
        }

        private void SubscribeEvents()
        {
            IronSourceEvents.onSdkInitializationCompletedEvent += () =>
            {
                Event("OnSdkInitializationCompleted");
                Initialized = true;
                if (Placements.LoadInterstitial) IronSource.Agent.loadInterstitial();
                //if (Placements.LoadRewarded) IronSource.Agent.loadRewardedVideo(); // Loaded automatically by default.
                if (Placements.LoadBanner)
                {
                    BannerSize.SetAdaptive(true);
                    IronSource.Agent.loadBanner(BannerSize, BannerPosition);
                    IronSource.Agent.hideBanner();
                }
            };

            IronSourceEvents.onInterstitialAdReadyEvent += () => Event("OnInterstitialAdReady");
            IronSourceEvents.onInterstitialAdLoadFailedEvent += error => Event("OnInterstitialAdLoadFailed", "Error", error);
            IronSourceEvents.onInterstitialAdShowSucceededEvent += () => Event("OnInterstitialAdShowSucceeded");
            IronSourceEvents.onInterstitialAdShowFailedEvent += error => Event("OnInterstitialAdShowFailed", "Error", error);
            IronSourceEvents.onInterstitialAdClickedEvent += () => Event("OnInterstitialAdClicked");
            IronSourceEvents.onInterstitialAdOpenedEvent += () => Event("OnInterstitialAdOpened");
            IronSourceEvents.onInterstitialAdClosedEvent += () =>
            {
                Event("InterstitialAdClosed");
                IronSource.Agent.loadInterstitial();
            };
            IronSourceEvents.onRewardedVideoAdReadyEvent += () => Event("OnRewardedVideoAdReady");
            IronSourceEvents.onRewardedVideoAdLoadFailedEvent += error => Event("OnRewardedVideoAdLoadFailed", "Error", error);
            IronSourceEvents.onRewardedVideoAdOpenedEvent += () => Event("OnRewardedVideoAdOpened");
            IronSourceEvents.onRewardedVideoAdClosedEvent += () => Event("OnRewardedVideoAdClosed");
            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += available => Event("OnRewardedVideoAvailabilityChanged", "Available", available);
            IronSourceEvents.onRewardedVideoAdStartedEvent += () => Event("OnRewardedVideoAdStarted");
            IronSourceEvents.onRewardedVideoAdEndedEvent += () =>
            {
                Event("OnRewardedVideoAdEnded");
                //IronSource.Agent.loadRewardedVideo(); // Loaded automatically by default.
            };
            IronSourceEvents.onRewardedVideoAdRewardedEvent += placement =>
            {
                Event("OnRewardedVideoAdRewarded", "Reward", placement.getRewardName(), "Amount", placement.getRewardAmount());
                GiveReward(placement.getRewardName(), placement.getRewardAmount());
            };
            IronSourceEvents.onRewardedVideoAdShowFailedEvent += error => Event("OnRewardedVideoAdShowFailed", "Error", error);
            IronSourceEvents.onRewardedVideoAdClickedEvent += placement => Event("OnRewardedVideoAdClicked", "Placement", placement);
            IronSourceEvents.onBannerAdLoadedEvent += ()=>
            {
                Event("OnBannerAdLoaded");
                BannerLoaded = true;
            };
            IronSourceEvents.onBannerAdLoadFailedEvent += error => Event("OnBannerAdLoadFailed", "Error", error);
            IronSourceEvents.onBannerAdClickedEvent += () => Event("OnBannerAdClicked");
            IronSourceEvents.onBannerAdScreenPresentedEvent += () => Event("OnBannerAdScreenPresented");
            IronSourceEvents.onBannerAdScreenDismissedEvent += () => Event("OnBannerAdScreenDismissed");
            IronSourceEvents.onBannerAdLeftApplicationEvent += () => Event("OnBannerAdLeftApplication");
        }

        #endif
    }
}