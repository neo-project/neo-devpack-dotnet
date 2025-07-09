using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Neo;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo.Wallets;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests;

public class NeoContractToolkitTests : TestBase, IAsyncLifetime
{
    private ServiceProvider _serviceProvider;
    private NeoContractToolkit _toolkit;
    private Mock<IContractCompiler> _mockCompiler;
    private Mock<IContractDeployer> _mockDeployer;
    private Mock<IContractInvoker> _mockInvoker;
    private Mock<IWalletManager> _mockWalletManager;

    public async Task InitializeAsync()
    {
        var services = new ServiceCollection();
        
        // Create mocks
        _mockCompiler = new Mock<IContractCompiler>();
        _mockDeployer = new Mock<IContractDeployer>();
        _mockInvoker = new Mock<IContractInvoker>();
        _mockWalletManager = new Mock<IWalletManager>();

        // Register mocks
        services.AddSingleton(_mockCompiler.Object);
        services.AddSingleton(_mockDeployer.Object);
        services.AddSingleton(_mockInvoker.Object);
        services.AddSingleton(_mockWalletManager.Object);

        _serviceProvider = services.BuildServiceProvider();
        _toolkit = new NeoContractToolkit(_serviceProvider);

        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _toolkit.DisposeAsync();
        _serviceProvider.Dispose();
    }

    [Fact]
    public void Constructor_WithNullServiceProvider_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new NeoContractToolkit(null!));
    }

    [Fact]
    public async Task CompileAsync_ShouldCallCompilerService()
    {
        // Arrange
        var projectPath = "test.csproj";
        var expectedContract = new CompiledContract { Name = "TestContract" };
        _mockCompiler.Setup(x => x.CompileAsync(projectPath))
            .ReturnsAsync(expectedContract);

        // Act
        var result = await _toolkit.CompileAsync(projectPath);

        // Assert
        Assert.Same(expectedContract, result);
        _mockCompiler.Verify(x => x.CompileAsync(projectPath), Times.Once);
    }

    [Fact]
    public async Task LoadContractAsync_ShouldCallCompilerService()
    {
        // Arrange
        var nefPath = "test.nef";
        var manifestPath = "test.manifest.json";
        var expectedContract = new CompiledContract { Name = "TestContract" };
        _mockCompiler.Setup(x => x.LoadContractAsync(nefPath, manifestPath))
            .ReturnsAsync(expectedContract);

        // Act
        var result = await _toolkit.LoadContractAsync(nefPath, manifestPath);

        // Assert
        Assert.Same(expectedContract, result);
        _mockCompiler.Verify(x => x.LoadContractAsync(nefPath, manifestPath), Times.Once);
    }

    [Fact]
    public async Task DeployAsync_ShouldCallDeployerService()
    {
        // Arrange
        var contract = new CompiledContract { Name = "TestContract" };
        var options = new DeploymentOptions();
        var initParams = new object[] { "param1" };
        var expectedInfo = new ContractDeploymentInfo();
        
        _mockDeployer.Setup(x => x.DeployAsync(contract, options, initParams))
            .ReturnsAsync(expectedInfo);

        // Act
        var result = await _toolkit.DeployAsync(contract, options, initParams);

        // Assert
        Assert.Same(expectedInfo, result);
        _mockDeployer.Verify(x => x.DeployAsync(contract, options, initParams), Times.Once);
    }

    [Fact]
    public async Task DeployFromSourceAsync_ShouldCompileAndDeploy()
    {
        // Arrange
        var projectPath = "test.csproj";
        var options = new DeploymentOptions();
        var initParams = new object[] { "param1" };
        var compiledContract = new CompiledContract { Name = "TestContract" };
        var expectedInfo = new ContractDeploymentInfo();

        _mockCompiler.Setup(x => x.CompileAsync(projectPath))
            .ReturnsAsync(compiledContract);
        _mockDeployer.Setup(x => x.DeployAsync(compiledContract, options, initParams))
            .ReturnsAsync(expectedInfo);

        // Act
        var result = await _toolkit.DeployFromSourceAsync(projectPath, options, initParams);

        // Assert
        Assert.Same(expectedInfo, result);
        _mockCompiler.Verify(x => x.CompileAsync(projectPath), Times.Once);
        _mockDeployer.Verify(x => x.DeployAsync(compiledContract, options, initParams), Times.Once);
    }

    [Fact]
    public async Task DeployFromFilesAsync_ShouldLoadAndDeploy()
    {
        // Arrange
        var nefPath = "test.nef";
        var manifestPath = "test.manifest.json";
        var options = new DeploymentOptions();
        var initParams = new object[] { "param1" };
        var compiledContract = new CompiledContract { Name = "TestContract" };
        var expectedInfo = new ContractDeploymentInfo();

        _mockCompiler.Setup(x => x.LoadContractAsync(nefPath, manifestPath))
            .ReturnsAsync(compiledContract);
        _mockDeployer.Setup(x => x.DeployAsync(compiledContract, options, initParams))
            .ReturnsAsync(expectedInfo);

        // Act
        var result = await _toolkit.DeployFromFilesAsync(nefPath, manifestPath, options, initParams);

        // Assert
        Assert.Same(expectedInfo, result);
        _mockCompiler.Verify(x => x.LoadContractAsync(nefPath, manifestPath), Times.Once);
        _mockDeployer.Verify(x => x.DeployAsync(compiledContract, options, initParams), Times.Once);
    }

    [Fact]
    public async Task ContractExistsAsync_ShouldCallDeployerService()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var rpcUrl = "http://localhost:50012";
        
        _mockDeployer.Setup(x => x.ContractExistsAsync(contractHash, rpcUrl))
            .ReturnsAsync(true);

        // Act
        var result = await _toolkit.ContractExistsAsync(contractHash, rpcUrl);

        // Assert
        Assert.True(result);
        _mockDeployer.Verify(x => x.ContractExistsAsync(contractHash, rpcUrl), Times.Once);
    }

    [Fact]
    public async Task CallAsync_ShouldCallInvokerService()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var method = "testMethod";
        var rpcUrl = "http://localhost:50012";
        var args = new object[] { "arg1", 42 };
        var expectedResult = "test result";

        _mockInvoker.Setup(x => x.CallAsync<string>(contractHash, method, args, rpcUrl))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _toolkit.CallAsync<string>(contractHash, method, rpcUrl, args);

        // Assert
        Assert.Equal(expectedResult, result);
        _mockInvoker.Verify(x => x.CallAsync<string>(contractHash, method, args, rpcUrl), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_ShouldCallInvokerService()
    {
        // Arrange
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var method = "testMethod";
        var options = new InvocationOptions();
        var args = new object[] { "arg1", 42 };
        var expectedTxHash = UInt256.Parse("0xabcdef1234567890123456789012345678901234567890123456789012345678");

        _mockInvoker.Setup(x => x.InvokeAsync(contractHash, method, args, options))
            .ReturnsAsync(expectedTxHash);

        // Act
        var result = await _toolkit.InvokeAsync(contractHash, method, options, args);

        // Assert
        Assert.Equal(expectedTxHash, result);
        _mockInvoker.Verify(x => x.InvokeAsync(contractHash, method, args, options), Times.Once);
    }

    [Fact]
    public void GetAccountFromWif_ShouldCallWalletManager()
    {
        // Arrange
        var wifKey = "L1QqQJnpBwbsPGAuutuzPTac8piqvbR1HRjrY5qHup48TBCBFe4g";
        var expectedAccount = new Account();

        _mockWalletManager.Setup(x => x.GetAccountFromWif(wifKey))
            .Returns(expectedAccount);

        // Act
        var result = _toolkit.GetAccountFromWif(wifKey);

        // Assert
        Assert.Same(expectedAccount, result);
        _mockWalletManager.Verify(x => x.GetAccountFromWif(wifKey), Times.Once);
    }

    [Fact]
    public void GetAccountAddress_ShouldCallWalletManager()
    {
        // Arrange
        var account = new Account();
        var expectedAddress = "NXXxXXxXXxXXxXXxXXxXXxXXxXXxXXxXXxX";

        _mockWalletManager.Setup(x => x.GetAccountAddress(account))
            .Returns(expectedAddress);

        // Act
        var result = _toolkit.GetAccountAddress(account);

        // Assert
        Assert.Equal(expectedAddress, result);
        _mockWalletManager.Verify(x => x.GetAccountAddress(account), Times.Once);
    }

    [Fact]
    public async Task GetGasBalanceAsync_ShouldCallWalletManager()
    {
        // Arrange
        var accountHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var rpcUrl = "http://localhost:50012";
        var expectedBalance = 100.5m;

        _mockWalletManager.Setup(x => x.GetGasBalanceAsync(accountHash, rpcUrl))
            .ReturnsAsync(expectedBalance);

        // Act
        var result = await _toolkit.GetGasBalanceAsync(accountHash, rpcUrl);

        // Assert
        Assert.Equal(expectedBalance, result);
        _mockWalletManager.Verify(x => x.GetGasBalanceAsync(accountHash, rpcUrl), Times.Once);
    }

    [Fact]
    public async Task DeployFromManifestAsync_WithEmptyManifest_ShouldThrowException()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var manifestPath = Path.Combine(tempDir, "manifest.json");
        
        try
        {
            await File.WriteAllTextAsync(manifestPath, "{}");
            var options = new DeploymentOptions();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _toolkit.DeployFromManifestAsync(manifestPath, options));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }
}