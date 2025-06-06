using System;

namespace Neo.SmartContract.Fuzzer.Coverage
{
    /// <summary>
    /// Represents coverage statistics for a contract
    /// </summary>
    public class CoverageStatistics
    {
        /// <summary>
        /// Gets or sets the method coverage ratio (0.0 to 1.0)
        /// </summary>
        public double MethodCoverage { get; set; }
        
        /// <summary>
        /// Gets or sets the branch coverage ratio (0.0 to 1.0)
        /// </summary>
        public double BranchCoverage { get; set; }
        
        /// <summary>
        /// Gets or sets the instruction coverage ratio (0.0 to 1.0)
        /// </summary>
        public double InstructionCoverage { get; set; }
        
        /// <summary>
        /// Gets or sets the number of covered methods
        /// </summary>
        public int CoveredMethods { get; set; }
        
        /// <summary>
        /// Gets or sets the total number of methods
        /// </summary>
        public int TotalMethods { get; set; }
        
        /// <summary>
        /// Gets or sets the number of covered instructions
        /// </summary>
        public int CoveredInstructions { get; set; }
        
        /// <summary>
        /// Gets or sets the total number of instructions
        /// </summary>
        public int TotalInstructions { get; set; }
        
        /// <summary>
        /// Gets or sets the number of covered branches
        /// </summary>
        public int CoveredBranches { get; set; }
        
        /// <summary>
        /// Gets or sets the total number of branches
        /// </summary>
        public int TotalBranches { get; set; }
    }
}
