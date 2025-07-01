using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.SmartContract;
using Neo.SmartContract.Deploy.Configuration;
using Neo.SmartContract.Deploy.Steps;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.Wallets;
using Neo.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Linq;

namespace Neo.SmartContract.Deploy.Services
{
    /// <summary>
    /// Core deployment service implementation
    /// </summary>
    public class DeploymentService : IDeploymentService
    {
        private readonly ILogger<DeploymentService> _logger;
        private readonly IBlockchainService _blockchain;
        private readonly IWalletService _wallet;
        private readonly IContractLoader _contractLoader;
        private readonly IDeploymentRecordService _recordService;
        private readonly IEnumerable<IDeploymentStep> _deploymentSteps;
        private readonly DeploymentOptions _options;
        private readonly NetworkOptions _networkOptions;

        public DeploymentService(
            ILogger<DeploymentService> logger,
            IBlockchainService blockchain,
            IWalletService wallet,
            IContractLoader contractLoader,
            IDeploymentRecordService recordService,
            IEnumerable<IDeploymentStep> deploymentSteps,
            IOptions<DeploymentOptions> options,
            IOptions<NetworkOptions> networkOptions)
        {
            _logger = logger;
            _blockchain = blockchain;
            _wallet = wallet;
            _contractLoader = contractLoader;
            _recordService = recordService;
            _deploymentSteps = deploymentSteps;
            _options = options.Value;
            _networkOptions = networkOptions.Value;
        }

        public async Task<DeploymentResult> DeployAllAsync()
        {
            var result = new DeploymentResult();
            var context = new DeploymentContext
            {
                Blockchain = _blockchain,
                Wallet = _wallet,
                ContractLoader = _contractLoader,
                DeploymentService = this
            };

            try
            {
                // Ensure wallet is unlocked
                await _wallet.UnlockWalletAsync();

                // Execute deployment steps in order
                foreach (var step in _deploymentSteps.OrderBy(s => s.Order))
                {
                    _logger.LogInformation("Executing deployment step: {StepName}", step.Name);
                    
                    if (!await step.ExecuteAsync(context))
                    {
                        throw new Exception($"Deployment step '{step.Name}' failed");
                    }
                }

                result.Success = true;
                result.DeployedContracts = context.DeployedContracts
                    .Select(kvp => new ContractDeploymentResult
                    {
                        Success = true,
                        Name = kvp.Key,
                        Hash = kvp.Value
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Deployment failed");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public async Task<ContractDeploymentResult> DeployContractAsync(
            string name, 
            byte[] nef, 
            ContractManifest manifest, 
            object[]? initParams = null)
        {
            try
            {
                _logger.LogInformation("Deploying contract: {Name}", name);

                var signer = _wallet.GetDefaultAccount();
                var client = _blockchain.GetRpcClient();

                // Build deployment script
                using var sb = new ScriptBuilder();
                // Build deployment parameters
                var deployParams = initParams != null && initParams.Length > 0 
                    ? new object[] { nef, manifest.ToJson().ToString(), initParams }
                    : new object[] { nef, manifest.ToJson().ToString() };
                ConvertArgs(sb, deployParams);
                sb.EmitPush(CallFlags.All);
                sb.EmitPush("deploy");
                sb.EmitPush(NativeContract.ContractManagement.Hash);
                sb.EmitSysCall(ApplicationEngine.System_Contract_Call);
                var script = sb.ToArray();

                // Estimate gas consumption
                var gasConsumed = await EstimateGasAsync(script, signer.ScriptHash);
                _logger.LogInformation("Estimated GAS consumption: {Gas}", gasConsumed);

                // Create and send transaction
                var tx = await _wallet.CreateTransactionAsync(script, signer.ScriptHash, gasConsumed);
                await _blockchain.SendTransactionAsync(tx);

                _logger.LogInformation("Contract deployment transaction sent: {TxId}", tx.Hash);

                // Calculate expected contract hash
                var contractHash = GetContractHash(signer.ScriptHash, nef, manifest);
                _logger.LogInformation("Expected contract hash: {Hash}", contractHash);

                // Wait for confirmation if configured
                if (_options.WaitForConfirmation)
                {
                    _logger.LogInformation("Waiting for transaction confirmation...");
                    await WaitForTransactionAsync(tx.Hash);
                }

                // Save deployment record
                var deploymentRecord = new DeploymentRecord
                {
                    ContractHash = contractHash,
                    TransactionHash = tx.Hash,
                    DeployedAt = DateTimeOffset.UtcNow,
                    DeployerAddress = signer.ScriptHash,
                    Version = "1.0.0",
                    NefHash = ComputeHash(nef),
                    ManifestHash = ComputeHash(System.Text.Encoding.UTF8.GetBytes(manifest.ToJson().ToString())),
                    Metadata = new Dictionary<string, string>
                    {
                        ["gasConsumed"] = gasConsumed.ToString(),
                        ["deploymentMethod"] = "toolkit"
                    }
                };

                await _recordService.SaveDeploymentAsync(name, _networkOptions.Network, deploymentRecord);
                _logger.LogInformation("Deployment record saved for {Name} on {Network}", name, _networkOptions.Network);

                return new ContractDeploymentResult
                {
                    Success = true,
                    Name = name,
                    Hash = contractHash,
                    TransactionId = tx.Hash,
                    GasConsumed = gasConsumed
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deploy contract {Name}", name);
                return new ContractDeploymentResult
                {
                    Success = false,
                    Name = name,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<UInt256> InvokeContractAsync(UInt160 contractHash, string method, params object[] parameters)
        {
            _logger.LogInformation("Invoking {Method} on contract {Hash}", method, contractHash);

            var signer = _wallet.GetDefaultAccount();
            
            // Build invocation script
            using var sb = new ScriptBuilder();
            ConvertArgs(sb, parameters);
            sb.EmitPush(CallFlags.All);
            sb.EmitPush(method);
            sb.EmitPush(contractHash);
            sb.EmitSysCall(ApplicationEngine.System_Contract_Call);
            var script = sb.ToArray();

            // Test invoke first for debugging
            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
            {
                var testResult = await _blockchain.TestInvokeAsync(script, new Signer { Account = signer.ScriptHash, Scopes = WitnessScope.CalledByEntry });
                Utilities.DebugHelper.PrintInvokeResult(_logger, testResult);
            }

            // Estimate gas and create transaction
            var gasConsumed = await EstimateGasAsync(script, signer.ScriptHash);
            var tx = await _wallet.CreateTransactionAsync(script, signer.ScriptHash, gasConsumed);
            
            // Print transaction details in debug mode
            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
            {
                Utilities.DebugHelper.PrintTransaction(_logger, tx);
            }
            
            await _blockchain.SendTransactionAsync(tx);
            
            _logger.LogInformation("Contract invocation transaction sent: {TxId}", tx.Hash);

            if (_options.WaitForConfirmation)
            {
                await WaitForTransactionAsync(tx.Hash);
            }

            return tx.Hash;
        }

        private async Task<long> EstimateGasAsync(byte[] script, UInt160 signer)
        {
            var client = _blockchain.GetRpcClient();
            var result = await client.InvokeScriptAsync(script, new[] { new Signer { Scopes = WitnessScope.CalledByEntry, Account = signer } });
            
            if (result.State != VMState.HALT)
            {
                throw new Exception($"Script execution failed: {result.Exception}");
            }

            return result.GasConsumed;
        }

        private async Task WaitForTransactionAsync(UInt256 txHash)
        {
            var client = _blockchain.GetRpcClient();
            var maxRetries = _options.ConfirmationRetries;
            var delay = TimeSpan.FromSeconds(_options.ConfirmationDelaySeconds);

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

        private static UInt160 GetContractHash(UInt160 sender, byte[] nefFile, ContractManifest manifest)
        {
            // NEF file has a header, we need to calculate checksum from the NEF structure
            var nef = NefFile.Parse(nefFile);
            return Helper.GetContractHash(sender, nef.CheckSum, manifest.Name);
        }

        private static string ComputeHash(byte[] data)
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