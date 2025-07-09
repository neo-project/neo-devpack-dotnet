using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Services;

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