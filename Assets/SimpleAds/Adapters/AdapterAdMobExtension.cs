using UnityEngine;

#if (UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR) && USE_ADMOB

using GoogleMobileAds.Api;

#endif

namespace Assets.SimpleAds.Adapters
{
    public partial class AdapterAdMob
    {
        [Header("Extra formats")]
        public CustomPlacement AndroidAppOpen; // https://developers.google.com/admob/unity/app-open
        public CustomPlacement IOSAppOpen;

        #if (UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR) && USE_ADMOB

        public AppOpenAd AppOpenAd;

        private bool _present;

        #if UNITY_ANDROID  || UNITY_EDITOR

        private CustomPlacement AppOpenPlacement => AndroidAppOpen;

        #elif UNITY_IOS

        private CustomPlacement AppOpenPlacement => IOSAppOpen;

        #endif

        public bool IsReadyAppOpen()
        {
            return AppOpenAd != null && !_present;
        }

        public void ShowAppOpen()
        {
            Event("ShowAppOpen");

            AppOpenAd.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
            AppOpenAd.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;
            AppOpenAd.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
            AppOpenAd.OnAdImpressionRecorded += OnAdImpressionRecorded;
            AppOpenAd.OnAdPaid += OnAdPaid;

            AppOpenAd.Show();
        }

        public void RequestAppOpen()
        {
            if (AppOpenAd != null)
            {
                AppOpenAd.Destroy();
                AppOpenAd = null;
            }

            var adRequest = new AdRequest();

            AppOpenAd.Load(AppOpenPlacement.PlacementId, ScreenOrientation.Portrait, adRequest, (ad, error) =>
            {
                if (error != null || ad == null)
                {
                    ThreadSaveEvent("OnAppOpenAdFailedToLoad", "Error", error?.ToString());
                }
                else
                {
                    AppOpenAd = ad;
                    ThreadSaveEvent("OnAppOpenAdLoaded", "MediationAdapter", AppOpenAd.GetResponseInfo().GetMediationAdapterClassName());
                }
            });
        }

        private void OnAdFullScreenContentFailed(AdError error)
        {
            ThreadSaveEvent("OnAdFullScreenContentFailed", "Error", error.ToString());
            AppOpenAd = null;
            RequestAppOpen();
        }

        private void OnAdFullScreenContentOpened()
        {
            ThreadSaveEvent("OnAdFullScreenContentOpened");
            _present = true;
        }

        private void OnAdFullScreenContentClosed()
        {
            ThreadSaveEvent("OnAdFullScreenContentClosed");
            _present = false;
            AppOpenAd = null;
            RequestAppOpen();
        }

        private void OnAdImpressionRecorded()
        {
            ThreadSaveEvent("OnAdImpressionRecorded");
        }

        private void OnAdPaid(AdValue value)
        {
            ThreadSaveEvent("OnAdPaid");
        }

        #endif
    }
}