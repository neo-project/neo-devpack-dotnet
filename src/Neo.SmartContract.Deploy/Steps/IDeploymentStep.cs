using Neo;
using Neo.SmartContract.Deploy.Services;

namespace Neo.SmartContract.Deploy.Steps
{
    /// <summary>
    /// Interface for deployment steps
    /// </summary>
    public interface IDeploymentStep
    {
        /// <summary>
        /// Step name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Execution order (lower values execute first)
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Execute the deployment step
        /// </summary>
        /// <param name="context">Deployment context</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> ExecuteAsync(DeploymentContext context);
    }

    /// <summary>
    /// Context shared between deployment steps
    /// </summary>
    public class DeploymentContext
    {
        public IBlockchainService Blockchain { get; set; } = null!;
        public IWalletService Wallet { get; set; } = null!;
        public IContractLoader ContractLoader { get; set; } = null!;
        public IDeploymentService DeploymentService { get; set; } = null!;
        
        /// <summary>
        /// Deployed contracts by name
        /// </summary>
        public Dictionary<string, UInt160> DeployedContracts { get; } = new();
        
        /// <summary>
        /// Shared data between steps
        /// </summary>
        public Dictionary<string, object> Data { get; } = new();
    }
}