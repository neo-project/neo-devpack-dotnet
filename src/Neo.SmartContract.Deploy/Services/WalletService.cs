using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo;
using Neo.Cryptography.ECC;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.SmartContract;
using Neo.SmartContract.Deploy.Configuration;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.Wallets;
using Neo.Wallets.NEP6;

namespace Neo.SmartContract.Deploy.Services
{
    /// <summary>
    /// Implementation of wallet management service
    /// </summary>
    public class WalletService : IWalletService
    {
        private readonly ILogger<WalletService> _logger;
        private readonly IBlockchainService _blockchain;
        private readonly NetworkOptions _networkOptions;
        private readonly DeploymentOptions _deploymentOptions;
        private NEP6Wallet? _wallet;
        private WalletAccount? _defaultAccount;

        public WalletService(
            ILogger<WalletService> logger, 
            IBlockchainService blockchain, 
            IOptions<NetworkOptions> networkOptions,
            IOptions<DeploymentOptions> deploymentOptions)
        {
            _logger = logger;
            _blockchain = blockchain;
            _networkOptions = networkOptions.Value;
            _deploymentOptions = deploymentOptions.Value;
        }

        public async Task<bool> UnlockWalletAsync()
        {
            try
            {
                // Get network-specific wallet configuration
                var walletConfig = _networkOptions.Wallet;
                if (walletConfig == null)
                {
                    throw new InvalidOperationException("No wallet configuration found for current network");
                }

                // Load wallet if not already loaded
                if (_wallet == null)
                {
                    if (!File.Exists(walletConfig.WalletPath))
                    {
                        throw new FileNotFoundException($"Wallet file not found: {walletConfig.WalletPath}");
                    }

                    // Get password from configuration or environment (fallback to old WALLET_PASSWORD env var)
                    var password = walletConfig.Password ?? 
                        Environment.GetEnvironmentVariable("WALLET_PASSWORD") ?? 
                        throw new InvalidOperationException("Wallet password not configured");

                    _wallet = new NEP6Wallet(walletConfig.WalletPath, password, ProtocolSettings.Default);

                    // Verify password
                    if (!_wallet.VerifyPassword(password))
                    {
                        throw new InvalidOperationException("Invalid wallet password");
                    }

                    _logger.LogInformation("Wallet unlocked successfully: {WalletPath}", walletConfig.WalletPath);

                }

                // Set default account
                var defaultAddress = walletConfig.DefaultAccount ?? _wallet.GetAccounts().FirstOrDefault()?.Address;
                if (string.IsNullOrEmpty(defaultAddress))
                {
                    throw new InvalidOperationException("No accounts found in wallet");
                }

                _defaultAccount = _wallet.GetAccount(defaultAddress);
                if (_defaultAccount == null)
                {
                    throw new InvalidOperationException($"Account {defaultAddress} not found in wallet");
                }

                _logger.LogInformation("Using account: {Address}", _defaultAccount.Address);

                // Log account balance
                var balance = await _blockchain.GetGasBalanceAsync(_defaultAccount.ScriptHash);
                _logger.LogInformation("Account GAS balance: {Balance}", balance);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unlock wallet");
                throw;
            }
        }

        public WalletAccount GetDefaultAccount()
        {
            if (_defaultAccount == null)
            {
                throw new InvalidOperationException("Wallet not unlocked. Call UnlockWalletAsync first.");
            }
            return _defaultAccount;
        }

        public async Task<Transaction> CreateTransactionAsync(byte[] script, UInt160 sender, long gasAmount)
        {
            if (_wallet == null)
            {
                throw new InvalidOperationException("Wallet not loaded");
            }

            var client = _blockchain.GetRpcClient();
            
            // Create transaction
            var tx = new Transaction
            {
                Version = 0,
                Nonce = (uint)new Random().Next(),
                SystemFee = gasAmount,
                NetworkFee = 0,
                ValidUntilBlock = await client.GetBlockCountAsync() + _deploymentOptions.ValidUntilBlockOffset,
                Signers = new[]
                {
                    new Signer
                    {
                        Account = sender,
                        Scopes = WitnessScope.CalledByEntry
                    }
                },
                Attributes = Array.Empty<TransactionAttribute>(),
                Script = script,
                Witnesses = null
            };

            // Check balance
            using var sb = new ScriptBuilder();
            ConvertArgs(sb, new object[] { sender });
            sb.EmitPush(CallFlags.All);
            sb.EmitPush("balanceOf");
            sb.EmitPush(NativeContract.GAS.Hash);
            sb.EmitSysCall(ApplicationEngine.System_Contract_Call);
            var balanceScript = sb.ToArray();

            var result = await client.InvokeScriptAsync(balanceScript);
            var balance = result.Stack[0].GetInteger();

            // Estimate network fee
            tx.NetworkFee = await EstimateNetworkFeeAsync(tx);
            
            var requiredGas = tx.SystemFee + tx.NetworkFee;
            if (balance < requiredGas)
            {
                throw new InvalidOperationException($"Insufficient GAS. Required: {requiredGas}, Available: {balance}");
            }

            // Sign transaction
            // ContractParametersContext constructor signature has changed
            var context = new ContractParametersContext(null!, tx, _deploymentOptions.ValidUntilBlockOffset);
            _wallet.Sign(context);
            
            if (!context.Completed)
            {
                throw new InvalidOperationException("Failed to sign transaction");
            }

            tx.Witnesses = context.GetWitnesses();
            return tx;
        }

        private Task<long> EstimateNetworkFeeAsync(Transaction tx)
        {
            if (_wallet == null)
            {
                throw new InvalidOperationException("Wallet not loaded");
            }

            var networkFee = 0L;

            foreach (var signer in tx.Signers)
            {
                var account = _wallet.GetAccount(signer.Account);
                if (account?.Contract?.Script != null)
                {
                    // Calculate script execution cost
                    networkFee += Neo.SmartContract.Helper.SignatureContractCost();
                }
                else
                {
                    // Default fee for standard account
                    networkFee += _deploymentOptions.DefaultNetworkFee;
                }
            }

            return Task.FromResult(networkFee);
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