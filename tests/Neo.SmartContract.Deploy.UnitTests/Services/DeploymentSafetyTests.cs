using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Services;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Shared;
using Neo.SmartContract.Deploy.UnitTests.Mocks;
using Neo.Network.RPC.Models;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests.Services;

public class DeploymentSafetyTests : TestBase
{
    private readonly Mock<ILogger<ContractDeployerService>> _mockLogger;
    private readonly Mock<IWalletManager> _mockWalletManager;
    private readonly TransactionBuilder _transactionBuilder;
    private readonly Mock<ILogger<TransactionConfirmationService>> _mockConfirmationLogger;
    private readonly TransactionConfirmationService _confirmationService;
    
    public DeploymentSafetyTests()
    {
        _mockLogger = new Mock<ILogger<ContractDeployerService>>();
        _mockWalletManager = new Mock<IWalletManager>();
        _transactionBuilder = new TransactionBuilder();
        _mockConfirmationLogger = new Mock<ILogger<TransactionConfirmationService>>();
        _confirmationService = new TransactionConfirmationService(_mockConfirmationLogger.Object);
        
        // Setup wallet manager
        var deployerAccount = UInt160.Parse("0xb1983fa2021e0c36e5e37c2771b8bb7b5c525688");
        _mockWalletManager.Setup(x => x.GetAccount(null)).Returns(deployerAccount);
        _mockWalletManager.Setup(x => x.SignTransactionAsync(It.IsAny<Neo.Network.P2P.Payloads.Transaction>(), It.IsAny<UInt160?>()))
            .Returns(Task.CompletedTask);
    }
    
    [Fact]
    public async Task DeployAsync_WithDryRun_ShouldNotSendTransaction()
    {
        // Arrange
        var mockInvokeResult = new RpcInvokeResult
        {
            State = Neo.VM.VMState.HALT,
            GasConsumed = 10000000, // 0.1 GAS
            Stack = new Neo.VM.Types.StackItem[] { Neo.VM.Types.StackItem.Null }
        };
        
        var mockRpcClient = new MockRpcClient(
            invokeResult: mockInvokeResult,
            blockCount: 1000
        );
        
        var mockRpcClientFactory = new MockRpcClientFactory(() => mockRpcClient);
        
        var deployerService = new ContractDeployerService(
            _mockLogger.Object,
            _mockWalletManager.Object,
            Configuration,
            mockRpcClientFactory,
            _transactionBuilder,
            _confirmationService
        );
        
        var compiledContract = CreateMockCompiledContract();
        var deploymentOptions = new DeploymentOptions
        {
            DryRun = true, // Enable dry-run
            RpcUrl = "http://localhost:50012",
            DeployerAccount = UInt160.Parse("0xb1983fa2021e0c36e5e37c2771b8bb7b5c525688"),
            GasLimit = 100000000
        };
        
        // Act
        var result = await deployerService.DeployAsync(compiledContract, deploymentOptions);
        
        // Assert
        Assert.True(result.Success);
        Assert.True(result.IsDryRun);
        Assert.Equal(UInt256.Zero, result.TransactionHash);
        Assert.Equal(10000000, result.GasConsumed);
        
        // Verify no transaction was signed or sent
        _mockWalletManager.Verify(x => x.SignTransactionAsync(It.IsAny<Neo.Network.P2P.Payloads.Transaction>(), It.IsAny<UInt160?>()), Times.Never);
        
        // Verify dry-run was logged
        _mockLogger.Verify(x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[DRY-RUN]")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
    
    [Fact]
    public async Task DeployAsync_WithVerification_WhenContractExists_ShouldNotFail()
    {
        // Arrange
        var mockInvokeResult = new RpcInvokeResult
        {
            State = Neo.VM.VMState.HALT,
            GasConsumed = 10000000,
            Stack = new Neo.VM.Types.StackItem[] { Neo.VM.Types.StackItem.Null }
        };
        
        var txHash = UInt256.Parse("0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef");
        
        var mockRpcClient = new MockRpcClient(
            invokeResult: mockInvokeResult,
            blockCount: 1000,
            contractExists: true, // Contract exists after deployment
            sentTxHash: txHash
        );
        
        var mockRpcClientFactory = new MockRpcClientFactory(() => mockRpcClient);
        
        var deployerService = new ContractDeployerService(
            _mockLogger.Object,
            _mockWalletManager.Object,
            Configuration,
            mockRpcClientFactory,
            _transactionBuilder,
            _confirmationService
        );
        
        var compiledContract = CreateMockCompiledContract();
        var deploymentOptions = new DeploymentOptions
        {
            DryRun = false,
            VerifyAfterDeploy = true, // Enable verification
            VerificationDelayMs = 100, // Short delay for tests
            RpcUrl = "http://localhost:50012",
            DeployerAccount = UInt160.Parse("0xb1983fa2021e0c36e5e37c2771b8bb7b5c525688"),
            GasLimit = 100000000,
            WaitForConfirmation = false
        };
        
        // Act
        var result = await deployerService.DeployAsync(compiledContract, deploymentOptions);
        
        // Assert
        Assert.True(result.Success);
        Assert.False(result.IsDryRun);
        Assert.Equal(txHash, result.TransactionHash);
        Assert.False(result.VerificationFailed);
        
        // Verify verification success was logged
        _mockLogger.Verify(x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("verification successful")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
    
    [Fact]
    public async Task DeployAsync_WithVerification_WhenContractNotFound_ShouldSetVerificationFailed()
    {
        // Arrange
        var mockInvokeResult = new RpcInvokeResult
        {
            State = Neo.VM.VMState.HALT,
            GasConsumed = 10000000,
            Stack = new Neo.VM.Types.StackItem[] { Neo.VM.Types.StackItem.Null }
        };
        
        var txHash = UInt256.Parse("0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef");
        
        var mockRpcClient = new MockRpcClient(
            invokeResult: mockInvokeResult,
            blockCount: 1000,
            contractExists: false, // Contract does not exist after deployment
            sentTxHash: txHash
        );
        
        var mockRpcClientFactory = new MockRpcClientFactory(() => mockRpcClient);
        
        var deployerService = new ContractDeployerService(
            _mockLogger.Object,
            _mockWalletManager.Object,
            Configuration,
            mockRpcClientFactory,
            _transactionBuilder,
            _confirmationService
        );
        
        var compiledContract = CreateMockCompiledContract();
        var deploymentOptions = new DeploymentOptions
        {
            DryRun = false,
            VerifyAfterDeploy = true,
            VerificationDelayMs = 100,
            RpcUrl = "http://localhost:50012",
            DeployerAccount = UInt160.Parse("0xb1983fa2021e0c36e5e37c2771b8bb7b5c525688"),
            GasLimit = 100000000,
            WaitForConfirmation = false
        };
        
        // Act
        var result = await deployerService.DeployAsync(compiledContract, deploymentOptions);
        
        // Assert
        Assert.True(result.Success); // Deployment itself succeeded
        Assert.False(result.IsDryRun);
        Assert.Equal(txHash, result.TransactionHash);
        Assert.True(result.VerificationFailed); // But verification failed
        
        // Verify verification failure was logged
        _mockLogger.Verify(x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("verification failed")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
    
    [Fact]
    public async Task DeployAsync_WithAllSafetyFeatures_ShouldWorkCorrectly()
    {
        // Arrange
        var mockInvokeResult = new RpcInvokeResult
        {
            State = Neo.VM.VMState.HALT,
            GasConsumed = 10000000,
            Stack = new Neo.VM.Types.StackItem[] { Neo.VM.Types.StackItem.Null }
        };
        
        var deploymentOptions = new DeploymentOptions
        {
            DryRun = true,
            VerifyAfterDeploy = true,
            VerificationDelayMs = 100,
            EnableRollback = true, // Future feature
            RpcUrl = "http://localhost:50012",
            DeployerAccount = UInt160.Parse("0xb1983fa2021e0c36e5e37c2771b8bb7b5c525688"),
            GasLimit = 100000000
        };
        
        // Test 1: Dry run first
        var mockRpcClient1 = new MockRpcClient(invokeResult: mockInvokeResult);
        var mockRpcClientFactory1 = new MockRpcClientFactory(() => mockRpcClient1);
        
        var deployerService1 = new ContractDeployerService(
            _mockLogger.Object,
            _mockWalletManager.Object,
            Configuration,
            mockRpcClientFactory1,
            _transactionBuilder,
            _confirmationService
        );
        
        var compiledContract = CreateMockCompiledContract();
        var dryRunResult = await deployerService1.DeployAsync(compiledContract, deploymentOptions);
        
        Assert.True(dryRunResult.Success);
        Assert.True(dryRunResult.IsDryRun);
        Assert.Equal(10000000, dryRunResult.GasConsumed);
        
        // Test 2: Actual deployment with verification
        deploymentOptions.DryRun = false;
        
        var txHash = UInt256.Parse("0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef");
        var mockRpcClient2 = new MockRpcClient(
            invokeResult: mockInvokeResult,
            blockCount: 1000,
            contractExists: true,
            sentTxHash: txHash
        );
        var mockRpcClientFactory2 = new MockRpcClientFactory(() => mockRpcClient2);
        
        var deployerService2 = new ContractDeployerService(
            _mockLogger.Object,
            _mockWalletManager.Object,
            Configuration,
            mockRpcClientFactory2,
            _transactionBuilder,
            _confirmationService
        );
        
        var actualResult = await deployerService2.DeployAsync(compiledContract, deploymentOptions);
        
        Assert.True(actualResult.Success);
        Assert.False(actualResult.IsDryRun);
        Assert.False(actualResult.VerificationFailed);
        Assert.Equal(txHash, actualResult.TransactionHash);
    }
}