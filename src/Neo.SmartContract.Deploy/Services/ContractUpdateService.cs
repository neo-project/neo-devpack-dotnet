using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Neo;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using System;
using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy.Services;

/// <summary>
/// Service for updating deployed contracts
/// </summary>
public class ContractUpdateService : IContractUpdateService
{
    private readonly ILogger<ContractUpdateService> _logger;
    private readonly IContractDeployer _deployer;
    private readonly IDeploymentRecordService _recordService;
    private readonly IConfiguration _configuration;
    private readonly IWalletManager _walletManager;

    public ContractUpdateService(
        ILogger<ContractUpdateService> logger,
        IContractDeployer deployer,
        IDeploymentRecordService recordService,
        IConfiguration configuration,
        IWalletManager walletManager)
    {
        _logger = logger;
        _deployer = deployer;
        _recordService = recordService;
        _configuration = configuration;
        _walletManager = walletManager;
    }

    public async Task<ContractUpdateResult> UpdateContractAsync(
        string contractName,
        string network,
        byte[] nefBytes,
        string manifest,
        byte[]? updateData,
        string? version)
    {
        try
        {
            _logger.LogInformation("Updating contract {ContractName} on {Network}", contractName, network);

            // Check if contract exists in deployment records
            var isDeployed = await _recordService.IsDeployedAsync(contractName, network);
            if (!isDeployed)
            {
                return new ContractUpdateResult
                {
                    Success = false,
                    ErrorMessage = $"Contract {contractName} not found in deployment records for {network}"
                };
            }

            // Get deployment record to find contract hash
            var deploymentRecord = await _recordService.GetDeploymentRecordAsync(contractName, network);
            if (deploymentRecord?.ContractHash == null)
            {
                return new ContractUpdateResult
                {
                    Success = false,
                    ErrorMessage = $"Could not find contract hash for {contractName} on {network}"
                };
            }

            var contractHash = UInt160.Parse(deploymentRecord.ContractHash);

            // Create compiled contract from provided data
            var compiledContract = new CompiledContract
            {
                Name = contractName,
                NefBytes = nefBytes,
                Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(manifest)
            };

            // Create deployment options
            var deploymentOptions = new DeploymentOptions
            {
                DeployerAccount = _walletManager.IsWalletLoaded ? _walletManager.GetAccount() : null,
                GasLimit = _configuration.GetValue<long>("Deployment:GasLimit", 100_000_000),
                WaitForConfirmation = _configuration.GetValue<bool>("Deployment:WaitForConfirmation", true),
                DryRun = _configuration.GetValue<bool>("Deployment:DryRun", false),
                VerifyAfterDeploy = _configuration.GetValue<bool>("Deployment:VerifyAfterDeploy", true),
                RpcUrl = _configuration.GetValue<string>($"Networks:{network}:RpcUrl") ?? _configuration.GetValue<string>("Network:RpcUrl") ?? "http://localhost:10332"
            };

            // Use the contract deployer's update functionality
            var updateResult = await _deployer.UpdateAsync(compiledContract, contractHash, deploymentOptions);

            if (updateResult.Success)
            {
                // Update deployment record with new version and update history
                await _recordService.RecordUpdateAsync(contractName, network, new DeploymentRecord
                {
                    ContractName = contractName,
                    ContractHash = contractHash.ToString(),
                    TransactionHash = updateResult.TransactionHash?.ToString(),
                    UpdatedAt = DateTime.UtcNow,
                    Version = version ?? "unknown",
                    UpdateHistory = new[]
                    {
                        new UpdateHistoryEntry
                        {
                            TransactionHash = updateResult.TransactionHash?.ToString(),
                            UpdatedAt = DateTime.UtcNow,
                            PreviousVersion = deploymentRecord.Version,
                            NewVersion = version ?? "unknown"
                        }
                    }
                });

                _logger.LogInformation("Contract {ContractName} updated successfully on {Network}", contractName, network);
            }

            return new ContractUpdateResult
            {
                Success = updateResult.Success,
                TransactionHash = updateResult.TransactionHash,
                ErrorMessage = updateResult.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update contract {ContractName} on {Network}", contractName, network);
            return new ContractUpdateResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
