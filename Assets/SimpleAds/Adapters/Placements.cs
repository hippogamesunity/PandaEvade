using System;
using UnityEngine;

namespace Assets.SimpleAds.Adapters
{
    [Serializable]
    public class Placements
    {
        [Header("Interstitial")]
        public bool LoadInterstitial = true;
        public string Interstitial;

        [Header("Rewarded")]
        public bool LoadRewarded = true;
        public string Rewarded;

        [Header("Banner")]
        public bool LoadBanner = true;
        public string Banner;
    }
}