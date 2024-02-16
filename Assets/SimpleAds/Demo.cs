using UnityEngine;
using UnityEngine.UI;

namespace Assets.SimpleAds
{
    public class Demo : MonoBehaviour
    {
        public AdManager AdManager;

        public Image StatusInterstitial;
        public Image StatusRewarded;
        public Image StatusBanner;

        public void Start()
        {
            InvokeRepeating("UpdateStatus", 1, 1);
        }

        public void UpdateStatus()
        {
            StatusInterstitial.color = AdManager.IsReadyInterstitial() ? Color.green : Color.red;
            StatusRewarded.color = AdManager.IsReadyRewarded() ? Color.green : Color.red;
            StatusBanner.color = AdManager.IsReadyBanner() ? Color.green : Color.red;
        }

        public void ShowInterstitial()
        {
            if (AdManager.IsReadyInterstitial())
            {
                AdManager.ShowInterstitial();
            }
            else
            {
                Debug.LogWarning("Not ready.");
            }
        }

        public void ShowRewarded()
        {
            if (AdManager.IsReadyRewarded())
            {
                //AdManager.ShowRewarded(() => Debug.Log("Reward callback!"));
                AdManager.ShowRewarded((provideName, reward, amount) => Debug.Log("Reward callback!"));
            }
            else
            {
                Debug.LogWarning("Not ready.");
            }
        }

        public void ShowBanner()
        {
            if (AdManager.IsReadyBanner())
            {
                AdManager.ShowBanner();
            }
            else
            {
                Debug.LogWarning("Not ready.");
            }
        }
    }
}