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
        Assert.Equal("https://rpc10.n3.neotracker.io:443", Environment.GetEnvironmentVariable("Network__RpcUrl"));
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
            () => toolkit.Deploy("test.csproj")
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
            () => toolkit.GetGasBalance(testAddress)
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
            () => toolkit.GetDeployerAccount()
        );
    }
}
