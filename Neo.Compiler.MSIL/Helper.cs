using System;
using System.Security.Cryptography;
using System.Text;

namespace Neo2.Compiler
{
    public static class Helper
    {
        public static uint ToInteropMethodHash(this string method)
        {
            return ToInteropMethodHash(Encoding.ASCII.GetBytes(method));
        }

        public static uint ToInteropMethodHash(this byte[] method)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return BitConverter.ToUInt32(sha.ComputeHash(method), 0);
            }
        }
        public static byte[] HexString2Bytes(string str)
        {
            if (str.IndexOf("0x") == 0)
                str = str.Substring(2);
            byte[] outd = new byte[str.Length / 2];
            for (var i = 0; i < str.Length / 2; i++)
            {
                outd[i] = byte.Parse(str.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return outd;
        }
        public static byte[] OpDataToBytes(string opdata)
        {
            try  // convert hex string to byte[]
            {
                return HexString2Bytes(opdata);
            }
            catch
            {
                return System.Text.Encoding.UTF8.GetBytes(opdata);
            }
        }
    }
}
