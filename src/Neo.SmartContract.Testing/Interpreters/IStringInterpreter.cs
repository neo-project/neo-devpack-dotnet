using System;

namespace Neo.SmartContract.Testing.Interpreters
{
    public interface IStringInterpreter
    {
        /// <summary>
        /// Get string from bytes
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Value</returns>
        public string GetString(ReadOnlySpan<byte> bytes);
    }
}
