using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neo.SmartContract.Deploy.Configuration;
using Neo.SmartContract.Deploy.Services;
using Neo.SmartContract.Deploy.Steps;
using Neo.SmartContract.Deploy.Utilities;

namespace Neo.SmartContract.Deploy
{
    /// <summary>
    /// Extension methods for registering deployment services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Neo smart contract deployment services
        /// </summary>
        public static IServiceCollection AddNeoDeploymentServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuration - network options now include wallet configuration
            services.Configure<DeploymentOptions>(configuration.GetSection("Deployment"));
            services.Configure<NetworkOptions>(configuration.GetSection("Network"));

            // Core services (always required)
            services.AddSingleton<IBlockchainService, BlockchainService>();
            services.AddSingleton<IWalletService, WalletService>();
            services.AddSingleton<IContractLoader, ContractLoader>();
            services.AddSingleton<IDeploymentService, DeploymentService>();
            services.AddSingleton<IDeploymentRecordService, DeploymentRecordService>();
            services.AddSingleton<IContractUpdateService, ContractUpdateService>();

            // Utilities
            services.AddSingleton<IContractInvoker, ContractInvoker>();

            return services;
        }

        /// <summary>
        /// Add Neo Express support for local development (optional)
        /// </summary>
        public static IServiceCollection AddNeoExpressSupport(this IServiceCollection services, IConfiguration configuration)
        {
            // Neo Express configuration
            services.Configure<NeoExpressOptions>(configuration.GetSection("NeoExpress"));
            
            // Neo Express services
            services.AddSingleton<INeoExpressService, NeoExpressService>();
            
            // Replace wallet service with Neo Express version when enabled
            var useNeoExpress = configuration.GetValue<bool>("Deployment:UseNeoExpress");
            if (useNeoExpress)
            {
                services.AddSingleton<IWalletService, NeoExpressWalletService>();
            }

            return services;
        }

        /// <summary>
        /// Add Neo Express setup step
        /// </summary>
        public static IServiceCollection AddNeoExpressSetup(this IServiceCollection services)
        {
            services.AddTransient<IDeploymentStep, NeoExpressSetupStep>();
            return services;
        }

        /// <summary>
        /// Add a deployment step
        /// </summary>
        public static IServiceCollection AddDeploymentStep<TStep>(this IServiceCollection services)
            where TStep : class, IDeploymentStep
        {
            services.AddTransient<IDeploymentStep, TStep>();
            return services;
        }
    }
}