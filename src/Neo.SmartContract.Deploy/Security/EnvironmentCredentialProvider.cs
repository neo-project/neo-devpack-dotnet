using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neo.SmartContract.Deploy.Security.Interfaces;

namespace Neo.SmartContract.Deploy.Security;

/// <summary>
/// Credential provider that reads from environment variables
/// </summary>
public class EnvironmentCredentialProvider : ICredentialProvider
{
    private readonly ILogger<EnvironmentCredentialProvider>? _logger;
    private readonly string _prefix;
    private readonly bool _allowSet;

    /// <inheritdoc />
    public string ProviderName => "EnvironmentCredentialProvider";

    /// <inheritdoc />
    public bool SupportsPersistence => false;

    /// <summary>
    /// Initialize a new instance of EnvironmentCredentialProvider
    /// </summary>
    /// <param name="prefix">Prefix for environment variables (e.g., "NEO_DEPLOY_")</param>
    /// <param name="allowSet">Whether to allow setting environment variables</param>
    /// <param name="logger">Optional logger</param>
    public EnvironmentCredentialProvider(string prefix = "NEO_DEPLOY_", bool allowSet = false, ILogger<EnvironmentCredentialProvider>? logger = null)
    {
        _prefix = prefix ?? string.Empty;
        _allowSet = allowSet;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string?> GetCredentialAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }

        await Task.CompletedTask; // Keep async for consistency

        var envKey = NormalizeKey(key);
        var value = Environment.GetEnvironmentVariable(envKey);
        
        if (value != null)
        {
            _logger?.LogDebug("Credential found for key: {Key} (env: {EnvKey})", key, envKey);
        }
        else
        {
            _logger?.LogDebug("Credential not found for key: {Key} (env: {EnvKey})", key, envKey);
        }

        return value;
    }

    /// <inheritdoc />
    public async Task SetCredentialAsync(string key, string value)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (!_allowSet)
        {
            throw new NotSupportedException("Setting environment variables is not allowed for this provider. Enable it via the constructor.");
        }

        await Task.CompletedTask; // Keep async for consistency

        var envKey = NormalizeKey(key);
        Environment.SetEnvironmentVariable(envKey, value);
        _logger?.LogInformation("Environment variable set: {EnvKey}", envKey);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveCredentialAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }

        if (!_allowSet)
        {
            throw new NotSupportedException("Modifying environment variables is not allowed for this provider. Enable it via the constructor.");
        }

        await Task.CompletedTask; // Keep async for consistency

        var envKey = NormalizeKey(key);
        var exists = Environment.GetEnvironmentVariable(envKey) != null;
        
        if (exists)
        {
            Environment.SetEnvironmentVariable(envKey, null);
            _logger?.LogInformation("Environment variable removed: {EnvKey}", envKey);
        }

        return exists;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }

        await Task.CompletedTask; // Keep async for consistency

        var envKey = NormalizeKey(key);
        return Environment.GetEnvironmentVariable(envKey) != null;
    }

    /// <inheritdoc />
    public async Task ClearAsync()
    {
        if (!_allowSet)
        {
            throw new NotSupportedException("Modifying environment variables is not allowed for this provider. Enable it via the constructor.");
        }

        await Task.CompletedTask; // Keep async for consistency

        // Get all environment variables and clear those with our prefix
        var envVars = Environment.GetEnvironmentVariables();
        var keysToRemove = new List<string>();

        foreach (string? envKey in envVars.Keys)
        {
            if (envKey != null && envKey.StartsWith(_prefix, StringComparison.OrdinalIgnoreCase))
            {
                keysToRemove.Add(envKey);
            }
        }

        foreach (var envKey in keysToRemove)
        {
            Environment.SetEnvironmentVariable(envKey, null);
            _logger?.LogDebug("Environment variable cleared: {EnvKey}", envKey);
        }

        _logger?.LogInformation("Cleared {Count} environment variables with prefix: {Prefix}", keysToRemove.Count, _prefix);
    }

    /// <summary>
    /// Normalize a key to an environment variable name
    /// </summary>
    /// <param name="key">The key to normalize</param>
    /// <returns>Normalized environment variable name</returns>
    private string NormalizeKey(string key)
    {
        // Convert to uppercase and replace special characters with underscores
        var normalized = key.ToUpperInvariant()
            .Replace('.', '_')
            .Replace('-', '_')
            .Replace(' ', '_');

        return $"{_prefix}{normalized}";
    }

    /// <summary>
    /// Get all credentials matching the configured prefix
    /// </summary>
    /// <returns>Dictionary of key-value pairs</returns>
    public Dictionary<string, string> GetAllCredentials()
    {
        var result = new Dictionary<string, string>();
        var envVars = Environment.GetEnvironmentVariables();

        foreach (string? envKey in envVars.Keys)
        {
            if (envKey != null && envKey.StartsWith(_prefix, StringComparison.OrdinalIgnoreCase))
            {
                var value = envVars[envKey] as string;
                if (value != null)
                {
                    // Remove prefix to get the original key
                    var key = envKey.Substring(_prefix.Length);
                    result[key] = value;
                }
            }
        }

        _logger?.LogDebug("Found {Count} credentials with prefix: {Prefix}", result.Count, _prefix);
        return result;
    }
}