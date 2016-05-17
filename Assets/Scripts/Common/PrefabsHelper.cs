using System;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public static class PrefabsHelper
    {
        public static GameObject Instantiate(string name, Transform parent)
        {
            if (name == "Buildings/Lighthouse/Lighthouse2A") name = "Buildings/Lighthouse/Lighthouse1A";

            try
            {
                var instance = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("Prefabs/" + name, typeof(GameObject)));

                instance.name = name;
                instance.transform.parent = parent;
                instance.transform.localScale = Vector3.one;

                return instance;
            }
            catch
            {
                throw new Exception("Prefab not found: " + name);
            }
        }
    }
}