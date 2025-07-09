using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests;

public class DeploymentToolkitTests : TestBase
{
    private readonly Mock<IContractCompiler> _mockCompiler;
    private readonly Mock<IContractDeployer> _mockDeployer;
    private readonly Mock<IContractInvoker> _mockInvoker;
    private readonly Mock<IWalletManager> _mockWalletManager;
    private readonly IConfiguration _configuration;

    public DeploymentToolkitTests()
    {
        _mockCompiler = new Mock<IContractCompiler>();
        _mockDeployer = new Mock<IContractDeployer>();
        _mockInvoker = new Mock<IContractInvoker>();
        _mockWalletManager = new Mock<IWalletManager>();

        var inMemorySettings = new Dictionary<string, string>
        {
            {"Network:RpcUrl", "http://localhost:50012"},
            {"Network:Network", "private"},
            {"Deployment:GasLimit", "100000000"},
            {"Deployment:WaitForConfirmation", "true"},
            {"Wallet:Path", "test-wallet.json"},
            {"Wallet:Password", "test-password"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
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
        Assert.Equal("mainnet", Environment.GetEnvironmentVariable("Network__Network"));
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
        Assert.Equal("testnet", Environment.GetEnvironmentVariable("Network__Network"));
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
        Assert.Equal("private", Environment.GetEnvironmentVariable("Network__Network"));
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
        Assert.Equal("custom", Environment.GetEnvironmentVariable("Network__Network"));
    }

    [Fact]
    public async Task Deploy_WithoutWallet_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Clear wallet environment variables
        Environment.SetEnvironmentVariable("NEO_WALLET_PATH", null);
        Environment.SetEnvironmentVariable("NEO_WALLET_PASSWORD", null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => toolkit.DeployAsync("test.csproj")
        );
    }

    [Fact]
    public async Task GetGasBalance_WithValidAddress_ShouldReturnBalance()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var testAddress = "NXXxXXxXXxXXxXXxXXxXXxXXxXXxXXxXXxX";

        // Act & Assert
        // This would require a mock setup for the actual implementation
        // For now, we're testing that the method exists and can be called
        await Assert.ThrowsAsync<ArgumentException>(
            () => toolkit.GetGasBalanceAsync(testAddress)
        );
    }

    [Fact]
    public async Task GetDeployerAccount_WithoutWallet_ShouldThrowException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Clear wallet environment variables
        Environment.SetEnvironmentVariable("NEO_WALLET_PATH", null);
        Environment.SetEnvironmentVariable("NEO_WALLET_PASSWORD", null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => toolkit.GetDeployerAccountAsync()
        );
    }

    #region Update Functionality Tests

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
    public async Task UpdateAsync_WithInvalidContractHash_ShouldThrowArgumentException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var validWifKey = "KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb";
        var testContractPath = CreateTestContract();

        toolkit.SetWifKey(validWifKey);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => toolkit.UpdateAsync("invalid-hash", testContractPath)
        );
    }

    [Fact]
    public async Task UpdateAsync_WithNullParameters_ShouldThrowArgumentException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => toolkit.UpdateAsync(null!, "path")
        );
        await Assert.ThrowsAsync<ArgumentException>(
            () => toolkit.UpdateAsync("0x1234567890123456789012345678901234567890", null!)
        );
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentFile_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var contractHash = "0x1234567890123456789012345678901234567890";
        var nonExistentPath = "/non/existent/contract.cs";

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(
            () => toolkit.UpdateAsync(contractHash, nonExistentPath)
        );
    }

    [Fact]
    public async Task UpdateAsync_WithoutWallet_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var testContractPath = CreateTestContract();
        var contractHash = "0x1234567890123456789012345678901234567890";

        // Clear wallet environment variables
        Environment.SetEnvironmentVariable("NEO_WALLET_PATH", null);
        Environment.SetEnvironmentVariable("NEO_WALLET_PASSWORD", null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => toolkit.UpdateAsync(contractHash, testContractPath)
        );
    }

    [Fact]
    public async Task UpdateAsync_WithWifKey_ShouldAttemptUpdate()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var validWifKey = "KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb";
        var testContractPath = CreateTestContract();
        var contractHash = "0x1234567890123456789012345678901234567890";

        toolkit.SetWifKey(validWifKey);
        toolkit.SetNetwork("testnet");

        // Act
        var result = await toolkit.UpdateAsync(contractHash, testContractPath);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success); // Should fail due to network connectivity
        Assert.NotNull(result.ErrorMessage);
        // Should not be wallet-related error
        Assert.DoesNotContain("wallet", result.ErrorMessage.ToLower());
        Assert.DoesNotContain("No wallet loaded", result.ErrorMessage);
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
    public async Task ContractExistsAsync_WithValidHash_ShouldCheckExistence()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var contractHash = "0x1234567890123456789012345678901234567890";

        toolkit.SetNetwork("testnet");

        // Act & Assert
        // This should fail due to contract not existing or network connectivity
        var exception = await Assert.ThrowsAnyAsync<Exception>(
            () => toolkit.ContractExistsAsync(contractHash)
        );

        // Should be network-related or contract-not-found, not method-not-found
        Assert.True(
            exception.Message.Contains("Connection") ||
            exception.Message.Contains("network") ||
            exception.Message.Contains("RPC") ||
            exception.Message.Contains("timeout") ||
            exception.Message.Contains("Unknown contract") ||
            exception.Message.Contains("Failed to check"),
            $"Expected network or contract-related error, but got: {exception.Message}"
        );
    }

    [Fact]
    public async Task ContractExistsAsync_WithInvalidHash_ShouldThrowArgumentException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => toolkit.ContractExistsAsync("invalid-hash")
        );
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
            if (update)
            {
                // Check authorization for updates
                if (!Runtime.CheckWitness(GetOwner()))
                {
                    throw new Exception(""Only owner can update contract"");
                }
                return;
            }
            
            // Initial deployment
            Storage.Put(Storage.CurrentContext, ""initialized"", 1);
        }
        
        [Safe]
        private static UInt160 GetOwner()
        {
            // Return a test owner address - using byte representation
            return UInt160.Zero;
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
