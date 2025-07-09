using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo.SmartContract.Deploy.HealthChecks.Interfaces;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Security.Interfaces;

namespace Neo.SmartContract.Deploy.HealthChecks;

/// <summary>
/// Health check for wallet services
/// </summary>
public class WalletHealthCheck : IHealthCheck
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<WalletHealthCheck>? _logger;
    private readonly string? _testWif;

    /// <inheritdoc />
    public string Name => "WalletService";

    /// <summary>
    /// Initialize a new instance of WalletHealthCheck
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="logger">Optional logger</param>
    /// <param name="testWif">Optional test WIF for health check validation</param>
    public WalletHealthCheck(IServiceProvider serviceProvider, ILogger<WalletHealthCheck>? logger = null, string? testWif = null)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger;
        _testWif = testWif;
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var data = new Dictionary<string, object>();

        try
        {
            _logger?.LogDebug("Starting wallet service health check");

            // Check if wallet manager is available
            var walletManager = _serviceProvider.GetService<IWalletManager>();
            if (walletManager == null)
            {
                stopwatch.Stop();
                var result = HealthCheckResult.Unhealthy(
                    "Wallet manager service is not available",
                    data: data);
                result.Duration = stopwatch.Elapsed;
                return result;
            }

            data["WalletManagerAvailable"] = true;

            // Check credential providers
            var credentialProviderHealth = await CheckCredentialProvidersAsync(cancellationToken);
            data["CredentialProviders"] = credentialProviderHealth;

            // Test wallet functionality if test WIF is provided
            if (!string.IsNullOrEmpty(_testWif))
            {
                var walletFunctionalityHealth = await TestWalletFunctionalityAsync(walletManager, cancellationToken);
                data["WalletFunctionality"] = walletFunctionalityHealth;
            }

            stopwatch.Stop();

            // Determine overall health
            var hasCredentialProvider = credentialProviderHealth["Available"] is bool available && available;
            var walletFunctional = !data.ContainsKey("WalletFunctionality") || 
                                 (data["WalletFunctionality"] is Dictionary<string, object> func && 
                                  func["Status"]?.ToString() == "Healthy");

            if (!hasCredentialProvider)
            {
                var result = HealthCheckResult.Degraded(
                    "Wallet service is available but no credential providers are configured",
                    data: data);
                result.Duration = stopwatch.Elapsed;
                return result;
            }

            if (!walletFunctional)
            {
                var result = HealthCheckResult.Degraded(
                    "Wallet service is available but functionality test failed",
                    data: data);
                result.Duration = stopwatch.Elapsed;
                return result;
            }

            var healthyResult = HealthCheckResult.Healthy(
                "Wallet service is healthy and functional",
                data);
            healthyResult.Duration = stopwatch.Elapsed;
            return healthyResult;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Wallet service health check failed");
            stopwatch.Stop();
            
            var result = HealthCheckResult.Unhealthy(
                "Wallet service health check failed",
                ex,
                data);
            result.Duration = stopwatch.Elapsed;
            return result;
        }
    }

    private async Task<Dictionary<string, object>> CheckCredentialProvidersAsync(CancellationToken cancellationToken)
    {
        var result = new Dictionary<string, object>();

        try
        {
            // Check for any registered credential providers
            var credentialProviders = _serviceProvider.GetServices<ICredentialProvider>().ToList();
            
            result["Count"] = credentialProviders.Count;
            result["Available"] = credentialProviders.Count > 0;

            if (credentialProviders.Count > 0)
            {
                var providers = new List<Dictionary<string, object>>();
                
                foreach (var provider in credentialProviders)
                {
                    var providerInfo = new Dictionary<string, object>
                    {
                        ["Name"] = provider.ProviderName,
                        ["SupportsPersistence"] = provider.SupportsPersistence
                    };

                    // Test basic functionality
                    try
                    {
                        var testKey = $"healthcheck_test_{Guid.NewGuid():N}";
                        await provider.SetCredentialAsync(testKey, "test_value");
                        var retrieved = await provider.GetCredentialAsync(testKey);
                        await provider.RemoveCredentialAsync(testKey);

                        providerInfo["Functional"] = retrieved == "test_value";
                        providerInfo["Status"] = "Healthy";
                    }
                    catch (Exception ex)
                    {
                        providerInfo["Functional"] = false;
                        providerInfo["Status"] = "Unhealthy";
                        providerInfo["Error"] = ex.Message;
                        _logger?.LogWarning(ex, "Credential provider {Provider} functionality test failed", provider.ProviderName);
                    }

                    providers.Add(providerInfo);
                }

                result["Providers"] = providers;
            }
        }
        catch (Exception ex)
        {
            result["Available"] = false;
            result["Error"] = ex.Message;
            _logger?.LogWarning(ex, "Failed to check credential providers");
        }

        return result;
    }

    private async Task<Dictionary<string, object>> TestWalletFunctionalityAsync(IWalletManager walletManager, CancellationToken cancellationToken)
    {
        var result = new Dictionary<string, object>();

        try
        {
            // Test account creation from WIF
            var account = walletManager.GetAccountFromWif(_testWif!);
            result["AccountCreation"] = "Success";

            // Test address generation
            var address = walletManager.GetAccountAddress(account);
            result["AddressGeneration"] = !string.IsNullOrEmpty(address) ? "Success" : "Failed";
            
            if (!string.IsNullOrEmpty(address))
            {
                result["Address"] = address;
                result["ScriptHash"] = account.ScriptHash.ToString();
            }

            result["Status"] = "Healthy";
            
            await Task.CompletedTask; // Keep async
        }
        catch (Exception ex)
        {
            result["Status"] = "Unhealthy";
            result["Error"] = ex.Message;
            _logger?.LogWarning(ex, "Wallet functionality test failed");
        }

        return result;
    }

    /// <summary>
    /// Perform a detailed wallet health check
    /// </summary>
    /// <param name="testRpcUrl">Optional RPC URL to test balance queries</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed health check result</returns>
    public async Task<HealthCheckResult> CheckHealthDetailedAsync(string? testRpcUrl = null, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var data = new Dictionary<string, object>();

        try
        {
            // First perform basic check
            var basicResult = await CheckHealthAsync(cancellationToken);
            data["BasicCheck"] = basicResult.Data;

            if (basicResult.Status == HealthStatus.Unhealthy)
            {
                return basicResult;
            }

            // Additional detailed checks
            var walletManager = _serviceProvider.GetRequiredService<IWalletManager>();

            // Test balance query if RPC URL provided
            if (!string.IsNullOrEmpty(testRpcUrl) && !string.IsNullOrEmpty(_testWif))
            {
                var balanceCheck = await TestBalanceQueryAsync(walletManager, testRpcUrl, cancellationToken);
                data["BalanceQuery"] = balanceCheck;
            }

            // Test multiple account creation
            var multiAccountTest = await TestMultipleAccountsAsync(walletManager, cancellationToken);
            data["MultipleAccounts"] = multiAccountTest;

            stopwatch.Stop();

            // Evaluate overall health
            var allHealthy = true;
            if (data.ContainsKey("BalanceQuery") && data["BalanceQuery"] is Dictionary<string, object> balance)
            {
                allHealthy &= balance["Status"]?.ToString() == "Success";
            }
            if (data["MultipleAccounts"] is Dictionary<string, object> multiAccount)
            {
                allHealthy &= multiAccount["Status"]?.ToString() == "Success";
            }

            if (allHealthy)
            {
                var result = HealthCheckResult.Healthy(
                    "Wallet service detailed check passed",
                    data);
                result.Duration = stopwatch.Elapsed;
                return result;
            }
            else
            {
                var result = HealthCheckResult.Degraded(
                    "Wallet service has some functionality issues",
                    data: data);
                result.Duration = stopwatch.Elapsed;
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Detailed wallet health check failed");
            stopwatch.Stop();
            
            var result = HealthCheckResult.Unhealthy(
                "Detailed wallet health check failed",
                ex,
                data);
            result.Duration = stopwatch.Elapsed;
            return result;
        }
    }

    private async Task<Dictionary<string, object>> TestBalanceQueryAsync(IWalletManager walletManager, string rpcUrl, CancellationToken cancellationToken)
    {
        var result = new Dictionary<string, object>();

        try
        {
            var account = walletManager.GetAccountFromWif(_testWif!);
            var balance = await walletManager.GetGasBalanceAsync(account.ScriptHash, rpcUrl);
            
            result["Status"] = "Success";
            result["Balance"] = balance;
            result["RpcUrl"] = rpcUrl;
        }
        catch (Exception ex)
        {
            result["Status"] = "Failed";
            result["Error"] = ex.Message;
            _logger?.LogWarning(ex, "Balance query test failed");
        }

        return result;
    }

    private async Task<Dictionary<string, object>> TestMultipleAccountsAsync(IWalletManager walletManager, CancellationToken cancellationToken)
    {
        var result = new Dictionary<string, object>();

        try
        {
            // Generate test WIFs
            var testWifs = new[]
            {
                "L1QqQJnpBwbsPGAuutuzPTac8piqvbR1HRjrY5qHup48TBCBFe4g",
                "L2QTooFoDFyRFTxmtiVHt5CfsXfVnexdbENGDkkrrgTTryiLsPMG",
                "L3Ke6P7V5qWFiNJZRqHLVXWGEWwDMfTe8s5zj8g8mJR1JdyL7eVq"
            };

            var accounts = new List<string>();
            foreach (var wif in testWifs)
            {
                var account = walletManager.GetAccountFromWif(wif);
                var address = walletManager.GetAccountAddress(account);
                accounts.Add(address);
            }

            result["Status"] = "Success";
            result["AccountsCreated"] = accounts.Count;
            result["UniqueAddresses"] = accounts.Distinct().Count();
            
            await Task.CompletedTask; // Keep async
        }
        catch (Exception ex)
        {
            result["Status"] = "Failed";
            result["Error"] = ex.Message;
            _logger?.LogWarning(ex, "Multiple accounts test failed");
        }

        return result;
    }
}