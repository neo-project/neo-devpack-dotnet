using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Neo.SmartContract.Deploy.Security;

/// <summary>
/// Credential provider that retrieves credentials from environment variables
/// </summary>
public class EnvironmentCredentialProvider : ICredentialProvider
{
    private readonly ILogger<EnvironmentCredentialProvider> _logger;

    public EnvironmentCredentialProvider(ILogger<EnvironmentCredentialProvider> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get wallet password from environment variable
    /// </summary>
    public Task<string> GetWalletPasswordAsync(string walletPath)
    {
        _logger.LogDebug("Retrieving wallet password from environment for wallet: {WalletPath}", walletPath);

        // Try specific wallet password first
        var walletName = System.IO.Path.GetFileNameWithoutExtension(walletPath);
        var specificPassword = Environment.GetEnvironmentVariable($"NEO_WALLET_PASSWORD_{walletName?.ToUpperInvariant()}");

        if (!string.IsNullOrEmpty(specificPassword))
        {
            _logger.LogDebug("Using wallet-specific password from environment");
            return Task.FromResult(specificPassword);
        }

        // Fall back to general wallet password
        var generalPassword = Environment.GetEnvironmentVariable("NEO_WALLET_PASSWORD");

        if (!string.IsNullOrEmpty(generalPassword))
        {
            _logger.LogDebug("Using general wallet password from environment");
            return Task.FromResult(generalPassword);
        }

        throw new InvalidOperationException(
            $"Wallet password not found in environment variables. " +
            $"Set NEO_WALLET_PASSWORD or NEO_WALLET_PASSWORD_{walletName?.ToUpperInvariant()}");
    }

    /// <summary>
    /// Get RPC credentials from environment variables
    /// </summary>
    public Task<(string username, string password)?> GetRpcCredentialsAsync(string rpcUrl)
    {
        var uri = new Uri(rpcUrl);
        var hostKey = uri.Host.Replace(".", "_").ToUpperInvariant();

        var username = Environment.GetEnvironmentVariable($"NEO_RPC_USER_{hostKey}") ??
                      Environment.GetEnvironmentVariable("NEO_RPC_USER");

        var password = Environment.GetEnvironmentVariable($"NEO_RPC_PASSWORD_{hostKey}") ??
                      Environment.GetEnvironmentVariable("NEO_RPC_PASSWORD");

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            _logger.LogDebug("Using RPC credentials from environment for {Host}", uri.Host);
            return Task.FromResult<(string, string)?>((username, password));
        }

        return Task.FromResult<(string, string)?>(null);
    }

    /// <summary>
    /// No cache to clear for environment provider
    /// </summary>
    public void ClearCache()
    {
        // No cache to clear
    }
}
