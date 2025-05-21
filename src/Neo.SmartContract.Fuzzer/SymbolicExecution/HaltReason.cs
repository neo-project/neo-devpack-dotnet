namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Represents the reason why a symbolic execution path halted.
    /// </summary>
    public enum HaltReason
    {
        /// <summary>
        /// The execution completed normally.
        /// </summary>
        Normal,

        /// <summary>
        /// The execution was aborted.
        /// </summary>
        Abort,

        /// <summary>
        /// The execution encountered an error.
        /// </summary>
        Error,

        /// <summary>
        /// The execution reached a maximum depth limit.
        /// </summary>
        MaxDepthReached,

        /// <summary>
        /// The execution reached a maximum state count limit.
        /// </summary>
        MaxStatesReached,

        /// <summary>
        /// The execution reached a timeout.
        /// </summary>
        Timeout,

        /// <summary>
        /// The execution was halted for an unspecified reason.
        /// </summary>
        Unknown
    }
}
