using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Services;

namespace Neo.SmartContract.Deploy;

/// <summary>
/// Builder class for creating and configuring NeoContractToolkit instances
/// </summary>
public class NeoContractToolkitBuilder
{
    private readonly ServiceCollection _services;
    private Action<ILoggingBuilder>? _loggingConfiguration;
    private bool _useCustomCompiler = false;
    private bool _useCustomDeployer = false;
    private bool _useCustomInvoker = false;
    private bool _useCustomWalletManager = false;

    /// <summary>
    /// Create a new toolkit builder
    /// </summary>
    public NeoContractToolkitBuilder()
    {
        _services = new ServiceCollection();
    }

    /// <summary>
    /// Configure logging for the toolkit
    /// </summary>
    /// <param name="configure">Logging configuration action</param>
    /// <returns>Builder for chaining</returns>
    public NeoContractToolkitBuilder ConfigureLogging(Action<ILoggingBuilder> configure)
    {
        _loggingConfiguration = configure;
        return this;
    }

    /// <summary>
    /// Use a custom contract compiler implementation
    /// </summary>
    /// <typeparam name="TCompiler">Custom compiler type</typeparam>
    /// <returns>Builder for chaining</returns>
    public NeoContractToolkitBuilder UseCompiler<TCompiler>() where TCompiler : class, IContractCompiler
    {
        _services.AddTransient<IContractCompiler, TCompiler>();
        _useCustomCompiler = true;
        return this;
    }

    /// <summary>
    /// Use a custom contract deployer implementation
    /// </summary>
    /// <typeparam name="TDeployer">Custom deployer type</typeparam>
    /// <returns>Builder for chaining</returns>
    public NeoContractToolkitBuilder UseDeployer<TDeployer>() where TDeployer : class, IContractDeployer
    {
        _services.AddTransient<IContractDeployer, TDeployer>();
        _useCustomDeployer = true;
        return this;
    }

    /// <summary>
    /// Use a custom contract invoker implementation
    /// </summary>
    /// <typeparam name="TInvoker">Custom invoker type</typeparam>
    /// <returns>Builder for chaining</returns>
    public NeoContractToolkitBuilder UseInvoker<TInvoker>() where TInvoker : class, IContractInvoker
    {
        _services.AddTransient<IContractInvoker, TInvoker>();
        _useCustomInvoker = true;
        return this;
    }

    /// <summary>
    /// Use a custom wallet manager implementation
    /// </summary>
    /// <typeparam name="TWalletManager">Custom wallet manager type</typeparam>
    /// <returns>Builder for chaining</returns>
    public NeoContractToolkitBuilder UseWalletManager<TWalletManager>() where TWalletManager : class, IWalletManager
    {
        _services.AddTransient<IWalletManager, TWalletManager>();
        _useCustomWalletManager = true;
        return this;
    }

    /// <summary>
    /// Add custom services to the container
    /// </summary>
    /// <param name="configure">Service configuration action</param>
    /// <returns>Builder for chaining</returns>
    public NeoContractToolkitBuilder ConfigureServices(Action<IServiceCollection> configure)
    {
        configure(_services);
        return this;
    }

    /// <summary>
    /// Build the configured toolkit instance
    /// </summary>
    /// <returns>Configured NeoContractToolkit instance</returns>
    public NeoContractToolkit Build()
    {
        // Add configuration services
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        _services.AddSingleton<IConfiguration>(configuration);

        // Add default implementations if not customized
        if (!_useCustomCompiler)
        {
            _services.AddTransient<IContractCompiler, ContractCompilerService>();
        }

        if (!_useCustomDeployer)
        {
            _services.AddTransient<IContractDeployer, ContractDeployerService>();
        }

        if (!_useCustomInvoker)
        {
            _services.AddTransient<IContractInvoker, ContractInvokerService>();
        }

        if (!_useCustomWalletManager)
        {
            _services.AddTransient<IWalletManager, WalletManagerService>();
        }

        // Add logging
        _services.AddLogging(builder =>
        {
            if (_loggingConfiguration != null)
            {
                _loggingConfiguration(builder);
            }
            else
            {
                // Default to console logging
                builder.AddConsole();
                builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
            }
        });

        // Add the main toolkit
        _services.AddTransient<NeoContractToolkit>();
        
        // Add multi-contract deployment service
        _services.AddTransient<MultiContractDeploymentService>();

        // Build service provider and return toolkit
        var serviceProvider = _services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<NeoContractToolkit>();
    }

    /// <summary>
    /// Create a new builder instance
    /// </summary>
    /// <returns>New toolkit builder</returns>
    public static NeoContractToolkitBuilder Create()
    {
        return new NeoContractToolkitBuilder();
    }
}
