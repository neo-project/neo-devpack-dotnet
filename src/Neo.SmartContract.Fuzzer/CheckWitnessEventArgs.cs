using Neo.Cryptography.ECC;
using System;

namespace Neo.SmartContract.Fuzzer
{
    /// <summary>
    /// Event arguments for a witness check.
    /// </summary>
    public class CheckWitnessEventArgs : EventArgs
    {
        /// <summary>
        /// The hash that was checked.
        /// </summary>
        public UInt160 Hash { get; }

        /// <summary>
        /// The result of the check.
        /// </summary>
        public bool Result { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckWitnessEventArgs"/> class.
        /// </summary>
        /// <param name="hash">The hash that was checked.</param>
        /// <param name="result">The result of the check.</param>
        public CheckWitnessEventArgs(UInt160 hash, bool result)
        {
            Hash = hash;
            Result = result;
        }
    }
}
