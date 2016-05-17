using System;

namespace Assets.Scripts.Common
{
    public static class ByteHelper
    {
        public static byte[] Join(params byte[][] arrays)
        {
            var length = 0;

            foreach (var array in arrays)
            {
                length += array.Length;
            }

            var rv = new byte[length];
            var offset = 0;

            foreach (var array in arrays)
            {
                Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }

            return rv;
        }
    }
}