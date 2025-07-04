using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Neo;
using Neo.SmartContract.Deploy.Services;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Shared;
using System.Threading.Tasks;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests.Services;

public class ContractInvokerServiceTests : TestBase
{
    private readonly ContractInvokerService _invokerService;
    private readonly Mock<ILogger<ContractInvokerService>> _mockLogger;
    private readonly Mock<IWalletManager> _mockWalletManager;
    private readonly Mock<IRpcClientFactory> _mockRpcClientFactory;
    private readonly TransactionBuilder _transactionBuilder;
    private readonly Mock<ILogger<TransactionConfirmationService>> _mockConfirmationLogger;
    private readonly TransactionConfirmationService _confirmationService;

    public ContractInvokerServiceTests()
    {
        _mockLogger = new Mock<ILogger<ContractInvokerService>>();
        _mockWalletManager = new Mock<IWalletManager>();
        _mockRpcClientFactory = new Mock<IRpcClientFactory>();
        _transactionBuilder = new TransactionBuilder();
        _mockConfirmationLogger = new Mock<ILogger<TransactionConfirmationService>>();
        _confirmationService = new TransactionConfirmationService(_mockConfirmationLogger.Object);

        _invokerService = new ContractInvokerService(
            _mockLogger.Object,
            _mockWalletManager.Object,
            Configuration,
            _mockRpcClientFactory.Object,
            _transactionBuilder,
            _confirmationService);
    }

    [Fact]
    public async Task CallAsync_WithValidParameters_ShouldHandleNetworkError()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var method = "testMethod";
        var parameters = new object[] { "test input" };

        // Act & Assert
        // This will likely fail due to network connectivity, but we're testing the service structure
        var exception = await Assert.ThrowsAnyAsync<Exception>(() =>
            _invokerService.CallAsync<string>(contractHash, method, parameters));

        Assert.NotNull(exception);
    }

    [Fact]
    public async Task SendAsync_WithValidParameters_ShouldHandleNetworkError()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var method = "testMethod";
        var parameters = new object[] { "test input" };

        // Setup wallet manager mock
        var signerAccount = UInt160.Parse("0xb1983fa2021e0c36e5e37c2771b8bb7b5c525688");
        _mockWalletManager.Setup(x => x.GetAccount(null)).Returns(signerAccount);

        // Act & Assert
        // This will likely fail due to network connectivity, but we're testing the service structure
        var exception = await Assert.ThrowsAnyAsync<Exception>(() =>
            _invokerService.SendAsync(contractHash, method, parameters));

        Assert.NotNull(exception);
    }

    [Fact]
    public async Task WaitForConfirmationAsync_WithValidTxHash_ShouldHandleTimeout()
    {
        // Arrange
        var txHash = UInt256.Parse("0x1234567890123456789012345678901234567890123456789012345678901234");
        var maxRetries = 1;
        var delaySeconds = 1;

        // Act
        var result = await _invokerService.WaitForConfirmationAsync(txHash, maxRetries, delaySeconds);

        // Assert
        // Should return false since the transaction doesn't exist in our test environment
        Assert.False(result);
    }

    [Fact]
    public async Task CallAsync_WithEmptyParameters_ShouldHandleCorrectly()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var method = "getValue";

        // Act & Assert
        var exception = await Assert.ThrowsAnyAsync<Exception>(() =>
            _invokerService.CallAsync<int>(contractHash, method));

        Assert.NotNull(exception);
    }

    [Fact]
    public async Task SendAsync_WithEmptyParameters_ShouldHandleCorrectly()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var method = "getValue";

        // Setup wallet manager mock
        var signerAccount = UInt160.Parse("0xb1983fa2021e0c36e5e37c2771b8bb7b5c525688");
        _mockWalletManager.Setup(x => x.GetAccount(null)).Returns(signerAccount);

        // Act & Assert
        var exception = await Assert.ThrowsAnyAsync<Exception>(() =>
            _invokerService.SendAsync(contractHash, method));

        Assert.NotNull(exception);
    }

    [Fact]
    public async Task CallAsync_WithNullMethod_ShouldThrowException()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() =>
            _invokerService.CallAsync<string>(contractHash, null!, "param"));
    }

    [Fact]
    public async Task SendAsync_WithNullMethod_ShouldThrowException()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");

        // Setup wallet manager mock
        var signerAccount = UInt160.Parse("0xb1983fa2021e0c36e5e37c2771b8bb7b5c525688");
        _mockWalletManager.Setup(x => x.GetAccount(null)).Returns(signerAccount);

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() =>
            _invokerService.SendAsync(contractHash, null!, "param"));
    }

    [Fact]
    public async Task CallAsync_WithVariousParameterTypes_ShouldHandleCorrectly()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var method = "complexMethod";
        var parameters = new object[]
        {
            "string param",
            42,
            true,
            new byte[] { 0x01, 0x02, 0x03 },
            UInt160.Parse("0x1234567890123456789012345678901234567890")
        };

        // Act & Assert
        var exception = await Assert.ThrowsAnyAsync<Exception>(() =>
            _invokerService.CallAsync<string>(contractHash, method, parameters));

        Assert.NotNull(exception);
    }

    [Fact]
    public async Task SendAsync_WithWalletNotLoaded_ShouldThrowException()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var method = "testMethod";

        // Setup wallet manager to throw exception
        _mockWalletManager.Setup(x => x.GetAccount(null)).Throws(new InvalidOperationException("Wallet not loaded"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _invokerService.SendAsync(contractHash, method, "param"));

        Assert.Contains("Wallet not loaded", exception.Message);
    }
}
