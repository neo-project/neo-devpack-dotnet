using System;

namespace Neo.SmartContract.Fuzzer
{
    /// <summary>
    /// Information about a witness check made during contract execution.
    /// </summary>
    public class WitnessCheckInfo
    {
        /// <summary>
        /// The account that was checked.
        /// </summary>
        public string Account { get; set; } = string.Empty;

        /// <summary>
        /// The timestamp of the check (relative to execution start).
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// The result of the check.
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// Whether the result was used in a condition.
        /// </summary>
        public bool ResultUsed { get; set; }
    }
}
