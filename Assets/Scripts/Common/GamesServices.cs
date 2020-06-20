using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Assets.Scripts.Common
{
    public static class GamesServices
    {
        public static bool EnableLog = true;
        public static bool Busy { get; private set; }

        public static void PostScores(Dictionary<string, long> scores)
        {
            PostScores(scores, ((success, exception) => { }));
        }

        public static void PostScores(Dictionary<string, long> scores, Action<bool, Exception> callback)
        {
            AuthorizedAction(() => ReportScores(scores, callback), callback);
        }

        public static void PostAchievements(List<string> achievements)
        {
            PostAchievements(achievements, ((success, exception) => { }));
        }

        public static void PostAchievements(List<string> achievements, Action<bool, Exception> callback)
        {
            AuthorizedAction(() => UnlockAchievements(achievements, callback), callback);
        }

        public static void LoadAchievements(Action<bool, IAchievement[], Exception> callback)
        {
            AuthorizedAction(() => { Social.Active.LoadAchievements(achievements => Complete((success, exception) => callback(true, achievements, null))); }, (success, exception) => callback(success, null, exception));
        }

        public static void AuthorizedAction(Action action)
        {
            AuthorizedAction(() => { action(); Busy = false; }, null);
        }

        private static void ReportScores(Dictionary<string, long> scores, Action<bool, Exception> callback)
        {
            if (scores.Count == 0)
            {
                Complete(callback);
            }
            else
            {
                var first = scores.First();

                Social.Active.ReportScore(first.Value, first.Key, success =>
                {
                    if (success)
                    {
                        scores.Remove(first.Key);
                        ReportScores(scores, callback);
                    }
                    else
                    {
                        Break("Social.Active.ReportScore failed", callback);
                    }
                });
            }
        }

        private static void UnlockAchievements(List<string> achievements, Action<bool, Exception> callback)
        {
            if (achievements.Count == 0)
            {
                Complete(callback);
            }
            else
            {
                Social.Active.ReportProgress(achievements[0], 100, success =>
                {
                    if (success)
                    {
                        achievements.Remove(achievements[0]);
                        UnlockAchievements(achievements, callback);
                    }
                    else
                    {
                        Break("Social.Active.ReportProgress failed", callback);
                    }
                });
            }
        }

        private static void AuthorizedAction(Action action, Action<bool, Exception> callback)
        {
            if (Busy)
            {
                WriteLog("busy...");
                return;
            }

            if (callback == null) callback = (success, exception) => { };

            Busy = true;

            if (Social.localUser.authenticated)
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Break(e.Message, callback);
                }
            }
            else
            {
                WriteLog("authentication...");

                #if UNITY_ANDROID

                var config = new GooglePlayGames.BasicApi.PlayGamesClientConfiguration.Builder().Build();

                GooglePlayGames.PlayGamesPlatform.InitializeInstance(config);
                GooglePlayGames.PlayGamesPlatform.DebugLogEnabled = true;
                GooglePlayGames.PlayGamesPlatform.Activate();

                #endif

                Social.localUser.Authenticate(authenticated =>
                {
                    if (authenticated)
                    {
                        try
                        {
                            action();
                        }
                        catch (Exception e)
                        {
                            Break(e.Message, callback);
                        }
                    }
                    else
                    {
                        Break("Social.localUser.Authenticate failed", callback);
                    }
                });
            }
        }

        private static void Complete(Action action, Action<bool, Exception> callback)
        {
            action();
            Busy = false;
            callback(true, null);
        }

        private static void Complete(Action<bool, Exception> callback)
        {
            Busy = false;
            callback(true, null);
        }

        private static void Break(string error, Action<bool, Exception> callback)
        {
            WriteLog(error);
            Busy = false;
            callback(false, new Exception(error));
        }

        private static void WriteLog(string message, params object[] args)
        {
            if (EnableLog)
            {
                Debug.Log(string.Format("{0}: {1}", typeof(GamesServices).Name, string.Format(message, args)));
            }
        }
    }
}