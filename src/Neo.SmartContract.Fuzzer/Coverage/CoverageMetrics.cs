namespace Neo.SmartContract.Fuzzer.Coverage
{
    /// <summary>
    /// Represents coverage metrics for a contract
    /// </summary>
    public class CoverageMetrics
    {
        /// <summary>
        /// Gets or sets the method coverage percentage
        /// </summary>
        public double MethodCoverage { get; set; }
        
        /// <summary>
        /// Gets or sets the instruction coverage percentage
        /// </summary>
        public double InstructionCoverage { get; set; }
        
        /// <summary>
        /// Gets or sets the branch coverage percentage
        /// </summary>
        public double BranchCoverage { get; set; }
        
        /// <summary>
        /// Gets or sets the path coverage percentage
        /// </summary>
        public double PathCoverage { get; set; }
        
        /// <summary>
        /// Gets or sets the total number of methods
        /// </summary>
        public int TotalMethods { get; set; }
        
        /// <summary>
        /// Gets or sets the number of covered methods
        /// </summary>
        public int CoveredMethods { get; set; }
        
        /// <summary>
        /// Gets or sets the total number of instructions
        /// </summary>
        public int TotalInstructions { get; set; }
        
        /// <summary>
        /// Gets or sets the number of covered instructions
        /// </summary>
        public int CoveredInstructions { get; set; }
        
        /// <summary>
        /// Gets or sets the total number of branches
        /// </summary>
        public int TotalBranches { get; set; }
        
        /// <summary>
        /// Gets or sets the number of covered branches
        /// </summary>
        public int CoveredBranches { get; set; }
        
        /// <summary>
        /// Gets or sets the total number of paths
        /// </summary>
        public int TotalPaths { get; set; }
        
        /// <summary>
        /// Gets or sets the number of covered paths
        /// </summary>
        public int CoveredPaths { get; set; }
        
        /// <summary>
        /// Gets or sets the method coverage details
        /// </summary>
        public MethodCoverage[] MethodDetails { get; set; } = System.Array.Empty<MethodCoverage>();
        
        /// <summary>
        /// Gets or sets the branch coverage details
        /// </summary>
        public BranchCoverage[] BranchDetails { get; set; } = System.Array.Empty<BranchCoverage>();
    }
}
