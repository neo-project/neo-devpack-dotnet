using Microsoft.Extensions.Logging;
using Neo;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using System;
using System.Collections.Generic;
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

    public Task<T?> CallAsync<T>(UInt160 contractHash, string method, params object[] parameters)
    {
        _logger.LogDebug("Mock calling contract {ContractHash} method {Method}", contractHash, method);

        // Return mock values based on method name for testing
        var methodLower = method.ToLowerInvariant();

        T? result;
        if (methodLower == "getvalue")
        {
            // Return different value if contract was updated
            if (_updatedContracts.ContainsKey(contractHash))
            {
                result = (T)(object)100;
            }
            else
            {
                result = (T)(object)42;
            }
        }
        else if (methodLower == "testmethod")
        {
            if (parameters.Length > 0 && parameters[0] is string input)
            {
                result = (T)(object)$"Hello {input}";
            }
            else
            {
                result = (T)(object)"Hello World";
            }
        }
        else if (methodLower == "getname")
        {
            // For multi-contract tests, return a name
            result = (T)(object)"MyContract";
        }
        else if (methodLower == "getregisteredcontract")
        {
            // For registry lookups
            if (parameters.Length > 0 && parameters[0] is string)
            {
                // Return the contract hash passed as the second deploy param
                result = default(T); // Would need actual tracking
            }
            else
            {
                result = default(T);
            }
        }
        else if (typeof(T) == typeof(string))
        {
            result = (T)(object)"MockValue";
        }
        else if (typeof(T) == typeof(int))
        {
            result = (T)(object)0;
        }
        else if (typeof(T) == typeof(bool))
        {
            result = (T)(object)true;
        }
        else
        {
            result = default(T);
        }

        return Task.FromResult(result);
    }

    public Task<UInt256> SendAsync(UInt160 contractHash, string method, params object[] parameters)
    {
        return SendAsync(contractHash, method, null, parameters);
    }

    public Task<UInt256> SendAsync(UInt160 contractHash, string method, DeploymentOptions? options, params object[] parameters)
    {
        _logger.LogDebug("Mock sending transaction to contract {ContractHash} method {Method}", contractHash, method);

        // Return a mock transaction hash
        var txHash = new UInt256(Guid.NewGuid().ToByteArray().Concat(Guid.NewGuid().ToByteArray()).ToArray());
        return Task.FromResult(txHash);
    }

    public Task<bool> WaitForConfirmationAsync(UInt256 txHash, int maxRetries = 30, int delaySeconds = 5)
    {
        _logger.LogDebug("Mock waiting for transaction {TxHash} confirmation", txHash);

        // Always return true for mock
        return Task.FromResult(true);
    }
}
