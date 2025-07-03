using Microsoft.Extensions.Logging;
using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Manifest;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Neo.SmartContract.Deploy.UnitTests.TestEngineServices;

/// <summary>
/// Simple mock implementation of IContractDeployer for integration tests
/// </summary>
public class SimpleTestDeployer : IContractDeployer
{
    private readonly ILogger<SimpleTestDeployer> _logger;
    private readonly IServiceProvider _serviceProvider;

    public SimpleTestDeployer(ILogger<SimpleTestDeployer> logger, IServiceProvider serviceProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task<ContractDeploymentInfo> DeployAsync(CompiledContract contract, DeploymentOptions options, object[]? initParams = null)
    {
        _logger.LogInformation("Mock deploying contract {ContractName}", contract.Name);

        // Parse NEF and manifest to validate them
        var nef = NefFile.Parse(contract.NefBytes);
        var manifest = ContractManifest.Parse(contract.ManifestBytes);

        // Generate a deterministic hash based on the contract content
        var hash = Neo.Cryptography.Helper.Sha256(contract.NefBytes);
        var contractHash = new UInt160(hash.Take(20).ToArray());

        // Generate a mock transaction hash
        var txHash = new UInt256(Guid.NewGuid().ToByteArray().Concat(Guid.NewGuid().ToByteArray()).ToArray());

        return new ContractDeploymentInfo
        {
            ContractName = contract.Name,
            ContractHash = contractHash,
            TransactionHash = txHash,
            Success = true,
            DeployedAt = DateTime.UtcNow,
            BlockIndex = 1,
            NetworkMagic = 0,
            GasConsumed = 1000000
        };
    }

    public async Task<ContractDeploymentInfo> UpdateAsync(CompiledContract contract, UInt160 contractHash, DeploymentOptions options)
    {
        _logger.LogInformation("Mock updating contract {ContractName} at {ContractHash}", contract.Name, contractHash);

        // Parse NEF and manifest to validate them
        var nef = NefFile.Parse(contract.NefBytes);
        var manifest = ContractManifest.Parse(contract.ManifestBytes);

        // Notify the invoker that this contract was updated
        var invoker = _serviceProvider.GetService<IContractInvoker>();
        if (invoker is SimpleTestInvoker testInvoker)
        {
            testInvoker.MarkContractAsUpdated(contractHash);
        }

        // Generate a mock transaction hash
        var txHash = new UInt256(Guid.NewGuid().ToByteArray().Concat(Guid.NewGuid().ToByteArray()).ToArray());

        return new ContractDeploymentInfo
        {
            ContractName = contract.Name,
            ContractHash = contractHash,
            TransactionHash = txHash,
            Success = true,
            DeployedAt = DateTime.UtcNow,
            BlockIndex = 2,
            NetworkMagic = 0,
            GasConsumed = 500000
        };
    }

    public async Task<bool> ContractExistsAsync(UInt160 contractHash, string rpcUrl)
    {
        // For testing, just return true for non-zero hashes
        return contractHash != UInt160.Zero;
    }
}
