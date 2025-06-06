namespace Neo.SmartContract.Fuzzer.Feedback
{
    /// <summary>
    /// Defines the types of feedback that can guide the fuzzing process.
    /// </summary>
    public enum FeedbackType
    {
        /// <summary>
        /// Feedback from a crash (exception or VM fault).
        /// </summary>
        Crash,

        /// <summary>
        /// Feedback from new code coverage.
        /// </summary>
        NewCoverage,

        /// <summary>
        /// Feedback from a state change.
        /// </summary>
        StateChange,

        /// <summary>
        /// Feedback from static analysis.
        /// </summary>
        StaticHint,

        /// <summary>
        /// Feedback from high gas usage.
        /// </summary>
        HighGasUsage,

        /// <summary>
        /// Feedback from an invariant violation.
        /// </summary>
        InvariantViolation
    }
}
