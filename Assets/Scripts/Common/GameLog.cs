using System;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public static class GameLog
    {
        private const string LogKey = "GameLog";

        public static void Write(string message)
        {
            Debug.Log(message);

            if (PlayerPrefs.HasKey(LogKey) && PlayerPrefs.GetString(LogKey).Length > 1024 * 1024)
            {
                PlayerPrefs.SetString(LogKey, "");
            }

            PlayerPrefs.SetString(LogKey, string.Format("[{0}] {1}{2}", DateTime.Now, message, Environment.NewLine));
        }

        public static void Write(Exception exception)
        {
            Write(exception.ToString());
        }

        public static string Read()
        {
            return PlayerPrefs.GetString(LogKey);
        }
    }
}