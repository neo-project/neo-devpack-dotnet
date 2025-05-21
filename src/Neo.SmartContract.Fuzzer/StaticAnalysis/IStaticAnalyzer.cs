using System.Collections.Generic;

namespace Neo.SmartContract.Fuzzer.StaticAnalysis
{
    /// <summary>
    /// Interface for static analyzers that can provide hints for fuzzing.
    /// </summary>
    public interface IStaticAnalyzer
    {
        /// <summary>
        /// Analyzes the target and returns hints for fuzzing.
        /// </summary>
        /// <returns>A collection of static analysis hints.</returns>
        IEnumerable<StaticAnalysisHint> Analyze();
    }
}
