using System;
using System.Collections.Concurrent;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neo.SmartContract.Deploy.Security.Interfaces;

namespace Neo.SmartContract.Deploy.Security;

/// <summary>
/// Secure credential provider that encrypts credentials in memory and on disk
/// </summary>
public class SecureCredentialProvider : ICredentialProvider, IDisposable
{
    private readonly ILogger<SecureCredentialProvider>? _logger;
    private readonly string? _persistencePath;
    private readonly byte[] _encryptionKey;
    private readonly ConcurrentDictionary<string, byte[]> _credentials;
    private readonly object _persistenceLock = new();
    private bool _disposed;

    /// <inheritdoc />
    public string ProviderName => "SecureCredentialProvider";

    /// <inheritdoc />
    public bool SupportsPersistence => !string.IsNullOrEmpty(_persistencePath);

    /// <summary>
    /// Initialize a new instance of SecureCredentialProvider
    /// </summary>
    /// <param name="encryptionKey">Encryption key (must be 32 bytes for AES-256)</param>
    /// <param name="persistencePath">Optional path to persist encrypted credentials</param>
    /// <param name="logger">Optional logger</param>
    public SecureCredentialProvider(byte[] encryptionKey, string? persistencePath = null, ILogger<SecureCredentialProvider>? logger = null)
    {
        if (encryptionKey == null || encryptionKey.Length != 32)
        {
            throw new ArgumentException("Encryption key must be 32 bytes for AES-256", nameof(encryptionKey));
        }

        _encryptionKey = (byte[])encryptionKey.Clone();
        _persistencePath = persistencePath;
        _logger = logger;
        _credentials = new ConcurrentDictionary<string, byte[]>();

        if (SupportsPersistence)
        {
            LoadFromDisk();
        }
    }

    /// <summary>
    /// Create a SecureCredentialProvider with a derived key from a password
    /// </summary>
    /// <param name="password">Password to derive key from</param>
    /// <param name="salt">Salt for key derivation (should be unique per application)</param>
    /// <param name="persistencePath">Optional path to persist encrypted credentials</param>
    /// <param name="logger">Optional logger</param>
    /// <returns>SecureCredentialProvider instance</returns>
    public static SecureCredentialProvider CreateWithPassword(string password, byte[] salt, string? persistencePath = null, ILogger<SecureCredentialProvider>? logger = null)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("Password cannot be null or empty", nameof(password));
        }
        if (salt == null || salt.Length < 16)
        {
            throw new ArgumentException("Salt must be at least 16 bytes", nameof(salt));
        }

        using var deriveBytes = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        var key = deriveBytes.GetBytes(32);
        return new SecureCredentialProvider(key, persistencePath, logger);
    }

    /// <inheritdoc />
    public async Task<string?> GetCredentialAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }

        await Task.CompletedTask; // Keep async for consistency

        if (_credentials.TryGetValue(key, out var encryptedData))
        {
            try
            {
                var decrypted = Decrypt(encryptedData);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to decrypt credential for key: {Key}", key);
                return null;
            }
        }

        return null;
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

        await Task.CompletedTask; // Keep async for consistency

        try
        {
            var encrypted = Encrypt(Encoding.UTF8.GetBytes(value));
            _credentials[key] = encrypted;
            
            _logger?.LogDebug("Credential set for key: {Key}", key);

            if (SupportsPersistence)
            {
                SaveToDisk();
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to set credential for key: {Key}", key);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> RemoveCredentialAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }

        await Task.CompletedTask; // Keep async for consistency

        var removed = _credentials.TryRemove(key, out _);
        
        if (removed)
        {
            _logger?.LogDebug("Credential removed for key: {Key}", key);
            
            if (SupportsPersistence)
            {
                SaveToDisk();
            }
        }

        return removed;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }

        await Task.CompletedTask; // Keep async for consistency
        return _credentials.ContainsKey(key);
    }

    /// <inheritdoc />
    public async Task ClearAsync()
    {
        await Task.CompletedTask; // Keep async for consistency
        
        _credentials.Clear();
        _logger?.LogInformation("All credentials cleared");

        if (SupportsPersistence && File.Exists(_persistencePath))
        {
            try
            {
                File.Delete(_persistencePath);
                _logger?.LogDebug("Persistence file deleted");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to delete persistence file");
            }
        }
    }

    private byte[] Encrypt(byte[] data)
    {
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        
        // Write IV to the beginning of the stream
        ms.Write(aes.IV, 0, aes.IV.Length);
        
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            cs.Write(data, 0, data.Length);
        }

        return ms.ToArray();
    }

    private byte[] Decrypt(byte[] encryptedData)
    {
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;

        // Extract IV from the beginning of the encrypted data
        var iv = new byte[aes.IV.Length];
        Array.Copy(encryptedData, 0, iv, 0, iv.Length);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(encryptedData, iv.Length, encryptedData.Length - iv.Length);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var resultMs = new MemoryStream();
        
        cs.CopyTo(resultMs);
        return resultMs.ToArray();
    }

    private void SaveToDisk()
    {
        if (!SupportsPersistence) return;

        lock (_persistenceLock)
        {
            try
            {
                var directory = Path.GetDirectoryName(_persistencePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Create a dictionary for serialization
                var dataToSave = new Dictionary<string, string>();
                foreach (var kvp in _credentials)
                {
                    dataToSave[kvp.Key] = Convert.ToBase64String(kvp.Value);
                }

                var json = JsonSerializer.Serialize(dataToSave, new JsonSerializerOptions { WriteIndented = true });
                var encryptedJson = Encrypt(Encoding.UTF8.GetBytes(json));
                
                File.WriteAllBytes(_persistencePath!, encryptedJson);
                _logger?.LogDebug("Credentials saved to disk");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to save credentials to disk");
            }
        }
    }

    private void LoadFromDisk()
    {
        if (!SupportsPersistence || !File.Exists(_persistencePath)) return;

        lock (_persistenceLock)
        {
            try
            {
                var encryptedData = File.ReadAllBytes(_persistencePath!);
                var decryptedJson = Decrypt(encryptedData);
                var json = Encoding.UTF8.GetString(decryptedJson);
                
                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (data != null)
                {
                    foreach (var kvp in data)
                    {
                        _credentials[kvp.Key] = Convert.FromBase64String(kvp.Value);
                    }
                    _logger?.LogDebug("Credentials loaded from disk");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to load credentials from disk");
            }
        }
    }

    /// <summary>
    /// Dispose of resources
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;

        // Clear sensitive data from memory
        Array.Clear(_encryptionKey, 0, _encryptionKey.Length);
        _credentials.Clear();
        
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}