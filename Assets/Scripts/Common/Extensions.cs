using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public static class Extensions
    {
        public static string ToPriceLong(this long value)
        {
            return string.Format("{0,2:N0}", value).Replace(",", ".").Replace(" ", "");
        }

        public static void SetText(this UILabel label, string text, params object[] args)
        {
            if (label == null)
            {
                throw new Exception("UILabel is not assigned!");
            }

            if (string.IsNullOrEmpty(text))
            {
                label.text = null;

                return;
            }

            if (args != null && args.Length > 0)
            {
                try
                {
                    text = string.Format(text, args);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    text = e.Message;
                }
            }

            if (text.Contains("[-]"))
            {
                text = text.Replace("[grey]", "[505050]");
                text = text.Replace("[red]", "[c83232]");
                text = text.Replace("[blue]", "[006464]");
                text = text.Replace("[green]", "[009600]");
                text = text.Replace("[violet]", "[c832c8]");
                text = text.Replace("[yellow]", "[ffff00]");
                text = text.Replace("[orange]", "[ff5000]");
            }

            text = text.Replace("[newline]", "\n");
            label.text = text;
        }

        public static void SetText(this UILabel label, float value)
        {
            label.SetText(Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        public static void SetTextLong(this UILabel label, double value)
        {
            label.SetText(Convert.ToString(Math.Floor(value), CultureInfo.InvariantCulture));
        }

        public static void SetLocalizedText(this UILabel label, string text, params object[] args)
        {
            if (string.IsNullOrEmpty(text))
            {
                label.text = null;

                return;
            }

            foreach (Match match in new Regex(@"%[\w\.]+?%").Matches(text))
            {
                var localization = Localization.Get(match.Value);

                text = text.Replace(match.Value, localization == match.Value ? "" : localization);
            }

            label.SetText(text, args);
        }

        public static void SetLocalizedText(this UILabel label, float value)
        {
            label.SetLocalizedText(Convert.ToString(value));
        }

        public static bool HasComponent<T>(this GameObject component) where  T : Component
        {
            return component.GetComponent<T>() != null;
        }

        public static long RoundToLong(this float value)
        {
            try
            {
                checked
                {
                    return (long) Math.Round(value);
                }
            }
            catch (OverflowException)
            {
                return long.MaxValue;
            }
        }

        public static long RoundToLong(this double value)
        {
            try
            {
                checked
                {
                    return (long) Math.Round(value);
                }
            }
            catch (OverflowException)
            {
                return long.MaxValue;
            }
        }

        public static T ToEnum<T>(this string value)
        {
            return (T) Enum.Parse(typeof(T), value);
        }

        public static long SafeSum(this long value, long add)
        {
            try
            {
                checked
                {
                    return value + add;
                }
            }
            catch (OverflowException)
            {
                return long.MaxValue;
            }
        }

        public static List<T> Shuffle<T>(this List<T> source)
        {
            source.Sort((i, j) => CRandom.GetRandom(3) - 1);

            return source;
        }

        public static string ToBitString(this BitArray bits)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < bits.Count; i++)
            {
                sb.Append(bits[i] ? '1' : '0');
            }

            return sb.ToString();
        }

        public static void Clear(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }
        }

        public static string LocalizedFormat(this string key, params object[] args)
        {
            return string.Format(Localization.Get(key), args);
        }
    }
}