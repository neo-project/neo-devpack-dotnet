using Neo;
using Neo.SmartContract.Manifest;

namespace Neo.SmartContract.Deploy.Services
{
    /// <summary>
    /// Service for updating deployed contracts
    /// </summary>
    public interface IContractUpdateService
    {
        /// <summary>
        /// Update a deployed contract
        /// </summary>
        /// <param name="contractName">Contract name</param>
        /// <param name="network">Target network</param>
        /// <param name="nef">New NEF bytes</param>
        /// <param name="manifest">New manifest</param>
        /// <param name="updateData">Optional update data</param>
        /// <param name="version">New version</param>
        Task<ContractUpdateResult> UpdateContractAsync(
            string contractName, 
            string network,
            byte[] nef, 
            ContractManifest manifest,
            object? updateData = null,
            string? version = null);

        /// <summary>
        /// Check if a contract can be updated
        /// </summary>
        Task<UpdateCheckResult> CheckUpdateAsync(string contractName, string network);

        /// <summary>
        /// Update multiple contracts in order
        /// </summary>
        Task<BatchUpdateResult> UpdateContractsAsync(string network, params string[] contractNames);
    }

    /// <summary>
    /// Result of a contract update operation
    /// </summary>
    public class ContractUpdateResult
    {
        public bool Success { get; set; }
        public string ContractName { get; set; } = string.Empty;
        public UInt160? ContractHash { get; set; }
        public UInt256? TransactionHash { get; set; }
        public string? PreviousVersion { get; set; }
        public string? NewVersion { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// Result of update check
    /// </summary>
    public class UpdateCheckResult
    {
        public bool CanUpdate { get; set; }
        public bool IsDeployed { get; set; }
        public string? CurrentVersion { get; set; }
        public UInt160? ContractHash { get; set; }
        public string? Reason { get; set; }
        public bool HasUpdateMethod { get; set; }
    }

    /// <summary>
    /// Result of batch update operation
    /// </summary>
    public class BatchUpdateResult
    {
        public bool Success { get; set; }
        public List<ContractUpdateResult> Updates { get; set; } = new();
        public int SuccessCount => Updates.Count(u => u.Success);
        public int FailureCount => Updates.Count(u => !u.Success);
    }
}