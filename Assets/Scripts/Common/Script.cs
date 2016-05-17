using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public abstract class Script : MonoBehaviour
    {
        public readonly int Id = GetId();
        public static readonly Dictionary<Type, object> ScriptCache = new Dictionary<Type, object>();
  
        public T Get<T>() where T : MonoBehaviour
        {
            return GetComponent<T>();
        }

        public static T Find<T>() where T : MonoBehaviour
        {
            return FindObjectOfType<T>();
        }

        public static T FindSingle<T>() where T : MonoBehaviour
        {
            var type = typeof(T);

            if (!ScriptCache.ContainsKey(type))
            {
                ScriptCache.Add(type, Find<T>());
            }

            return (T) ScriptCache[type];
        }

        private static int _id = 55555;

        private static int GetId()
        {
            return ++_id;
        }
    }
}