using Neo;

namespace Neo.SmartContract.Deploy.Services
{
    /// <summary>
    /// Service for managing deployment records across different networks
    /// </summary>
    public interface IDeploymentRecordService
    {
        /// <summary>
        /// Save a deployment record
        /// </summary>
        Task SaveDeploymentAsync(string contractName, string network, DeploymentRecord record);

        /// <summary>
        /// Get deployment record for a contract on a specific network
        /// </summary>
        Task<DeploymentRecord?> GetDeploymentAsync(string contractName, string network);

        /// <summary>
        /// Get all deployments for a contract across all networks
        /// </summary>
        Task<Dictionary<string, DeploymentRecord>> GetAllDeploymentsAsync(string contractName);

        /// <summary>
        /// Check if a contract is deployed on a specific network
        /// </summary>
        Task<bool> IsDeployedAsync(string contractName, string network);

        /// <summary>
        /// Remove a deployment record
        /// </summary>
        Task RemoveDeploymentAsync(string contractName, string network);

        /// <summary>
        /// Get all deployed contracts for a network
        /// </summary>
        Task<Dictionary<string, DeploymentRecord>> GetNetworkDeploymentsAsync(string network);
    }

    /// <summary>
    /// Deployment record for a contract
    /// </summary>
    public class DeploymentRecord
    {
        /// <summary>
        /// Contract hash/address
        /// </summary>
        public UInt160 ContractHash { get; set; } = null!;

        /// <summary>
        /// Deployment transaction hash
        /// </summary>
        public UInt256 TransactionHash { get; set; } = null!;

        /// <summary>
        /// Deployment timestamp
        /// </summary>
        public DateTimeOffset DeployedAt { get; set; }

        /// <summary>
        /// Deployer address
        /// </summary>
        public UInt160 DeployerAddress { get; set; } = null!;

        /// <summary>
        /// Contract version (for tracking updates)
        /// </summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// NEF file hash for verification
        /// </summary>
        public string NefHash { get; set; } = string.Empty;

        /// <summary>
        /// Manifest hash for verification
        /// </summary>
        public string ManifestHash { get; set; } = string.Empty;

        /// <summary>
        /// Additional metadata
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; } = new();

        /// <summary>
        /// Update history
        /// </summary>
        public List<UpdateRecord> UpdateHistory { get; set; } = new();
    }

    /// <summary>
    /// Record of a contract update
    /// </summary>
    public class UpdateRecord
    {
        /// <summary>
        /// Update transaction hash
        /// </summary>
        public UInt256 TransactionHash { get; set; } = null!;

        /// <summary>
        /// Update timestamp
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }

        /// <summary>
        /// Previous version
        /// </summary>
        public string PreviousVersion { get; set; } = string.Empty;

        /// <summary>
        /// New version
        /// </summary>
        public string NewVersion { get; set; } = string.Empty;

        /// <summary>
        /// Update description
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}