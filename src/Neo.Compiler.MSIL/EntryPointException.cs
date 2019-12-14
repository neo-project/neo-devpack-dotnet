using System;

namespace Neo.Compiler
{
    public class EntryPointException : Exception
    {
        /// <summary>
        /// Number of entry point founds
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="count">Number of entry point founds</param>
        /// <param name="message">Message</param>
        public EntryPointException(int count, string message) : base(message)
        {
            Count = count;
        }
    }
}
