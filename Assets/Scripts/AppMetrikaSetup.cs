using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteInEditMode]
    public class AppMetrikaSetup : MonoBehaviour
    {
        public string APIKeyGooglePlay;
        public string APIKeyAppStore;

        public void OnValidate()
        {
            #if UNITY_ANDROID

            FindObjectOfType<AppMetrica>().APIKey = APIKeyGooglePlay;

            #elif UNITY_IPHONE

            FindObjectOfType<AppMetrica>().APIKey = APIKeyAppStore;

            #endif
        }
    }
}