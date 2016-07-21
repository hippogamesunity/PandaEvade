using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using SimpleJSON;
using UnityEngine;

namespace Assets.Scripts
{
    public class Profile
    {
        public ProtectedValue BestScore;
        public ProtectedValue Sound;
        public List<BallId> UnlockedItems; 

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
            var unlockedItems = new JSONArray();

            foreach (var item in UnlockedItems)
            {
                unlockedItems.Add(((int) item).ToString());
            }

            return new JSONClass
            {
                { "BestScore", BestScore.ToJson() },
                { "Sound", Sound.ToJson() },
                { "UnlockedItems", unlockedItems }
            };
        }

        private static Profile FromJson(JSONNode json)
        {
            var profile = new Profile
            {
                BestScore = ProtectedValue.FromJson(json["BestScore"]),
                Sound = ProtectedValue.FromJson(json["Sound"]),
                UnlockedItems = json["UnlockedItems"].Childs.Select(i => i.Value.ToEnum<BallId>()).ToList()
            };

            return profile;
        }
    }
}