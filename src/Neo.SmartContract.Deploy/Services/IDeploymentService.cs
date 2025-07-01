using Neo;
using Neo.SmartContract.Manifest;

namespace Neo.SmartContract.Deploy.Services
{
    /// <summary>
    /// Main service interface for deploying Neo N3 smart contracts
    /// </summary>
    public interface IDeploymentService
    {
        /// <summary>
        /// Deploy all contracts using registered deployment steps
        /// </summary>
        Task<DeploymentResult> DeployAllAsync();

        /// <summary>
        /// Deploy a single contract
        /// </summary>
        /// <param name="name">Contract name</param>
        /// <param name="nef">NEF file bytes</param>
        /// <param name="manifest">Contract manifest</param>
        /// <param name="initParams">Optional initialization parameters</param>
        Task<ContractDeploymentResult> DeployContractAsync(string name, byte[] nef, ContractManifest manifest, object[]? initParams = null);

        /// <summary>
        /// Invoke a method on a deployed contract
        /// </summary>
        /// <param name="contractHash">Contract script hash</param>
        /// <param name="method">Method name</param>
        /// <param name="parameters">Method parameters</param>
        Task<UInt256> InvokeContractAsync(UInt160 contractHash, string method, params object[] parameters);
    }

    /// <summary>
    /// Overall deployment result
    /// </summary>
    public class DeploymentResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public List<ContractDeploymentResult> DeployedContracts { get; set; } = new();
    }

    /// <summary>
    /// Individual contract deployment result
    /// </summary>
    public class ContractDeploymentResult
    {
        public bool Success { get; set; }
        public string Name { get; set; } = string.Empty;
        public UInt160? Hash { get; set; }
        public UInt256? TransactionId { get; set; }
        public long GasConsumed { get; set; }
        public string? ErrorMessage { get; set; }
    }
}