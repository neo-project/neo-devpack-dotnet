using Neo;
using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy.Interfaces;

/// <summary>
/// Interface for contract update operations
/// </summary>
public interface IContractUpdateService
{
    /// <summary>
    /// Update an existing smart contract
    /// </summary>
    /// <param name="contractName">Contract name</param>
    /// <param name="network">Network name</param>
    /// <param name="nefBytes">NEF bytes</param>
    /// <param name="manifest">Contract manifest</param>
    /// <param name="updateData">Update data</param>
    /// <param name="version">Contract version</param>
    /// <returns>Update result</returns>
    Task<ContractUpdateResult> UpdateContractAsync(
        string contractName,
        string network,
        byte[] nefBytes,
        string manifest,
        byte[]? updateData,
        string? version);
}

/// <summary>
/// Contract update result
/// </summary>
public class ContractUpdateResult
{
    /// <summary>
    /// Gets or sets whether the update was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the transaction hash of the update
    /// </summary>
    public UInt256? TransactionHash { get; set; }

    /// <summary>
    /// Gets or sets the error message if the update failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}
