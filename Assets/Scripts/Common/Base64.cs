using System;
using System.Text;

namespace Assets.Scripts.Common
{
	public static class Base64
    {
        public static string Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);

            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string WeakEncrypt(string value)
        {
            var base64 = Encode(value);
            var chars = base64.ToCharArray();

            for (var i = 1; i < chars.Length; i += 2)
            {
                var c = chars[i];

                chars[i] = chars[i - 1];
                chars[i - 1] = c;
            }

            return new string(chars);
        }
    }
}