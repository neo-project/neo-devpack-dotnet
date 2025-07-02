namespace Neo.SmartContract.Deploy.Models
{
    /// <summary>
    /// Configuration for a smart contract deployment
    /// </summary>
    public class ContractConfig
    {
        /// <summary>
        /// Name of the contract (must match project folder name in src/)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Initialization parameters for the contract
        /// Supports contract references using {{ContractName}} syntax
        /// </summary>
        public object[]? InitParams { get; set; }

        /// <summary>
        /// List of contract dependencies (contract names that must be deployed first)
        /// </summary>
        public string[] Dependencies { get; set; } = Array.Empty<string>();
    }
}
