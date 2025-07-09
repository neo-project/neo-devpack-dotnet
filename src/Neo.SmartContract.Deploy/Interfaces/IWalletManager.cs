using System.Threading.Tasks;
using Neo;
using Neo.Cryptography.ECC;
using Neo.Wallets;

namespace Neo.SmartContract.Deploy.Interfaces;

/// <summary>
/// Interface for wallet management services
/// </summary>
public interface IWalletManager
{
    /// <summary>
    /// Get account from WIF private key
    /// </summary>
    /// <param name="wifKey">WIF private key</param>
    /// <returns>Account</returns>
    Account GetAccountFromWif(string wifKey);

    /// <summary>
    /// Get account address
    /// </summary>
    /// <param name="account">Account</param>
    /// <returns>Account address</returns>
    string GetAccountAddress(Account account);

    /// <summary>
    /// Get account script hash
    /// </summary>
    /// <param name="account">Account</param>
    /// <returns>Account script hash</returns>
    UInt160 GetAccountScriptHash(Account account);

    /// <summary>
    /// Get gas balance for an account
    /// </summary>
    /// <param name="accountHash">Account script hash</param>
    /// <param name="rpcUrl">RPC URL</param>
    /// <returns>GAS balance</returns>
    Task<decimal> GetGasBalanceAsync(UInt160 accountHash, string rpcUrl);

    /// <summary>
    /// Create a signature contract from public key
    /// </summary>
    /// <param name="publicKey">Public key</param>
    /// <returns>Script hash of the contract</returns>
    UInt160 CreateSignatureContract(ECPoint publicKey);
}