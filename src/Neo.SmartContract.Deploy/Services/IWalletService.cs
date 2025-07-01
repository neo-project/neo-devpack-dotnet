using Neo;
using Neo.Network.P2P.Payloads;
using Neo.Wallets;

namespace Neo.SmartContract.Deploy.Services
{
    /// <summary>
    /// Service for wallet management
    /// </summary>
    public interface IWalletService
    {
        /// <summary>
        /// Get the default wallet account
        /// </summary>
        WalletAccount GetDefaultAccount();

        /// <summary>
        /// Create a transaction with the specified script
        /// </summary>
        /// <param name="script">Script to execute</param>
        /// <param name="sender">Sender account</param>
        /// <param name="gasAmount">System fee amount</param>
        Task<Transaction> CreateTransactionAsync(byte[] script, UInt160 sender, long gasAmount);

        /// <summary>
        /// Unlock the wallet
        /// </summary>
        Task<bool> UnlockWalletAsync();
    }
}