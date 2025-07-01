using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.SmartContract.Deploy.Configuration;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.Wallets;
using System.Security.Cryptography;
using System.Text;

namespace Neo.SmartContract.Deploy.Services
{
    /// <summary>
    /// Implementation of contract update service
    /// </summary>
    public class ContractUpdateService : IContractUpdateService
    {
        private readonly ILogger<ContractUpdateService> _logger;
        private readonly IBlockchainService _blockchain;
        private readonly IWalletService _wallet;
        private readonly IContractLoader _contractLoader;
        private readonly IDeploymentRecordService _recordService;
        private readonly NetworkOptions _networkOptions;
        private readonly DeploymentOptions _deploymentOptions;

        public ContractUpdateService(
            ILogger<ContractUpdateService> logger,
            IBlockchainService blockchain,
            IWalletService wallet,
            IContractLoader contractLoader,
            IDeploymentRecordService recordService,
            IOptions<NetworkOptions> networkOptions,
            IOptions<DeploymentOptions> deploymentOptions)
        {
            _logger = logger;
            _blockchain = blockchain;
            _wallet = wallet;
            _contractLoader = contractLoader;
            _recordService = recordService;
            _networkOptions = networkOptions.Value;
            _deploymentOptions = deploymentOptions.Value;
        }

        public async Task<ContractUpdateResult> UpdateContractAsync(
            string contractName,
            string network,
            byte[] nef,
            ContractManifest manifest,
            object? updateData = null,
            string? version = null)
        {
            var result = new ContractUpdateResult
            {
                ContractName = contractName,
                Timestamp = DateTimeOffset.UtcNow
            };

            try
            {
                // Check if contract can be updated
                var checkResult = await CheckUpdateAsync(contractName, network);
                if (!checkResult.CanUpdate)
                {
                    result.Success = false;
                    result.ErrorMessage = checkResult.Reason ?? "Contract cannot be updated";
                    return result;
                }

                result.ContractHash = checkResult.ContractHash;
                result.PreviousVersion = checkResult.CurrentVersion;
                result.NewVersion = version ?? IncrementVersion(checkResult.CurrentVersion ?? "1.0.0");

                _logger.LogInformation("Updating contract {Name} on {Network} from v{OldVersion} to v{NewVersion}",
                    contractName, network, result.PreviousVersion, result.NewVersion);

                // Get deployer account
                var signer = _wallet.GetDefaultAccount();

                // Build update script
                using var sb = new ScriptBuilder();
                // Build update parameters
                var updateParams = updateData != null 
                    ? new object[] { nef, manifest.ToJson().ToString(), updateData }
                    : new object[] { nef, manifest.ToJson().ToString() };
                ConvertArgs(sb, updateParams);
                sb.EmitPush(CallFlags.All);
                sb.EmitPush("update");
                sb.EmitPush(checkResult.ContractHash!);
                sb.EmitSysCall(ApplicationEngine.System_Contract_Call);
                var script = sb.ToArray();

                // Estimate gas
                var testResult = await _blockchain.TestInvokeAsync(
                    script,
                    new Signer { Account = signer.ScriptHash, Scopes = WitnessScope.CalledByEntry }
                );

                if (testResult.State != VMState.HALT)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Update simulation failed: {testResult.Exception}";
                    return result;
                }

                // Create and send transaction
                var tx = await _wallet.CreateTransactionAsync(script, signer.ScriptHash, testResult.GasConsumed);
                await _blockchain.SendTransactionAsync(tx);

                result.TransactionHash = tx.Hash;
                _logger.LogInformation("Contract update transaction sent: {TxId}", tx.Hash);

                // Wait for confirmation if configured
                if (_deploymentOptions.WaitForConfirmation)
                {
                    await WaitForTransactionAsync(tx.Hash);
                }

                // Update deployment record
                var deploymentRecord = await _recordService.GetDeploymentAsync(contractName, network);
                if (deploymentRecord != null)
                {
                    deploymentRecord.Version = result.NewVersion;
                    deploymentRecord.NefHash = ComputeHash(nef);
                    deploymentRecord.ManifestHash = ComputeHash(Encoding.UTF8.GetBytes(manifest.ToJson().ToString()));
                    
                    deploymentRecord.UpdateHistory.Add(new UpdateRecord
                    {
                        TransactionHash = tx.Hash,
                        UpdatedAt = result.Timestamp,
                        PreviousVersion = result.PreviousVersion!,
                        NewVersion = result.NewVersion,
                        Description = $"Updated via deployment toolkit"
                    });

                    await _recordService.SaveDeploymentAsync(contractName, network, deploymentRecord);
                }

                result.Success = true;
                _logger.LogInformation("Contract {Name} updated successfully on {Network}", contractName, network);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update contract {Name} on {Network}", contractName, network);
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public async Task<UpdateCheckResult> CheckUpdateAsync(string contractName, string network)
        {
            var result = new UpdateCheckResult();

            try
            {
                // Check if contract is deployed
                var deployment = await _recordService.GetDeploymentAsync(contractName, network);
                if (deployment == null)
                {
                    result.CanUpdate = false;
                    result.IsDeployed = false;
                    result.Reason = "Contract not found in deployment records";
                    return result;
                }

                result.IsDeployed = true;
                result.ContractHash = deployment.ContractHash;
                result.CurrentVersion = deployment.Version;

                // Verify contract exists on chain
                var client = _blockchain.GetRpcClient();
                var contractState = await client.GetContractStateAsync(deployment.ContractHash.ToString());
                
                if (contractState == null)
                {
                    result.CanUpdate = false;
                    result.Reason = "Contract not found on blockchain";
                    return result;
                }

                // Check if contract has update method
                var updateMethod = contractState.Manifest.Abi.Methods.FirstOrDefault(m => m.Name == "update");
                result.HasUpdateMethod = updateMethod != null;

                if (!result.HasUpdateMethod)
                {
                    result.CanUpdate = false;
                    result.Reason = "Contract does not have an update method";
                    return result;
                }

                // Verify network matches
                if (!network.Equals(_networkOptions.Network, StringComparison.OrdinalIgnoreCase))
                {
                    result.CanUpdate = false;
                    result.Reason = $"Network mismatch: deployment record is for {network}, but current network is {_networkOptions.Network}";
                    return result;
                }

                result.CanUpdate = true;
                result.Reason = "Contract can be updated";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check update status for {Contract} on {Network}", contractName, network);
                result.CanUpdate = false;
                result.Reason = $"Error checking contract: {ex.Message}";
            }

            return result;
        }

        public async Task<BatchUpdateResult> UpdateContractsAsync(string network, params string[] contractNames)
        {
            var result = new BatchUpdateResult();

            // Ensure wallet is unlocked
            await _wallet.UnlockWalletAsync();

            foreach (var contractName in contractNames)
            {
                try
                {
                    _logger.LogInformation("Updating contract {Name} ({Index}/{Total})", 
                        contractName, result.Updates.Count + 1, contractNames.Length);

                    // Load contract files
                    var contract = await _contractLoader.LoadContractAsync(contractName);
                    
                    // Update contract
                    var updateResult = await UpdateContractAsync(
                        contractName,
                        network,
                        contract.NefBytes,
                        contract.Manifest
                    );

                    result.Updates.Add(updateResult);

                    if (!updateResult.Success)
                    {
                        _logger.LogWarning("Failed to update {Name}: {Error}", 
                            contractName, updateResult.ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update contract {Name}", contractName);
                    result.Updates.Add(new ContractUpdateResult
                    {
                        Success = false,
                        ContractName = contractName,
                        ErrorMessage = ex.Message,
                        Timestamp = DateTimeOffset.UtcNow
                    });
                }
            }

            result.Success = result.SuccessCount > 0 && result.FailureCount == 0;
            return result;
        }

        private async Task WaitForTransactionAsync(UInt256 txHash)
        {
            var client = _blockchain.GetRpcClient();
            var maxRetries = _deploymentOptions.ConfirmationRetries;
            var delay = TimeSpan.FromSeconds(_deploymentOptions.ConfirmationDelaySeconds);

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var tx = await client.GetRawTransactionAsync(txHash.ToString());
                    if (tx != null)
                    {
                        _logger.LogInformation("Transaction confirmed in block: {Block}", tx.BlockHash);
                        return;
                    }
                }
                catch { }

                await Task.Delay(delay);
            }

            throw new TimeoutException($"Transaction {txHash} was not confirmed within the timeout period");
        }

        private string IncrementVersion(string version)
        {
            var parts = version.Split('.');
            if (parts.Length != 3)
            {
                return "1.0.1";
            }

            if (int.TryParse(parts[2], out var patch))
            {
                return $"{parts[0]}.{parts[1]}.{patch + 1}";
            }

            return "1.0.1";
        }

        private string ComputeHash(byte[] data)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(data);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        private static void ConvertArgs(ScriptBuilder script, object[] args)
        {
            if (args is null || args.Length == 0)
                script.Emit(OpCode.NEWARRAY0);
            else
            {
                for (int i = args.Length - 1; i >= 0; i--)
                {
                    script.EmitPush(args[i]);
                }
                script.EmitPush(args.Length);
                script.Emit(OpCode.PACK);
            }
        }
    }
}