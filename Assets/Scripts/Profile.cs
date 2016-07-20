using Assets.Scripts.Common;
using SimpleJSON;
using UnityEngine;

namespace Assets.Scripts
{
    public class Profile
    {
        public ProtectedValue BestScore;
        public ProtectedValue Sound;

        private static Profile _instance;
        private const string ProfileKey = "T";

        private Profile()
        {
        }

        public static Profile Instance
        {
            get { return _instance ?? Load(); }
        }

        public static Profile Load()
        {
            _instance = PlayerPrefs.HasKey(ProfileKey) ? FromJson(JSONNode.LoadFromCompressedBase64(PlayerPrefs.GetString(ProfileKey))) : DefaultProfile;

            return _instance;
        }

        private static Profile DefaultProfile
        {
            get
            {
                return new Profile
                {
                    BestScore = 0,
                    Sound = true
                };
            }
        }

        public void Save()
        {
            PlayerPrefs.SetString(ProfileKey, ToJson().SaveToCompressedBase64());
            PlayerPrefs.Save();
        }

        private JSONNode ToJson()
        {
            return new JSONClass
            {
                { "BestScore", BestScore.ToJson() },
                { "Sound", Sound.ToJson() }
            };
        }

        private static Profile FromJson(JSONNode json)
        {
            var profile = new Profile
            {
                BestScore = ProtectedValue.FromJson(json["BestScore"]),
                Sound = ProtectedValue.FromJson(json["Sound"])
            };

            return profile;
        }
    }
}