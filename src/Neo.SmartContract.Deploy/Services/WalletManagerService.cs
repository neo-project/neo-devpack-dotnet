using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.Network.P2P.Payloads;
using Neo.Wallets;
using Neo.SmartContract.Deploy.Exceptions;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Security;

namespace Neo.SmartContract.Deploy.Services;

/// <summary>
/// Wallet management service implementation
/// </summary>
public class WalletManagerService : IWalletManager, IDisposable
{
    private readonly ILogger<WalletManagerService> _logger;
    private readonly ICredentialProvider? _credentialProvider;
    private readonly SemaphoreSlim _walletLock = new SemaphoreSlim(1, 1);
    private Wallet? _wallet;
    private WalletAccount? _defaultAccount;
    private bool _disposed = false;

    public WalletManagerService(ILogger<WalletManagerService> logger, ICredentialProvider? credentialProvider = null)
    {
        _logger = logger;
        _credentialProvider = credentialProvider;
    }

    /// <summary>
    /// Check if wallet is loaded
    /// </summary>
    public bool IsWalletLoaded => _wallet != null;

    /// <summary>
    /// Load a wallet from file
    /// </summary>
    /// <param name="walletPath">Path to wallet file</param>
    /// <param name="password">Wallet password</param>
    /// <exception cref="ArgumentException">Thrown when parameters are invalid</exception>
    /// <exception cref="FileNotFoundException">Thrown when wallet file not found</exception>
    /// <exception cref="WalletException">Thrown when wallet loading fails</exception>
    public async Task LoadWalletAsync(string walletPath, string password)
    {
        if (string.IsNullOrWhiteSpace(walletPath))
            throw new ArgumentException("Wallet path cannot be empty", nameof(walletPath));
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        _logger.LogInformation("Loading wallet from {WalletPath}", walletPath);

        if (!File.Exists(walletPath))
        {
            throw new FileNotFoundException($"Wallet file not found: {walletPath}");
        }

        await _walletLock.WaitAsync();
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
        finally
        {
            _walletLock.Release();
        }
    }

    /// <summary>
    /// Load a wallet using secure credential provider
    /// </summary>
    /// <param name="walletPath">Path to wallet file</param>
    /// <exception cref="ArgumentException">Thrown when wallet path is invalid</exception>
    /// <exception cref="FileNotFoundException">Thrown when wallet file not found</exception>
    /// <exception cref="WalletException">Thrown when wallet loading fails</exception>
    /// <exception cref="InvalidOperationException">Thrown when no credential provider is configured</exception>
    public async Task LoadWalletSecurelyAsync(string walletPath)
    {
        if (_credentialProvider == null)
        {
            throw new InvalidOperationException(
                "No credential provider configured. Please configure a secure credential provider in services.");
        }

        var password = await _credentialProvider.GetWalletPasswordAsync(walletPath);
        await LoadWalletAsync(walletPath, password);

        // Clear password from memory
        if (!string.IsNullOrEmpty(password))
        {
            // In a real implementation, you might want to use SecureString
            password = null;
        }
    }

    /// <summary>
    /// Get an account from the wallet
    /// </summary>
    /// <param name="accountAddress">Account address (optional - uses default if not specified)</param>
    /// <returns>Account script hash</returns>
    /// <exception cref="InvalidOperationException">Thrown when no wallet is loaded</exception>
    /// <exception cref="ArgumentException">Thrown when account not found</exception>
    public UInt160 GetAccount(string? accountAddress = null)
    {
        if (!IsWalletLoaded || _wallet == null)
        {
            throw new InvalidOperationException("No wallet loaded. Call LoadWalletAsync first.");
        }

        if (string.IsNullOrEmpty(accountAddress))
        {
            if (_defaultAccount == null)
            {
                throw new InvalidOperationException("No default account available.");
            }
            return _defaultAccount.ScriptHash;
        }

        // Parse address and find account
        UInt160 scriptHash;
        try
        {
            scriptHash = accountAddress.ToScriptHash(Neo.ProtocolSettings.Default.AddressVersion);
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid account address: {accountAddress}", ex);
        }

        var account = _wallet.GetAccount(scriptHash);
        if (account == null)
        {
            throw new ArgumentException($"Account {accountAddress} not found in wallet.");
        }

        return scriptHash;
    }

    /// <summary>
    /// Sign a transaction
    /// </summary>
    /// <param name="transaction">Transaction to sign</param>
    /// <param name="account">Account to sign with (optional)</param>
    /// <exception cref="ArgumentNullException">Thrown when transaction is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when no wallet is loaded</exception>
    /// <exception cref="WalletException">Thrown when signing fails</exception>
    public async Task SignTransactionAsync(Transaction transaction, UInt160? account = null)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

        await _walletLock.WaitAsync();
        try
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
        finally
        {
            _walletLock.Release();
        }
    }

    /// <summary>
    /// Get all accounts in the wallet
    /// </summary>
    /// <returns>List of account script hashes</returns>
    public IEnumerable<UInt160> GetAccounts()
    {
        if (!IsWalletLoaded || _wallet == null)
        {
            return Enumerable.Empty<UInt160>();
        }

        return _wallet.GetAccounts().Select(a => a.ScriptHash);
    }

    #region IDisposable Implementation

    /// <summary>
    /// Dispose of the wallet manager and its resources
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected dispose method
    /// </summary>
    /// <param name="disposing">True if disposing managed resources</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose wallet if it implements IDisposable
                if (_wallet is IDisposable disposableWallet)
                {
                    disposableWallet.Dispose();
                }
                _wallet = null;
                _defaultAccount = null;

                // Dispose of the semaphore
                _walletLock?.Dispose();
            }

            _disposed = true;
        }
    }

    #endregion
}
