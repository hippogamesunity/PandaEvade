using System.Collections.Generic;

namespace Assets.Scripts.Common
{
    /// <summary>
    /// Simple and fast Base64 encoding algorithm with byte reverse. Use for data protection in RAM. Use for unsafe data storing outside RAM. Do NOT use for secure data encryption.
    /// </summary>
    public class B64R
    {
        private static bool _debug = true;

        public static string Encode(string value)
        {
            if (_debug)
            {
                return value;
            }

            var base64 = Base64.Encode(value);
            var chars = base64.ToCharArray();

            Reverse(chars);

            return new string(chars);
        }

        public static string Decode(string value)
        {
            if (_debug)
            {
                return value;
            }

            var chars = value.ToCharArray();

            Reverse(chars);

            return Base64.Decode(new string(chars));
        }

        private static void Reverse(IList<char> chars)
        {
            for (var i = 1; i < chars.Count; i += 2)
            {
                var c = chars[i];

                chars[i] = chars[i - 1];
                chars[i - 1] = c;
            }
        }
    }
}