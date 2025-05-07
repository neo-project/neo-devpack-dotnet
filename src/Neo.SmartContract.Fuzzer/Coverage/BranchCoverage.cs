namespace Neo.SmartContract.Fuzzer.Coverage
{
    /// <summary>
    /// Represents coverage information for a branch
    /// </summary>
    public class BranchCoverage
    {
        /// <summary>
        /// Gets or sets the ID of the branch (instruction offset)
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets whether the true branch has been covered
        /// </summary>
        public bool TrueCovered { get; set; }
        
        /// <summary>
        /// Gets or sets whether the false branch has been covered
        /// </summary>
        public bool FalseCovered { get; set; }
    }
}
