using System;

namespace Neo.SmartContract.Fuzzer
{
    /// <summary>
    /// Information about an external call made during contract execution.
    /// </summary>
    public class ExternalCallInfo
    {
        /// <summary>
        /// The target of the external call (contract hash or address).
        /// </summary>
        public string Target { get; set; } = string.Empty;

        /// <summary>
        /// The target contract of the external call (contract hash or address).
        /// </summary>
        public string TargetContract { get; set; } = string.Empty;

        /// <summary>
        /// The method that was called.
        /// </summary>
        public string Method { get; set; } = string.Empty;

        /// <summary>
        /// The timestamp of the call (relative to execution start).
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Whether the call was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The return value of the call, if any.
        /// </summary>
        public string? ReturnValue { get; set; }
    }
}
