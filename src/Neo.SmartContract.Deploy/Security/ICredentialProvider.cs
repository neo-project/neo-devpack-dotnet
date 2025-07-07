using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy.Security;

/// <summary>
/// Interface for secure credential retrieval
/// </summary>
public interface ICredentialProvider
{
    /// <summary>
    /// Get wallet password securely
    /// </summary>
    /// <param name="walletPath">Path to the wallet file</param>
    /// <returns>The wallet password</returns>
    Task<string> GetWalletPasswordAsync(string walletPath);

    /// <summary>
    /// Get RPC credentials if needed
    /// </summary>
    /// <param name="rpcUrl">RPC endpoint URL</param>
    /// <returns>Username and password tuple, or null if not needed</returns>
    Task<(string username, string password)?> GetRpcCredentialsAsync(string rpcUrl);

    /// <summary>
    /// Clear any cached credentials
    /// </summary>
    void ClearCache();
}
