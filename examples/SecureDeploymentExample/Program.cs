using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo.SmartContract.Deploy;
using Neo.SmartContract.Deploy.Extensions;
using Neo.SmartContract.Deploy.HealthChecks;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Monitoring;
using Neo.SmartContract.Deploy.Security.Interfaces;

namespace SecureDeploymentExample;

/// <summary>
/// Example demonstrating secure contract deployment with enhanced features
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Configure services with security features
        var services = new ServiceCollection();
        
        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Add base deployment services
        services.AddNeoContractDeploy(configuration);

        // Configure secure credential storage
        ConfigureSecureCredentials(services);

        // Add health checks
        services.AddNeoHealthChecks("https://seed1.neo.org:443");

        // Add monitoring and metrics
        services.AddDeploymentMetrics(TimeSpan.FromDays(7));
        services.AddPerformanceMonitoring();
        services.AddDeploymentRecordService("./deployment-history.json");

        // Build service provider
        var serviceProvider = services.BuildServiceProvider();

        try
        {
            // Run the secure deployment example
            await RunSecureDeployment(serviceProvider);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Deployment failed: {ex.Message}");
            Environment.Exit(1);
        }
    }

    private static void ConfigureSecureCredentials(IServiceCollection services)
    {
        // Example 1: Using environment variables (recommended for CI/CD)
        if (Environment.GetEnvironmentVariable("USE_ENV_CREDENTIALS") == "true")
        {
            services.AddEnvironmentCredentialProvider("NEO_DEPLOY_");
            return;
        }

        // Example 2: Using secure encrypted storage (recommended for production)
        var encryptionKey = GetOrCreateEncryptionKey();
        services.AddSecureCredentialProvider(encryptionKey, "./credentials.enc");

        // Example 3: Using password-derived encryption
        // var password = GetSecurePassword();
        // var salt = GetApplicationSalt();
        // services.AddSecureCredentialProviderWithPassword(password, salt, "./credentials.enc");

        // Example 4: Using credential chain (try secure first, fallback to env)
        // services.AddCredentialProviderChain(
        //     primary: s => s.AddSecureCredentialProvider(encryptionKey, "./credentials.enc"),
        //     fallback: s => s.AddEnvironmentCredentialProvider("NEO_DEPLOY_")
        // );
    }

    private static async Task RunSecureDeployment(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        var toolkit = serviceProvider.GetRequiredService<NeoContractToolkit>();
        var credentialProvider = serviceProvider.GetRequiredService<ICredentialProvider>();
        var healthService = serviceProvider.GetRequiredService<HealthCheckService>();
        var metrics = serviceProvider.GetRequiredService<DeploymentMetrics>();
        var perfMonitor = serviceProvider.GetRequiredService<PerformanceMonitor>();
        var recordService = serviceProvider.GetRequiredService<DeploymentRecordService>();

        logger.LogInformation("Starting secure deployment example");

        // Step 1: Store credentials securely (if not already stored)
        await SetupCredentials(credentialProvider, logger);

        // Step 2: Run health checks
        logger.LogInformation("Running pre-deployment health checks...");
        var healthReport = await healthService.CheckHealthAsync();
        
        logger.LogInformation("Health check result: {Status}", healthReport.Status);
        if (healthReport.Status == HealthStatus.Unhealthy)
        {
            logger.LogError("Health checks failed. Aborting deployment.");
            return;
        }

        // Step 3: Configure secure deployment options
        var deploymentOptions = new DeploymentOptions
        {
            // Security settings
            RequireSecureCredentials = true,
            CredentialKey = "mainnet_deployer_wif",
            ValidateRpcCertificate = true,
            
            // Network configuration
            RpcUrl = "https://seed1.neo.org:443",
            NetworkMagic = 860833102, // Mainnet
            
            // Performance and monitoring
            EnablePerformanceMonitoring = true,
            EnableMetrics = true,
            RecordDeploymentHistory = true,
            DeploymentRecordPath = "./deployment-history.json",
            
            // Safety features
            PerformHealthChecks = true,
            AbortOnHealthCheckFailure = true,
            EnableAutoRetry = true,
            MaxAutoRetries = 3,
            WaitForConfirmation = true,
            VerifyAfterDeploy = true,
            
            // Environment tagging
            Environment = "production",
            DeploymentTags = new Dictionary<string, string>
            {
                ["version"] = "1.0.0",
                ["purpose"] = "example",
                ["team"] = "dev-team"
            },
            
            // Deployment hooks
            Hooks = new DeploymentHooks
            {
                PreDeployment = async (options) =>
                {
                    logger.LogInformation("Pre-deployment hook: Validating deployment window");
                    await Task.Delay(100); // Simulate validation
                },
                PostDeployment = async (info) =>
                {
                    logger.LogInformation("Post-deployment hook: Contract deployed at {Hash}", 
                        info.ContractHash);
                },
                OnFailure = async (ex) =>
                {
                    logger.LogError(ex, "Deployment failed hook triggered");
                    await Task.CompletedTask;
                }
            }
        };

        // Step 4: Compile the contract
        logger.LogInformation("Compiling contract...");
        var projectPath = "../TestContract/TestContract.csproj";
        
        CompiledContract contract;
        using (var tracker = perfMonitor.StartTracking("compilation", "contract_compile"))
        {
            contract = await toolkit.CompileAsync(projectPath);
            tracker.Stop();
        }
        
        logger.LogInformation("Contract compiled: {Name}", contract.Name);

        // Step 5: Deploy with monitoring
        logger.LogInformation("Deploying contract...");
        
        ContractDeploymentInfo deploymentInfo;
        var deploymentStart = DateTime.UtcNow;
        
        try
        {
            using (var tracker = perfMonitor.StartTracking("deployment", "contract_deploy"))
            {
                deploymentInfo = await toolkit.DeployAsync(contract, deploymentOptions);
                tracker.Stop();
            }
            
            var deploymentDuration = DateTime.UtcNow - deploymentStart;
            
            // Record successful deployment
            metrics.RecordDeployment(
                contract.Name,
                deploymentInfo.ContractHash.ToString(),
                true,
                deploymentDuration);
            
            await recordService.RecordDeploymentAsync(
                deploymentInfo,
                deploymentOptions,
                new Dictionary<string, object>
                {
                    ["compiler_version"] = "3.6.2",
                    ["deployment_method"] = "secure_example"
                });
            
            logger.LogInformation("Contract deployed successfully!");
            logger.LogInformation("Contract Hash: {Hash}", deploymentInfo.ContractHash);
            logger.LogInformation("Transaction Hash: {TxHash}", deploymentInfo.TransactionHash);
            logger.LogInformation("Gas Used: {Gas}", deploymentInfo.GasConsumed);
        }
        catch (Exception ex)
        {
            var deploymentDuration = DateTime.UtcNow - deploymentStart;
            
            // Record failed deployment
            metrics.RecordDeployment(
                contract.Name,
                string.Empty,
                false,
                deploymentDuration,
                ex.Message);
            
            await recordService.RecordFailedDeploymentAsync(
                contract.Name,
                ex,
                deploymentOptions);
            
            throw;
        }

        // Step 6: Display metrics and statistics
        await DisplayMetricsAndStatistics(metrics, perfMonitor, recordService, logger);
    }

    private static async Task SetupCredentials(ICredentialProvider credentialProvider, ILogger logger)
    {
        // Check if credentials already exist
        if (await credentialProvider.ExistsAsync("mainnet_deployer_wif"))
        {
            logger.LogInformation("Credentials already configured");
            return;
        }

        // In a real application, you would get this from a secure source
        // For this example, we'll prompt the user
        Console.Write("Enter deployer WIF key (or press Enter to use test key): ");
        var wif = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(wif))
        {
            // Use a test key for demonstration (DO NOT USE IN PRODUCTION!)
            wif = "L1QqQJnpBwbsPGAuutuzPTac8piqvbR1HRjrY5qHup48TBCBFe4g";
            logger.LogWarning("Using test WIF key. DO NOT use this in production!");
        }

        await credentialProvider.SetCredentialAsync("mainnet_deployer_wif", wif);
        logger.LogInformation("Credentials stored securely");
    }

    private static byte[] GetOrCreateEncryptionKey()
    {
        // In a real application, this key should be stored securely
        // (e.g., in Azure Key Vault, AWS KMS, or HSM)
        var keyFile = "./encryption.key";
        
        if (System.IO.File.Exists(keyFile))
        {
            return System.IO.File.ReadAllBytes(keyFile);
        }

        // Generate a new key
        var key = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(key);
        }
        
        System.IO.File.WriteAllBytes(keyFile, key);
        Console.WriteLine("Generated new encryption key. Store this securely!");
        
        return key;
    }

    private static async Task DisplayMetricsAndStatistics(
        DeploymentMetrics metrics,
        PerformanceMonitor perfMonitor,
        DeploymentRecordService recordService,
        ILogger logger)
    {
        logger.LogInformation("=== Deployment Metrics ===");
        
        // Display deployment statistics
        var stats = metrics.GetStatistics();
        logger.LogInformation("Total Deployments: {Total}", stats.TotalDeployments);
        logger.LogInformation("Successful: {Success} ({Rate:P})", 
            stats.SuccessfulDeployments, 
            stats.DeploymentSuccessRate);
        logger.LogInformation("Average Deployment Time: {Time}ms", 
            stats.AverageDeploymentTime.TotalMilliseconds);

        // Display performance metrics
        logger.LogInformation("\n=== Performance Report ===");
        var perfReport = perfMonitor.GetReport();
        Console.WriteLine(perfReport);

        // Display recent deployments
        logger.LogInformation("\n=== Recent Deployments ===");
        var recentDeployments = await Task.Run(() => recordService.GetAllRecords(true));
        
        foreach (var deployment in recentDeployments.Take(5))
        {
            logger.LogInformation("{Time}: {Contract} - {Status}",
                deployment.Timestamp,
                deployment.ContractName,
                deployment.Success ? "Success" : "Failed");
        }

        // Export deployment history
        var exportPath = "./deployment-export.json";
        await recordService.ExportToJsonAsync(exportPath);
        logger.LogInformation("\nDeployment history exported to: {Path}", exportPath);
    }
}