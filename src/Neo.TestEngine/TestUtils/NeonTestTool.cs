using Neo.VM;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Neo.TestingEngine
{
    public static class NeonTestTool
    {
        /// <summary>
        /// Is not the official script hash, just a unique hash related to the script used for unit test purpose
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns>UInt160</returns>
        public static UInt160 ScriptHash(this ExecutionContext context)
        {
            using (var sha = SHA1.Create())
            {
                return new UInt160(sha.ComputeHash(((byte[])context.Script)));
            }
        }

        public static string Bytes2HexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var d in data)
            {
                sb.Append(d.ToString("x02"));
            }
            return sb.ToString();
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
    }
}
