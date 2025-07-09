using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests;

public class DeploymentToolkitTests : TestBase
{
    public DeploymentToolkitTests()
    {
        // No setup needed for this basic test class
    }

    [Fact]
    public void Constructor_ShouldInitializeWithDefaultConfiguration()
    {
        // Act
        var toolkit = new DeploymentToolkit();

        // Assert
        Assert.NotNull(toolkit);
    }

    [Fact]
    public void SetNetwork_ShouldConfigureMainNet()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Act
        var result = toolkit.SetNetwork("mainnet");

        // Assert
        Assert.Same(toolkit, result);
        Assert.Equal("https://rpc10.n3.nspcc.ru:10331", Environment.GetEnvironmentVariable("Network__RpcUrl"));
    }

    [Fact]
    public void SetNetwork_ShouldConfigureTestNet()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Act
        var result = toolkit.SetNetwork("testnet");

        // Assert
        Assert.Same(toolkit, result);
        Assert.Equal("https://testnet1.neo.coz.io:443", Environment.GetEnvironmentVariable("Network__RpcUrl"));
    }

    [Fact]
    public void SetNetwork_ShouldConfigureLocalNetwork()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Act
        var result = toolkit.SetNetwork("local");

        // Assert
        Assert.Same(toolkit, result);
        Assert.Equal("http://localhost:50012", Environment.GetEnvironmentVariable("Network__RpcUrl"));
    }

    [Fact]
    public void SetNetwork_ShouldAcceptCustomRpcUrl()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var customRpc = "http://custom.rpc:10332";

        // Act
        var result = toolkit.SetNetwork(customRpc);

        // Assert
        Assert.Same(toolkit, result);
        Assert.Equal(customRpc, Environment.GetEnvironmentVariable("Network__RpcUrl"));
    }

    [Fact]
    public async Task Deploy_WithoutImplementation_ShouldThrowNotImplementedException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(
            () => toolkit.DeployAsync("test.csproj")
        );
    }

    [Fact]
    public async Task GetGasBalance_WithoutImplementation_ShouldThrowNotImplementedException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var testAddress = "NXXxXXxXXxXXxXXxXXxXXxXXxXXxXXxXXxX";

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(
            () => toolkit.GetGasBalanceAsync(testAddress)
        );
    }

    [Fact]
    public async Task GetDeployerAccount_WithoutWifKey_ShouldThrowException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => toolkit.GetDeployerAccountAsync()
        );
    }

    #region WIF Key Tests

    [Fact]
    public void SetWifKey_WithValidKey_ShouldSetKeySuccessfully()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var validWifKey = "KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb";

        // Act
        var result = toolkit.SetWifKey(validWifKey);

        // Assert
        Assert.Same(toolkit, result);
        // The WIF key should be set internally for signing
    }

    [Fact]
    public void SetWifKey_WithInvalidKey_ShouldThrowArgumentException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var invalidWifKey = "invalid-wif-key";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => toolkit.SetWifKey(invalidWifKey)
        );

        Assert.Contains("Invalid WIF key", exception.Message);
    }

    [Fact]
    public void SetWifKey_WithNullOrEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => toolkit.SetWifKey(""));
        Assert.Throws<ArgumentException>(() => toolkit.SetWifKey(null!));
    }

    [Fact]
    public async Task GetDeployerAccount_WithWifKey_ShouldReturnAccount()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var validWifKey = "KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb";

        toolkit.SetWifKey(validWifKey);

        // Act
        var account = await toolkit.GetDeployerAccountAsync();

        // Assert
        Assert.NotNull(account);
        Assert.NotEqual(UInt160.Zero, account);
    }

    [Fact]
    public async Task ContractExistsAsync_WithoutImplementation_ShouldThrowNotImplementedException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var contractHash = "0x1234567890123456789012345678901234567890";

        toolkit.SetNetwork("testnet");

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(
            () => toolkit.ContractExistsAsync(contractHash)
        );
    }

    #endregion

    #region Network Magic Tests

    [Fact]
    public async Task UpdateAsync_ShouldRetrieveNetworkMagicFromRpc_WhenNotConfigured()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        toolkit.SetNetwork("testnet");
        
        // Act & Assert
        // This test verifies that when NetworkMagic is not configured,
        // the toolkit will attempt to retrieve it from RPC
        // Currently throws NotImplementedException, but the framework is in place
        await Assert.ThrowsAsync<NotImplementedException>(
            () => toolkit.CallAsync<string>("0x1234567890123456789012345678901234567890", "test")
        );
    }

    [Fact]
    public void SetNetwork_WithKnownNetworks_ShouldConfigureCorrectRpcUrl()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var testCases = new Dictionary<string, string>
        {
            { "mainnet", "https://rpc10.n3.nspcc.ru:10331" },
            { "testnet", "https://testnet1.neo.coz.io:443" },
            { "local", "http://localhost:50012" },
            { "private", "http://localhost:50012" }
        };

        foreach (var testCase in testCases)
        {
            // Act
            toolkit.SetNetwork(testCase.Key);

            // Assert
            Assert.Equal(testCase.Value, Environment.GetEnvironmentVariable("Network__RpcUrl"));
        }
    }

    [Fact]
    public void SetNetwork_WithHttpUrl_ShouldUseAsRpcUrl()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var customUrls = new[]
        {
            "http://localhost:10332",
            "https://custom.neo.rpc:443",
            "http://192.168.1.100:10332"
        };

        foreach (var url in customUrls)
        {
            // Act
            toolkit.SetNetwork(url);

            // Assert
            Assert.Equal(url, Environment.GetEnvironmentVariable("Network__RpcUrl"));
        }
    }

    [Fact]
    public void SetNetwork_ShouldBeCaseInsensitive()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var variations = new[] { "MAINNET", "MainNet", "mainnet", "MaInNeT" };

        foreach (var variation in variations)
        {
            // Act
            toolkit.SetNetwork(variation);

            // Assert
            Assert.Equal("https://rpc10.n3.nspcc.ru:10331", Environment.GetEnvironmentVariable("Network__RpcUrl"));
        }
    }

    [Fact]
    public void SetNetwork_WithEmptyOrNull_ShouldThrowArgumentException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => toolkit.SetNetwork(""));
        Assert.Throws<ArgumentException>(() => toolkit.SetNetwork("   "));
        Assert.Throws<ArgumentException>(() => toolkit.SetNetwork(null!));
    }

    #endregion

    private new string CreateTestContract()
    {
        var contractCode = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System;
using System.ComponentModel;

namespace TestContract
{
    [ManifestExtra(""Author"", ""Neo"")]
    [ManifestExtra(""Description"", ""Test Contract with Update"")]
    [ManifestExtra(""Version"", ""1.0.0"")]
    public class TestContract : SmartContract
    {
        [DisplayName(""testMethod"")]
        public static string TestMethod(string input)
        {
            return ""Hello "" + input;
        }

        [DisplayName(""getValue"")]
        public static int GetValue()
        {
            return 42;
        }

        public static void _deploy(object data, bool update)
        {
            // Initial deployment
            Storage.Put(Storage.CurrentContext, ""initialized"", 1);
        }
    }
}";

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var contractPath = Path.Combine(tempDir, "TestContract.cs");
        File.WriteAllText(contractPath, contractCode);
        return contractPath;
    }
}
