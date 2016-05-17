using System;
using SimpleJSON;

namespace Assets.Scripts.Common
{
    public class ProtectedValueStrong
    {
        private readonly string _protected = B64R.Encode("0");

        private ProtectedValueStrong(string value)
        {
            _protected = value;
        }

        public ProtectedValueStrong(object value)
        {
            _protected = B64R.Encode(Convert.ToString(value));
        }

        #region Implicit

        public static implicit operator ProtectedValueStrong(bool value)
        {
            return new ProtectedValueStrong(value);
        }

        public static implicit operator ProtectedValueStrong(int value)
        {
            return new ProtectedValueStrong(value);
        }

        public static implicit operator ProtectedValueStrong(long value)
        {
            return new ProtectedValueStrong(value);
        }

        public static implicit operator ProtectedValueStrong(string value)
        {
            return new ProtectedValueStrong(B64R.Encode(value));
        }

        public static implicit operator ProtectedValueStrong(DateTime value)
        {
            return new ProtectedValueStrong(value.Ticks);
        }

        public static implicit operator ProtectedValueStrong(float value)
        {
            return new ProtectedValueStrong(value);
        }

        public static implicit operator ProtectedValueStrong(double value)
        {
            return new ProtectedValueStrong(value);
        }

        #endregion

        #region Types

        public double DefaultValue
        {
            get
            {
                return double.Parse(B64R.Decode(_protected));
            }
        }

        public bool Bool
        {
            get { return bool.Parse(B64R.Decode(_protected)); }
        }

        public int Int
        {
            get { return int.Parse(B64R.Decode(_protected)); }
        }

        public long Long
        {
			get { return long.Parse(B64R.Decode(_protected)); }
        }

        public float Float
        {
            get { return float.Parse(B64R.Decode(_protected)); }
        }

        public double Double
        {
            get { return double.Parse(B64R.Decode(_protected)); }
        }

        public string String
        {
            get { return B64R.Decode(_protected); }
        }

        public DateTime DateTime
        {
            get { return new DateTime(long.Parse(B64R.Decode(_protected))); }
        }

        #endregion

        #region JSON

        public JSONData ToJson()
        {
            return new JSONData(_protected);
        }

        public static ProtectedValueStrong FromJson(JSONNode json)
        {
            return new ProtectedValueStrong(json.Value);
        }

        #endregion

        #region Common

        public override int GetHashCode()
        {
            return _protected != null ? _protected.GetHashCode() : 0;
        }

        public static bool operator !=(ProtectedValueStrong a, ProtectedValueStrong b)
        {
            return !(a == b);
        }

        public static bool operator ==(ProtectedValueStrong a, ProtectedValueStrong b)
        {
            if ((ReferenceEquals(null, a) && !ReferenceEquals(null, b)) || (!ReferenceEquals(null, a) && ReferenceEquals(null, b)))
            {
                return false;
            }

            if (ReferenceEquals(null, a))
            {
                return true;
            }

            return a._protected == b._protected;
        }

        public static ProtectedValueStrong operator ++(ProtectedValueStrong value)
        {
            return value.DefaultValue + 1;
        }

        public static ProtectedValueStrong operator --(ProtectedValueStrong value)
        {
            return value.DefaultValue - 1;
        }

        public static ProtectedValueStrong operator +(ProtectedValueStrong a, ProtectedValueStrong b)
        {
            return a.DefaultValue + b.DefaultValue;
        }

        public static ProtectedValueStrong operator -(ProtectedValueStrong a, ProtectedValueStrong b)
        {
            return a.DefaultValue - b.DefaultValue;
        }

        public static ProtectedValueStrong operator *(ProtectedValueStrong a, ProtectedValueStrong b)
        {
            return a.DefaultValue * b.DefaultValue;
        }

        public static ProtectedValueStrong operator /(ProtectedValueStrong a, ProtectedValueStrong b)
        {
            return a.DefaultValue / b.DefaultValue;
        }

        public bool Equals(ProtectedValueStrong other)
        {
            return this == other;
        }

        public static bool operator >(ProtectedValueStrong a, ProtectedValueStrong b)
        {
            return a.DefaultValue > b.DefaultValue;
        }

        public static bool operator <(ProtectedValueStrong a, ProtectedValueStrong b)
        {
            return a.DefaultValue < b.DefaultValue;
        }

        public static bool operator >=(ProtectedValueStrong a, ProtectedValueStrong b)
        {
            return a.DefaultValue >= b.DefaultValue;
        }

        public static bool operator <=(ProtectedValueStrong a, ProtectedValueStrong b)
        {
            return a.DefaultValue <= b.DefaultValue;
        }

        public override bool Equals(object value)
        {
            if (ReferenceEquals(null, value)) return false;

            return value as ProtectedValueStrong == this;
        }

        public ProtectedValueStrong Copy()
        {
            return new ProtectedValueStrong(_protected);
        }

        public override string ToString()
        {
            return String;
        }

        public long Round()
        {
            return (long) Math.Floor(Double);
        }

        #endregion
    }
}