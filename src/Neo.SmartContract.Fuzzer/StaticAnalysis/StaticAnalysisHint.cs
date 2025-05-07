namespace Neo.SmartContract.Fuzzer.StaticAnalysis
{
    /// <summary>
    /// Represents a hint from static analysis that can guide the fuzzing process.
    /// </summary>
    public class StaticAnalysisHint
    {
        /// <summary>
        /// Gets or sets the file path where the hint was found.
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the line number where the hint was found.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the type of risk identified.
        /// </summary>
        public string RiskType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a description of the hint.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the priority of this hint.
        /// Higher values indicate higher priority.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the method name related to this hint.
        /// </summary>
        public string? MethodName { get; set; }

        /// <summary>
        /// Gets or sets the parameter name related to this hint.
        /// </summary>
        public string? ParameterName { get; set; }

        /// <summary>
        /// Creates a clone of this hint.
        /// </summary>
        /// <returns>A new instance of <see cref="StaticAnalysisHint"/> with the same values.</returns>
        public StaticAnalysisHint Clone()
        {
            return new StaticAnalysisHint
            {
                FilePath = FilePath,
                LineNumber = LineNumber,
                RiskType = RiskType,
                Description = Description,
                Priority = Priority,
                MethodName = MethodName,
                ParameterName = ParameterName
            };
        }
    }
}
