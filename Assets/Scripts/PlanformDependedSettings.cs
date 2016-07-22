namespace Assets.Scripts
{
    public static class PlanformDependedSettings
    {

        #if UNITY_ANDROID || UNITY_WEBGL || UNITY_WEBPLAYER || UNITY_EDITOR

        public const string StoreLink = "https://play.google.com/store/apps/details?id=com.ZenBen&referrer=utm_source%3Drate";
        public const string StoreShortLinkVKPromo = "https://goo.gl/gJaq4l";
        
        #elif UNITY_IPHONE

        public const string StorePublicKey = "47b2af86289b4b4d8bd4887348716dc6";
        public const string StoreLink = "itms-apps://itunes.apple.com/app/id1097114930";
        public const string StoreShortLink = "";

        #endif
    }
}