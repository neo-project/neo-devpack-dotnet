using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy.Interfaces;

/// <summary>
/// Interface for managing deployment records
/// </summary>
public interface IDeploymentRecordService
{
    /// <summary>
    /// Save deployment record
    /// </summary>
    /// <param name="contractName">Contract name</param>
    /// <param name="network">Network name</param>
    /// <param name="record">Deployment record</param>
    /// <returns>Task</returns>
    Task SaveDeploymentRecordAsync(string contractName, string network, object record);

    /// <summary>
    /// Get deployment record
    /// </summary>
    /// <param name="contractName">Contract name</param>
    /// <param name="network">Network name</param>
    /// <returns>Deployment record</returns>
    Task<T?> GetDeploymentRecordAsync<T>(string contractName, string network) where T : class;

    /// <summary>
    /// Check if contract is deployed
    /// </summary>
    /// <param name="contractName">Contract name</param>
    /// <param name="network">Network name</param>
    /// <returns>True if deployed</returns>
    Task<bool> IsDeployedAsync(string contractName, string network);
}