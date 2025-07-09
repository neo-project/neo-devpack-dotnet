using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Neo.SmartContract.Deploy.Security;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests.Security;

public class EnvironmentCredentialProviderTests : IDisposable
{
    private readonly Mock<ILogger<EnvironmentCredentialProvider>> _loggerMock;
    private readonly List<string> _envVarsToCleanup;

    public EnvironmentCredentialProviderTests()
    {
        _loggerMock = new Mock<ILogger<EnvironmentCredentialProvider>>();
        _envVarsToCleanup = new List<string>();
    }

    [Fact]
    public void Constructor_DefaultValues_CreatesInstance()
    {
        // Act
        var provider = new EnvironmentCredentialProvider(logger: _loggerMock.Object);

        // Assert
        Assert.NotNull(provider);
        Assert.Equal("EnvironmentCredentialProvider", provider.ProviderName);
        Assert.False(provider.SupportsPersistence);
    }

    [Fact]
    public async Task GetCredential_ExistingVariable_ReturnsValue()
    {
        // Arrange
        const string prefix = "TEST_PREFIX_";
        const string key = "MyKey";
        const string value = "MyValue";
        var envKey = $"{prefix}{key.ToUpperInvariant()}";
        
        Environment.SetEnvironmentVariable(envKey, value);
        _envVarsToCleanup.Add(envKey);
        
        var provider = new EnvironmentCredentialProvider(prefix, false, _loggerMock.Object);

        // Act
        var result = await provider.GetCredentialAsync(key);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public async Task GetCredential_NonExistentVariable_ReturnsNull()
    {
        // Arrange
        var provider = new EnvironmentCredentialProvider("TEST_", false, _loggerMock.Object);

        // Act
        var result = await provider.GetCredentialAsync("NonExistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SetCredential_AllowedSet_SetsVariable()
    {
        // Arrange
        const string prefix = "TEST_SET_";
        const string key = "SetKey";
        const string value = "SetValue";
        var envKey = $"{prefix}{key.ToUpperInvariant()}";
        _envVarsToCleanup.Add(envKey);
        
        var provider = new EnvironmentCredentialProvider(prefix, true, _loggerMock.Object);

        // Act
        await provider.SetCredentialAsync(key, value);

        // Assert
        Assert.Equal(value, Environment.GetEnvironmentVariable(envKey));
    }

    [Fact]
    public async Task SetCredential_NotAllowed_ThrowsException()
    {
        // Arrange
        var provider = new EnvironmentCredentialProvider("TEST_", false, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotSupportedException>(() => 
            provider.SetCredentialAsync("key", "value"));
    }

    [Fact]
    public async Task RemoveCredential_AllowedAndExists_RemovesVariable()
    {
        // Arrange
        const string prefix = "TEST_REMOVE_";
        const string key = "RemoveKey";
        var envKey = $"{prefix}{key.ToUpperInvariant()}";
        
        Environment.SetEnvironmentVariable(envKey, "value");
        _envVarsToCleanup.Add(envKey);
        
        var provider = new EnvironmentCredentialProvider(prefix, true, _loggerMock.Object);

        // Act
        var removed = await provider.RemoveCredentialAsync(key);

        // Assert
        Assert.True(removed);
        Assert.Null(Environment.GetEnvironmentVariable(envKey));
    }

    [Fact]
    public async Task RemoveCredential_NotAllowed_ThrowsException()
    {
        // Arrange
        var provider = new EnvironmentCredentialProvider("TEST_", false, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotSupportedException>(() => 
            provider.RemoveCredentialAsync("key"));
    }

    [Fact]
    public async Task ExistsAsync_ExistingVariable_ReturnsTrue()
    {
        // Arrange
        const string prefix = "TEST_EXISTS_";
        const string key = "ExistsKey";
        var envKey = $"{prefix}{key.ToUpperInvariant()}";
        
        Environment.SetEnvironmentVariable(envKey, "value");
        _envVarsToCleanup.Add(envKey);
        
        var provider = new EnvironmentCredentialProvider(prefix, false, _loggerMock.Object);

        // Act
        var exists = await provider.ExistsAsync(key);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_NonExistentVariable_ReturnsFalse()
    {
        // Arrange
        var provider = new EnvironmentCredentialProvider("TEST_", false, _loggerMock.Object);

        // Act
        var exists = await provider.ExistsAsync("NonExistent");

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task ClearAsync_Allowed_ClearsAllPrefixedVariables()
    {
        // Arrange
        const string prefix = "TEST_CLEAR_";
        var keys = new[] { "KEY1", "KEY2", "KEY3" };
        
        foreach (var key in keys)
        {
            var envKey = $"{prefix}{key}";
            Environment.SetEnvironmentVariable(envKey, "value");
            _envVarsToCleanup.Add(envKey);
        }
        
        var provider = new EnvironmentCredentialProvider(prefix, true, _loggerMock.Object);

        // Act
        await provider.ClearAsync();

        // Assert
        foreach (var key in keys)
        {
            Assert.Null(Environment.GetEnvironmentVariable($"{prefix}{key}"));
        }
    }

    [Fact]
    public async Task ClearAsync_NotAllowed_ThrowsException()
    {
        // Arrange
        var provider = new EnvironmentCredentialProvider("TEST_", false, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotSupportedException>(() => provider.ClearAsync());
    }

    [Fact]
    public void GetAllCredentials_ReturnsAllPrefixedVariables()
    {
        // Arrange
        const string prefix = "TEST_GETALL_";
        var credentials = new Dictionary<string, string>
        {
            ["KEY1"] = "value1",
            ["KEY2"] = "value2",
            ["KEY_WITH_UNDERSCORE"] = "value3"
        };
        
        foreach (var kvp in credentials)
        {
            var envKey = $"{prefix}{kvp.Key}";
            Environment.SetEnvironmentVariable(envKey, kvp.Value);
            _envVarsToCleanup.Add(envKey);
        }
        
        var provider = new EnvironmentCredentialProvider(prefix, false, _loggerMock.Object);

        // Act
        var result = provider.GetAllCredentials();

        // Assert
        Assert.Equal(credentials.Count, result.Count);
        foreach (var kvp in credentials)
        {
            Assert.Contains(kvp.Key, result.Keys);
            Assert.Equal(kvp.Value, result[kvp.Key]);
        }
    }

    [Theory]
    [InlineData("key.with.dots", "KEY_WITH_DOTS")]
    [InlineData("key-with-dashes", "KEY_WITH_DASHES")]
    [InlineData("key with spaces", "KEY_WITH_SPACES")]
    [InlineData("MixedCase", "MIXEDCASE")]
    public async Task KeyNormalization_VariousFormats_NormalizesCorrectly(string input, string expected)
    {
        // Arrange
        const string prefix = "TEST_NORM_";
        var envKey = $"{prefix}{expected}";
        const string value = "normalized_value";
        
        Environment.SetEnvironmentVariable(envKey, value);
        _envVarsToCleanup.Add(envKey);
        
        var provider = new EnvironmentCredentialProvider(prefix, false, _loggerMock.Object);

        // Act
        var result = await provider.GetCredentialAsync(input);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public async Task EmptyKey_ThrowsException()
    {
        // Arrange
        var provider = new EnvironmentCredentialProvider("TEST_", false, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            provider.GetCredentialAsync(""));
    }

    [Fact]
    public async Task SetCredential_EmptyKey_ThrowsException()
    {
        // Arrange
        var provider = new EnvironmentCredentialProvider("TEST_", true, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            provider.SetCredentialAsync("", "value"));
    }

    [Fact]
    public async Task SetCredential_NullValue_ThrowsException()
    {
        // Arrange
        var provider = new EnvironmentCredentialProvider("TEST_", true, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            provider.SetCredentialAsync("key", null!));
    }

    [Fact]
    public void EmptyPrefix_WorksCorrectly()
    {
        // Arrange
        const string key = "EMPTY_PREFIX_KEY";
        const string value = "test_value";
        
        Environment.SetEnvironmentVariable(key, value);
        _envVarsToCleanup.Add(key);
        
        var provider = new EnvironmentCredentialProvider("", false, _loggerMock.Object);

        // Act
        var result = provider.GetCredentialAsync("empty.prefix.key").Result;

        // Assert
        Assert.Equal(value, result);
    }

    public void Dispose()
    {
        // Cleanup any environment variables that were set during tests
        foreach (var envVar in _envVarsToCleanup)
        {
            Environment.SetEnvironmentVariable(envVar, null);
        }
    }
}