using System.Collections.Generic;

namespace Neo.SmartContract.Fuzzer.Coverage
{
    /// <summary>
    /// Represents coverage information for a method
    /// </summary>
    public class MethodCoverage
    {
        /// <summary>
        /// Gets or sets the name of the method
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the parameter types of the method
        /// </summary>
        public string[] Parameters { get; set; } = System.Array.Empty<string>();
        
        /// <summary>
        /// Gets or sets the return type of the method
        /// </summary>
        public string ReturnType { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the number of times the method has been executed
        /// </summary>
        public int ExecutionCount { get; set; }
        
        /// <summary>
        /// Gets or sets the set of covered instruction offsets
        /// </summary>
        public HashSet<int> CoveredInstructions { get; set; } = new HashSet<int>();
        
        /// <summary>
        /// Gets or sets the total number of instructions in the method
        /// </summary>
        public int TotalInstructions { get; set; }
    }
}
