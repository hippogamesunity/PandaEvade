using System;
using System.Collections.Generic;
using System.Linq;
using Assets.SimpleAds.Adapters;
using UnityEditor;
using UnityEngine;
using Logger = Assets.SimpleAds.Service.Logger;

#if USE_REMOTE_WATERFALL

using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;

#endif

namespace Assets.SimpleAds
{
    public class AdManager : Logger
    {
        [Header("Networks")]
        public bool UseUnityAds;
        public bool UseAdMob;
        public bool UseIronSource;
        public bool UseVungle;
        public bool UseYandexAds;
        public List<AdAdapter> Adapters;

        [Header("Settings")]
        public bool AutoInitialize = true;
        public bool UseRemoteWaterfall;
        public bool FetchRemoteConfig = true;

        [Header("Waterfalls")]
        public List<string> WaterfallInterstitial = new List<string> { "AdMob", "IronSource", "UnityAds", "Vungle", "YandexAds" };
        public List<string> WaterfallRewarded = new List<string> { "AdMob", "IronSource", "UnityAds", "Vungle", "YandexAds" };
        public List<string> WaterfallBanner = new List<string> { "AdMob", "IronSource", "UnityAds", "Vungle", "YandexAds" };

        public static AdManager Instance;

        public void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            if (AutoInitialize)
            {
                Initialize();
            }

            #if USE_REMOTE_WATERFALL

            HandleFetchCompleted();

            if (FetchRemoteConfig)
            {
                FetchConfigs();
            }

            #endif
        }

        public void Initialize()
        {
            Event("Initialize");
            Adapters.ForEach(i => i.Initialize());
        }

        public bool IsReadyInterstitial()
        {
            return Adapters.Any(i => i.IsReadyInterstitial());
        }

        public bool IsReadyRewarded()
        {
            return Adapters.Any(i => i.IsReadyRewarded());
        }

        public bool IsReadyBanner()
        {
            return Adapters.Any(i => i.IsReadyBanner());
        }

        public void ShowInterstitial()
        {
            var adapter = OrderAdapters(WaterfallInterstitial).FirstOrDefault(i => i.IsReadyInterstitial());

            if (adapter == null)
            {
                Debug.LogWarning("Interstitial not ready.");
            }
            else
            {
                Event("ShowInterstitial", "Provider", adapter.ProviderName);
                adapter.ShowInterstitial();
            }
        }

        public void ShowRewarded(Action callback)
        {
            var adapter = OrderAdapters(WaterfallRewarded).FirstOrDefault(i => i.IsReadyRewarded());

            if (adapter == null)
            {
                Debug.LogWarning("Rewarded not ready.");
            }
            else
            {
                Event("AdManager.ShowRewarded", "Provider", adapter.ProviderName);
                adapter.ShowRewarded((providerName, reward, amount) => callback?.Invoke());
            }
        }

        public void ShowRewarded(Action<string, string, float> callback)
        {
            var adapter = OrderAdapters(WaterfallRewarded).FirstOrDefault(i => i.IsReadyRewarded());

            if (adapter == null)
            {
                Debug.LogWarning("Rewarded not ready.");
            }
            else
            {
                Event("AdManager.ShowRewarded", "Provider", adapter.ProviderName);
                adapter.ShowRewarded(callback);
            }
        }

        private AdAdapter _bannerAdapter;

        public void ShowBanner()
        {
            var adapter = OrderAdapters(WaterfallBanner).FirstOrDefault(i => i.IsReadyBanner());

            if (_bannerAdapter != null && _bannerAdapter != adapter)
            {
                _bannerAdapter.HideBanner();
            }

            _bannerAdapter = adapter;

            if (_bannerAdapter == null)
            {
                Debug.LogWarning("Banner not ready.");
            }
            else
            {
                Event("ShowBanner", "Provider", _bannerAdapter.ProviderName);
                _bannerAdapter.ShowBanner();
            }
        }

        public void HideBanner()
        {
            _bannerAdapter?.HideBanner();
        }

        private IEnumerable<AdAdapter> OrderAdapters(List<string> waterfall)
        {
            return Adapters.OrderBy(i => waterfall.Contains(i.ProviderName) ? waterfall.IndexOf(i.ProviderName) : 999);
        }

        #if USE_REMOTE_WATERFALL

        private void HandleFetchCompleted()
        {
            RemoteConfigService.Instance.FetchCompleted += response =>
            {
                if (response.status == ConfigRequestStatus.Success)
                {
                    Debug.Log($"<color=yellow>RemoteConfigService.Instance.FetchCompleted ({response.requestOrigin})</color>");

                    if (RemoteConfigService.Instance.appConfig.HasKey("AdManager.Waterfall.Interstitial"))
                    {
                        var value = RemoteConfigService.Instance.appConfig.GetString("AdManager.Waterfall.Interstitial");

                        Event("RemoteConfig.FetchCompleted", "WaterfallInterstitial", value);
                        WaterfallInterstitial = value.Split(',').ToList();
                    }

                    if (RemoteConfigService.Instance.appConfig.HasKey("AdManager.Waterfall.Rewarded"))
                    {
                        var value = RemoteConfigService.Instance.appConfig.GetString("AdManager.Waterfall.Rewarded");

                        Event("RemoteConfig.FetchCompleted", "WaterfallRewarded", value);
                        WaterfallRewarded = value.Split(',').ToList();
                    }

                    if (RemoteConfigService.Instance.appConfig.HasKey("AdManager.Waterfall.Banner"))
                    {
                        var value = RemoteConfigService.Instance.appConfig.GetString("AdManager.Waterfall.Banner");

                        Event("RemoteConfig.FetchCompleted", "WaterfallBanner", value);
                        WaterfallBanner = value.Split(',').ToList();
                    }
                }
            };
        }

        private static bool _fetched;

        private static async void FetchConfigs()
        {
            if (_fetched) return;

            try
            {
                await UnityServices.InitializeAsync();

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                RemoteConfigService.Instance.FetchConfigs(new UserAttributes(), new AppAttributes());

                _fetched = true;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }

        private struct UserAttributes
        {
        }

        private struct AppAttributes
        {
        }

        #endif

        #if UNITY_EDITOR

        void OnValidate()
        {
            if (Application.isPlaying || !gameObject.activeSelf) return;

            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';').ToList();
            var copy = defines.ToList();

            foreach (var define in new[] { "USE_REMOTE_WATERFALL", "USE_UNITYADS", "USE_ADMOB", "USE_IRONSOURCE", "USE_VUNGLE", "USE_YANDEXADS" })
            {
                if (defines.Contains(define)) defines.Remove(define);
            }
            
            if (UseRemoteWaterfall) defines.Add("USE_REMOTE_WATERFALL");
            if (UseUnityAds) defines.Add("USE_UNITYADS");
            if (UseAdMob) defines.Add("USE_ADMOB");
            if (UseIronSource) defines.Add("USE_IRONSOURCE");
            if (UseVungle) defines.Add("USE_VUNGLE");
            if (UseYandexAds) defines.Add("USE_YANDEXADS");

            if (!defines.OrderBy(i => i).SequenceEqual(copy.OrderBy(i => i)))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", defines));
            }

            UpdateAdapters();
        }

        private void UpdateAdapters()
        {
            Adapters.Clear();

            if (UseUnityAds) Adapters.Add(GetComponent<AdapterUnityAds>() ?? gameObject.AddComponent<AdapterUnityAds>());
            if (UseAdMob) Adapters.Add(GetComponent<AdapterAdMob>() ?? gameObject.AddComponent<AdapterAdMob>());
            if (UseIronSource) Adapters.Add(GetComponent<AdapterIronSource>() ?? gameObject.AddComponent<AdapterIronSource>());
            if (UseVungle) Adapters.Add(GetComponent<AdapterVungle>() ?? gameObject.AddComponent<AdapterVungle>());
            if (UseYandexAds) Adapters.Add(GetComponent<AdapterYandexAds>() ?? gameObject.AddComponent<AdapterYandexAds>());
        }

        #endif
    }
}