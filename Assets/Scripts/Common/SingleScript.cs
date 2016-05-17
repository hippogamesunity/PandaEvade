namespace Assets.Scripts.Common
{
    public abstract class SingleScript<T> : Script where T : Script // TODO: Doesn't work for web builds
    {
        private static T _instance;

        public static T Instance
        {
            get { return _instance ?? (_instance = FindObjectOfType<T>()); }
        }
    }
}