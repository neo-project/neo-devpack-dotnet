using System;

namespace Neo.SmartContract.Fuzzer.Extensions
{
    /// <summary>
    /// Extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Gets the message from a string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>The string itself.</returns>
        public static string Message(this string str)
        {
            return str;
        }
    }
}
