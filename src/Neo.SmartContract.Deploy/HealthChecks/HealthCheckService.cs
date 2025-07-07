using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Neo.SmartContract.Deploy.HealthChecks;

/// <summary>
/// Service that aggregates all health checks
/// </summary>
public class HealthCheckService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HealthCheckService> _logger;
    private readonly Dictionary<string, IHealthCheck> _healthChecks;

    public HealthCheckService(
        IServiceProvider serviceProvider,
        ILogger<HealthCheckService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _healthChecks = new Dictionary<string, IHealthCheck>();

        // Register default health checks
        RegisterDefaultHealthChecks();
    }

    private void RegisterDefaultHealthChecks()
    {
        // Try to register available health checks
        TryRegisterHealthCheck<RpcHealthCheck>("rpc");
        TryRegisterHealthCheck<WalletHealthCheck>("wallet");
        TryRegisterHealthCheck<DeploymentServiceHealthCheck>("services");
    }

    private void TryRegisterHealthCheck<T>(string name) where T : IHealthCheck
    {
        try
        {
            var healthCheck = _serviceProvider.GetService<T>();
            if (healthCheck != null)
            {
                _healthChecks[name] = healthCheck;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to register health check {Name}", name);
        }
    }

    /// <summary>
    /// Register a custom health check
    /// </summary>
    public void RegisterHealthCheck(string name, IHealthCheck healthCheck)
    {
        _healthChecks[name] = healthCheck ?? throw new ArgumentNullException(nameof(healthCheck));
    }

    /// <summary>
    /// Check health of all registered components
    /// </summary>
    public async Task<HealthReport> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        var report = new HealthReport
        {
            CheckedAt = DateTime.UtcNow
        };

        var tasks = _healthChecks.Select(async kvp =>
        {
            try
            {
                var result = await kvp.Value.CheckHealthAsync(cancellationToken);
                return (kvp.Key, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check {Name} threw an exception", kvp.Key);
                return (kvp.Key, HealthCheckResult.Unhealthy($"Health check failed with exception: {ex.Message}", ex));
            }
        });

        var results = await Task.WhenAll(tasks);

        foreach (var (name, result) in results)
        {
            report.Results[name] = result;
        }

        // Calculate overall status
        if (report.Results.Values.Any(r => r.Status == HealthStatus.Unhealthy))
        {
            report.Status = HealthStatus.Unhealthy;
        }
        else if (report.Results.Values.Any(r => r.Status == HealthStatus.Degraded))
        {
            report.Status = HealthStatus.Degraded;
        }
        else
        {
            report.Status = HealthStatus.Healthy;
        }

        report.TotalDuration = TimeSpan.FromMilliseconds(
            report.Results.Values.Sum(r => r.Duration.TotalMilliseconds)
        );

        return report;
    }

    /// <summary>
    /// Check health of a specific component
    /// </summary>
    public async Task<HealthCheckResult?> CheckHealthAsync(string name, CancellationToken cancellationToken = default)
    {
        if (!_healthChecks.TryGetValue(name, out var healthCheck))
        {
            return null;
        }

        try
        {
            return await healthCheck.CheckHealthAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check {Name} threw an exception", name);
            return HealthCheckResult.Unhealthy($"Health check failed with exception: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Get list of registered health check names
    /// </summary>
    public IEnumerable<string> GetHealthCheckNames()
    {
        return _healthChecks.Keys;
    }
}

/// <summary>
/// Aggregate health report
/// </summary>
public class HealthReport
{
    /// <summary>
    /// Overall health status
    /// </summary>
    public HealthStatus Status { get; set; } = HealthStatus.Healthy;

    /// <summary>
    /// Individual health check results
    /// </summary>
    public Dictionary<string, HealthCheckResult> Results { get; set; } = new();

    /// <summary>
    /// When the health check was performed
    /// </summary>
    public DateTime CheckedAt { get; set; }

    /// <summary>
    /// Total duration of all health checks
    /// </summary>
    public TimeSpan TotalDuration { get; set; }

    /// <summary>
    /// Convert to a simple dictionary for JSON serialization
    /// </summary>
    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            ["status"] = Status.ToString(),
            ["checkedAt"] = CheckedAt.ToString("O"),
            ["totalDuration"] = $"{TotalDuration.TotalMilliseconds}ms",
            ["checks"] = Results.ToDictionary(
                kvp => kvp.Key,
                kvp => new Dictionary<string, object>
                {
                    ["status"] = kvp.Value.Status.ToString(),
                    ["description"] = kvp.Value.Description,
                    ["duration"] = $"{kvp.Value.Duration.TotalMilliseconds}ms",
                    ["data"] = kvp.Value.Data,
                    ["error"] = kvp.Value.Exception?.Message
                }
            )
        };
    }
}
