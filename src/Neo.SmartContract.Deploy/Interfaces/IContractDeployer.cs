using Neo;
using Neo.SmartContract.Deploy.Models;
using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy.Interfaces;

/// <summary>
/// Interface for contract deployment services
/// </summary>
public interface IContractDeployer
{
    /// <summary>
    /// Deploy a compiled contract
    /// </summary>
    /// <param name="contract">Compiled contract to deploy</param>
    /// <param name="options">Deployment options</param>
    /// <param name="initParams">Initialization parameters</param>
    /// <returns>Deployment result</returns>
    Task<ContractDeploymentInfo> DeployAsync(CompiledContract contract, DeploymentOptions options, object[]? initParams = null);

    /// <summary>
    /// Check if a contract exists on the network
    /// </summary>
    /// <param name="contractHash">Contract hash to check</param>
    /// <returns>True if contract exists, false otherwise</returns>
    Task<bool> ContractExistsAsync(UInt160 contractHash);

    /// <summary>
    /// Check if a contract exists on the network
    /// </summary>
    /// <param name="contractHash">Contract hash to check</param>
    /// <param name="rpcUrl">RPC URL to connect to</param>
    /// <returns>True if contract exists, false otherwise</returns>
    Task<bool> ContractExistsAsync(UInt160 contractHash, string rpcUrl);
}
