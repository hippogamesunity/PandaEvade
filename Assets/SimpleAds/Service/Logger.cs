using UnityEngine;

namespace Assets.SimpleAds.Service
{
    public abstract class Logger : MonoBehaviour
    {
        public void Event(string eventName)
        {
            eventName = $"{GetType().Name}.{eventName}";
            // TODO: Add logging/analytic here.
            Debug.Log(eventName);
        }

        public void Event(string eventName, string paramName, object paramValue)
        {
            eventName = $"{GetType().Name}.{eventName}";
            // TODO: Add logging/analytic here.
            Debug.Log($"{eventName}: {paramName}={paramValue}");
        }

        public void Event(string eventName, string paramName1, object paramValue1, string paramName2, object paramValue2)
        {
            eventName = $"{GetType().Name}.{eventName}";
            // TODO: Add logging/analytic here.
            Debug.Log($"{eventName}: {paramName1}={paramValue1}, {paramName2}={paramValue2}");
        }
    }
}