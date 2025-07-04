using System;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.Cryptography.ECC;
using Neo.Extensions;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.Wallets;
using Neo.SmartContract.Deploy.Exceptions;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Shared;

namespace Neo.SmartContract.Deploy.Services;

/// <summary>
/// Contract deployment service implementation
/// </summary>
public class ContractDeployerService : IContractDeployer
{
    private readonly ILogger<ContractDeployerService> _logger;
    private readonly IWalletManager _walletManager;
    private readonly IConfiguration _configuration;
    private readonly IRpcClientFactory _rpcClientFactory;
    private readonly TransactionBuilder _transactionBuilder;
    private readonly TransactionConfirmationService _confirmationService;

    public ContractDeployerService(
        ILogger<ContractDeployerService> logger,
        IWalletManager walletManager,
        IConfiguration configuration,
        IRpcClientFactory rpcClientFactory,
        TransactionBuilder transactionBuilder,
        TransactionConfirmationService confirmationService)
    {
        _logger = logger;
        _walletManager = walletManager;
        _configuration = configuration;
        _rpcClientFactory = rpcClientFactory;
        _transactionBuilder = transactionBuilder;
        _confirmationService = confirmationService;
    }

    /// <summary>
    /// Deploy a compiled contract
    /// </summary>
    /// <param name="contract">Compiled contract to deploy</param>
    /// <param name="options">Deployment options</param>
    /// <param name="initParams">Initialization parameters</param>
    /// <returns>Deployment result</returns>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null</exception>
    /// <exception cref="ContractDeploymentException">Thrown when deployment fails</exception>
    public async Task<ContractDeploymentInfo> DeployAsync(CompiledContract contract, DeploymentOptions options, object[]? initParams = null)
    {
        if (contract == null) throw new ArgumentNullException(nameof(contract));
        if (options == null) throw new ArgumentNullException(nameof(options));

        _logger.LogInformation("Deploying contract {ContractName}", contract.Name);

        try
        {
            // Create RPC client using factory
            var client = _rpcClientFactory.CreateClient();

            // Get current block count for ValidUntilBlock
            var blockCount = await client.GetBlockCountAsync();

            // Create deployment script using helper
            var script = ScriptBuilderHelper.BuildDeploymentScript(
                contract.NefBytes,
                contract.ManifestBytes,
                null, // No initialization method at deployment
                initParams);

            // Get deployer account from options or wallet manager
            var deployerAccount = options.DeployerAccount;
            if (deployerAccount == null)
            {
                if (!_walletManager.IsWalletLoaded)
                    throw new InvalidOperationException("Wallet not loaded. Please load a wallet before deployment.");
                deployerAccount = _walletManager.GetAccount();
            }

            // Create signer
            var signer = new Signer
            {
                Account = deployerAccount,
                Scopes = WitnessScope.CalledByEntry
            };

            // Test invoke first to get gas consumption and contract hash
            var invokeResult = await client.InvokeScriptAsync(script, signer);
            if (invokeResult.State != VMState.HALT)
            {
                throw new ContractDeploymentException(contract.Name, invokeResult.Exception ?? "Deployment script failed");
            }

            // Extract contract hash from deployment result
            UInt160 contractHash;
            if (invokeResult.Stack.Length > 0 && !invokeResult.Stack[0].IsNull)
            {
                // The deploy method returns the contract state, we need to extract the hash
                var deployResult = invokeResult.Stack[0];
                if (deployResult is Neo.VM.Types.Array array && array.Count >= 5)
                {
                    contractHash = new UInt160(array[2].GetSpan()); // Hash is at index 2 in ContractState array
                }
                else
                {
                    // Calculate contract hash from NEF and sender
                    var nef = NefFile.Parse(contract.NefBytes);
                    contractHash = Helper.GetContractHash(deployerAccount, nef.CheckSum, contract.Manifest.Name);
                }
            }
            else
            {
                // Calculate contract hash from NEF and sender  
                var nef = NefFile.Parse(contract.NefBytes);
                contractHash = Helper.GetContractHash(deployerAccount, nef.CheckSum, contract.Manifest.Name);
            }

            // Get deployment configuration
            var deploymentConfig = _configuration.GetSection("Deployment").Get<DeploymentConfiguration>() ?? new DeploymentConfiguration();

            // Create transaction using builder
            var txOptions = new TransactionBuildOptions
            {
                Sender = deployerAccount,
                Script = script,
                SystemFee = invokeResult.GasConsumed,
                NetworkFee = deploymentConfig.DefaultNetworkFee,
                ValidUntilBlock = blockCount + deploymentConfig.ValidUntilBlockOffset
            };
            var tx = _transactionBuilder.Build(txOptions);

            // Sign transaction using wallet manager
            await _walletManager.SignTransactionAsync(tx, deployerAccount);

            // Send transaction
            var txHash = await client.SendRawTransactionAsync(tx);

            // Wait for confirmation if requested
            if (deploymentConfig.WaitForConfirmation)
            {
                var confirmOptions = new TransactionConfirmationOptions
                {
                    Timeout = TimeSpan.FromSeconds(deploymentConfig.ConfirmationRetries * deploymentConfig.ConfirmationDelaySeconds),
                    PollingInterval = TimeSpan.FromSeconds(deploymentConfig.ConfirmationDelaySeconds)
                };
                await _confirmationService.WaitForConfirmationAsync(client, txHash, confirmOptions);
            }

            // Get network magic from version info
            var version = await client.GetVersionAsync();
            var networkMagic = version.Protocol.Network;

            var result = new ContractDeploymentInfo
            {
                ContractName = contract.Name,
                ContractHash = contractHash,
                TransactionHash = txHash,
                BlockIndex = blockCount,
                NetworkMagic = networkMagic,
                DeployedAt = DateTime.UtcNow,
                GasConsumed = invokeResult.GasConsumed,
                Success = true
            };

            _logger.LogInformation("Contract {ContractName} deployed successfully with hash {ContractHash}",
                contract.Name, result.ContractHash);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deploy contract {ContractName}", contract.Name);

            return new ContractDeploymentInfo
            {
                ContractName = contract.Name,
                Success = false,
                ErrorMessage = ex.Message,
                DeployedAt = DateTime.UtcNow
            };
        }
    }

    /// <summary>
    /// Update an existing contract
    /// </summary>
    /// <param name="contract">New contract version</param>
    /// <param name="contractHash">Hash of contract to update</param>
    /// <param name="options">Deployment options</param>
    /// <returns>Update result</returns>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null</exception>
    /// <exception cref="ContractDeploymentException">Thrown when update fails</exception>
    public async Task<ContractDeploymentInfo> UpdateAsync(CompiledContract contract, UInt160 contractHash, DeploymentOptions options)
    {
        if (contract == null) throw new ArgumentNullException(nameof(contract));
        if (contractHash == null) throw new ArgumentNullException(nameof(contractHash));
        if (options == null) throw new ArgumentNullException(nameof(options));

        _logger.LogInformation("Updating contract {ContractHash} with new version", contractHash);

        try
        {
            // Create RPC client using factory
            var client = _rpcClientFactory.CreateClient();

            // Check if contract exists
            var exists = await ContractExistsAsync(contractHash, _rpcClientFactory.GetRpcUrl());
            if (!exists)
            {
                throw new ContractDeploymentException(contract.Name, $"Contract {contractHash} does not exist on the network");
            }

            // Get current block count for ValidUntilBlock
            var blockCount = await client.GetBlockCountAsync();

            // Create update script using helper
            var script = ScriptBuilderHelper.BuildUpdateScript(
                contractHash,
                contract.NefBytes,
                contract.ManifestBytes);

            // Get deployer account from options or wallet manager
            var deployerAccount = options.DeployerAccount ?? _walletManager.GetAccount();

            // Create signer
            var signer = new Signer
            {
                Account = deployerAccount,
                Scopes = WitnessScope.CalledByEntry
            };

            // Test invoke first to get gas consumption
            var invokeResult = await client.InvokeScriptAsync(script, signer);
            if (invokeResult.State != VMState.HALT)
            {
                throw new ContractDeploymentException(contract.Name, invokeResult.Exception ?? "Update script failed");
            }

            // Get deployment configuration
            var deploymentConfig = _configuration.GetSection("Deployment").Get<DeploymentConfiguration>() ?? new DeploymentConfiguration();

            // Create transaction using builder
            var txOptions = new TransactionBuildOptions
            {
                Sender = deployerAccount,
                Script = script,
                SystemFee = invokeResult.GasConsumed,
                NetworkFee = deploymentConfig.DefaultNetworkFee,
                ValidUntilBlock = blockCount + deploymentConfig.ValidUntilBlockOffset
            };
            var tx = _transactionBuilder.Build(txOptions);

            // Sign transaction using wallet manager
            await _walletManager.SignTransactionAsync(tx, deployerAccount);

            // Send transaction
            var txHash = await client.SendRawTransactionAsync(tx);

            // Wait for confirmation if requested
            if (deploymentConfig.WaitForConfirmation)
            {
                var confirmOptions = new TransactionConfirmationOptions
                {
                    Timeout = TimeSpan.FromSeconds(deploymentConfig.ConfirmationRetries * deploymentConfig.ConfirmationDelaySeconds),
                    PollingInterval = TimeSpan.FromSeconds(deploymentConfig.ConfirmationDelaySeconds)
                };
                await _confirmationService.WaitForConfirmationAsync(client, txHash, confirmOptions);
            }

            // Get network magic from version info
            var version = await client.GetVersionAsync();
            var networkMagic = version.Protocol.Network;

            var result = new ContractDeploymentInfo
            {
                ContractName = contract.Name,
                ContractHash = contractHash, // Same hash for update
                TransactionHash = txHash,
                BlockIndex = blockCount,
                NetworkMagic = networkMagic,
                DeployedAt = DateTime.UtcNow,
                GasConsumed = invokeResult.GasConsumed,
                Success = true
            };

            _logger.LogInformation("Contract {ContractHash} updated successfully", contractHash);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update contract {ContractHash}", contractHash);

            return new ContractDeploymentInfo
            {
                ContractName = contract.Name,
                ContractHash = contractHash,
                Success = false,
                ErrorMessage = ex.Message,
                DeployedAt = DateTime.UtcNow
            };
        }
    }

    /// <summary>
    /// Check if a contract exists on the network
    /// </summary>
    /// <param name="contractHash">Contract hash to check</param>
    /// <param name="rpcUrl">RPC URL to connect to</param>
    /// <returns>True if contract exists, false otherwise</returns>
    /// <exception cref="ArgumentNullException">Thrown when contractHash is null</exception>
    /// <exception cref="ArgumentException">Thrown when rpcUrl is empty</exception>
    /// <exception cref="ContractDeploymentException">Thrown when check fails</exception>
    public async Task<bool> ContractExistsAsync(UInt160 contractHash, string rpcUrl)
    {
        if (contractHash == null) throw new ArgumentNullException(nameof(contractHash));
        if (string.IsNullOrWhiteSpace(rpcUrl)) throw new ArgumentException("RPC URL cannot be empty", nameof(rpcUrl));

        _logger.LogInformation("Checking if contract {ContractHash} exists on {RpcUrl}", contractHash, rpcUrl);

        try
        {
            var client = new RpcClient(new Uri(rpcUrl));

            // Try to get contract state
            var contractState = await client.GetContractStateAsync(contractHash.ToString());

            return contractState != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check contract existence for {ContractHash}", contractHash);
            throw new ContractDeploymentException("Contract", $"Failed to check contract existence: {ex.Message}", ex);
        }
    }
}
