using System;
using System.Collections;
using System.Text.RegularExpressions;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts
{
    public class VK : Script
    {
        private const string AuthorizePattern = "https://oauth.vk.com/authorize?client_id=CLIENT_ID&scope=SCOPE&redirect_uri=https://oauth.vk.com/blank.html&response_type=token&display=touch";
        private const string ApiPattern = "https://api.vk.com/method/METHOD_NAME?PARAMETERS&access_token=ACCESS_TOKEN";
	
        public void Authorize(string appId, string scope, Action<bool, string, string> callback)
        {
            #if UNITY_ANDROID || UNITY_IPHONE

            var browser = gameObject.AddComponent<UniWebView>();
            var authorizeLink = AuthorizePattern.Replace("CLIENT_ID", appId).Replace("SCOPE", scope);

            Debug.Log("VK: authorizeLink = " + authorizeLink);

            browser.Load(authorizeLink);
            browser.insets.left = browser.insets.right = 50;
            browser.insets.top = browser.insets.bottom = 100;
            browser.Show();
            browser.ShowToolBar(true);
            browser.OnLoadComplete += (view, success, message) =>
            {
                Debug.Log("VK: OnLoadComplete, view.currentUrl = " + view.currentUrl);

                var errorToken = new Regex(@"error=(?<error>\w+)");

                if (errorToken.IsMatch(view.currentUrl))
                {
                    browser.Hide();
                    Destroy(browser);
                    callback(false, null, null);
                }
                else
                {
                    var accessTokenRegex = new Regex(@"access_token=(?<access_token>\w+)");

                    if (accessTokenRegex.IsMatch(view.currentUrl))
                    {
                        var userIdRegex = new Regex(@"user_id=(?<user_id>\w+)");
                        var accessToken = accessTokenRegex.Match(view.currentUrl).Groups["access_token"].Value;
                        var userId = userIdRegex.Match(view.currentUrl).Groups["user_id"].Value;

                        browser.Hide();
                        Destroy(browser);
                        callback(true, accessToken, userId);
                    }
                }
            };

            #endif
        }

        public void Execute(string methodName, string accessToken, Hashtable parameters, Action<string> callback)
        {
            StartCoroutine(ExecuteCoroutine(methodName, accessToken, parameters, callback));
        }

        private static IEnumerator ExecuteCoroutine(string methodName, string accessToken, IDictionary parameters, Action<string> callback)
        {
            var url = ApiPattern.Replace("METHOD_NAME", methodName);
            var paramValues = "";
		
            foreach (var item in parameters.Keys)
            {
                paramValues += string.Format("{0}={1}&", item, WWW.EscapeURL(parameters[item].ToString()));
            }
		
            url = url.Replace("PARAMETERS", paramValues);
            url = url.Replace("ACCESS_TOKEN", accessToken);

            Debug.Log("VK: api request = " + url);
		
            var www = new WWW (url);

            yield return www;

            Debug.Log ("VK: www.text = " + www.text);

            if (callback != null)
            {
                callback(www.text);
            }
        }
    }
}