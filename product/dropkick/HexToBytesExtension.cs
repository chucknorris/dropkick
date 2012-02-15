using System;
using System.Globalization;
using System.Text;

namespace dropkick
{
    public static class HexToBytesExtension
    {
        public static string ToHex(this byte[] bytes)
        {
            var s = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                s.Append(b.ToString("x2"));
            return s.ToString();
        }

        public static byte[] FromHexToBytes(this string hex)
        {
            var chars = hex.ToCharArray();
            var bytes = new byte[hex.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                var strIdx = i * 2;
                var s = new string(chars, strIdx, 2);
                System.Diagnostics.Debug.WriteLine(s);
                bytes[i] = Byte.Parse(s, NumberStyles.HexNumber);
            }
            return bytes;
        }
    }
}
