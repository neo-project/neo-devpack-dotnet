using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Neo.SmartContract.Deploy.HealthChecks;

/// <summary>
/// Health check for deployment services
/// </summary>
public class DeploymentServiceHealthCheck : IHealthCheck
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DeploymentServiceHealthCheck> _logger;

    public DeploymentServiceHealthCheck(
        IServiceProvider serviceProvider,
        ILogger<DeploymentServiceHealthCheck> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var data = new Dictionary<string, object>();
        var issues = new List<string>();

        try
        {
            // Check if all required services can be resolved
            var requiredServices = new[]
            {
                ("ContractCompiler", typeof(Interfaces.IContractCompiler)),
                ("ContractDeployer", typeof(Interfaces.IContractDeployer)),
                ("ContractInvoker", typeof(Interfaces.IContractInvoker)),
                ("WalletManager", typeof(Interfaces.IWalletManager)),
                ("RpcClientFactory", typeof(Shared.IRpcClientFactory))
            };

            foreach (var (name, serviceType) in requiredServices)
            {
                try
                {
                    var service = _serviceProvider.GetService(serviceType);
                    data[$"{name}Available"] = service != null;

                    if (service == null)
                    {
                        issues.Add($"{name} service not registered");
                    }
                }
                catch (Exception ex)
                {
                    data[$"{name}Available"] = false;
                    data[$"{name}Error"] = ex.Message;
                    issues.Add($"{name} service failed to resolve: {ex.Message}");
                }
            }

            // Check credential provider (optional but recommended)
            try
            {
                var credentialProvider = _serviceProvider.GetService<Security.ICredentialProvider>();
                data["CredentialProviderAvailable"] = credentialProvider != null;

                if (credentialProvider == null)
                {
                    issues.Add("Credential provider not configured (security feature)");
                }
            }
            catch (Exception ex)
            {
                data["CredentialProviderAvailable"] = false;
                data["CredentialProviderError"] = ex.Message;
            }

            stopwatch.Stop();
            data["CheckDuration"] = stopwatch.ElapsedMilliseconds;

            if (issues.Count == 0)
            {
                var result = HealthCheckResult.Healthy(
                    "All deployment services are properly configured",
                    data);
                result.Duration = stopwatch.Elapsed;
                return Task.FromResult(result);
            }
            else if (issues.Any(i => i.Contains("service not registered")))
            {
                var result = HealthCheckResult.Unhealthy(
                    $"Critical services missing: {string.Join("; ", issues)}",
                    data: data);
                result.Duration = stopwatch.Elapsed;
                return Task.FromResult(result);
            }
            else
            {
                var result = HealthCheckResult.Degraded(
                    $"Some optional services have issues: {string.Join("; ", issues)}",
                    data: data);
                result.Duration = stopwatch.Elapsed;
                return Task.FromResult(result);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Deployment service health check failed");

            var result = HealthCheckResult.Unhealthy(
                $"Deployment service health check failed: {ex.Message}",
                ex,
                data);
            result.Duration = stopwatch.Elapsed;
            return Task.FromResult(result);
        }
    }
}
