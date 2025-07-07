using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Security;
using Neo.SmartContract.Deploy.Services;
using Neo.SmartContract.Deploy.Shared;
using Neo.SmartContract.Deploy.UnitTests.Mocks;
using Neo.Network.RPC.Models;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests;

public class DeploymentToolkitAdvancedTests : IDisposable
{
    private readonly string _testWalletPath;
    private readonly string _testConfigPath;
    
    public DeploymentToolkitAdvancedTests()
    {
        _testWalletPath = Path.Combine(Path.GetTempPath(), $"test-wallet-{Guid.NewGuid()}.json");
        _testConfigPath = Path.Combine(Path.GetTempPath(), $"test-config-{Guid.NewGuid()}.json");
        
        // Create test configuration file
        var config = @"{
            ""Network"": {
                ""RpcUrl"": ""http://localhost:50012"",
                ""Network"": ""private""
            },
            ""Wallet"": {
                ""Path"": """ + _testWalletPath.Replace("\\", "\\\\") + @"""
            },
            ""Deployment"": {
                ""GasLimit"": 100000000,
                ""WaitForConfirmation"": true
            }
        }";
        
        File.WriteAllText(_testConfigPath, config);
    }
    
    public void Dispose()
    {
        if (File.Exists(_testWalletPath)) File.Delete(_testWalletPath);
        if (File.Exists(_testConfigPath)) File.Delete(_testConfigPath);
    }
    
    [Fact]
    public async Task DeployAsync_WithDryRun_ShouldNotSendTransaction()
    {
        // Arrange
        var mockInvokeResult = new RpcInvokeResult
        {
            State = Neo.VM.VMState.HALT,
            GasConsumed = 5000000,
            Stack = new Neo.VM.Types.StackItem[] { Neo.VM.Types.StackItem.Null }
        };
        
        var mockRpcClient = new MockRpcClient(
            invokeResult: mockInvokeResult,
            blockCount: 1000,
            contractExists: false,
            sentTxHash: UInt256.Parse("0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef")
        );
        
        var services = new ServiceCollection();
        ConfigureTestServices(services, mockRpcClient);
        
        var serviceProvider = services.BuildServiceProvider();
        var toolkit = CreateToolkitWithMocks(serviceProvider);
        
        // Create test contract file
        var testContractPath = Path.Combine(Path.GetTempPath(), "TestContract.cs");
        File.WriteAllText(testContractPath, "// Test contract");
        
        try
        {
            // Act
            var result = await toolkit.DeployAsync(testContractPath, new object[] { "test" });
            
            // Assert
            Assert.True(result.IsDryRun);
            Assert.Equal(UInt256.Zero, result.TransactionHash);
            Assert.Equal(5000000, result.GasConsumed);
            Assert.True(result.Success);
        }
        finally
        {
            if (File.Exists(testContractPath)) File.Delete(testContractPath);
        }
    }
    
    [Fact]
    public async Task DeployAsync_WithVerification_ShouldCheckContractExists()
    {
        // Arrange
        var mockInvokeResult = new RpcInvokeResult
        {
            State = Neo.VM.VMState.HALT,
            GasConsumed = 5000000,
            Stack = new Neo.VM.Types.StackItem[] { Neo.VM.Types.StackItem.Null }
        };
        
        var mockRpcClient = new MockRpcClient(
            invokeResult: mockInvokeResult,
            blockCount: 1000,
            contractExists: true,
            sentTxHash: UInt256.Parse("0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef")
        );
        
        var services = new ServiceCollection();
        ConfigureTestServices(services, mockRpcClient);
        
        var serviceProvider = services.BuildServiceProvider();
        var toolkit = CreateToolkitWithMocks(serviceProvider);
        
        var testContractPath = Path.Combine(Path.GetTempPath(), "TestContract.cs");
        File.WriteAllText(testContractPath, "// Test contract");
        
        try
        {
            // Configure deployment options with verification
            var deploymentOptions = new DeploymentOptions
            {
                VerifyAfterDeploy = true,
                VerificationDelayMs = 100, // Short delay for tests
                DryRun = false
            };
            
            // Act
            var result = await toolkit.DeployAsync(testContractPath);
            
            // Assert
            Assert.False(result.IsDryRun);
            Assert.NotEqual(UInt256.Zero, result.TransactionHash);
            Assert.False(result.VerificationFailed);
            Assert.True(result.Success);
        }
        finally
        {
            if (File.Exists(testContractPath)) File.Delete(testContractPath);
        }
    }
    
    [Fact]
    public async Task GetGasBalanceAsync_ShouldCallCorrectContract()
    {
        // Arrange
        var gasBalance = new System.Numerics.BigInteger(500_00000000); // 500 GAS
        var mockInvokeResult = new RpcInvokeResult
        {
            State = Neo.VM.VMState.HALT,
            GasConsumed = 100000,
            Stack = new Neo.VM.Types.StackItem[] { Neo.VM.Types.Integer(gasBalance) }
        };
        
        var mockRpcClient = new MockRpcClient(invokeResult: mockInvokeResult);
        
        var services = new ServiceCollection();
        ConfigureTestServices(services, mockRpcClient);
        
        var serviceProvider = services.BuildServiceProvider();
        var toolkit = CreateToolkitWithMocks(serviceProvider);
        
        // Act
        var balance = await toolkit.GetGasBalanceAsync();
        
        // Assert
        Assert.Equal(500m, balance);
    }
    
    [Fact]
    public async Task SecureCredentialProvider_ShouldLoadFromEnvironment()
    {
        // Arrange
        var testPassword = "test-password-12345";
        var walletPath = "test-wallet.json";
        Environment.SetEnvironmentVariable("NEO_WALLET_PASSWORD", testPassword);
        
        try
        {
            var logger = new Mock<ILogger<SecureCredentialProvider>>();
            var config = new ConfigurationBuilder().Build();
            var provider = new SecureCredentialProvider(logger.Object, config);
            
            // Act
            var password = await provider.GetWalletPasswordAsync(walletPath);
            
            // Assert
            Assert.Equal(testPassword, password);
        }
        finally
        {
            Environment.SetEnvironmentVariable("NEO_WALLET_PASSWORD", null);
        }
    }
    
    [Fact]
    public async Task WalletManager_WithCredentialProvider_ShouldLoadSecurely()
    {
        // Arrange
        var mockCredentialProvider = new Mock<ICredentialProvider>();
        mockCredentialProvider.Setup(x => x.GetWalletPasswordAsync(It.IsAny<string>()))
            .ReturnsAsync("secure-password");
        
        var logger = new Mock<ILogger<WalletManagerService>>();
        var walletManager = new WalletManagerService(logger.Object, mockCredentialProvider.Object);
        
        // Create a test wallet file
        var walletJson = @"{
            ""name"": ""test"",
            ""version"": ""1.0"",
            ""scrypt"": {
                ""n"": 16384,
                ""r"": 8,
                ""p"": 8
            },
            ""accounts"": []
        }";
        File.WriteAllText(_testWalletPath, walletJson);
        
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => 
            await walletManager.LoadWalletSecurelyAsync(_testWalletPath));
        
        // Verify credential provider was called
        mockCredentialProvider.Verify(x => x.GetWalletPasswordAsync(_testWalletPath), Times.Once);
    }
    
    private void ConfigureTestServices(IServiceCollection services, RpcClient mockRpcClient)
    {
        // Configure logging
        services.AddLogging(builder => builder.AddConsole());
        
        // Add configuration
        var config = new ConfigurationBuilder()
            .AddJsonFile(_testConfigPath)
            .Build();
        services.AddSingleton<IConfiguration>(config);
        
        // Add mocked services
        services.AddSingleton<IRpcClientFactory>(new MockRpcClientFactory(() => mockRpcClient));
        services.AddSingleton<ICredentialProvider, EnvironmentCredentialProvider>();
        
        // Add real services that work with mocks
        services.AddTransient<TransactionBuilder>();
        services.AddTransient<TransactionConfirmationService>();
        services.AddTransient<IContractCompiler, MockContractCompiler>();
        services.AddTransient<IContractDeployer, ContractDeployerService>();
        services.AddTransient<IContractInvoker, ContractInvokerService>();
        services.AddTransient<IWalletManager, MockWalletManager>();
        services.AddTransient<IDeploymentRecordService, DeploymentRecordService>();
        services.AddTransient<IContractUpdateService, ContractUpdateService>();
        services.AddTransient<MultiContractDeploymentService>();
        services.AddTransient<NeoContractToolkit>();
    }
    
    private DeploymentToolkit CreateToolkitWithMocks(IServiceProvider serviceProvider)
    {
        // Use reflection to inject the service provider
        var toolkit = new DeploymentToolkit(_testConfigPath);
        var toolkitType = typeof(DeploymentToolkit);
        var serviceProviderField = toolkitType.GetField("_serviceProvider", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        serviceProviderField?.SetValue(toolkit, serviceProvider);
        
        return toolkit;
    }
}

// Mock implementations for testing
public class MockContractCompiler : IContractCompiler
{
    public Task<CompiledContract> CompileAsync(CompilationOptions options)
    {
        var manifest = new ContractManifest
        {
            Name = options.ContractName ?? "TestContract",
            Groups = Array.Empty<ContractGroup>(),
            Features = ContractFeatures.HasStorage,
            SupportedStandards = Array.Empty<string>(),
            Abi = new ContractAbi
            {
                Methods = Array.Empty<ContractMethodDescriptor>(),
                Events = Array.Empty<ContractEventDescriptor>()
            },
            Permissions = new[] { ContractPermission.DefaultPermission },
            Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
            Extra = null
        };
        
        return Task.FromResult(new CompiledContract
        {
            Name = options.ContractName ?? "TestContract",
            NefBytes = new byte[] { 0x01, 0x02, 0x03 },
            Manifest = manifest,
            DebugInfo = null
        });
    }
    
    public Task<CompiledContract> LoadAsync(string nefPath, string manifestPath)
    {
        return CompileAsync(new CompilationOptions { ContractName = "LoadedContract" });
    }
}

public class MockWalletManager : IWalletManager
{
    private readonly UInt160 _mockAccount = UInt160.Parse("0xb1983fa2021e0c36e5e37c2771b8bb7b5c525688");
    
    public bool IsWalletLoaded => true;
    
    public Task LoadWalletAsync(string walletPath, string password)
    {
        return Task.CompletedTask;
    }
    
    public Task LoadWalletSecurelyAsync(string walletPath)
    {
        return Task.CompletedTask;
    }
    
    public UInt160 GetAccount(string? accountAddress = null)
    {
        return _mockAccount;
    }
    
    public Task SignTransactionAsync(Neo.Network.P2P.Payloads.Transaction transaction, UInt160? account = null)
    {
        // Mock signing
        return Task.CompletedTask;
    }
    
    public IEnumerable<UInt160> GetAccounts()
    {
        return new[] { _mockAccount };
    }
}