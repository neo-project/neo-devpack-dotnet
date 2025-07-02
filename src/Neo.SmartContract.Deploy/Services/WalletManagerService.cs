using Microsoft.Extensions.Logging;
using Neo;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Exceptions;
using Neo.Wallets;

namespace Neo.SmartContract.Deploy.Services;

/// <summary>
/// Wallet management service implementation
/// </summary>
public class WalletManagerService : IWalletManager
{
    private readonly ILogger<WalletManagerService> _logger;
    private Wallet? _wallet;
    private WalletAccount? _defaultAccount;

    public WalletManagerService(ILogger<WalletManagerService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Check if wallet is loaded
    /// </summary>
    public bool IsWalletLoaded => _wallet != null;

    /// <summary>
    /// Load a wallet from file
    /// </summary>
    public async Task LoadWalletAsync(string walletPath, string password)
    {
        _logger.LogInformation("Loading wallet from {WalletPath}", walletPath);

        if (!File.Exists(walletPath))
        {
            throw new FileNotFoundException($"Wallet file not found: {walletPath}");
        }

        try
        {
            // Load NEP-6 wallet with protocol settings
            var protocolSettings = Neo.ProtocolSettings.Default;
            _wallet = Neo.Wallets.NEP6.NEP6Wallet.Open(walletPath, password, protocolSettings);

            // Get the first account with a private key
            _defaultAccount = _wallet.GetAccounts().FirstOrDefault(a => a.HasKey);

            if (_defaultAccount == null)
            {
                throw new WalletException("No account with private key found in wallet");
            }

            _logger.LogInformation("Wallet loaded successfully from {WalletPath} with default account {Account}",
                walletPath, _defaultAccount.Address);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load wallet from {WalletPath}", walletPath);
            throw;
        }
    }

    /// <summary>
    /// Get account script hash
    /// </summary>
    public UInt160 GetAccount(string? accountAddress = null)
    {
        if (!IsWalletLoaded || _wallet == null)
        {
            throw new InvalidOperationException("No wallet loaded. Call LoadWalletAsync first.");
        }

        if (accountAddress == null)
        {
            if (_defaultAccount == null)
            {
                throw new InvalidOperationException("No default account available in wallet.");
            }
            return _defaultAccount.ScriptHash;
        }

        // Find account by address
        var account = _wallet.GetAccounts().FirstOrDefault(a => a.Address == accountAddress);
        if (account != null)
        {
            return account.ScriptHash;
        }

        // Try to parse as script hash
        if (UInt160.TryParse(accountAddress, out var scriptHash))
        {
            account = _wallet.GetAccount(scriptHash);
            if (account != null)
            {
                return scriptHash;
            }
        }

        throw new ArgumentException($"Account {accountAddress} not found in wallet.");
    }

    /// <summary>
    /// Sign a transaction
    /// </summary>
    public async Task SignTransactionAsync(Transaction transaction, UInt160? account = null)
    {
        if (!IsWalletLoaded || _wallet == null)
        {
            throw new InvalidOperationException("No wallet loaded. Call LoadWalletAsync first.");
        }

        var signerScriptHash = account ?? _defaultAccount?.ScriptHash;
        if (signerScriptHash == null)
        {
            throw new InvalidOperationException("No account specified and no default account available.");
        }

        var signerAccount = _wallet.GetAccount(signerScriptHash);
        if (signerAccount == null || !signerAccount.HasKey)
        {
            throw new WalletException($"Account {signerScriptHash} not found or has no private key");
        }

        _logger.LogInformation("Signing transaction {TxHash} with account {Account}",
            transaction.Hash, signerAccount.Address);

        try
        {
            var context = new Neo.SmartContract.ContractParametersContext(null, transaction, _wallet.ProtocolSettings.Network);
            if (!_wallet.Sign(context))
            {
                throw new WalletException("Failed to sign transaction");
            }

            transaction.Witnesses = context.GetWitnesses();
            _logger.LogInformation("Transaction signed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sign transaction {TxHash}", transaction.Hash);
            throw;
        }
    }

    /// <summary>
    /// Get all accounts in the wallet
    /// </summary>
    public IEnumerable<UInt160> GetAccounts()
    {
        if (!IsWalletLoaded || _wallet == null)
        {
            return Enumerable.Empty<UInt160>();
        }

        return _wallet.GetAccounts().Select(a => a.ScriptHash);
    }
}
