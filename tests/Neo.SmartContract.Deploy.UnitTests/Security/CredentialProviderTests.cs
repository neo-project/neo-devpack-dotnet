using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Neo.SmartContract.Deploy.Security;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests.Security;

public class CredentialProviderTests
{
    [Fact]
    public async Task EnvironmentCredentialProvider_WithGeneralPassword_ShouldReturn()
    {
        // Arrange
        var password = "test-password-general";
        Environment.SetEnvironmentVariable("NEO_WALLET_PASSWORD", password);

        try
        {
            var logger = new Mock<ILogger<EnvironmentCredentialProvider>>();
            var provider = new EnvironmentCredentialProvider(logger.Object);

            // Act
            var result = await provider.GetWalletPasswordAsync("wallet.json");

            // Assert
            Assert.Equal(password, result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("NEO_WALLET_PASSWORD", null);
        }
    }

    [Fact]
    public async Task EnvironmentCredentialProvider_WithSpecificPassword_ShouldPreferSpecific()
    {
        // Arrange
        var generalPassword = "general-password";
        var specificPassword = "specific-password";
        Environment.SetEnvironmentVariable("NEO_WALLET_PASSWORD", generalPassword);
        Environment.SetEnvironmentVariable("NEO_WALLET_PASSWORD_TESTWALLET", specificPassword);

        try
        {
            var logger = new Mock<ILogger<EnvironmentCredentialProvider>>();
            var provider = new EnvironmentCredentialProvider(logger.Object);

            // Act
            var result = await provider.GetWalletPasswordAsync("testwallet.json");

            // Assert
            Assert.Equal(specificPassword, result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("NEO_WALLET_PASSWORD", null);
            Environment.SetEnvironmentVariable("NEO_WALLET_PASSWORD_TESTWALLET", null);
        }
    }

    [Fact]
    public async Task EnvironmentCredentialProvider_WithoutPassword_ShouldThrow()
    {
        // Arrange
        var logger = new Mock<ILogger<EnvironmentCredentialProvider>>();
        var provider = new EnvironmentCredentialProvider(logger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => provider.GetWalletPasswordAsync("wallet.json"));
    }

    [Fact]
    public async Task EnvironmentCredentialProvider_GetRpcCredentials_ShouldReturnWhenSet()
    {
        // Arrange
        var username = "rpc-user";
        var password = "rpc-password";
        Environment.SetEnvironmentVariable("NEO_RPC_USER", username);
        Environment.SetEnvironmentVariable("NEO_RPC_PASSWORD", password);

        try
        {
            var logger = new Mock<ILogger<EnvironmentCredentialProvider>>();
            var provider = new EnvironmentCredentialProvider(logger.Object);

            // Act
            var result = await provider.GetRpcCredentialsAsync("http://localhost:10332");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(username, result.Value.username);
            Assert.Equal(password, result.Value.password);
        }
        finally
        {
            Environment.SetEnvironmentVariable("NEO_RPC_USER", null);
            Environment.SetEnvironmentVariable("NEO_RPC_PASSWORD", null);
        }
    }

    [Fact]
    public async Task SecureCredentialProvider_WithCache_ShouldReusePassword()
    {
        // Arrange
        var password = "cached-password";
        Environment.SetEnvironmentVariable("NEO_WALLET_PASSWORD", password);

        try
        {
            var logger = new Mock<ILogger<SecureCredentialProvider>>();
            var config = new ConfigurationBuilder().Build();
            var provider = new SecureCredentialProvider(logger.Object, config);

            // Act
            var result1 = await provider.GetWalletPasswordAsync("wallet.json");
            Environment.SetEnvironmentVariable("NEO_WALLET_PASSWORD", "different-password");
            var result2 = await provider.GetWalletPasswordAsync("wallet.json");

            // Assert
            Assert.Equal(password, result1);
            Assert.Equal(password, result2); // Should use cached value
        }
        finally
        {
            Environment.SetEnvironmentVariable("NEO_WALLET_PASSWORD", null);
        }
    }

    [Fact]
    public async Task SecureCredentialProvider_ClearCache_ShouldRemoveCachedPasswords()
    {
        // Arrange
        var password = "cached-password";
        Environment.SetEnvironmentVariable("NEO_WALLET_PASSWORD", password);

        try
        {
            var logger = new Mock<ILogger<SecureCredentialProvider>>();
            var config = new ConfigurationBuilder().Build();
            var provider = new SecureCredentialProvider(logger.Object, config);

            // Prime the cache
            var _ = await provider.GetWalletPasswordAsync("wallet.json");

            // Act
            provider.ClearCache();
            Environment.SetEnvironmentVariable("NEO_WALLET_PASSWORD", "new-password");
            var result = await provider.GetWalletPasswordAsync("wallet.json");

            // Assert
            Assert.Equal("new-password", result); // Should not use cached value
        }
        finally
        {
            Environment.SetEnvironmentVariable("NEO_WALLET_PASSWORD", null);
        }
    }

    [Fact]
    public async Task SecureCredentialProvider_WithConfiguration_ShouldUseAsLastResort()
    {
        // Arrange
        var configPassword = "config-password";
        var configData = new Dictionary<string, string>
        {
            ["Wallet:Password"] = configPassword
        };

        var logger = new Mock<ILogger<SecureCredentialProvider>>();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();
        var provider = new SecureCredentialProvider(logger.Object, config);

        // Act
        var result = await provider.GetWalletPasswordAsync("wallet.json");

        // Assert
        Assert.Equal(configPassword, result);

        // Verify warning was logged
        logger.Verify(x => x.Log(
            Microsoft.Extensions.Logging.LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("not recommended for production")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
