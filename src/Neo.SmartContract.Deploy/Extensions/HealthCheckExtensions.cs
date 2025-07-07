using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Neo.SmartContract.Deploy.HealthChecks;
using Neo.SmartContract.Deploy.Monitoring;

namespace Neo.SmartContract.Deploy.Extensions;

/// <summary>
/// Extension methods for health checks and monitoring
/// </summary>
public static class HealthCheckExtensions
{
    /// <summary>
    /// Check the health of the deployment toolkit
    /// </summary>
    public static async Task<HealthReport> CheckHealthAsync(this DeploymentToolkit toolkit, CancellationToken cancellationToken = default)
    {
        var serviceProvider = GetServiceProvider(toolkit);
        var healthCheckService = serviceProvider.GetService<HealthCheckService>();

        if (healthCheckService == null)
        {
            throw new InvalidOperationException("Health check service not registered. Ensure services are properly configured.");
        }

        return await healthCheckService.CheckHealthAsync(cancellationToken);
    }

    /// <summary>
    /// Get deployment metrics
    /// </summary>
    public static MetricsSnapshot GetMetrics(this DeploymentToolkit toolkit)
    {
        var serviceProvider = GetServiceProvider(toolkit);
        var metrics = serviceProvider.GetService<DeploymentMetrics>();

        if (metrics == null)
        {
            throw new InvalidOperationException("Metrics service not registered. Ensure services are properly configured.");
        }

        return metrics.GetSnapshot();
    }

    /// <summary>
    /// Check health of a specific component
    /// </summary>
    public static async Task<HealthCheckResult?> CheckHealthAsync(this DeploymentToolkit toolkit, string componentName, CancellationToken cancellationToken = default)
    {
        var serviceProvider = GetServiceProvider(toolkit);
        var healthCheckService = serviceProvider.GetService<HealthCheckService>();

        if (healthCheckService == null)
        {
            throw new InvalidOperationException("Health check service not registered. Ensure services are properly configured.");
        }

        return await healthCheckService.CheckHealthAsync(componentName, cancellationToken);
    }

    /// <summary>
    /// Get service provider from toolkit using reflection
    /// </summary>
    private static IServiceProvider GetServiceProvider(DeploymentToolkit toolkit)
    {
        var type = toolkit.GetType();
        var field = type.GetField("_serviceProvider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (field == null)
        {
            throw new InvalidOperationException("Unable to access service provider from toolkit");
        }

        var serviceProvider = field.GetValue(toolkit) as IServiceProvider;

        if (serviceProvider == null)
        {
            throw new InvalidOperationException("Service provider is not initialized");
        }

        return serviceProvider;
    }
}

/// <summary>
/// Extension methods for NeoContractToolkit health checks
/// </summary>
public static class NeoContractToolkitHealthExtensions
{
    /// <summary>
    /// Check the health of the toolkit
    /// </summary>
    public static async Task<HealthReport> CheckHealthAsync(this NeoContractToolkit toolkit, CancellationToken cancellationToken = default)
    {
        var healthCheckService = toolkit.GetService<HealthCheckService>();
        return await healthCheckService.CheckHealthAsync(cancellationToken);
    }

    /// <summary>
    /// Get deployment metrics
    /// </summary>
    public static MetricsSnapshot GetMetrics(this NeoContractToolkit toolkit)
    {
        var metrics = toolkit.GetService<DeploymentMetrics>();
        return metrics.GetSnapshot();
    }
}
