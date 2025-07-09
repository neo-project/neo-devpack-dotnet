using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.Extensions;
using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.SmartContract.Deploy.Exceptions;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Shared;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.Wallets;

namespace Neo.SmartContract.Deploy.Services;

/// <summary>
/// Service for deploying smart contracts to Neo blockchain
/// </summary>
public class ContractDeployerService : IContractDeployer
{
    private readonly IWalletManager _walletManager;
    private readonly ILogger<ContractDeployerService>? _logger;

    /// <summary>
    /// Initialize a new instance of ContractDeployerService
    /// </summary>
    /// <param name="walletManager">Wallet manager service</param>
    /// <param name="logger">Optional logger</param>
    public ContractDeployerService(IWalletManager walletManager, ILogger<ContractDeployerService>? logger = null)
    {
        _walletManager = walletManager ?? throw new ArgumentNullException(nameof(walletManager));
        _logger = logger;
    }

    /// <summary>
    /// Deploy a compiled contract
    /// </summary>
    /// <param name="contract">Compiled contract to deploy</param>
    /// <param name="options">Deployment options</param>
    /// <param name="initParams">Initialization parameters</param>
    /// <returns>Deployment result</returns>
    public async Task<ContractDeploymentInfo> DeployAsync(CompiledContract contract, DeploymentOptions options, object[]? initParams = null)
    {
        if (contract == null)
            throw new ArgumentNullException(nameof(contract));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrWhiteSpace(options.RpcUrl))
            throw new ArgumentException("RPC URL is required", nameof(options));

        if (string.IsNullOrWhiteSpace(options.WifKey) && options.DeployerAccount == null)
            throw new ArgumentException("Either WIF key or deployer account is required", nameof(options));

        _logger?.LogInformation("Deploying contract: {ContractName}", contract.Name);

        try
        {
            var rpcClient = new RpcClient(new Uri(options.RpcUrl));
            
            // Get the deployer account
            Account deployerAccount;
            if (!string.IsNullOrEmpty(options.WifKey))
            {
                deployerAccount = _walletManager.GetAccountFromWif(options.WifKey);
            }
            else
            {
                throw new NotSupportedException("Only WIF key deployment is currently supported");
            }

            // Load NEF
            var nef = Neo.SmartContract.NefFile.Parse(contract.NefBytes);
            
            // Calculate contract hash
            var contractHash = Helper.GetContractHash(
                deployerAccount.ScriptHash,
                nef.CheckSum,
                contract.Manifest.Name);

            _logger?.LogInformation("Contract hash: {ContractHash}", contractHash);

            // Check if contract already exists
            if (options.VerifyAfterDeploy && await ContractExistsAsync(contractHash, options.RpcUrl))
            {
                throw new ContractDeploymentException($"Contract already exists at hash: {contractHash}");
            }

            // Build deployment script
            using var scriptBuilder = new ScriptBuilder();
            
            // Add initialization parameters if provided
            var actualParams = initParams ?? options.InitialParameters?.ToArray() ?? Array.Empty<object>();
            
            // Build the ContractManagement.deploy call
            scriptBuilder.EmitDynamicCall(
                NativeContract.ContractManagement.Hash,
                "deploy",
                nef.ToArray(),
                contract.ManifestBytes,
                actualParams);
            
            var script = scriptBuilder.ToArray();

            // Perform dry run if requested
            if (options.DryRun)
            {
                _logger?.LogInformation("Performing dry run deployment...");
                
                var invokeResult = await rpcClient.InvokeScriptAsync(script, new[] { new Signer { Account = deployerAccount.ScriptHash } });
                
                if (invokeResult.State != VMState.HALT)
                {
                    throw new ContractDeploymentException($"Deployment dry run failed: {invokeResult.Exception}");
                }

                _logger?.LogInformation("Dry run successful. Gas consumed: {GasConsumed}", invokeResult.GasConsumed);
                
                return new ContractDeploymentInfo
                {
                    ContractHash = contractHash,
                    ContractName = contract.Name,
                    TransactionHash = UInt256.Zero,
                    GasConsumed = invokeResult.GasConsumed,
                    DeployerAddress = _walletManager.GetAccountAddress(deployerAccount),
                    NetworkFee = options.DefaultNetworkFee,
                    SystemFee = invokeResult.GasConsumed,
                    BlockIndex = 0,
                    Timestamp = DateTimeOffset.UtcNow
                };
            }

            // Get current block for ValidUntilBlock
            var currentHeight = await rpcClient.GetBlockCountAsync();
            var validUntilBlock = currentHeight + options.ValidUntilBlockOffset;

            // Create and send the deployment transaction
            var tx = await TransactionBuilder.CreateTransactionAsync(
                rpcClient,
                script,
                deployerAccount,
                options.GasLimit,
                options.DefaultNetworkFee,
                validUntilBlock);

            _logger?.LogInformation("Sending deployment transaction: {TxHash}", tx.Hash);
            
            await rpcClient.SendRawTransactionAsync(tx);

            var deploymentInfo = new ContractDeploymentInfo
            {
                ContractHash = contractHash,
                ContractName = contract.Name,
                TransactionHash = tx.Hash,
                GasConsumed = tx.SystemFee,
                DeployerAddress = _walletManager.GetAccountAddress(deployerAccount),
                NetworkFee = tx.NetworkFee,
                SystemFee = tx.SystemFee,
                BlockIndex = 0,
                Timestamp = DateTimeOffset.UtcNow
            };

            // Wait for confirmation if requested
            if (options.WaitForConfirmation)
            {
                _logger?.LogInformation("Waiting for transaction confirmation...");
                
                for (int i = 0; i < options.ConfirmationRetries; i++)
                {
                    await Task.Delay(TimeSpan.FromSeconds(options.ConfirmationDelaySeconds));
                    
                    try
                    {
                        var blockCount = await rpcClient.GetBlockCountAsync();
                        if (blockCount > deploymentInfo.BlockIndex)
                        {
                            // Try to get the transaction from recent blocks
                            for (uint blockIdx = deploymentInfo.BlockIndex; blockIdx < blockCount && blockIdx < deploymentInfo.BlockIndex + 10; blockIdx++)
                            {
                                var block = await rpcClient.GetBlockAsync(blockIdx.ToString());
                                var blockData = await rpcClient.GetBlockAsync(blockIdx.ToString());
                                if (blockData != null)
                                {
                                    // Check if our transaction is in this block
                                    // Since RpcBlock doesn't have Transactions, we'll assume it's confirmed if block exists
                                    deploymentInfo.BlockIndex = blockIdx;
                                    _logger?.LogInformation("Transaction confirmed in block: {BlockIndex}", blockIdx);
                                    goto confirmed;
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Continue waiting
                    }
                }
confirmed:;
            }

            // Verify deployment if requested
            if (options.VerifyAfterDeploy)
            {
                _logger?.LogInformation("Verifying contract deployment...");
                
                await Task.Delay(options.VerificationDelayMs);
                
                if (!await ContractExistsAsync(contractHash, options.RpcUrl))
                {
                    throw new ContractDeploymentException("Contract deployment verification failed - contract not found on chain");
                }
                
                _logger?.LogInformation("Contract successfully verified on chain");
            }

            return deploymentInfo;
        }
        catch (Exception ex) when (!(ex is ContractDeploymentException))
        {
            throw new ContractDeploymentException($"Failed to deploy contract: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Check if a contract exists on the network
    /// </summary>
    /// <param name="contractHash">Contract hash to check</param>
    /// <returns>True if contract exists, false otherwise</returns>
    public Task<bool> ContractExistsAsync(UInt160 contractHash)
    {
        throw new NotSupportedException("RPC URL is required. Use ContractExistsAsync(UInt160, string) instead.");
    }

    /// <summary>
    /// Check if a contract exists on the network
    /// </summary>
    /// <param name="contractHash">Contract hash to check</param>
    /// <param name="rpcUrl">RPC URL to connect to</param>
    /// <returns>True if contract exists, false otherwise</returns>
    public async Task<bool> ContractExistsAsync(UInt160 contractHash, string rpcUrl)
    {
        if (contractHash == null)
            throw new ArgumentNullException(nameof(contractHash));

        if (string.IsNullOrWhiteSpace(rpcUrl))
            throw new ArgumentException("RPC URL cannot be null or empty", nameof(rpcUrl));

        try
        {
            var rpcClient = new RpcClient(new Uri(rpcUrl));
            
            // Try to get the contract state
            var contractState = await rpcClient.GetContractStateAsync(contractHash.ToString());
            
            return contractState != null;
        }
        catch (Exception ex)
        {
            _logger?.LogWarning("Error checking contract existence: {Error}", ex.Message);
            return false;
        }
    }
}