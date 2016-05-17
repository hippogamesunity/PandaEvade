using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using SimpleJSON;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public class ProtectedValue
    {
        public static List<Type> SupportedTypes = new List<Type> { typeof(bool), typeof(long), typeof(double), typeof(string) };
        public static string SessionKey;

        private readonly byte[] _protected;
        private readonly Type _type;
        private static byte[] _key;

        private static byte[] Key
        {
            get { return _key ?? (_key = Encoding.UTF8.GetBytes(SessionKey ?? Md5.Encode(SystemInfo.deviceModel))); }
        }

        private ProtectedValue(byte[] bytes, Type type)
        {
            _protected = Encode(bytes);
            _type = type;
        }

        private static byte[] Copy(byte[] bytes)
        {
            var copy = new byte[bytes.Length];

            Array.Copy(bytes, copy, bytes.Length);

            return copy;
        }

        private static byte[] Encode(byte[] bytes)
        {
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= Key[i % Key.Length];
            }

            return bytes;
        }

        private static byte[] EncodeCopy(byte[] bytes)
        {
            return Encode(Copy(bytes));
        }

        #region Implicit

        public static implicit operator ProtectedValue(bool value)
        {
            return new ProtectedValue(BitConverter.GetBytes(value), value.GetType());
        }

        public static implicit operator ProtectedValue(int value)
        {
            return (long) value;
        }

        public static implicit operator ProtectedValue(long value)
        {
            return new ProtectedValue(BitConverter.GetBytes(value), value.GetType());
        }

        public static implicit operator ProtectedValue(string value)
        {
            return new ProtectedValue(Encoding.UTF8.GetBytes(value), value.GetType());
        }

        public static implicit operator ProtectedValue(DateTime value)
        {
            return new ProtectedValue(BitConverter.GetBytes(value.Ticks), value.Ticks.GetType());
        }

        public static implicit operator ProtectedValue(float value)
        {
            return (double) value;
        }

        public static implicit operator ProtectedValue(double value)
        {
            return new ProtectedValue(BitConverter.GetBytes(value), value.GetType());
        }

        #endregion

        #region Types

        public bool Bool
        {
            get { return BitConverter.ToBoolean(EncodeCopy(_protected), 0); }
        }

        public int Int
        {
            get { return (int) Long; }
        }

        public long Long
        {
            get { return _type == typeof(long) ? BitConverter.ToInt64(EncodeCopy(_protected), 0) : _type == typeof(double) ? (long) Double : GetUnknownType(); }
        }

        public float Float
        {
            get { return (float) Double; }
        }

        public double Double
        {
            get { return _type == typeof(double) ? BitConverter.ToDouble(EncodeCopy(_protected), 0) : _type == typeof(long) ? Long : GetUnknownType(); }
        }

        private long GetUnknownType()
        {
            throw new NotImplementedException(_type + " " + String);
        }

        public string String
        {
            get { return Encoding.UTF8.GetString(EncodeCopy(_protected)); }
        }

        public DateTime DateTime
        {
            get { return new DateTime(Long); }
        }

        #endregion

        #region JSON

        public JSONNode ToJson()
        {
            var array = new JSONArray();

            array.Add(ToString());
            array.Add(SupportedTypes.IndexOf(_type).ToString());

            return array;
        }

        public static ProtectedValue FromJson(JSONNode json)
        {
            var array = json.AsArray;
            var type = SupportedTypes[int.Parse(array[1])];

            if (type == typeof(bool)) return bool.Parse(array[0]);
            if (type == typeof(long)) return long.Parse(array[0]);
            if (type == typeof(double)) return double.Parse(array[0]);
            if (type == typeof(string)) return array[0].Value;

            throw new NotSupportedException(json.ToString());
        }

        #endregion

        #region Common

        public override int GetHashCode()
        {
            return _protected != null ? _protected.GetHashCode() : 0;
        }

        public static bool operator !=(ProtectedValue a, ProtectedValue b)
        {
            return !(a == b);
        }

        public static bool operator ==(ProtectedValue a, ProtectedValue b)
        {
            if ((ReferenceEquals(null, a) && !ReferenceEquals(null, b)) || (!ReferenceEquals(null, a) && ReferenceEquals(null, b)))
            {
                return false;
            }

            if (ReferenceEquals(null, a))
            {
                return true;
            }

            if (a._type == typeof(long) && b._type == typeof(long)) return a.Long == b.Long;
            if (a._type == typeof(double) && b._type == typeof(double)) return a.Double == b.Double;
            if (a._type == typeof(long) && b._type == typeof(double)) return a.Long == b.Double;
            if (a._type == typeof(double) && b._type == typeof(long)) return a.Double == b.Long;

            return a._protected.SequenceEqual(b._protected);
        }

        public static ProtectedValue operator ++(ProtectedValue value)
        {
            return value + 1;
        }

        public static ProtectedValue operator --(ProtectedValue value)
        {
            return value - 1;
        }

        public static ProtectedValue operator +(ProtectedValue a, ProtectedValue b)
        {
            if (a._type == typeof(long) && b._type == typeof(long)) return a.Long + b.Long;
            if (a._type == typeof(double) && b._type == typeof(double)) return a.Double + b.Double;
            if (a._type == typeof(long) && b._type == typeof(double)) return a.Long + b.Double;
            if (a._type == typeof(double) && b._type == typeof(long)) return a.Double + b.Long;
            
            throw new NotImplementedException(a._type + " + " + b._type);
        }

        public static ProtectedValue operator -(ProtectedValue a, ProtectedValue b)
        {
            if (a._type == typeof(long) && b._type == typeof(long)) return a.Long - b.Long;
            if (a._type == typeof(double) && b._type == typeof(double)) return a.Double - b.Double;
            if (a._type == typeof(long) && b._type == typeof(double)) return a.Long - b.Double;
            if (a._type == typeof(double) && b._type == typeof(long)) return a.Double - b.Long;

            throw new NotImplementedException(a._type + " - " + b._type);
        }

        public static ProtectedValue operator *(ProtectedValue a, ProtectedValue b)
        {
            if (a._type == typeof(long) && b._type == typeof(long)) return a.Long * b.Long;
            if (a._type == typeof(double) && b._type == typeof(double)) return a.Double * b.Double;
            if (a._type == typeof(long) && b._type == typeof(double)) return a.Long * b.Double;
            if (a._type == typeof(double) && b._type == typeof(long)) return a.Double * b.Long;

            throw new NotImplementedException(a._type + " * " + b._type);
        }

        public static ProtectedValue operator /(ProtectedValue a, ProtectedValue b)
        {
            if (a._type == typeof(long) && b._type == typeof(long)) return a.Long / b.Long;
            if (a._type == typeof(double) && b._type == typeof(double)) return a.Double / b.Double;
            if (a._type == typeof(long) && b._type == typeof(double)) return a.Long / b.Double;
            if (a._type == typeof(double) && b._type == typeof(long)) return a.Double / b.Long;

            throw new NotImplementedException(a._type + " / " + b._type);
        }

        public bool Equals(ProtectedValue other)
        {
            return this == other;
        }

        public static bool operator >(ProtectedValue a, ProtectedValue b)
        {
            if (a._type == typeof(long) && b._type == typeof(long)) return a.Long > b.Long;
            if (a._type == typeof(double) && b._type == typeof(double)) return a.Double > b.Double;
            if (a._type == typeof(long) && b._type == typeof(double)) return a.Long > b.Double;
            if (a._type == typeof(double) && b._type == typeof(long)) return a.Double > b.Long;

            throw new NotImplementedException(a._type + " / " + b._type);
        }

        public static bool operator <(ProtectedValue a, ProtectedValue b)
        {
            if (a._type == typeof(long) && b._type == typeof(long)) return a.Long < b.Long;
            if (a._type == typeof(double) && b._type == typeof(double)) return a.Double < b.Double;
            if (a._type == typeof(long) && b._type == typeof(double)) return a.Long < b.Double;
            if (a._type == typeof(double) && b._type == typeof(long)) return a.Double < b.Long;

            throw new NotImplementedException(a._type + " < " + b._type);
        }

        public static bool operator >(ProtectedValue a, double b)
        {
            if (a._type == typeof(long)) return a.Long > b;
            if (a._type == typeof(double)) return a.Double > b;

            throw new NotImplementedException(a._type + " / " + b.GetType());
        }

        public static bool operator <(ProtectedValue a, double b)
        {
            if (a._type == typeof(long)) return a.Long < b;
            if (a._type == typeof(double)) return a.Double < b;

            throw new NotImplementedException(a._type + " < " + b.GetType());
        }

        public static bool operator >=(ProtectedValue a, ProtectedValue b)
        {
            return a > b || a == b;
        }

        public static bool operator <=(ProtectedValue a, ProtectedValue b)
        {
            return a < b || a == b;
        }

        public static bool operator >=(ProtectedValue a, double b)
        {
            return a > b || a == b;
        }

        public static bool operator <=(ProtectedValue a, double b)
        {
            return a < b || a == b;
        }

        public override bool Equals(object value)
        {
            if (ReferenceEquals(null, value)) return false;

            return value as ProtectedValue == this;
        }

        public ProtectedValue Copy()
        {
            return new ProtectedValue(EncodeCopy(_protected), _type);
        }

        public override string ToString()
        {
            if (_type == typeof(bool)) return Bool.ToString();
            if (_type == typeof(long)) return Long.ToString();
            if (_type == typeof(double)) return Double.ToString(CultureInfo.InvariantCulture);
            if (_type == typeof(string)) return String;

            throw new NotImplementedException(_type.ToString());
        }

        #endregion
    }
}