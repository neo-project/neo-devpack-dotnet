using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Services;
using Neo.SmartContract.Deploy.Shared;

namespace Neo.SmartContract.Deploy;

/// <summary>
/// Extension methods for service collection to register deployment services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add Neo contract deployment services to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddNeoContractDeployment(this IServiceCollection services)
    {
        // Register shared utilities
        services.AddSingleton<IRpcClientFactory, RpcClientFactory>();
        services.AddSingleton<TransactionBuilder>();
        services.AddSingleton<TransactionConfirmationService>();

        // Register core services
        services.AddTransient<IContractCompiler, ContractCompilerService>();
        services.AddTransient<IContractDeployer, ContractDeployerService>();
        services.AddTransient<IContractInvoker, ContractInvokerService>();
        services.AddTransient<IWalletManager, WalletManagerService>();
        services.AddTransient<IDeploymentRecordService, DeploymentRecordService>();
        services.AddTransient<IContractUpdateService, ContractUpdateService>();
        services.AddTransient<MultiContractDeploymentService>();

        // Register main toolkit
        services.AddTransient<NeoContractToolkit>();
        services.AddTransient<DeploymentToolkit>();

        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
        });

        return services;
    }

    /// <summary>
    /// Create a default toolkit instance with console logging
    /// </summary>
    /// <returns>Configured toolkit instance</returns>
    public static NeoContractToolkit CreateDefaultToolkit()
    {
        return NeoContractToolkitBuilder.Create()
            .ConfigureLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
            })
            .Build();
    }

    /// <summary>
    /// Create a toolkit instance with debug logging
    /// </summary>
    /// <returns>Configured toolkit instance with debug logging</returns>
    public static NeoContractToolkit CreateDebugToolkit()
    {
        return NeoContractToolkitBuilder.Create()
            .ConfigureLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
            })
            .Build();
    }
}
