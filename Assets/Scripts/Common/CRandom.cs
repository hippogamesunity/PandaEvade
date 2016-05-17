using System;
using System.Security.Cryptography;

namespace Assets.Scripts.Common
{
	public static class CRandom
	{
	    private static readonly byte[] Buffer = new byte[1024];
	    private static int _bufferOffset = Buffer.Length;
	    private static readonly RNGCryptoServiceProvider CryptoProvider = new RNGCryptoServiceProvider();

	    public static int GetRandom()
	    {
	        if (_bufferOffset >= Buffer.Length)
	        {
	            FillBuffer();
	        }

	        var val = BitConverter.ToInt32(Buffer, _bufferOffset) & 0x7fffffff;

	        _bufferOffset += sizeof (int);

	        return val;
	    }


        // 0 <= result < maxValue
	    public static int GetRandom(int maxValue)
	    {
	        return GetRandom() % maxValue;
	    }

	    public static int GetRandom(int minValue, int maxValue)
	    {
	        if (maxValue < minValue)
	        {
	            throw new ArgumentOutOfRangeException();
	        }

	        var range = maxValue - minValue;

	        return minValue + GetRandom(range);
	    }

	    /// <summary>
	    /// Chance 0-100
	    /// </summary>
	    public static bool Chance(int chance)
	    {
	        return GetRandom(0, 101) < chance;
	    }

	    /// <summary>
	    /// Chance 0-1f
	    /// </summary>
	    public static bool Chance(float chance)
	    {
	        return Chance((int) (100 * chance));
	    }

        private static void FillBuffer()
        {
            CryptoProvider.GetBytes(Buffer);
            _bufferOffset = 0;
        }
	}
}
