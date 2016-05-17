using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Scripts
{
    public static class ColorHelper
    {
        public static Color GetColor(string hex)
        {
            const string pattern = "^#{0,1}(?<value>[A-Fa-f0-9]{6})$";

            hex = Regex.Match(hex, pattern).Groups["value"].Value;

            var r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            return new Color32(r, g, b, 255);
        }

        public static Color GetColor(int r, int g, int b)
        {
            return new Color(r / 255f, g / 255f, b / 255f, 1);
        }

        public static Color GetColor(int r, int g, int b, int a)
        {
            return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        }
    }
}
