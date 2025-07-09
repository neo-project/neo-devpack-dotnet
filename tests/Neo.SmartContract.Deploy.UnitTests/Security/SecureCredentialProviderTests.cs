using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Neo.SmartContract.Deploy.Security;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests.Security;

public class SecureCredentialProviderTests : IDisposable
{
    private readonly byte[] _testKey;
    private readonly string _tempPath;
    private readonly Mock<ILogger<SecureCredentialProvider>> _loggerMock;

    public SecureCredentialProviderTests()
    {
        _testKey = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(_testKey);
        }
        _tempPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid():N}.enc");
        _loggerMock = new Mock<ILogger<SecureCredentialProvider>>();
    }

    [Fact]
    public void Constructor_ValidKey_CreatesInstance()
    {
        // Act
        using var provider = new SecureCredentialProvider(_testKey, null, _loggerMock.Object);

        // Assert
        Assert.NotNull(provider);
        Assert.Equal("SecureCredentialProvider", provider.ProviderName);
        Assert.False(provider.SupportsPersistence);
    }

    [Fact]
    public void Constructor_InvalidKeyLength_ThrowsException()
    {
        // Arrange
        var invalidKey = new byte[16]; // Should be 32 bytes

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new SecureCredentialProvider(invalidKey));
    }

    [Fact]
    public void Constructor_WithPersistencePath_SupportsPersistence()
    {
        // Act
        using var provider = new SecureCredentialProvider(_testKey, _tempPath, _loggerMock.Object);

        // Assert
        Assert.True(provider.SupportsPersistence);
    }

    [Fact]
    public async Task SetAndGetCredential_ValidInput_ReturnsCorrectValue()
    {
        // Arrange
        using var provider = new SecureCredentialProvider(_testKey, null, _loggerMock.Object);
        const string key = "test_key";
        const string value = "test_value_123";

        // Act
        await provider.SetCredentialAsync(key, value);
        var retrieved = await provider.GetCredentialAsync(key);

        // Assert
        Assert.Equal(value, retrieved);
    }

    [Fact]
    public async Task GetCredential_NonExistentKey_ReturnsNull()
    {
        // Arrange
        using var provider = new SecureCredentialProvider(_testKey, null, _loggerMock.Object);

        // Act
        var result = await provider.GetCredentialAsync("non_existent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveCredential_ExistingKey_ReturnsTrue()
    {
        // Arrange
        using var provider = new SecureCredentialProvider(_testKey, null, _loggerMock.Object);
        const string key = "test_key";
        await provider.SetCredentialAsync(key, "value");

        // Act
        var removed = await provider.RemoveCredentialAsync(key);
        var exists = await provider.ExistsAsync(key);

        // Assert
        Assert.True(removed);
        Assert.False(exists);
    }

    [Fact]
    public async Task RemoveCredential_NonExistentKey_ReturnsFalse()
    {
        // Arrange
        using var provider = new SecureCredentialProvider(_testKey, null, _loggerMock.Object);

        // Act
        var removed = await provider.RemoveCredentialAsync("non_existent");

        // Assert
        Assert.False(removed);
    }

    [Fact]
    public async Task ExistsAsync_ExistingKey_ReturnsTrue()
    {
        // Arrange
        using var provider = new SecureCredentialProvider(_testKey, null, _loggerMock.Object);
        const string key = "test_key";
        await provider.SetCredentialAsync(key, "value");

        // Act
        var exists = await provider.ExistsAsync(key);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ClearAsync_RemovesAllCredentials()
    {
        // Arrange
        using var provider = new SecureCredentialProvider(_testKey, null, _loggerMock.Object);
        await provider.SetCredentialAsync("key1", "value1");
        await provider.SetCredentialAsync("key2", "value2");

        // Act
        await provider.ClearAsync();

        // Assert
        Assert.False(await provider.ExistsAsync("key1"));
        Assert.False(await provider.ExistsAsync("key2"));
    }

    [Fact]
    public void CreateWithPassword_ValidInput_CreatesInstance()
    {
        // Arrange
        const string password = "strong_password_123!";
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Act
        using var provider = SecureCredentialProvider.CreateWithPassword(password, salt, null, _loggerMock.Object);

        // Assert
        Assert.NotNull(provider);
    }

    [Fact]
    public void CreateWithPassword_EmptyPassword_ThrowsException()
    {
        // Arrange
        var salt = new byte[16];

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            SecureCredentialProvider.CreateWithPassword("", salt));
    }

    [Fact]
    public void CreateWithPassword_InvalidSalt_ThrowsException()
    {
        // Arrange
        var invalidSalt = new byte[8]; // Should be at least 16 bytes

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            SecureCredentialProvider.CreateWithPassword("password", invalidSalt));
    }

    [Fact]
    public async Task Persistence_SaveAndLoad_PreservesCredentials()
    {
        // Arrange
        const string key1 = "persisted_key1";
        const string value1 = "persisted_value1";
        const string key2 = "persisted_key2";
        const string value2 = "persisted_value2";

        // Act - Save
        using (var provider1 = new SecureCredentialProvider(_testKey, _tempPath, _loggerMock.Object))
        {
            await provider1.SetCredentialAsync(key1, value1);
            await provider1.SetCredentialAsync(key2, value2);
        }

        // Act - Load
        using var provider2 = new SecureCredentialProvider(_testKey, _tempPath, _loggerMock.Object);
        var retrieved1 = await provider2.GetCredentialAsync(key1);
        var retrieved2 = await provider2.GetCredentialAsync(key2);

        // Assert
        Assert.Equal(value1, retrieved1);
        Assert.Equal(value2, retrieved2);
    }

    [Fact]
    public async Task Encryption_DifferentKeys_CannotDecrypt()
    {
        // Arrange
        var key1 = new byte[32];
        var key2 = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(key1);
            rng.GetBytes(key2);
        }

        const string testKey = "test_key";
        const string testValue = "test_value";

        // Act - Save with key1
        using (var provider1 = new SecureCredentialProvider(key1, _tempPath, _loggerMock.Object))
        {
            await provider1.SetCredentialAsync(testKey, testValue);
        }

        // Act - Try to load with key2
        using var provider2 = new SecureCredentialProvider(key2, _tempPath, _loggerMock.Object);
        var retrieved = await provider2.GetCredentialAsync(testKey);

        // Assert - Should fail to decrypt correctly
        Assert.NotEqual(testValue, retrieved);
    }

    [Fact]
    public async Task SetCredential_NullKey_ThrowsException()
    {
        // Arrange
        using var provider = new SecureCredentialProvider(_testKey, null, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            provider.SetCredentialAsync(null!, "value"));
    }

    [Fact]
    public async Task SetCredential_NullValue_ThrowsException()
    {
        // Arrange
        using var provider = new SecureCredentialProvider(_testKey, null, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            provider.SetCredentialAsync("key", null!));
    }

    [Fact]
    public async Task LargeCredential_StoresAndRetrievesCorrectly()
    {
        // Arrange
        using var provider = new SecureCredentialProvider(_testKey, null, _loggerMock.Object);
        const string key = "large_key";
        var largeValue = new string('x', 10000); // 10KB string

        // Act
        await provider.SetCredentialAsync(key, largeValue);
        var retrieved = await provider.GetCredentialAsync(key);

        // Assert
        Assert.Equal(largeValue, retrieved);
    }

    [Fact]
    public async Task MultipleCredentials_IndependentEncryption()
    {
        // Arrange
        using var provider = new SecureCredentialProvider(_testKey, null, _loggerMock.Object);
        var credentials = new Dictionary<string, string>
        {
            ["key1"] = "value1",
            ["key2"] = "value2",
            ["key3"] = "value3"
        };

        // Act
        foreach (var kvp in credentials)
        {
            await provider.SetCredentialAsync(kvp.Key, kvp.Value);
        }

        // Assert
        foreach (var kvp in credentials)
        {
            var retrieved = await provider.GetCredentialAsync(kvp.Key);
            Assert.Equal(kvp.Value, retrieved);
        }
    }

    public void Dispose()
    {
        if (File.Exists(_tempPath))
        {
            File.Delete(_tempPath);
        }
    }
}