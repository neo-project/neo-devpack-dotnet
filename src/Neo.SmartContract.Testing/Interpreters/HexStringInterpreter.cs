using Neo.Extensions;
using System;
using System.Text.RegularExpressions;

namespace Neo.SmartContract.Testing.Interpreters
{
    public class HexStringInterpreter : IStringInterpreter
    {
        public static readonly Regex HexRegex = new(@"^[a-zA-Z0-9_]+$");

        /// <summary>
        /// Get string from bytes
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Value</returns>
        public virtual string GetString(ReadOnlySpan<byte> bytes)
        {
            return bytes.ToHexString();
        }
    }
}
