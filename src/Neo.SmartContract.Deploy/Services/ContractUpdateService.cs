using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Shared;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.Wallets;
using Neo.Cryptography.ECC;
using Neo.Json;

namespace Neo.SmartContract.Deploy.Services;

/// <summary>
/// Service for updating deployed smart contracts
/// </summary>
public class ContractUpdateService : IContractUpdateService
{
    private readonly ILogger<ContractUpdateService> _logger;
    private readonly IWalletManager _walletManager;

    public ContractUpdateService(ILogger<ContractUpdateService> logger, IWalletManager walletManager)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _walletManager = walletManager ?? throw new ArgumentNullException(nameof(walletManager));
    }

    /// <inheritdoc/>
    public async Task<ContractUpdateInfo> UpdateAsync(UInt160 contractHash, CompiledContract contract, UpdateOptions options, object[]? updateParams = null)
    {
        if (contractHash == null)
            throw new ArgumentNullException(nameof(contractHash));

        if (contract == null)
            throw new ArgumentNullException(nameof(contract));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrWhiteSpace(options.WifKey))
            throw new ArgumentException("WIF key is required for contract update", nameof(options));

        if (options.UpdateNefOnly && options.UpdateManifestOnly)
            throw new ArgumentException("Cannot set both UpdateNefOnly and UpdateManifestOnly", nameof(options));

        var updateInfo = new ContractUpdateInfo
        {
            ContractName = contract.Name,
            ContractHash = contractHash,
            UpdatedAt = DateTime.UtcNow,
            NetworkMagic = options.NetworkMagic,
            IsDryRun = options.DryRun,
            Timestamp = DateTimeOffset.UtcNow
        };

        try
        {
            _logger.LogInformation("Starting contract update for {ContractName} ({ContractHash})", 
                contract.Name, contractHash);

            // Create RPC client
            using var rpcClient = new RpcClient(new Uri(options.RpcUrl));

            // Check if contract exists
            var contractState = await rpcClient.GetContractStateAsync(contractHash.ToString());
            if (contractState == null)
            {
                throw new InvalidOperationException($"Contract {contractHash} not found on network");
            }

            // Store version information if available
            updateInfo.PreviousVersion = contractState.Manifest.Extra?["Version"]?.GetString();
            updateInfo.NewVersion = contract.Manifest?.Extra?["Version"]?.GetString();

            // Get updater account
            var privateKey = Neo.Wallets.Wallet.GetPrivateKeyFromWIF(options.WifKey);
            var keyPair = new KeyPair(privateKey);
            var accountContract = Neo.SmartContract.Contract.CreateSignatureContract(keyPair.PublicKey);
            var account = accountContract.ScriptHash;
            updateInfo.UpdaterAddress = account.ToAddress(Neo.ProtocolSettings.Default.AddressVersion);

            // Check if contract can be updated
            var canUpdate = await CanUpdateAsync(contractHash, options.RpcUrl);
            if (!canUpdate)
            {
                throw new InvalidOperationException($"Contract {contractHash} cannot be updated. Check update permissions in manifest.");
            }

            // Prepare update parameters
            byte[]? nefBytes = null;
            byte[]? manifestBytes = null;

            if (!options.UpdateManifestOnly && contract.NefBytes != null && contract.NefBytes.Length > 0)
            {
                nefBytes = contract.NefBytes;
            }

            if (!options.UpdateNefOnly && contract.Manifest != null)
            {
                manifestBytes = System.Text.Encoding.UTF8.GetBytes(contract.Manifest.ToJson().ToString());
            }

            // Build update script
            var updateScript = ScriptBuilderHelper.BuildUpdateScript(nefBytes, manifestBytes, updateParams);

            // Create signers
            var signers = new[]
            {
                new Signer
                {
                    Account = account,
                    Scopes = WitnessScope.CalledByEntry,
                    AllowedContracts = new[] { contractHash, NativeContract.ContractManagement.Hash },
                    AllowedGroups = Array.Empty<ECPoint>()
                }
            }.Concat(options.AdditionalSigners ?? Array.Empty<Signer>()).ToArray();

            // Build transaction
            var tx = await BuildUpdateTransactionAsync(rpcClient, updateScript, signers, options);

            // Sign transaction
            // keyPair already created above
            var signature = tx.Sign(keyPair, options.NetworkMagic);
            tx.Witnesses = new[] { new Witness { InvocationScript = signature, VerificationScript = accountContract.Script } };

            if (options.DryRun)
            {
                _logger.LogInformation("Dry run mode - simulating contract update");
                
                // Simulate the transaction
                var result = await rpcClient.InvokeScriptAsync(updateScript, signers);
                
                updateInfo.GasConsumed = result.GasConsumed;
                updateInfo.Success = result.State == VMState.HALT;
                
                if (!updateInfo.Success)
                {
                    updateInfo.ErrorMessage = $"Update simulation failed: {result.Exception}";
                    _logger.LogError("Contract update simulation failed: {Error}", result.Exception);
                }
                else
                {
                    _logger.LogInformation("Contract update simulation successful. Gas consumed: {GasConsumed}", result.GasConsumed);
                }
                
                return updateInfo;
            }

            // Send transaction
            _logger.LogInformation("Sending update transaction...");
            var txHash = await rpcClient.SendRawTransactionAsync(tx);
            updateInfo.TransactionHash = txHash;

            _logger.LogInformation("Update transaction sent. TxHash: {TxHash}", txHash);

            // Wait for confirmation if requested
            if (options.WaitForConfirmation)
            {
                _logger.LogInformation("Waiting for transaction confirmation...");
                
                var confirmed = await WaitForTransactionConfirmationAsync(rpcClient, txHash, options.ConfirmationTimeout);
                
                if (confirmed)
                {
                    var application = await rpcClient.GetApplicationLogAsync(txHash.ToString());
                    
                    if (application != null)
                    {
                        updateInfo.BlockIndex = application.BlockHash != null ? 
                            (await rpcClient.GetBlockAsync(application.BlockHash.ToString()))?.Block?.Index ?? 0 : 0;
                        updateInfo.GasConsumed = application.Executions?[0].GasConsumed ?? 0;
                        updateInfo.Success = application.Executions?[0].VMState == VMState.HALT;
                        
                        if (!updateInfo.Success)
                        {
                            updateInfo.ErrorMessage = application.Executions?[0].ExceptionMessage;
                            _logger.LogError("Contract update failed: {Error}", updateInfo.ErrorMessage);
                        }
                    }
                }
                else
                {
                    updateInfo.Success = false;
                    updateInfo.ErrorMessage = "Transaction confirmation timeout";
                    _logger.LogWarning("Update transaction confirmation timeout");
                }
            }
            else
            {
                // Assume success if not waiting for confirmation
                updateInfo.Success = true;
            }

            // Verify update if requested and successful
            if (updateInfo.Success && options.VerifyAfterUpdate && options.WaitForConfirmation)
            {
                _logger.LogInformation("Verifying contract update...");
                
                try
                {
                    var updatedContract = await rpcClient.GetContractStateAsync(contractHash.ToString());
                    
                    if (updatedContract == null)
                    {
                        updateInfo.VerificationFailed = true;
                        _logger.LogWarning("Contract verification failed - contract not found after update");
                    }
                    else
                    {
                        // Verify update was applied
                        bool updateApplied = true;
                        
                        if (nefBytes != null)
                        {
                            // Compare NEF bytes
                            updateApplied &= updatedContract.Script.Span.SequenceEqual(nefBytes);
                        }
                        
                        if (manifestBytes != null && contract.Manifest != null)
                        {
                            var updatedManifestJson = updatedContract.Manifest.ToJson().ToString();
                            var expectedManifestJson = contract.Manifest.ToJson().ToString();
                            updateApplied &= updatedManifestJson == expectedManifestJson;
                        }
                        
                        if (!updateApplied)
                        {
                            updateInfo.VerificationFailed = true;
                            _logger.LogWarning("Contract verification failed - update not fully applied");
                        }
                        else
                        {
                            _logger.LogInformation("Contract update verified successfully");
                        }
                    }
                }
                catch (Exception ex)
                {
                    updateInfo.VerificationFailed = true;
                    _logger.LogError(ex, "Error during contract verification");
                }
            }

            // Set fees
            updateInfo.NetworkFee = tx.NetworkFee;
            updateInfo.SystemFee = tx.SystemFee;

            if (updateInfo.Success)
            {
                _logger.LogInformation("Contract {ContractName} updated successfully. TxHash: {TxHash}", 
                    contract.Name, updateInfo.TransactionHash);
            }

            return updateInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contract {ContractName}", contract.Name);
            
            updateInfo.Success = false;
            updateInfo.ErrorMessage = ex.Message;
            
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> CanUpdateAsync(UInt160 contractHash, string rpcUrl)
    {
        try
        {
            using var rpcClient = new RpcClient(new Uri(rpcUrl));
            
            // Get contract state
            var contractState = await rpcClient.GetContractStateAsync(contractHash.ToString());
            if (contractState == null)
            {
                return false;
            }

            // Check if contract has update method in manifest
            var updateMethod = contractState.Manifest.Abi.Methods?.FirstOrDefault(m => m.Name == "update");
            if (updateMethod == null)
            {
                // Contract doesn't have explicit update method, check permissions
                var updatePermission = contractState.Manifest.Permissions?.Any(p => 
                    p.Contract.IsWildcard && p.Methods.IsWildcard) ?? false;
                
                if (!updatePermission)
                {
                    updatePermission = contractState.Manifest.Permissions?.Any(p =>
                        (p.Contract.IsHash && p.Contract.Hash == NativeContract.ContractManagement.Hash) &&
                        (p.Methods.IsWildcard || p.Methods.Contains("update"))) ?? false;
                }
                
                return updatePermission;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if contract {ContractHash} can be updated", contractHash);
            return false;
        }
    }

    private async Task<Transaction> BuildUpdateTransactionAsync(RpcClient rpcClient, byte[] script, Signer[] signers, UpdateOptions options)
    {
        // Get current block for transaction validity
        var currentHeight = await rpcClient.GetBlockCountAsync();
        var currentBlock = await rpcClient.GetBlockAsync((currentHeight - 1).ToString());
        
        // Create transaction
        var tx = new Transaction
        {
            Version = 0,
            Nonce = (uint)new Random().Next(),
            SystemFee = options.GasLimit,
            NetworkFee = 0,
            ValidUntilBlock = currentHeight + 100, // Valid for next 100 blocks
            Signers = signers,
            Attributes = Array.Empty<TransactionAttribute>(),
            Script = script,
            Witnesses = Array.Empty<Witness>()
        };

        // Calculate network fee
        var txSize = tx.Size;
        var feePerByte = 1000; // Default fee per byte
        var executionFee = 1000000; // Base execution fee
        
        tx.NetworkFee = (long)(((txSize + 200) * feePerByte + executionFee) * options.NetworkFeeMultiplier);
        
        // Apply max network fee limit
        if (tx.NetworkFee > options.MaxNetworkFee)
        {
            tx.NetworkFee = options.MaxNetworkFee;
        }

        return tx;
    }

    private async Task<bool> WaitForTransactionConfirmationAsync(RpcClient rpcClient, UInt256 txHash, int timeoutSeconds)
    {
        var endTime = DateTime.UtcNow.AddSeconds(timeoutSeconds);
        
        while (DateTime.UtcNow < endTime)
        {
            try
            {
                var tx = await rpcClient.GetRawTransactionAsync(txHash.ToString());
                if (tx != null && tx.Confirmations > 0)
                {
                    return true;
                }
            }
            catch
            {
                // Transaction might not be available yet
            }

            await Task.Delay(1000); // Wait 1 second before retry
        }

        return false;
    }
}