using System;
using System.Text;

namespace Assets.Scripts.Common
{
    /// <summary>
    /// Simple and fast Base64 XOR encoding with dynamic key (generated on each app run). Use for data protection in RAM. Do NOT use for data storing outside RAM. Do NOT use for secure data encryption.
    /// </summary>
    public class B64X
    {
        public static byte[] Key = Guid.NewGuid().ToByteArray();

        public static string Encode(string value)
        {
            return Convert.ToBase64String(Encode(Encoding.UTF8.GetBytes(value), Key));
        }

        public static string Decode(string value)
        {
            return Encoding.UTF8.GetString(Encode(Convert.FromBase64String(value), Key));
        }

        public static string Encrypt(string value, string key)
        {
            return Convert.ToBase64String(Encode(Encoding.UTF8.GetBytes(value), Encoding.UTF8.GetBytes(key)));
        }

        public static string Decrypt(string value, string key)
        {
            return Encoding.UTF8.GetString(Encode(Convert.FromBase64String(value), Encoding.UTF8.GetBytes(key)));
        }

        private static byte[] Encode(byte[] bytes, byte[] key)
        {
            var j = 0;

            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= key[j];

                if (++j == key.Length)
                {
                    j = 0;
                }
            }

            return bytes;
        }
    }
}