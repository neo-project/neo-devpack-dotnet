using System;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.Cryptography.ECC;
using Neo.Extensions;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.SmartContract.Deploy.Exceptions;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.VM;
using Neo.Wallets;

namespace Neo.SmartContract.Deploy.Services;

/// <summary>
/// Service for managing wallets and accounts
/// </summary>
public class WalletManagerService : IWalletManager
{
    private const string GAS_CONTRACT_HASH = "0xd2a4cff31913016155e38e474a2c06d08be276cf";
    private const decimal GAS_DECIMALS = 100_000_000m;
    
    private readonly ILogger<WalletManagerService>? _logger;

    /// <summary>
    /// Initialize a new instance of WalletManagerService
    /// </summary>
    /// <param name="logger">Optional logger</param>
    public WalletManagerService(ILogger<WalletManagerService>? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get account from WIF private key
    /// </summary>
    /// <param name="wifKey">WIF private key</param>
    /// <returns>Account</returns>
    public Account GetAccountFromWif(string wifKey)
    {
        if (string.IsNullOrWhiteSpace(wifKey))
            throw new ArgumentException("WIF key cannot be null or empty", nameof(wifKey));

        try
        {
            var privateKey = Wallet.GetPrivateKeyFromWIF(wifKey);
            var keyPair = new KeyPair(privateKey);
            var contract = Contract.CreateSignatureContract(keyPair.PublicKey);
            
            return new Account
            {
                PrivateKey = privateKey,
                PublicKey = keyPair.PublicKey,
                ScriptHash = contract.ScriptHash
            };
        }
        catch (Exception ex)
        {
            throw new ContractDeploymentException($"Invalid WIF key: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Get account address
    /// </summary>
    /// <param name="account">Account</param>
    /// <returns>Account address</returns>
    public string GetAccountAddress(Account account)
    {
        if (account == null)
            throw new ArgumentNullException(nameof(account));

        return account.ScriptHash.ToAddress(ProtocolSettings.Default.AddressVersion);
    }

    /// <summary>
    /// Get account script hash
    /// </summary>
    /// <param name="account">Account</param>
    /// <returns>Account script hash</returns>
    public UInt160 GetAccountScriptHash(Account account)
    {
        if (account == null)
            throw new ArgumentNullException(nameof(account));

        return account.ScriptHash;
    }

    /// <summary>
    /// Get gas balance for an account
    /// </summary>
    /// <param name="accountHash">Account script hash</param>
    /// <param name="rpcUrl">RPC URL</param>
    /// <returns>GAS balance</returns>
    public async Task<decimal> GetGasBalanceAsync(UInt160 accountHash, string rpcUrl)
    {
        if (accountHash == null)
            throw new ArgumentNullException(nameof(accountHash));

        if (string.IsNullOrWhiteSpace(rpcUrl))
            throw new ArgumentException("RPC URL cannot be null or empty", nameof(rpcUrl));

        _logger?.LogInformation("Getting GAS balance for account: {Account} from {RpcUrl}", 
            accountHash.ToAddress(ProtocolSettings.Default.AddressVersion), rpcUrl);

        try
        {
            var rpcClient = new RpcClient(new Uri(rpcUrl));
            var gasHash = UInt160.Parse(GAS_CONTRACT_HASH);
            
            // Build the script to check balance
            using var scriptBuilder = new ScriptBuilder();
            scriptBuilder.EmitDynamicCall(gasHash, "balanceOf", accountHash);
            
            var script = scriptBuilder.ToArray();
            var result = await rpcClient.InvokeScriptAsync(script);
            
            if (result.State != VMState.HALT)
            {
                throw new ContractDeploymentException($"Failed to get balance: {result.Exception}");
            }

            if (result.Stack.Length > 0)
            {
                var balance = result.Stack[0].GetInteger();
                return (decimal)balance / GAS_DECIMALS;
            }

            return 0m;
        }
        catch (Exception ex) when (!(ex is ContractDeploymentException))
        {
            throw new ContractDeploymentException($"Failed to get GAS balance: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Create a signature contract from public key
    /// </summary>
    /// <param name="publicKey">Public key</param>
    /// <returns>Script hash of the contract</returns>
    public UInt160 CreateSignatureContract(ECPoint publicKey)
    {
        if (publicKey == null)
            throw new ArgumentNullException(nameof(publicKey));

        var contract = Contract.CreateSignatureContract(publicKey);
        return contract.ScriptHash;
    }
}