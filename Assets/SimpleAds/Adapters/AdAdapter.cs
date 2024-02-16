using System;
using Assets.SimpleAds.Service;
using UnityEngine;

namespace Assets.SimpleAds.Adapters
{
    public abstract class AdAdapter : Service.Logger
    {
        [Header("Android")]
        public string AndroidKey;
        public Placements AndroidPlacements;

        [Header("iOS")]
        public string IOSKey;
        public Placements IOSPlacements;

        #if UNITY_ANDROID

        protected string Key => AndroidKey;
        protected Placements Placements => AndroidPlacements;

        #elif UNITY_IOS

        protected string Key => IOSKey;
        protected Placements Placements => IOSPlacements;

        #elif UNITY_EDITOR

        protected string Key => AndroidKey;
        protected Placements Placements => AndroidPlacements;

        #endif

        public virtual string ProviderName { get; } = null;
        public bool Initialized { get; protected set; }

        public virtual void Initialize()
        {
        }

        public virtual bool IsReadyInterstitial()
        {
            return false;
        }

        public virtual bool IsReadyRewarded()
        {
            return false;
        }

        public virtual bool IsReadyBanner()
        {
            return false;
        }

        public virtual void ShowInterstitial()
        {
        }

        public virtual void ShowRewarded(Action<string, string, float> callback)
        {
            RewardAction = callback;
        }

        public virtual void ShowBanner()
        {
        }

        public virtual void HideBanner()
        {
        }

        protected Action<string, string, float> RewardAction;

        protected void GiveReward(string reward, float amount)
        {
            if (RewardAction == null) return;

            var action = RewardAction;

            ThreadSaveExecutor.Execute(() => action(ProviderName, reward, amount));
            RewardAction = null;
        }
    }
}