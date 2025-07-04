using System.Threading.Tasks;
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
    /// <typeparam name="T">Expected return type</typeparam>
    /// <param name="contractHash">Contract hash</param>
    /// <param name="method">Method name to call</param>
    /// <param name="parameters">Method parameters</param>
    /// <returns>Method return value</returns>
    /// <exception cref="Neo.SmartContract.Deploy.Exceptions.ContractInvocationException">Thrown when invocation fails</exception>
    Task<T?> CallAsync<T>(UInt160 contractHash, string method, params object[] parameters);

    /// <summary>
    /// Send a transaction to invoke a contract method
    /// </summary>
    /// <param name="contractHash">Contract hash</param>
    /// <param name="method">Method name to invoke</param>
    /// <param name="parameters">Method parameters</param>
    /// <returns>Transaction hash</returns>
    /// <exception cref="Neo.SmartContract.Deploy.Exceptions.ContractInvocationException">Thrown when invocation fails</exception>
    Task<UInt256> SendAsync(UInt160 contractHash, string method, params object[] parameters);

    /// <summary>
    /// Wait for transaction confirmation
    /// </summary>
    /// <param name="txHash">Transaction hash to wait for</param>
    /// <param name="maxRetries">Maximum number of retry attempts</param>
    /// <param name="delaySeconds">Delay between attempts in seconds</param>
    /// <returns>True if confirmed, false if timeout</returns>
    Task<bool> WaitForConfirmationAsync(UInt256 txHash, int maxRetries = 30, int delaySeconds = 5);
}
