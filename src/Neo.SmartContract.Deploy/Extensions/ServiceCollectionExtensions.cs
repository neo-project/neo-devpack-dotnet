using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Services;
using Neo.SmartContract.Deploy.Security;
using Neo.SmartContract.Deploy.Security.Interfaces;
using Neo.SmartContract.Deploy.HealthChecks;
using Neo.SmartContract.Deploy.HealthChecks.Interfaces;
using Neo.SmartContract.Deploy.Monitoring;

namespace Neo.SmartContract.Deploy.Extensions;

/// <summary>
/// Extension methods for IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add Neo contract deployment services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddNeoContractDeploy(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        // Add configuration
        services.AddSingleton(configuration);

        // Add core services
        services.AddSingleton<IContractCompiler, ContractCompilerService>();
        services.AddSingleton<IWalletManager, WalletManagerService>();
        services.AddSingleton<IContractInvoker, ContractInvokerService>();
        services.AddSingleton<IContractDeployer, ContractDeployerService>();
        services.AddSingleton<IContractUpdateService, ContractUpdateService>();

        // Add the toolkit
        services.AddSingleton<NeoContractToolkit>();

        // Add multi-contract deployment service
        services.AddSingleton<IMultiContractDeploymentService, MultiContractDeploymentService>();

        // Add security services
        services.AddSingleton<DeploymentRecordService>();
        services.AddSingleton<DeploymentMetrics>();
        services.AddSingleton<PerformanceMonitor>();
        services.AddSingleton<HealthCheckService>();

        // Add HTTP client for health checks
        services.AddHttpClient();

        return services;
    }

    /// <summary>
    /// Add Neo contract deployment services with custom configuration
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configureOptions">Configuration action</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddNeoContractDeploy(this IServiceCollection services, Action<NeoContractDeployOptions> configureOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configureOptions == null)
            throw new ArgumentNullException(nameof(configureOptions));

        var options = new NeoContractDeployOptions();
        configureOptions(options);

        // Build configuration from options
        var configBuilder = new ConfigurationBuilder();
        
        if (!string.IsNullOrEmpty(options.ConfigurationPath))
        {
            configBuilder.AddJsonFile(options.ConfigurationPath, optional: false);
        }
        else
        {
            configBuilder.AddJsonFile("appsettings.json", optional: true);
        }
        
        if (options.AddEnvironmentVariables)
        {
            configBuilder.AddEnvironmentVariables();
        }

        var configuration = configBuilder.Build();

        return services.AddNeoContractDeploy(configuration);
    }

    /// <summary>
    /// Replace the default contract compiler
    /// </summary>
    /// <typeparam name="TCompiler">Compiler implementation type</typeparam>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection UseContractCompiler<TCompiler>(this IServiceCollection services)
        where TCompiler : class, IContractCompiler
    {
        services.AddSingleton<IContractCompiler, TCompiler>();
        return services;
    }

    /// <summary>
    /// Replace the default contract deployer
    /// </summary>
    /// <typeparam name="TDeployer">Deployer implementation type</typeparam>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection UseContractDeployer<TDeployer>(this IServiceCollection services)
        where TDeployer : class, IContractDeployer
    {
        services.AddSingleton<IContractDeployer, TDeployer>();
        return services;
    }

    /// <summary>
    /// Replace the default contract invoker
    /// </summary>
    /// <typeparam name="TInvoker">Invoker implementation type</typeparam>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection UseContractInvoker<TInvoker>(this IServiceCollection services)
        where TInvoker : class, IContractInvoker
    {
        services.AddSingleton<IContractInvoker, TInvoker>();
        return services;
    }

    /// <summary>
    /// Replace the default wallet manager
    /// </summary>
    /// <typeparam name="TWalletManager">Wallet manager implementation type</typeparam>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection UseWalletManager<TWalletManager>(this IServiceCollection services)
        where TWalletManager : class, IWalletManager
    {
        services.AddSingleton<IWalletManager, TWalletManager>();
        return services;
    }

    /// <summary>
    /// Replace the default contract update service
    /// </summary>
    /// <typeparam name="TUpdateService">Update service implementation type</typeparam>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection UseContractUpdateService<TUpdateService>(this IServiceCollection services)
        where TUpdateService : class, IContractUpdateService
    {
        services.AddSingleton<IContractUpdateService, TUpdateService>();
        return services;
    }
}

/// <summary>
/// Options for configuring Neo contract deployment services
/// </summary>
public class NeoContractDeployOptions
{
    /// <summary>
    /// Path to configuration file
    /// </summary>
    public string? ConfigurationPath { get; set; }

    /// <summary>
    /// Whether to add environment variables to configuration
    /// </summary>
    public bool AddEnvironmentVariables { get; set; } = true;
}

/// <summary>
/// Extension methods for security configuration
/// </summary>
public static class SecurityExtensions
{
    /// <summary>
    /// Add secure credential provider with encryption
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="encryptionKey">32-byte encryption key for AES-256</param>
    /// <param name="persistencePath">Optional path to persist encrypted credentials</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddSecureCredentialProvider(
        this IServiceCollection services, 
        byte[] encryptionKey, 
        string? persistencePath = null)
    {
        services.AddSingleton<ICredentialProvider>(sp =>
        {
            var logger = sp.GetService<ILogger<SecureCredentialProvider>>();
            return new SecureCredentialProvider(encryptionKey, persistencePath, logger);
        });
        return services;
    }

    /// <summary>
    /// Add secure credential provider with password-derived encryption
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="password">Password for key derivation</param>
    /// <param name="salt">Salt for key derivation (minimum 16 bytes)</param>
    /// <param name="persistencePath">Optional path to persist encrypted credentials</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddSecureCredentialProviderWithPassword(
        this IServiceCollection services,
        string password,
        byte[] salt,
        string? persistencePath = null)
    {
        services.AddSingleton<ICredentialProvider>(sp =>
        {
            var logger = sp.GetService<ILogger<SecureCredentialProvider>>();
            return SecureCredentialProvider.CreateWithPassword(password, salt, persistencePath, logger);
        });
        return services;
    }

    /// <summary>
    /// Add environment-based credential provider
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="prefix">Prefix for environment variables (default: "NEO_DEPLOY_")</param>
    /// <param name="allowSet">Whether to allow setting environment variables</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddEnvironmentCredentialProvider(
        this IServiceCollection services,
        string prefix = "NEO_DEPLOY_",
        bool allowSet = false)
    {
        services.AddSingleton<ICredentialProvider>(sp =>
        {
            var logger = sp.GetService<ILogger<EnvironmentCredentialProvider>>();
            return new EnvironmentCredentialProvider(prefix, allowSet, logger);
        });
        return services;
    }

    /// <summary>
    /// Add multiple credential providers with priority
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configurePrimary">Configure primary provider</param>
    /// <param name="configureFallback">Configure fallback provider</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddCredentialProviderChain(
        this IServiceCollection services,
        Action<IServiceCollection> configurePrimary,
        Action<IServiceCollection> configureFallback)
    {
        // Create separate service collections for each provider
        var primaryServices = new ServiceCollection();
        var fallbackServices = new ServiceCollection();

        configurePrimary(primaryServices);
        configureFallback(fallbackServices);

        // Register a composite provider
        services.AddSingleton<ICredentialProvider>(sp =>
        {
            var primaryProvider = primaryServices.BuildServiceProvider().GetService<ICredentialProvider>();
            var fallbackProvider = fallbackServices.BuildServiceProvider().GetService<ICredentialProvider>();

            // Return a composite provider that tries primary first, then fallback
            return new CompositeCredentialProvider(primaryProvider!, fallbackProvider!);
        });

        return services;
    }

    /// <summary>
    /// Add health checks with default configuration
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="rpcUrl">RPC URL to monitor</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddNeoHealthChecks(
        this IServiceCollection services,
        string rpcUrl)
    {
        services.AddSingleton<IHealthCheck>(sp => 
            new DeploymentServiceHealthCheck(sp, sp.GetService<ILogger<DeploymentServiceHealthCheck>>()));
        
        services.AddSingleton<IHealthCheck>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();
            return new RpcHealthCheck(rpcUrl, httpClient, sp.GetService<ILogger<RpcHealthCheck>>());
        });

        services.AddSingleton<IHealthCheck>(sp =>
            new WalletHealthCheck(sp, sp.GetService<ILogger<WalletHealthCheck>>()));

        return services;
    }

    /// <summary>
    /// Configure deployment record service
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="persistencePath">Path to persist deployment records</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddDeploymentRecordService(
        this IServiceCollection services,
        string persistencePath)
    {
        services.AddSingleton(sp =>
        {
            var logger = sp.GetService<ILogger<DeploymentRecordService>>();
            return new DeploymentRecordService(persistencePath, logger);
        });
        return services;
    }

    /// <summary>
    /// Configure performance monitoring
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="maxSamplesPerCounter">Maximum samples to keep per counter</param>
    /// <param name="aggregationInterval">Interval for aggregating metrics</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddPerformanceMonitoring(
        this IServiceCollection services,
        int maxSamplesPerCounter = 1000,
        TimeSpan? aggregationInterval = null)
    {
        services.AddSingleton(sp =>
        {
            var logger = sp.GetService<ILogger<PerformanceMonitor>>();
            return new PerformanceMonitor(logger, maxSamplesPerCounter, aggregationInterval);
        });
        return services;
    }

    /// <summary>
    /// Configure deployment metrics
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="retentionPeriod">How long to keep metrics</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddDeploymentMetrics(
        this IServiceCollection services,
        TimeSpan? retentionPeriod = null)
    {
        services.AddSingleton(_ => new DeploymentMetrics(retentionPeriod));
        return services;
    }
}

/// <summary>
/// Composite credential provider that tries multiple providers in order
/// </summary>
internal class CompositeCredentialProvider : ICredentialProvider
{
    private readonly ICredentialProvider _primary;
    private readonly ICredentialProvider _fallback;

    public string ProviderName => "CompositeCredentialProvider";
    public bool SupportsPersistence => _primary.SupportsPersistence || _fallback.SupportsPersistence;

    public CompositeCredentialProvider(ICredentialProvider primary, ICredentialProvider fallback)
    {
        _primary = primary ?? throw new ArgumentNullException(nameof(primary));
        _fallback = fallback ?? throw new ArgumentNullException(nameof(fallback));
    }

    public async Task<string?> GetCredentialAsync(string key)
    {
        var result = await _primary.GetCredentialAsync(key);
        if (result != null) return result;
        
        return await _fallback.GetCredentialAsync(key);
    }

    public async Task SetCredentialAsync(string key, string value)
    {
        // Try to set in primary first
        try
        {
            await _primary.SetCredentialAsync(key, value);
        }
        catch
        {
            // If primary fails, try fallback
            await _fallback.SetCredentialAsync(key, value);
        }
    }

    public async Task<bool> RemoveCredentialAsync(string key)
    {
        var removed1 = await _primary.RemoveCredentialAsync(key);
        var removed2 = await _fallback.RemoveCredentialAsync(key);
        return removed1 || removed2;
    }

    public async Task<bool> ExistsAsync(string key)
    {
        if (await _primary.ExistsAsync(key)) return true;
        return await _fallback.ExistsAsync(key);
    }

    public async Task ClearAsync()
    {
        await _primary.ClearAsync();
        await _fallback.ClearAsync();
    }
}