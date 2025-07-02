using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.Extensions;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Exceptions;
using Neo.VM;
using Neo.Wallets;
using Neo.Cryptography.ECC;
using System.Numerics;

namespace Neo.SmartContract.Deploy.Services;

/// <summary>
/// Contract deployment service implementation
/// </summary>
public class ContractDeployerService : IContractDeployer
{
    private readonly ILogger<ContractDeployerService> _logger;
    private readonly IWalletManager _walletManager;
    private readonly IConfiguration _configuration;

    public ContractDeployerService(ILogger<ContractDeployerService> logger, IWalletManager walletManager, IConfiguration configuration)
    {
        _logger = logger;
        _walletManager = walletManager;
        _configuration = configuration;
    }

    /// <summary>
    /// Deploy a compiled contract
    /// </summary>
    public async Task<ContractDeploymentInfo> DeployAsync(CompiledContract contract, DeploymentOptions options, object[]? initParams = null)
    {
        if (contract == null) throw new ArgumentNullException(nameof(contract));
        if (options == null) throw new ArgumentNullException(nameof(options));
        
        var networkConfig = _configuration.GetSection("Network").Get<NetworkConfiguration>();
        if (networkConfig == null || string.IsNullOrEmpty(networkConfig.RpcUrl))
        {
            throw new InvalidOperationException("Network configuration not found. Please check appsettings.json");
        }

        _logger.LogInformation("Deploying contract {ContractName} to network {RpcUrl}",
            contract.Name, networkConfig.RpcUrl);

        try
        {
            // Create RPC client
            var client = new RpcClient(new Uri(networkConfig.RpcUrl));

            // Get current block count for ValidUntilBlock
            var blockCount = await client.GetBlockCountAsync();

            // Create deployment script
            using var sb = new ScriptBuilder();

            // Add initialization parameters if provided
            if (initParams != null && initParams.Length > 0)
            {
                for (int i = initParams.Length - 1; i >= 0; i--)
                {
                    sb.EmitPush(initParams[i]);
                }
                sb.EmitPush(initParams.Length);
                sb.Emit(OpCode.PACK);
            }
            else
            {
                sb.Emit(OpCode.NEWARRAY0);
            }

            // Deploy contract using ContractManagement.Deploy
            sb.EmitPush(contract.ManifestBytes);
            sb.EmitPush(contract.NefBytes);
            sb.EmitPush("deploy");
            sb.EmitPush(NativeContract.ContractManagement.Hash);
            sb.EmitPush(CallFlags.All);
            sb.EmitSysCall(ApplicationEngine.System_Contract_Call);

            var script = sb.ToArray();

            // Get deployer account from options or wallet manager
            var deployerAccount = options.DeployerAccount ?? _walletManager.GetAccount();

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
                if (deployResult is Neo.VM.Types.Array array && array.Count > 0)
                {
                    contractHash = new UInt160(array[1].GetSpan()); // Hash is second element
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

            // Create transaction
            var gasConsumed = invokeResult.GasConsumed;
            var networkFee = deploymentConfig.DefaultNetworkFee;
            var validUntilBlock = blockCount + deploymentConfig.ValidUntilBlockOffset;

            var tx = new Transaction
            {
                Version = 0,
                Nonce = (uint)new Random().Next(),
                SystemFee = gasConsumed,
                NetworkFee = networkFee,
                ValidUntilBlock = validUntilBlock,
                Signers = new[] { signer },
                Attributes = Array.Empty<TransactionAttribute>(),
                Script = script,
                Witnesses = Array.Empty<Witness>()
            };

            // Sign transaction using wallet manager
            await _walletManager.SignTransactionAsync(tx, deployerAccount);

            // Send transaction
            var txHash = await client.SendRawTransactionAsync(tx);

            // Wait for confirmation if requested
            if (deploymentConfig.WaitForConfirmation)
            {
                await WaitForTransactionConfirmationAsync(client, txHash, deploymentConfig);
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
                GasConsumed = gasConsumed,
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
    public async Task<ContractDeploymentInfo> UpdateAsync(CompiledContract contract, UInt160 contractHash, DeploymentOptions options)
    {
        if (contract == null) throw new ArgumentNullException(nameof(contract));
        if (contractHash == null) throw new ArgumentNullException(nameof(contractHash));
        if (options == null) throw new ArgumentNullException(nameof(options));
        
        var networkConfig = _configuration.GetSection("Network").Get<NetworkConfiguration>();
        if (networkConfig == null || string.IsNullOrEmpty(networkConfig.RpcUrl))
        {
            throw new InvalidOperationException("Network configuration not found. Please check appsettings.json");
        }

        _logger.LogInformation("Updating contract {ContractHash} with new version", contractHash);

        try
        {
            // Create RPC client
            var client = new RpcClient(new Uri(networkConfig.RpcUrl));

            // Check if contract exists
            var exists = await ContractExistsAsync(contractHash, networkConfig.RpcUrl);
            if (!exists)
            {
                throw new ContractDeploymentException(contract.Name, $"Contract {contractHash} does not exist on the network");
            }

            // Get current block count for ValidUntilBlock
            var blockCount = await client.GetBlockCountAsync();

            // Create update script using ContractManagement.Update
            using var sb = new ScriptBuilder();

            // Update contract parameters
            sb.EmitPush(contract.ManifestBytes);
            sb.EmitPush(contract.NefBytes);
            sb.EmitPush("update");
            sb.EmitPush(contractHash);
            sb.EmitPush(CallFlags.All);
            sb.EmitSysCall(ApplicationEngine.System_Contract_Call);

            var script = sb.ToArray();

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

            // Create transaction
            var gasConsumed = invokeResult.GasConsumed;
            var networkFee = deploymentConfig.DefaultNetworkFee;
            var validUntilBlock = blockCount + deploymentConfig.ValidUntilBlockOffset;

            var tx = new Transaction
            {
                Version = 0,
                Nonce = (uint)new Random().Next(),
                SystemFee = gasConsumed,
                NetworkFee = networkFee,
                ValidUntilBlock = validUntilBlock,
                Signers = new[] { signer },
                Attributes = Array.Empty<TransactionAttribute>(),
                Script = script,
                Witnesses = Array.Empty<Witness>()
            };

            // Sign transaction using wallet manager
            await _walletManager.SignTransactionAsync(tx, deployerAccount);

            // Send transaction
            var txHash = await client.SendRawTransactionAsync(tx);

            // Wait for confirmation if requested
            if (deploymentConfig.WaitForConfirmation)
            {
                await WaitForTransactionConfirmationAsync(client, txHash, deploymentConfig);
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
                GasConsumed = gasConsumed,
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
    public async Task<bool> ContractExistsAsync(UInt160 contractHash, string rpcUrl)
    {
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
            return false;
        }
    }

    private async Task WaitForTransactionConfirmationAsync(RpcClient client, UInt256 txHash, DeploymentConfiguration deploymentConfig)
    {
        for (int i = 0; i < deploymentConfig.ConfirmationRetries; i++)
        {
            try
            {
                var tx = await client.GetRawTransactionAsync(txHash.ToString());
                if (tx != null)
                {
                    _logger.LogInformation("Transaction {TxHash} confirmed", txHash);
                    return;
                }
            }
            catch { }

            await Task.Delay(TimeSpan.FromSeconds(deploymentConfig.ConfirmationDelaySeconds));
        }

        throw new ContractDeploymentException("Transaction", $"Transaction {txHash} was not confirmed after {deploymentConfig.ConfirmationRetries} attempts");
    }

}
