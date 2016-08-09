namespace Assets.Scripts
{
    public static class PlanformDependedSettings
    {

        #if UNITY_ANDROID || UNITY_WEBGL || UNITY_WEBPLAYER || UNITY_EDITOR

        public const string StoreLink = "https://play.google.com/store/apps/details?id=com.ZenBen&referrer=utm_source%3Drate";
                
        #elif UNITY_IPHONE

		public const string StoreLink = "itms-apps://itunes.apple.com/app/id1137708020";
		
        #endif
		
		public const string GooglePlayShortLinkVK = "https://goo.gl/gJaq4l";
		public const string AppStoreShortLinkVK = "https://goo.gl/upNDrZ";
    }
}