using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Services;

namespace Neo.SmartContract.Deploy;

/// <summary>
/// Builder for configuring NeoContractToolkit
/// </summary>
public class NeoContractToolkitBuilder
{
    private readonly IServiceCollection _services;
    private IConfiguration? _configuration;
    private Action<ILoggingBuilder>? _loggingBuilder;

    /// <summary>
    /// Initialize a new instance of NeoContractToolkitBuilder
    /// </summary>
    public NeoContractToolkitBuilder()
    {
        _services = new ServiceCollection();
        ConfigureDefaultServices();
    }

    /// <summary>
    /// Configure the toolkit with a configuration
    /// </summary>
    /// <param name="configuration">Configuration</param>
    /// <returns>This builder</returns>
    public NeoContractToolkitBuilder WithConfiguration(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _services.AddSingleton(_configuration);
        return this;
    }

    /// <summary>
    /// Configure the toolkit with a configuration action
    /// </summary>
    /// <param name="configureConfig">Configuration action</param>
    /// <returns>This builder</returns>
    public NeoContractToolkitBuilder WithConfiguration(Action<IConfigurationBuilder> configureConfig)
    {
        if (configureConfig == null)
            throw new ArgumentNullException(nameof(configureConfig));

        var builder = new ConfigurationBuilder();
        configureConfig(builder);
        _configuration = builder.Build();
        _services.AddSingleton(_configuration);
        return this;
    }

    /// <summary>
    /// Configure logging
    /// </summary>
    /// <param name="configureLogging">Logging configuration action</param>
    /// <returns>This builder</returns>
    public NeoContractToolkitBuilder WithLogging(Action<ILoggingBuilder> configureLogging)
    {
        _loggingBuilder = configureLogging ?? throw new ArgumentNullException(nameof(configureLogging));
        return this;
    }

    /// <summary>
    /// Configure services
    /// </summary>
    /// <param name="configureServices">Service configuration action</param>
    /// <returns>This builder</returns>
    public NeoContractToolkitBuilder ConfigureServices(Action<IServiceCollection> configureServices)
    {
        if (configureServices == null)
            throw new ArgumentNullException(nameof(configureServices));

        configureServices(_services);
        return this;
    }

    /// <summary>
    /// Use a custom contract compiler
    /// </summary>
    /// <typeparam name="TCompiler">Compiler type</typeparam>
    /// <returns>This builder</returns>
    public NeoContractToolkitBuilder UseCompiler<TCompiler>() where TCompiler : class, IContractCompiler
    {
        _services.AddSingleton<IContractCompiler, TCompiler>();
        return this;
    }

    /// <summary>
    /// Use a custom contract deployer
    /// </summary>
    /// <typeparam name="TDeployer">Deployer type</typeparam>
    /// <returns>This builder</returns>
    public NeoContractToolkitBuilder UseDeployer<TDeployer>() where TDeployer : class, IContractDeployer
    {
        _services.AddSingleton<IContractDeployer, TDeployer>();
        return this;
    }

    /// <summary>
    /// Use a custom contract invoker
    /// </summary>
    /// <typeparam name="TInvoker">Invoker type</typeparam>
    /// <returns>This builder</returns>
    public NeoContractToolkitBuilder UseInvoker<TInvoker>() where TInvoker : class, IContractInvoker
    {
        _services.AddSingleton<IContractInvoker, TInvoker>();
        return this;
    }

    /// <summary>
    /// Use a custom wallet manager
    /// </summary>
    /// <typeparam name="TWalletManager">Wallet manager type</typeparam>
    /// <returns>This builder</returns>
    public NeoContractToolkitBuilder UseWalletManager<TWalletManager>() where TWalletManager : class, IWalletManager
    {
        _services.AddSingleton<IWalletManager, TWalletManager>();
        return this;
    }

    /// <summary>
    /// Build the NeoContractToolkit
    /// </summary>
    /// <returns>Configured NeoContractToolkit</returns>
    public NeoContractToolkit Build()
    {
        // Add logging if configured
        if (_loggingBuilder != null)
        {
            _services.AddLogging(_loggingBuilder);
        }
        else
        {
            // Add default console logging
            _services.AddLogging(builder => builder.AddConsole());
        }

        // Add configuration if not already added
        if (_configuration == null)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables();
            
            _configuration = builder.Build();
            _services.AddSingleton(_configuration);
        }

        var serviceProvider = _services.BuildServiceProvider();
        return new NeoContractToolkit(serviceProvider);
    }

    private void ConfigureDefaultServices()
    {
        // Register default implementations
        _services.AddSingleton<IContractCompiler, ContractCompilerService>();
        _services.AddSingleton<IWalletManager, WalletManagerService>();
        _services.AddSingleton<IContractInvoker, ContractInvokerService>();
        _services.AddSingleton<IContractDeployer, ContractDeployerService>();
    }
}