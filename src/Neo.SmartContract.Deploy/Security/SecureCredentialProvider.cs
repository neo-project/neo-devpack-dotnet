using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Neo.SmartContract.Deploy.Security;

/// <summary>
/// Secure credential provider with multiple fallback mechanisms
/// </summary>
public class SecureCredentialProvider : ICredentialProvider, IDisposable
{
    private readonly ILogger<SecureCredentialProvider> _logger;
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, SecureString> _secureCache = new();
    private readonly object _cacheLock = new object();

    public SecureCredentialProvider(
        ILogger<SecureCredentialProvider> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Get wallet password using secure fallback chain
    /// </summary>
    public async Task<string> GetWalletPasswordAsync(string walletPath)
    {
        var cacheKey = $"wallet:{walletPath}";

        // Check secure cache first
        lock (_cacheLock)
        {
            if (_secureCache.TryGetValue(cacheKey, out var cachedPassword))
            {
                return ConvertToUnsecureString(cachedPassword);
            }
        }

        string? password = null;

        // 1. Try environment variable first (most secure)
        var walletName = System.IO.Path.GetFileNameWithoutExtension(walletPath);
        password = Environment.GetEnvironmentVariable($"NEO_WALLET_PASSWORD_{walletName?.ToUpperInvariant()}") ??
                   Environment.GetEnvironmentVariable("NEO_WALLET_PASSWORD");

        if (!string.IsNullOrEmpty(password))
        {
            _logger.LogDebug("Retrieved wallet password from environment variable");
        }
        else
        {
            // 2. Try Azure Key Vault or similar (if configured)
            password = await TryGetFromKeyVaultAsync($"neo-wallet-{walletName}");

            if (!string.IsNullOrEmpty(password))
            {
                _logger.LogDebug("Retrieved wallet password from Key Vault");
            }
            else
            {
                // 3. Try configuration as last resort (least secure)
                password = _configuration[$"Wallet:Passwords:{walletName}"] ??
                          _configuration["Wallet:Password"];

                if (!string.IsNullOrEmpty(password))
                {
                    _logger.LogWarning("Using wallet password from configuration file. This is not recommended for production!");
                }
            }
        }

        if (string.IsNullOrEmpty(password))
        {
            throw new InvalidOperationException(
                $"Unable to retrieve password for wallet: {walletPath}. " +
                "Please set NEO_WALLET_PASSWORD environment variable or configure a secure credential provider.");
        }

        // Cache securely
        lock (_cacheLock)
        {
            _secureCache[cacheKey] = ConvertToSecureString(password);
        }

        return password;
    }

    /// <summary>
    /// Get RPC credentials if needed
    /// </summary>
    public async Task<(string username, string password)?> GetRpcCredentialsAsync(string rpcUrl)
    {
        var uri = new Uri(rpcUrl);
        var hostKey = uri.Host.Replace(".", "_").ToUpperInvariant();

        // Try environment variables
        var username = Environment.GetEnvironmentVariable($"NEO_RPC_USER_{hostKey}") ??
                      Environment.GetEnvironmentVariable("NEO_RPC_USER");

        var password = Environment.GetEnvironmentVariable($"NEO_RPC_PASSWORD_{hostKey}") ??
                      Environment.GetEnvironmentVariable("NEO_RPC_PASSWORD");

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            // Try Key Vault
            username = await TryGetFromKeyVaultAsync($"neo-rpc-user-{uri.Host}");
            password = await TryGetFromKeyVaultAsync($"neo-rpc-password-{uri.Host}");
        }

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            return (username, password);
        }

        return null;
    }

    /// <summary>
    /// Clear cached credentials
    /// </summary>
    public void ClearCache()
    {
        lock (_cacheLock)
        {
            foreach (var secureString in _secureCache.Values)
            {
                secureString.Dispose();
            }
            _secureCache.Clear();
        }
        _logger.LogDebug("Credential cache cleared");
    }

    /// <summary>
    /// Try to retrieve secret from Azure Key Vault or similar service
    /// </summary>
    private async Task<string?> TryGetFromKeyVaultAsync(string secretName)
    {
        // This is a placeholder for Key Vault integration
        // In a real implementation, you would use Azure.Security.KeyVault.Secrets
        // or AWS Secrets Manager, HashiCorp Vault, etc.

        var keyVaultUrl = _configuration["Security:KeyVaultUrl"];
        if (string.IsNullOrEmpty(keyVaultUrl))
        {
            return null;
        }

        try
        {
            // Example implementation:
            // var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
            // var secret = await client.GetSecretAsync(secretName);
            // return secret.Value.Value;

            _logger.LogDebug("Key Vault integration not implemented. Returning null.");
            return await Task.FromResult<string?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve secret from Key Vault");
            return null;
        }
    }

    /// <summary>
    /// Convert string to SecureString
    /// </summary>
    private static SecureString ConvertToSecureString(string value)
    {
        var secureString = new SecureString();
        foreach (char c in value)
        {
            secureString.AppendChar(c);
        }
        secureString.MakeReadOnly();
        return secureString;
    }

    /// <summary>
    /// Convert SecureString back to string (use with caution)
    /// </summary>
    private static string ConvertToUnsecureString(SecureString secureString)
    {
        IntPtr unmanagedString = IntPtr.Zero;
        try
        {
            unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
            return Marshal.PtrToStringUni(unmanagedString) ?? string.Empty;
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
        }
    }

    public void Dispose()
    {
        ClearCache();
    }
}
