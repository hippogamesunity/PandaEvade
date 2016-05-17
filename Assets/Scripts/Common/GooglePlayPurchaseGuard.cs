using System;
using System.Security.Cryptography;

namespace Assets.Scripts.Common
{
    public static class GooglePlayPurchaseGuard
    {
        /// <summary>
        /// Verify Google Play purchase. Protect you app against hack via Freedom. More info: http://mrtn.me/blog/2012/11/15/checking-google-play-signatures-on-net/
        /// </summary>
        /// <param name="purchaseJson">Purchase JSON string</param>
        /// <param name="base64Signature">Purchase signature string</param>
        /// <param name="xmlPublicKey">XML public key. Use http://superdry.apphb.com/tools/online-rsa-key-converter to convert RSA public key from Developer Console</param>
        /// <returns></returns>
        public static bool Verify(string purchaseJson, string base64Signature, string xmlPublicKey)
        {
            using (var provider = new RSACryptoServiceProvider())
            {
                try
                {
                    provider.FromXmlString(xmlPublicKey);

                    var signature = Convert.FromBase64String(base64Signature);
                    var sha = new SHA1Managed();
                    var data = System.Text.Encoding.UTF8.GetBytes(purchaseJson);

                    return provider.VerifyData(data, sha, signature);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log(e);
                }

                return false;
            }
        }
    }
}