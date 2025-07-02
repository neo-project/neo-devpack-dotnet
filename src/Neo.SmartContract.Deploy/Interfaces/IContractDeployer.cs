using Neo;
using Neo.SmartContract.Deploy.Models;

/// <summary>
/// Contract deployment interface
/// </summary>
public interface IContractDeployer
{
    /// <summary>
    /// Deploy a compiled contract
    /// </summary>
    /// <param name="contract">Compiled contract to deploy</param>
    /// <param name="options">Deployment options</param>
    /// <returns>Deployment result</returns>
    Task<ContractDeploymentInfo> DeployAsync(CompiledContract contract, DeploymentOptions options, object[]? initParams = null);

    /// <summary>
    /// Update an existing contract
    /// </summary>
    /// <param name="contract">Updated compiled contract</param>
    /// <param name="contractHash">Hash of existing contract to update</param>
    /// <param name="options">Deployment options</param>
    /// <returns>Deployment result</returns>
    Task<ContractDeploymentInfo> UpdateAsync(CompiledContract contract, UInt160 contractHash, DeploymentOptions options);

    /// <summary>
    /// Check if a contract exists on the network
    /// </summary>
    /// <param name="contractHash">Contract hash to check</param>
    /// <param name="rpcUrl">Network RPC URL</param>
    /// <returns>True if contract exists</returns>
    Task<bool> ContractExistsAsync(UInt160 contractHash, string rpcUrl);
}
