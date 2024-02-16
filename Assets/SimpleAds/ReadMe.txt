Setup steps:
1. Chose ad networks: UnityAds, AdMob, IronSource, Vungle (at least one). Start from AdMob if you're new with ads.
2. Visit ad provider websites and register, add your app, create ad placements. Get App Key and Placement IDs.
3. Download and import official ad packages for Unity (for chosen networks).
4. Add AdManger to your scene and enable ad networks.
5. Adapter components will be added next to AdManager. Setup App Key and Placement IDs for each adapter.
6. Show ads with ShowInterstitial, ShowRewarded and ShowBanner.
7. Enable [Custom Proguard File] to keep java libs if [Minimize] enabled (proguard-user.txt included, just in case).
8. Check device logs with adb logcat to make sure that everything is fine.

Waterfall setup steps:
1. Enable Use Remote Waterfall in AdManager.
2. Import Remote Config from Package Manager.
3. Open Remote Config and create string keys: AdManager.Waterfall.Interstitial, AdManager.Waterfall.Rewarded, AdManager.Waterfall.Banner.
4. Set comma-separated values "IronSource,AdMob,UnityAds,Vungle" (change priority if needed), press [Push].
5. Run and check logs in Console.

What to do with other networks? Mediation? Bidding?
1. Visit Get started section.
2. Enable AdMob mediation, choose networks (for example, AdColony or AppLovin).
3. Install adapters for selected ad networks from Choose networks section.
4. Run on a device an check logs from AdapterAdMob.

Links:
https://unity.com/products/unity-ads
https://admob.google.com/
https://www.is.com/
https://app.vungle.com/