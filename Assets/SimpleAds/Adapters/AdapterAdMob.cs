using System;
using System.Collections.Generic;
using Assets.SimpleAds.Service;

#if (UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR) && USE_ADMOB

using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;

#endif

namespace Assets.SimpleAds.Adapters
{
    public partial class AdapterAdMob : AdAdapter
    {
        #if (UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR) && USE_ADMOB
		
		public AdPosition BannerPosition = AdPosition.Bottom;

        public override string ProviderName => "AdMob";

        public InterstitialAd Interstitial;
        public RewardedAd RewardedAd;
        public BannerView BannerView;
        
        public static bool InterstitialLoaded { get; private set; }
        public static bool RewardedAdLoaded { get; private set; }
        public static bool BannerLoaded { get; private set; }

        public static AdapterAdMob Instance;

        public void Awake()
        {
            Instance = this;
        }

        public override void Initialize()
        {
            if (Initialized) return;

            Event("Initialize");

            /* Uncomment for debug.

            ConsentInformation.Reset();

            var debugSettings = new ConsentDebugSettings
            {
                DebugGeography = DebugGeography.EEA, TestDeviceHashedIds = new List<string> { "CEF84C94BD8A1C431CA29AAAB78FDA8E" }
            };
            var request = new ConsentRequestParameters { ConsentDebugSettings = debugSettings };

            */

            var request = new ConsentRequestParameters();

            ConsentInformation.Update(request, consentError =>
            {
                ThreadSaveEvent("OnConsentInfoUpdated", "ConsentError", consentError?.Message);

                if (consentError == null)
                {
                    ConsentForm.LoadAndShowConsentFormIfRequired(formError =>
                    {
                        ThreadSaveEvent("OnLoadAndShowConsentFormIfRequired", "FormError", formError?.Message);

                        if (ConsentInformation.CanRequestAds())
                        {
                            MobileAds.Initialize(OnInitialized);
                        }
                    });
                }
            });
        }

        public override bool IsReadyInterstitial()
        {
            return Interstitial?.CanShowAd() == true;
        }

        public override bool IsReadyRewarded()
        {
            return RewardedAd?.CanShowAd() == true;
        }

        public override bool IsReadyBanner()
        {
            return BannerLoaded;
        }

        public override void ShowInterstitial()
        {
            Event("ShowInterstitial");
            Interstitial.Show();
        }

        public override void ShowRewarded(Action<string, string, float> rewardAction)
        {
            Event("ShowRewarded");
            RewardAction = rewardAction;
            RewardedAd.Show(reward =>
            {
                ThreadSaveEvent("OnRewardedAdUserEarnedReward", "Reward", reward.Type, "Amount", reward.Amount);
                GiveReward(reward.Type, (float) reward.Amount);
            });
        }

        public override void ShowBanner()
        {
            Event("ShowBanner");
            BannerView.Show();
        }

        public override void HideBanner()
        {
            Event("HideBanner");
            BannerView.Hide();
        }

        public void OnDestroy()
        {
            BannerView?.Destroy();
        }

        public void RequestInterstitial()
        {
            if (Interstitial != null)
            {
                Interstitial.Destroy();
                Interstitial = null;
            }

            var adRequest = new AdRequest();

            adRequest.Keywords.Add("unity-admob-sample");

            InterstitialAd.Load(Placements.Interstitial, adRequest, (ad, error) =>
            {
                if (error != null || ad == null)
                {
                    ThreadSaveEvent("OnInterstitialFailedToLoad", "Error", error?.ToString());
                }
                else
                {
                    Interstitial = ad;
                    InterstitialLoaded = true;
                    Interstitial.OnAdFullScreenContentClosed += RequestInterstitial;
                    ThreadSaveEvent("OnInterstitialLoaded", "MediationAdapter", Interstitial.GetResponseInfo().GetMediationAdapterClassName());
                }
            });
        }

        public void RequestRewardedAd()
        {
            if (RewardedAd != null)
            {
                RewardedAd.Destroy();
                RewardedAd = null;
            }

            var adRequest = new AdRequest();

            adRequest.Keywords.Add("unity-admob-sample");

            RewardedAd.Load(Placements.Rewarded, adRequest, (ad, error) =>
            {
                if (error != null || ad == null)
                {
                    ThreadSaveEvent("OnRewardedAdFailedToLoad", "Error", error?.ToString());
                }
                else
                {
                    RewardedAd = ad;
                    RewardedAdLoaded = true;
                    RewardedAd.OnAdFullScreenContentClosed += RequestRewardedAd;
                    ThreadSaveEvent("OnRewardedAdLoaded", "MediationAdapter", RewardedAd.GetResponseInfo().GetMediationAdapterClassName());
                }
            });
        }

        public void RequestBanner()
        {
            if (BannerView != null)
            {
                BannerView.Destroy();
                BannerView = null;
            }

            BannerView = new BannerView(Placements.Banner, AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth), BannerPosition);

            var adRequest = new AdRequest();

            adRequest.Keywords.Add("unity-admob-sample");

            BannerView.LoadAd(adRequest);
            BannerView.Hide();
            BannerView.OnBannerAdLoaded += () =>
            {
                ThreadSaveEvent("OnBannerAdLoaded", "MediationAdapter", BannerView.GetResponseInfo().GetMediationAdapterClassName());
                BannerLoaded = true;
            };
            BannerView.OnBannerAdLoadFailed += error => ThreadSaveEvent("OnBannerFailedToLoad", "Error", error.ToString());
        }

        private void OnInitialized(InitializationStatus status)
        {
            ThreadSaveEvent("OnInitialized");
            Initialized = true;

            if (Placements.LoadInterstitial && Placements.Interstitial != "") RequestInterstitial();
            if (Placements.LoadRewarded && Placements.Rewarded != "") RequestRewardedAd();
            if (Placements.LoadBanner && Placements.Banner != "") RequestBanner();
            if (AppOpenPlacement.Load && AppOpenPlacement.PlacementId != "") RequestAppOpen();

            foreach (var data in status.getAdapterStatusMap())
            {
                var adapterName = data.Key;
                var adapterStatus = data.Value;

                switch (adapterStatus.InitializationState)
                {
                    case AdapterState.Ready:
                        ThreadSaveEvent("AdapterReady", "Adapter", adapterName);
                        break;
                    case AdapterState.NotReady:
                        ThreadSaveEvent("AdapterNotReady", "Adapter", adapterName);
                        break;
                }
            }
        }

        private void ThreadSaveEvent(string eventName)
        {
            // AdMob events are fired not from the main thread, so we have to do the trick.
            ThreadSaveExecutor.Execute(() => Event(eventName));
        }

        private void ThreadSaveEvent(string eventName, string paramName, string paramValue)
        {
            // AdMob events are fired not from the main thread, so we have to do the trick.
            ThreadSaveExecutor.Execute(() => Event(eventName, paramName, paramValue));
        }

        private void ThreadSaveEvent(string eventName, string paramName1, object paramValue1, string paramName2, object paramValue2)
        {
            // AdMob events are fired not from the main thread, so we have to do the trick.
            ThreadSaveExecutor.Execute(() => Event(eventName, paramName1, paramValue1, paramName2, paramValue2));
        }

        #endif
    }
}