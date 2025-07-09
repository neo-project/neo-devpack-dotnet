using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Neo;
using Neo.SmartContract.Deploy.Exceptions;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Services;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests.Services;

public class ContractInvokerServiceTests : TestBase
{
    private readonly ContractInvokerService _invokerService;
    private readonly Mock<IWalletManager> _mockWalletManager;
    private readonly Mock<ILogger<ContractInvokerService>> _mockLogger;

    public ContractInvokerServiceTests()
    {
        _mockWalletManager = new Mock<IWalletManager>();
        _mockLogger = new Mock<ILogger<ContractInvokerService>>();
        _invokerService = new ContractInvokerService(_mockWalletManager.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullWalletManager_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new ContractInvokerService(null!));
    }

    [Fact]
    public async Task CallAsync_WithNullContractHash_ShouldThrowArgumentNullException()
    {
        // Arrange
        var method = "testMethod";
        var args = new object[] { "arg1" };
        var rpcUrl = "http://localhost:50012";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _invokerService.CallAsync<string>(null!, method, args, rpcUrl));
    }

    [Fact]
    public async Task CallAsync_WithNullMethod_ShouldThrowArgumentException()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var args = new object[] { "arg1" };
        var rpcUrl = "http://localhost:50012";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _invokerService.CallAsync<string>(contractHash, null!, args, rpcUrl));
    }

    [Fact]
    public async Task CallAsync_WithEmptyMethod_ShouldThrowArgumentException()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var args = new object[] { "arg1" };
        var rpcUrl = "http://localhost:50012";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _invokerService.CallAsync<string>(contractHash, "", args, rpcUrl));
    }

    [Fact]
    public async Task CallAsync_WithNullRpcUrl_ShouldThrowArgumentException()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var method = "testMethod";
        var args = new object[] { "arg1" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _invokerService.CallAsync<string>(contractHash, method, args, null!));
    }

    [Fact]
    public async Task CallAsync_WithEmptyRpcUrl_ShouldThrowArgumentException()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var method = "testMethod";
        var args = new object[] { "arg1" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _invokerService.CallAsync<string>(contractHash, method, args, ""));
    }

    [Fact]
    public async Task InvokeAsync_WithNullContractHash_ShouldThrowArgumentNullException()
    {
        // Arrange
        var method = "testMethod";
        var args = new object[] { "arg1" };
        var options = new InvocationOptions { RpcUrl = "http://localhost:50012", WifKey = "test" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _invokerService.InvokeAsync(null!, method, args, options));
    }

    [Fact]
    public async Task InvokeAsync_WithNullMethod_ShouldThrowArgumentException()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var args = new object[] { "arg1" };
        var options = new InvocationOptions { RpcUrl = "http://localhost:50012", WifKey = "test" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _invokerService.InvokeAsync(contractHash, null!, args, options));
    }

    [Fact]
    public async Task InvokeAsync_WithEmptyMethod_ShouldThrowArgumentException()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var args = new object[] { "arg1" };
        var options = new InvocationOptions { RpcUrl = "http://localhost:50012", WifKey = "test" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _invokerService.InvokeAsync(contractHash, "", args, options));
    }

    [Fact]
    public async Task InvokeAsync_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var method = "testMethod";
        var args = new object[] { "arg1" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _invokerService.InvokeAsync(contractHash, method, args, null!));
    }

    [Fact]
    public async Task InvokeAsync_WithoutRpcUrl_ShouldThrowArgumentException()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var method = "testMethod";
        var args = new object[] { "arg1" };
        var options = new InvocationOptions { WifKey = "test" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _invokerService.InvokeAsync(contractHash, method, args, options));
    }

    [Fact]
    public async Task InvokeAsync_WithoutWifKeyOrAccount_ShouldThrowArgumentException()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var method = "testMethod";
        var args = new object[] { "arg1" };
        var options = new InvocationOptions { RpcUrl = "http://localhost:50012" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _invokerService.InvokeAsync(contractHash, method, args, options));
    }

    [Fact]
    public async Task InvokeAsync_WithInvokerAccount_ShouldThrowContractDeploymentException()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var method = "testMethod";
        var args = new object[] { "arg1" };
        var options = new InvocationOptions 
        { 
            RpcUrl = "http://localhost:50012",
            InvokerAccount = UInt160.Parse("0xabcdef1234567890123456789012345678901234")
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ContractDeploymentException>(() =>
            _invokerService.InvokeAsync(contractHash, method, args, options));
        
        Assert.Contains("Only WIF key signing is currently supported", ex.Message);
        Assert.IsType<NotSupportedException>(ex.InnerException);
    }
}