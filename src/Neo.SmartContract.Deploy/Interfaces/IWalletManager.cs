using Neo;
using Neo.Network.P2P.Payloads;

namespace Neo.SmartContract.Deploy.Interfaces;

/// <summary>
/// Wallet management interface
/// </summary>
public interface IWalletManager
{
    /// <summary>
    /// Load a wallet from file
    /// </summary>
    /// <param name="walletPath">Path to wallet file</param>
    /// <param name="password">Wallet password</param>
    Task LoadWalletAsync(string walletPath, string password);

    /// <summary>
    /// Get account script hash
    /// </summary>
    /// <param name="accountAddress">Account address (optional - uses default if not specified)</param>
    /// <returns>Account script hash</returns>
    UInt160 GetAccount(string? accountAddress = null);

    /// <summary>
    /// Sign a transaction
    /// </summary>
    /// <param name="transaction">Transaction to sign</param>
    /// <param name="account">Account to sign with (optional - uses default if not specified)</param>
    Task SignTransactionAsync(Transaction transaction, UInt160? account = null);

    /// <summary>
    /// Check if wallet is loaded
    /// </summary>
    bool IsWalletLoaded { get; }

    /// <summary>
    /// Get all accounts in the wallet
    /// </summary>
    /// <returns>List of account script hashes</returns>
    IEnumerable<UInt160> GetAccounts();
}
