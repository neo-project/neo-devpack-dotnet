using System;

namespace Neo.SmartContract.Fuzzer
{
    /// <summary>
    /// Event arguments for a syscall.
    /// </summary>
    public class SysCallEventArgs : EventArgs
    {
        /// <summary>
        /// The syscall method that was called.
        /// </summary>
        public string Method { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SysCallEventArgs"/> class.
        /// </summary>
        /// <param name="method">The syscall method that was called.</param>
        public SysCallEventArgs(string method)
        {
            Method = method;
        }
    }
}
