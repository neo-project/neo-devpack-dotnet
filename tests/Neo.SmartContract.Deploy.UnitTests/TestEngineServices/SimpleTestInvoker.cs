using Microsoft.Extensions.Logging;
using Neo;
using Neo.SmartContract.Deploy.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy.UnitTests.TestEngineServices;

/// <summary>
/// Simple mock implementation of IContractInvoker for integration tests
/// </summary>
public class SimpleTestInvoker : IContractInvoker
{
    private readonly ILogger<SimpleTestInvoker> _logger;
    private readonly Dictionary<UInt160, bool> _updatedContracts = new();

    public SimpleTestInvoker(ILogger<SimpleTestInvoker> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void MarkContractAsUpdated(UInt160 contractHash)
    {
        _updatedContracts[contractHash] = true;
    }

    public async Task<T?> CallAsync<T>(UInt160 contractHash, string method, params object[] parameters)
    {
        _logger.LogDebug("Mock calling contract {ContractHash} method {Method}", contractHash, method);

        // Return mock values based on method name for testing
        var methodLower = method.ToLowerInvariant();

        if (methodLower == "getvalue")
        {
            // Return different value if contract was updated
            if (_updatedContracts.ContainsKey(contractHash))
            {
                return (T)(object)100;
            }
            return (T)(object)42;
        }
        else if (methodLower == "testmethod")
        {
            if (parameters.Length > 0 && parameters[0] is string input)
            {
                return (T)(object)$"Hello {input}";
            }
            return (T)(object)"Hello World";
        }
        else if (methodLower == "getname")
        {
            // For multi-contract tests, return a name
            return (T)(object)"MyContract";
        }
        else if (methodLower == "getregisteredcontract")
        {
            // For registry lookups
            if (parameters.Length > 0 && parameters[0] is string)
            {
                // Return the contract hash passed as the second deploy param
                return default(T); // Would need actual tracking
            }
        }

        // Default returns for common types
        if (typeof(T) == typeof(string))
        {
            return (T)(object)"MockValue";
        }
        else if (typeof(T) == typeof(int))
        {
            return (T)(object)0;
        }
        else if (typeof(T) == typeof(bool))
        {
            return (T)(object)true;
        }

        return default(T);
    }

    public async Task<UInt256> SendAsync(UInt160 contractHash, string method, params object[] parameters)
    {
        _logger.LogDebug("Mock sending transaction to contract {ContractHash} method {Method}", contractHash, method);

        // Return a mock transaction hash
        return new UInt256(Guid.NewGuid().ToByteArray().Concat(Guid.NewGuid().ToByteArray()).ToArray());
    }

    public async Task<bool> WaitForConfirmationAsync(UInt256 txHash, int maxRetries = 30, int delaySeconds = 5)
    {
        _logger.LogDebug("Mock waiting for transaction {TxHash} confirmation", txHash);

        // Always return true for mock
        return true;
    }
}
