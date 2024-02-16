using System;
using System.Collections.Generic;

#if (UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR) && USE_UNITYADS

using UnityEngine.Advertisements;

#endif

namespace Assets.SimpleAds.Adapters
{
    #if (UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR) && USE_UNITYADS

    public class AdapterUnityAds : AdAdapter, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        public BannerPosition BannerPosition = BannerPosition.BOTTOM_CENTER;

        public override string ProviderName => "UnityAds";

        public static AdapterUnityAds Instance;

        public List<string> Loaded { get; } = new List<string>();

        public void Awake()
        {
            Instance = this;
        }

        public override void Initialize()
        {
            if (Initialized) return;

            Event("Initialize");
            Advertisement.Initialize(Key, false, this);
        }

        public override bool IsReadyInterstitial()
        {
            return Loaded.Contains(Placements.Interstitial);
        }

        public override bool IsReadyRewarded()
        {
            return Loaded.Contains(Placements.Rewarded);
        }

        public override bool IsReadyBanner()
        {
            return Advertisement.Banner.isLoaded;
        }

        public override void ShowInterstitial()
        {
            Event("ShowInterstitial");
            Advertisement.Show(Placements.Interstitial, this);
        }

        public override void ShowRewarded(Action<string, string, float> rewardAction)
        {
            Event("ShowRewarded");
            RewardAction = rewardAction;
            Advertisement.Show(Placements.Rewarded, this);
        }

        public override void ShowBanner()
        {
            Event("ShowBanner");
            Advertisement.Banner.Show(Placements.Banner);
        }

        public override void HideBanner()
        {
            Event("HideBanner");
            Advertisement.Banner.Hide();
        }

        #region Callbacks

        public void OnInitializationComplete()
        {
            Event("OnInitializationComplete");
            Initialized = true;
            if (Placements.LoadInterstitial && Placements.Interstitial != "") Advertisement.Load(Placements.Interstitial, this);
            if (Placements.LoadRewarded && Placements.Rewarded != "") Advertisement.Load(Placements.Rewarded, this);
            if (Placements.LoadBanner && Placements.Banner != "")
            {
                var options = new BannerLoadOptions();

                options.loadCallback += () => Event("BannerLoaded");
                options.errorCallback += error => Event("BannerNotLoaded", "Error", error);

                Advertisement.Banner.SetPosition(BannerPosition);
                Advertisement.Banner.Load(Placements.Banner, options);
            }
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Event("OnInitializationFailed", "Error", error.ToString(), "Message", message);
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            Event("OnUnityAdsAdLoaded", "PlacementId", placementId);
            Loaded.Add(placementId);
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Event("OnUnityAdsFailedToLoad", "Error", error.ToString(), "Message", message);
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Event("OnUnityAdsShowFailure", "Error", error.ToString(), "Message", message);
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            Event("OnUnityAdsShowStart", "PlacementId", placementId);
            Loaded.Remove(placementId);
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            Event("OnUnityAdsShowClick", "PlacementId", placementId);
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState state)
        {
            Event("OnUnityAdsShowComplete", "PlacementId", placementId, "State", state.ToString());

            if (placementId == Placements.Rewarded && state == UnityAdsShowCompletionState.COMPLETED)
            {
                GiveReward(null, 1);
            }

            Advertisement.Load(placementId, this);
        }

        #endregion
    }

    #else

    public class AdapterUnityAds : AdAdapter
    {
    }

    #endif
}