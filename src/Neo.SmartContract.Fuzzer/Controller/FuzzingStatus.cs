using System;

namespace Neo.SmartContract.Fuzzer.Controller
{
    /// <summary>
    /// Represents the status of a fuzzing session.
    /// </summary>
    public class FuzzingStatus
    {
        /// <summary>
        /// Gets or sets the elapsed time of the fuzzing session.
        /// </summary>
        public TimeSpan ElapsedTime { get; set; }

        /// <summary>
        /// Gets or sets the total number of methods being fuzzed.
        /// </summary>
        public int TotalMethods { get; set; }

        /// <summary>
        /// Gets or sets the total number of executions performed.
        /// </summary>
        public int TotalExecutions { get; set; }

        /// <summary>
        /// Gets or sets the number of successful executions.
        /// </summary>
        public int SuccessfulExecutions { get; set; }

        /// <summary>
        /// Gets or sets the number of failed executions.
        /// </summary>
        public int FailedExecutions { get; set; }

        /// <summary>
        /// Gets or sets the number of issues found.
        /// </summary>
        public int IssuesFound { get; set; }

        /// <summary>
        /// Gets or sets the method coverage percentage.
        /// </summary>
        public double CodeCoverage { get; set; }

        /// <summary>
        /// Gets or sets the branch coverage percentage.
        /// </summary>
        public double BranchCoverage { get; set; }

        /// <summary>
        /// Gets or sets the instruction coverage percentage.
        /// </summary>
        public double InstructionCoverage { get; set; }

        /// <summary>
        /// Gets or sets the number of errors encountered during fuzzing.
        /// </summary>
        public int Errors { get; set; }

        /// <summary>
        /// Gets the success rate of executions.
        /// </summary>
        public double SuccessRate => TotalExecutions > 0 ? (double)SuccessfulExecutions / TotalExecutions : 0;

        /// <summary>
        /// Gets the execution rate (executions per second).
        /// </summary>
        public double ExecutionRate => ElapsedTime.TotalSeconds > 0 ? TotalExecutions / ElapsedTime.TotalSeconds : 0;
    }
}
