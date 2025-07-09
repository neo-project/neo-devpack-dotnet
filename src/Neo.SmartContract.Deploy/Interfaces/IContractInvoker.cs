using System.Threading.Tasks;
using Neo;
using Neo.SmartContract.Deploy.Models;

namespace Neo.SmartContract.Deploy.Interfaces;

/// <summary>
/// Interface for contract invocation services
/// </summary>
public interface IContractInvoker
{
    /// <summary>
    /// Call a contract method (read-only)
    /// </summary>
    /// <typeparam name="T">Return type</typeparam>
    /// <param name="contractHash">Contract hash</param>
    /// <param name="method">Method name</param>
    /// <param name="args">Method arguments</param>
    /// <param name="rpcUrl">RPC URL</param>
    /// <returns>Method return value</returns>
    Task<T> CallAsync<T>(UInt160 contractHash, string method, object[] args, string rpcUrl);

    /// <summary>
    /// Invoke a contract method (state-changing transaction)
    /// </summary>
    /// <param name="contractHash">Contract hash</param>
    /// <param name="method">Method name</param>
    /// <param name="args">Method arguments</param>
    /// <param name="options">Invocation options</param>
    /// <returns>Transaction hash</returns>
    Task<UInt256> InvokeAsync(UInt160 contractHash, string method, object[] args, InvocationOptions options);
}