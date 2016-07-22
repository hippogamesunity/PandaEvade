using System;
using System.Linq;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Localizer
    {
        private static readonly string[] Dictionaries =
        {
            "Common"
        };

        public static void Initialize()
        {
            var bytes = Dictionaries.Select(i => Resources.Load<TextAsset>(i).bytes).ToArray();

            Localization.LoadCSV(ByteHelper.Join(bytes));

            switch (Application.systemLanguage)
            {
                case SystemLanguage.Russian:
                case SystemLanguage.Ukrainian:
                case SystemLanguage.Belarusian:
                    Localization.language = "Russian";
                    break;
                default:
                    Localization.language = "English";
                    break;
            }
        }

        public static string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Localization key can't be null.");
            }

            var value = Localization.Get(key);

            if (string.IsNullOrEmpty(value))
            {
                throw new Exception(string.Format("Localization not found: {0}.", key));
            }

            return value;
        }
    }
}