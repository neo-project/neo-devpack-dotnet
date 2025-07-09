using Neo;
using Neo.SmartContract.Deploy.Models;
using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy.Interfaces;

/// <summary>
/// Interface for contract update services
/// </summary>
public interface IContractUpdateService
{
    /// <summary>
    /// Update an existing contract
    /// </summary>
    /// <param name="contractHash">Hash of the contract to update</param>
    /// <param name="contract">Updated compiled contract</param>
    /// <param name="options">Update options</param>
    /// <param name="updateParams">Update parameters for _deploy method</param>
    /// <returns>Update result</returns>
    Task<ContractUpdateInfo> UpdateAsync(UInt160 contractHash, CompiledContract contract, UpdateOptions options, object[]? updateParams = null);

    /// <summary>
    /// Check if a contract can be updated
    /// </summary>
    /// <param name="contractHash">Contract hash to check</param>
    /// <param name="rpcUrl">RPC URL to connect to</param>
    /// <returns>True if contract can be updated, false otherwise</returns>
    Task<bool> CanUpdateAsync(UInt160 contractHash, string rpcUrl);
}