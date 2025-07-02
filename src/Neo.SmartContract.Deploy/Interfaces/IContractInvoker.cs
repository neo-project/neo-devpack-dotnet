using Neo;

namespace Neo.SmartContract.Deploy.Interfaces;

/// <summary>
/// Interface for invoking deployed contracts
/// </summary>
public interface IContractInvoker
{
    /// <summary>
    /// Call a contract method without sending a transaction
    /// </summary>
    Task<T?> CallAsync<T>(UInt160 contractHash, string method, params object[] parameters);

    /// <summary>
    /// Send a transaction to invoke a contract method
    /// </summary>
    Task<UInt256> SendAsync(UInt160 contractHash, string method, params object[] parameters);

    /// <summary>
    /// Wait for transaction confirmation
    /// </summary>
    Task<bool> WaitForConfirmationAsync(UInt256 txHash, int maxRetries = 30, int delaySeconds = 5);
}
