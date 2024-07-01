using System;

namespace Neo.SmartContract.Testing.Interpreters
{
    public class HexStringInterpreter : IStringInterpreter
    {
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
