using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Neo;
using Neo.SmartContract.Deploy.Services;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.Wallets;
using Microsoft.Extensions.Logging;

namespace Neo.SmartContract.Deploy.UnitTests.Services;

public class ContractDeployerServiceTests : TestBase
{
    private readonly ContractDeployerService _deployerService;
    private readonly Mock<IWalletManager> _mockWalletManager;

    public ContractDeployerServiceTests()
    {
        _mockWalletManager = new Mock<IWalletManager>();
        _deployerService = new ContractDeployerService(_mockWalletManager.Object);
    }

    [Fact]
    public async Task DeployAsync_WithNullContract_ShouldThrowArgumentNullException()
    {
        // Arrange
        var deploymentOptions = CreateDeploymentOptions();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _deployerService.DeployAsync(null!, deploymentOptions));
    }

    [Fact]
    public async Task ContractExistsAsync_WithoutRpcUrl_ShouldThrowNotSupportedException()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");

        // Act & Assert
        await Assert.ThrowsAsync<NotSupportedException>(() =>
            _deployerService.ContractExistsAsync(contractHash));
    }

    [Fact]
    public async Task ContractExistsAsync_WithNullHash_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rpcUrl = "http://localhost:50012";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _deployerService.ContractExistsAsync(null!, rpcUrl));
    }

    private CompiledContract CreateMockCompiledContract()
    {
        return new CompiledContract
        {
            Name = "TestContract",
            NefFilePath = "/tmp/test.nef",
            ManifestFilePath = "/tmp/test.manifest.json",
            NefBytes = new byte[] { 0x4E, 0x45, 0x46, 0x33 }, // Simple NEF header
            Manifest = new Neo.SmartContract.Manifest.ContractManifest
            {
                Name = "TestContract"
            }
        };
    }

    [Fact]
    public async Task DeployAsync_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Arrange
        var compiledContract = CreateMockCompiledContract();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _deployerService.DeployAsync(compiledContract, null!));
    }

    [Fact]
    public async Task DeployAsync_WithMissingRpcUrl_ShouldThrowArgumentException()
    {
        // Arrange
        var compiledContract = CreateMockCompiledContract();
        var deploymentOptions = new DeploymentOptions
        {
            WifKey = "L1QqQJnpBwbsPGAuutuzPTac8piqvbR1HRjrY5qHup48TBCBFe4g",
            // RpcUrl is missing
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _deployerService.DeployAsync(compiledContract, deploymentOptions));
    }

    [Fact]
    public async Task DeployAsync_WithMissingWifKey_ShouldThrowArgumentException()
    {
        // Arrange
        var compiledContract = CreateMockCompiledContract();
        var deploymentOptions = new DeploymentOptions
        {
            RpcUrl = "http://localhost:50012",
            // WifKey is missing
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _deployerService.DeployAsync(compiledContract, deploymentOptions));
    }

    [Fact]
    public async Task ContractExistsAsync_WithEmptyRpcUrl_ShouldThrowArgumentException()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _deployerService.ContractExistsAsync(contractHash, ""));
    }

    private DeploymentOptions CreateDeploymentOptions()
    {
        return new DeploymentOptions
        {
            DeployerAccount = UInt160.Parse("0xb1983fa2021e0c36e5e37c2771b8bb7b5c525688"),
            WifKey = "L1QqQJnpBwbsPGAuutuzPTac8piqvbR1HRjrY5qHup48TBCBFe4g",
            RpcUrl = "http://localhost:50012",
            GasLimit = 50_000_000,
            WaitForConfirmation = false
        };
    }
}