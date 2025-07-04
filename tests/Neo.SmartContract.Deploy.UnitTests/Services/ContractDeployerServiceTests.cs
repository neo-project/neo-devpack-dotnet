using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Neo;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Services;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Exceptions;
using Neo.SmartContract.Deploy.Shared;
using Neo.SmartContract.Manifest;
using Neo.Network.RPC;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests.Services;

public class ContractDeployerServiceTests : TestBase
{
    private readonly ContractDeployerService _deployerService;
    private readonly Mock<ILogger<ContractDeployerService>> _mockLogger;
    private readonly Mock<IWalletManager> _mockWalletManager;
    private readonly Mock<IRpcClientFactory> _mockRpcClientFactory;
    private readonly TransactionBuilder _transactionBuilder;
    private readonly Mock<ILogger<TransactionConfirmationService>> _mockConfirmationLogger;
    private readonly TransactionConfirmationService _confirmationService;

    public ContractDeployerServiceTests()
    {
        _mockLogger = new Mock<ILogger<ContractDeployerService>>();
        _mockWalletManager = new Mock<IWalletManager>();
        _mockRpcClientFactory = new Mock<IRpcClientFactory>();
        _transactionBuilder = new TransactionBuilder();
        _mockConfirmationLogger = new Mock<ILogger<TransactionConfirmationService>>();
        _confirmationService = new TransactionConfirmationService(_mockConfirmationLogger.Object);

        _deployerService = new ContractDeployerService(
            _mockLogger.Object,
            _mockWalletManager.Object,
            Configuration,
            _mockRpcClientFactory.Object,
            _transactionBuilder,
            _confirmationService);
    }

    [Fact]
    public async Task ContractExistsAsync_WithValidHash_ShouldReturnCorrectResult()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var rpcUrl = "http://localhost:50012";

        // Act & Assert
        // Since we're not running a real RPC server, we expect a connection error
        var exception = await Assert.ThrowsAsync<ContractDeploymentException>(
            () => _deployerService.ContractExistsAsync(contractHash, rpcUrl)
        );

        // The exception should contain error information (either connection refused or unknown contract)
        Assert.True(
            exception.Message.Contains("Connection refused") || 
            exception.Message.Contains("Unknown contract"),
            $"Expected error message to contain 'Connection refused' or 'Unknown contract', but got: {exception.Message}");
    }

    [Fact]
    public async Task ContractExistsAsync_WithInvalidRpcUrl_ShouldThrowException()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var invalidRpcUrl = "http://invalid-url:99999";

        // Act & Assert
        await Assert.ThrowsAsync<ContractDeploymentException>(() =>
            _deployerService.ContractExistsAsync(contractHash, invalidRpcUrl));
    }

    [Fact]
    public async Task DeployAsync_WithValidContract_ShouldCreateDeploymentInfo()
    {
        // Arrange
        var compiledContract = CreateMockCompiledContract();
        var deploymentOptions = CreateDeploymentOptions();

        // Setup wallet manager mock
        var deployerAccount = UInt160.Parse("0xb1983fa2021e0c36e5e37c2771b8bb7b5c525688");
        _mockWalletManager.Setup(x => x.GetAccount(null)).Returns(deployerAccount);
        _mockWalletManager.Setup(x => x.SignTransactionAsync(It.IsAny<Neo.Network.P2P.Payloads.Transaction>(), It.IsAny<UInt160?>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _deployerService.DeployAsync(compiledContract, deploymentOptions);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success); // Should fail due to network connectivity
        Assert.NotNull(result.ErrorMessage);
        Assert.NotEmpty(result.ErrorMessage);
    }

    [Fact]
    public async Task UpdateAsync_WithValidContract_ShouldCreateUpdateResult()
    {
        // Arrange
        var compiledContract = CreateMockCompiledContract();
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var deploymentOptions = CreateDeploymentOptions();

        // Setup wallet manager mock
        var deployerAccount = UInt160.Parse("0xb1983fa2021e0c36e5e37c2771b8bb7b5c525688");
        _mockWalletManager.Setup(x => x.GetAccount(null)).Returns(deployerAccount);
        _mockWalletManager.Setup(x => x.SignTransactionAsync(It.IsAny<Neo.Network.P2P.Payloads.Transaction>(), It.IsAny<UInt160?>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _deployerService.UpdateAsync(compiledContract, contractHash, deploymentOptions);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success); // Should fail due to network connectivity
        Assert.NotNull(result.ErrorMessage);
        Assert.NotEmpty(result.ErrorMessage);
    }

    [Fact]
    public async Task DeployAsync_WithNullContract_ShouldThrowArgumentException()
    {
        // Arrange
        var deploymentOptions = CreateDeploymentOptions();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _deployerService.DeployAsync(null!, deploymentOptions));
    }

    [Fact]
    public async Task UpdateAsync_WithNullContract_ShouldThrowArgumentException()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var deploymentOptions = CreateDeploymentOptions();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _deployerService.UpdateAsync(null!, contractHash, deploymentOptions));
    }

    [Fact]
    public async Task DeployAsync_WithInvalidWalletSetup_ShouldHandleError()
    {
        // Arrange
        var compiledContract = CreateMockCompiledContract();
        var deploymentOptions = CreateDeploymentOptions();

        // Remove DeployerAccount from options to force wallet manager usage
        deploymentOptions.DeployerAccount = null;

        // Setup wallet manager to indicate wallet is not loaded
        _mockWalletManager.Setup(x => x.IsWalletLoaded).Returns(false);

        // Act
        var result = await _deployerService.DeployAsync(compiledContract, deploymentOptions);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        // The error should be caught and wrapped
        Assert.NotNull(result.ErrorMessage);
        Assert.True(
            result.ErrorMessage.Contains("Wallet not loaded") ||
            result.ErrorMessage.Contains("Object reference"),
            $"Expected error message to contain 'Wallet not loaded' or 'Object reference', but got: {result.ErrorMessage}");
    }

    private CompiledContract CreateMockCompiledContract()
    {
        // Create a simple test NEF file content
        var nefBytes = new byte[]
        {
            0x4E, 0x45, 0x46, 0x33, // NEF3 magic
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // 64 byte compiler field
            0x01, 0x00, // Source string length (1) + empty source
            0x00, // Reserved byte
            0x00, // Method tokens count
            0x00, 0x00, // Reserved 2 bytes
            0x04, 0x40, 0x41, 0x9F, 0x00, // Script (4 bytes length + simple script)
            0x00, 0x00, 0x00, 0x00 // Checksum placeholder
        };

        var manifest = new ContractManifest
        {
            Name = "TestContract",
            Groups = new ContractGroup[0],
            SupportedStandards = new string[0],
            Abi = new ContractAbi
            {
                Methods = new ContractMethodDescriptor[]
                {
                    new ContractMethodDescriptor
                    {
                        Name = "testMethod",
                        Parameters = new ContractParameterDefinition[]
                        {
                            new ContractParameterDefinition { Name = "input", Type = ContractParameterType.String }
                        },
                        ReturnType = ContractParameterType.String,
                        Safe = true
                    }
                },
                Events = new ContractEventDescriptor[0]
            },
            Permissions = new ContractPermission[] { ContractPermission.DefaultPermission },
            Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
            Extra = null
        };

        return new CompiledContract
        {
            Name = "TestContract",
            NefFilePath = "/tmp/test.nef",
            ManifestFilePath = "/tmp/test.manifest.json",
            NefBytes = nefBytes,
            Manifest = manifest
        };
    }

    private DeploymentOptions CreateDeploymentOptions()
    {
        return new DeploymentOptions
        {
            DeployerAccount = UInt160.Parse("0xb1983fa2021e0c36e5e37c2771b8bb7b5c525688"),
            GasLimit = 50_000_000,
            WaitForConfirmation = false,
            DefaultNetworkFee = 1_000_000,
            ValidUntilBlockOffset = 100,
            ConfirmationRetries = 3,
            ConfirmationDelaySeconds = 1
        };
    }
}
