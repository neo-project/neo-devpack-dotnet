using Neo.Network.RPC;
using System;

namespace Neo.SmartContract.Deploy.Shared;

/// <summary>
/// Factory interface for creating and managing RPC client instances
/// </summary>
public interface IRpcClientFactory
{
    /// <summary>
    /// Create a new RPC client instance
    /// </summary>
    /// <param name="networkName">Optional network name to use specific configuration</param>
    /// <returns>Configured RPC client</returns>
    RpcClient CreateClient(string? networkName = null);

    /// <summary>
    /// Get the RPC URL for the specified network
    /// </summary>
    /// <param name="networkName">Optional network name</param>
    /// <returns>RPC URL</returns>
    string GetRpcUrl(string? networkName = null);

    /// <summary>
    /// Get the current network name
    /// </summary>
    /// <returns>Network name</returns>
    string GetNetworkName();
}
