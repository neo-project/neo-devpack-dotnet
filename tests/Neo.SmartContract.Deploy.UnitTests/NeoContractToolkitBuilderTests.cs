using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.Cryptography.ECC;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Services;
using Neo.Wallets;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests;

public class NeoContractToolkitBuilderTests
{
    [Fact]
    public async Task Build_WithDefaults_ShouldCreateToolkit()
    {
        // Arrange
        var builder = new NeoContractToolkitBuilder();

        // Act
        await using var toolkit = builder.Build();

        // Assert
        Assert.NotNull(toolkit);
    }

    [Fact]
    public async Task WithConfiguration_WithNullConfiguration_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new NeoContractToolkitBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            builder.WithConfiguration((IConfiguration)null!));
    }

    [Fact]
    public async Task WithConfiguration_WithValidConfiguration_ShouldSetConfiguration()
    {
        // Arrange
        var builder = new NeoContractToolkitBuilder();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

        // Act
        var result = builder.WithConfiguration(configuration);
        await using var toolkit = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.NotNull(toolkit);
    }

    [Fact]
    public async Task WithConfiguration_WithConfigAction_ShouldConfigureCorrectly()
    {
        // Arrange
        var builder = new NeoContractToolkitBuilder();

        // Act
        var result = builder.WithConfiguration(config =>
        {
            config.AddInMemoryCollection();
        });
        await using var toolkit = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.NotNull(toolkit);
    }

    [Fact]
    public async Task WithConfiguration_WithNullConfigAction_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new NeoContractToolkitBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            builder.WithConfiguration((Action<IConfigurationBuilder>)null!));
    }

    [Fact]
    public async Task WithLogging_WithNullAction_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new NeoContractToolkitBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            builder.WithLogging(null!));
    }

    [Fact]
    public async Task WithLogging_WithValidAction_ShouldConfigureLogging()
    {
        // Arrange
        var builder = new NeoContractToolkitBuilder();

        // Act
        var result = builder.WithLogging(logging =>
        {
            logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
        });
        await using var toolkit = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.NotNull(toolkit);
    }

    [Fact]
    public async Task ConfigureServices_WithNullAction_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new NeoContractToolkitBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            builder.ConfigureServices(null!));
    }

    [Fact]
    public async Task ConfigureServices_WithValidAction_ShouldConfigureServices()
    {
        // Arrange
        var builder = new NeoContractToolkitBuilder();
        var customServiceAdded = false;

        // Act
        var result = builder.ConfigureServices(services =>
        {
            services.AddSingleton<object>(new object());
            customServiceAdded = true;
        });
        await using var toolkit = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.True(customServiceAdded);
        Assert.NotNull(toolkit);
    }

    [Fact]
    public async Task UseCompiler_ShouldReplaceDefaultCompiler()
    {
        // Arrange
        var builder = new NeoContractToolkitBuilder();

        // Act
        var result = builder.UseCompiler<MockCompiler>();
        await using var toolkit = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.NotNull(toolkit);
    }

    [Fact]
    public async Task UseDeployer_ShouldReplaceDefaultDeployer()
    {
        // Arrange
        var builder = new NeoContractToolkitBuilder();

        // Act
        var result = builder.UseDeployer<MockDeployer>();
        await using var toolkit = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.NotNull(toolkit);
    }

    [Fact]
    public async Task UseInvoker_ShouldReplaceDefaultInvoker()
    {
        // Arrange
        var builder = new NeoContractToolkitBuilder();

        // Act
        var result = builder.UseInvoker<MockInvoker>();
        await using var toolkit = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.NotNull(toolkit);
    }

    [Fact]
    public async Task UseWalletManager_ShouldReplaceDefaultWalletManager()
    {
        // Arrange
        var builder = new NeoContractToolkitBuilder();

        // Act
        var result = builder.UseWalletManager<MockWalletManager>();
        await using var toolkit = result.Build();

        // Assert
        Assert.Same(builder, result);
        Assert.NotNull(toolkit);
    }

    [Fact]
    public async Task Build_WithFullConfiguration_ShouldCreateCompleteToolkit()
    {
        // Arrange
        var builder = new NeoContractToolkitBuilder()
            .WithConfiguration(config =>
            {
                config.AddInMemoryCollection();
            })
            .WithLogging(logging =>
            {
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
            })
            .ConfigureServices(services =>
            {
                services.AddSingleton<object>(new object());
            })
            .UseCompiler<MockCompiler>()
            .UseDeployer<MockDeployer>()
            .UseInvoker<MockInvoker>()
            .UseWalletManager<MockWalletManager>();

        // Act
        await using var toolkit = builder.Build();

        // Assert
        Assert.NotNull(toolkit);
    }

    // Mock implementations for testing
    private class MockCompiler : IContractCompiler
    {
        public Task<CompiledContract> CompileAsync(string projectPath) => Task.FromResult(new CompiledContract());
        public Task<CompiledContract> LoadContractAsync(string nefPath, string manifestPath) => Task.FromResult(new CompiledContract());
    }

    private class MockDeployer : IContractDeployer
    {
        public Task<ContractDeploymentInfo> DeployAsync(CompiledContract contract, DeploymentOptions options, object[]? initParams = null) 
            => Task.FromResult(new ContractDeploymentInfo());
        public Task<bool> ContractExistsAsync(UInt160 contractHash) => Task.FromResult(false);
        public Task<bool> ContractExistsAsync(UInt160 contractHash, string rpcUrl) => Task.FromResult(false);
    }

    private class MockInvoker : IContractInvoker
    {
        public Task<T> CallAsync<T>(UInt160 contractHash, string method, object[] args, string rpcUrl) 
            => Task.FromResult(default(T)!);
        public Task<UInt256> InvokeAsync(UInt160 contractHash, string method, object[] args, InvocationOptions options) 
            => Task.FromResult(UInt256.Zero);
    }

    private class MockWalletManager : IWalletManager
    {
        public Account GetAccountFromWif(string wifKey) => new Account();
        public string GetAccountAddress(Account account) => "NXXxXXxXXxXXxXXxXXxXXxXXxXXxXXxXXxX";
        public UInt160 GetAccountScriptHash(Account account) => UInt160.Zero;
        public Task<decimal> GetGasBalanceAsync(UInt160 accountHash, string rpcUrl) => Task.FromResult(0m);
        public UInt160 CreateSignatureContract(ECPoint publicKey) => UInt160.Zero;
    }
}