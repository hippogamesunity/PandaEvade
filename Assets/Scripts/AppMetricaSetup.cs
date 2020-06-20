using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteInEditMode]
    public class AppMetricaSetup : MonoBehaviour
    {
        public string APIKeyGooglePlay;
        public string APIKeyAppStore;

        public void OnValidate()
        {
            #if UNITY_ANDROID

            FindObjectOfType<AppMetrica>().ApiKey = APIKeyGooglePlay;

            #elif UNITY_IPHONE

            FindObjectOfType<AppMetrica>().ApiKey = APIKeyAppStore;

            #endif
        }
    }
}