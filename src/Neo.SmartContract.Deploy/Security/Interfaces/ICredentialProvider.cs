using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy.Security.Interfaces;

/// <summary>
/// Interface for providing credentials in a secure manner
/// </summary>
public interface ICredentialProvider
{
    /// <summary>
    /// Get a credential value by key
    /// </summary>
    /// <param name="key">The credential key</param>
    /// <returns>The credential value or null if not found</returns>
    Task<string?> GetCredentialAsync(string key);

    /// <summary>
    /// Set a credential value
    /// </summary>
    /// <param name="key">The credential key</param>
    /// <param name="value">The credential value</param>
    Task SetCredentialAsync(string key, string value);

    /// <summary>
    /// Remove a credential
    /// </summary>
    /// <param name="key">The credential key</param>
    /// <returns>True if removed, false if not found</returns>
    Task<bool> RemoveCredentialAsync(string key);

    /// <summary>
    /// Check if a credential exists
    /// </summary>
    /// <param name="key">The credential key</param>
    /// <returns>True if exists</returns>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// Clear all credentials
    /// </summary>
    Task ClearAsync();

    /// <summary>
    /// Get the provider name
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Get whether the provider supports persistence
    /// </summary>
    bool SupportsPersistence { get; }
}