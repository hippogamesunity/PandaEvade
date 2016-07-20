using System;
using System.Collections;
using Assets.Scripts.Behaviour;
using Assets.Scripts.Common;
using SimpleJSON;
using UnityEngine;

namespace Assets.Scripts
{
    public static class VKontakteWall
    {
        public static event Action OnSuccess = () => { };
        public static event Action<string> OnError = error => Debug.Log(error);

        private static string _message;
        private static string _attachments;

        public static void Post(string appId, string message, string attachments)
        {
            _message = message;
            _attachments = attachments;

            Script.FindSingle<VK>().Authorize(appId, "wall", OnGetAccessToken);
        }

        private static void OnGetAccessToken(bool success, string accessToken, string userId)
        {
            if (success)
            {
                Script.FindSingle<VK>().Execute("wall.post", accessToken, new Hashtable { { "owner_id", userId }, { "message", _message }, { "attachments", _attachments } }, OnWallPost);
            }
            else
            {
                OnError(null);
            }
        }

        private static void OnWallPost(string text)
        {
            var json = JSON.Parse(text);

            if (!string.IsNullOrEmpty(json["error"]))
            {
                OnError(json["error"]["error_msg"]);
            }
            else
            {
                OnSuccess();
            }
        }
    }
}