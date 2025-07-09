using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo.SmartContract.Deploy.HealthChecks.Interfaces;

namespace Neo.SmartContract.Deploy.HealthChecks;

/// <summary>
/// Service to aggregate and run multiple health checks
/// </summary>
public class HealthCheckService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HealthCheckService>? _logger;
    private readonly List<IHealthCheck> _healthChecks;
    private readonly TimeSpan _defaultTimeout;

    /// <summary>
    /// Initialize a new instance of HealthCheckService
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="logger">Optional logger</param>
    /// <param name="defaultTimeout">Default timeout for health checks (default: 30 seconds)</param>
    public HealthCheckService(IServiceProvider serviceProvider, ILogger<HealthCheckService>? logger = null, TimeSpan? defaultTimeout = null)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger;
        _defaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(30);
        _healthChecks = new List<IHealthCheck>();
    }

    /// <summary>
    /// Add a health check
    /// </summary>
    /// <param name="healthCheck">Health check to add</param>
    public void AddHealthCheck(IHealthCheck healthCheck)
    {
        if (healthCheck == null) throw new ArgumentNullException(nameof(healthCheck));
        
        _healthChecks.Add(healthCheck);
        _logger?.LogDebug("Added health check: {HealthCheckName}", healthCheck.Name);
    }

    /// <summary>
    /// Add multiple health checks
    /// </summary>
    /// <param name="healthChecks">Health checks to add</param>
    public void AddHealthChecks(IEnumerable<IHealthCheck> healthChecks)
    {
        foreach (var healthCheck in healthChecks)
        {
            AddHealthCheck(healthCheck);
        }
    }

    /// <summary>
    /// Add all registered health checks from the service provider
    /// </summary>
    public void AddRegisteredHealthChecks()
    {
        var registeredChecks = _serviceProvider.GetServices<IHealthCheck>();
        AddHealthChecks(registeredChecks);
    }

    /// <summary>
    /// Run all health checks
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Aggregated health report</returns>
    public async Task<HealthReport> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var entries = new Dictionary<string, HealthReportEntry>();

        _logger?.LogInformation("Starting health checks for {Count} services", _healthChecks.Count);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(_defaultTimeout);

        // Run all health checks in parallel
        var tasks = _healthChecks.Select(async healthCheck =>
        {
            var entryStopwatch = Stopwatch.StartNew();
            try
            {
                var result = await healthCheck.CheckHealthAsync(cts.Token);
                entryStopwatch.Stop();
                
                return new HealthReportEntry
                {
                    Name = healthCheck.Name,
                    Status = result.Status,
                    Description = result.Description,
                    Exception = result.Exception,
                    Data = result.Data,
                    Duration = result.Duration
                };
            }
            catch (OperationCanceledException)
            {
                entryStopwatch.Stop();
                _logger?.LogWarning("Health check {HealthCheckName} timed out", healthCheck.Name);
                
                return new HealthReportEntry
                {
                    Name = healthCheck.Name,
                    Status = HealthStatus.Unhealthy,
                    Description = $"Health check timed out after {_defaultTimeout.TotalSeconds} seconds",
                    Exception = new TimeoutException($"Health check timed out after {_defaultTimeout.TotalSeconds} seconds"),
                    Duration = entryStopwatch.Elapsed
                };
            }
            catch (Exception ex)
            {
                entryStopwatch.Stop();
                _logger?.LogError(ex, "Health check {HealthCheckName} failed with exception", healthCheck.Name);
                
                return new HealthReportEntry
                {
                    Name = healthCheck.Name,
                    Status = HealthStatus.Unhealthy,
                    Description = $"Health check failed: {ex.Message}",
                    Exception = ex,
                    Duration = entryStopwatch.Elapsed
                };
            }
        }).ToList();

        var results = await Task.WhenAll(tasks);
        
        foreach (var entry in results)
        {
            entries[entry.Name] = entry;
        }

        stopwatch.Stop();

        // Calculate overall status
        var overallStatus = CalculateOverallStatus(entries.Values);

        var report = new HealthReport(entries, overallStatus, stopwatch.Elapsed);
        
        _logger?.LogInformation("Health check completed. Overall status: {Status}, Duration: {Duration}ms", 
            overallStatus, stopwatch.ElapsedMilliseconds);

        return report;
    }

    /// <summary>
    /// Run health checks for specific services
    /// </summary>
    /// <param name="serviceNames">Names of services to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Filtered health report</returns>
    public async Task<HealthReport> CheckHealthAsync(IEnumerable<string> serviceNames, CancellationToken cancellationToken = default)
    {
        var filteredChecks = _healthChecks.Where(hc => serviceNames.Contains(hc.Name, StringComparer.OrdinalIgnoreCase)).ToList();
        
        if (!filteredChecks.Any())
        {
            return new HealthReport(
                new Dictionary<string, HealthReportEntry>(),
                HealthStatus.Healthy,
                TimeSpan.Zero);
        }

        var originalChecks = _healthChecks.ToList();
        try
        {
            _healthChecks.Clear();
            _healthChecks.AddRange(filteredChecks);
            return await CheckHealthAsync(cancellationToken);
        }
        finally
        {
            _healthChecks.Clear();
            _healthChecks.AddRange(originalChecks);
        }
    }

    /// <summary>
    /// Get a summary of the last health check
    /// </summary>
    /// <param name="report">Health report</param>
    /// <returns>Summary string</returns>
    public string GetHealthSummary(HealthReport report)
    {
        var healthy = report.Entries.Count(e => e.Value.Status == HealthStatus.Healthy);
        var degraded = report.Entries.Count(e => e.Value.Status == HealthStatus.Degraded);
        var unhealthy = report.Entries.Count(e => e.Value.Status == HealthStatus.Unhealthy);

        return $"Health Check Summary - Status: {report.Status}, " +
               $"Services: {report.Entries.Count} total " +
               $"({healthy} healthy, {degraded} degraded, {unhealthy} unhealthy), " +
               $"Duration: {report.TotalDuration.TotalMilliseconds}ms";
    }

    private HealthStatus CalculateOverallStatus(IEnumerable<HealthReportEntry> entries)
    {
        var statuses = entries.Select(e => e.Status).ToList();

        if (statuses.Any(s => s == HealthStatus.Unhealthy))
        {
            return HealthStatus.Unhealthy;
        }
        
        if (statuses.Any(s => s == HealthStatus.Degraded))
        {
            return HealthStatus.Degraded;
        }

        return HealthStatus.Healthy;
    }

    /// <summary>
    /// Configure default health checks for the deployment toolkit
    /// </summary>
    /// <param name="rpcUrl">RPC URL to monitor</param>
    /// <param name="httpClient">HTTP client for RPC checks</param>
    public void ConfigureDefaultHealthChecks(string rpcUrl, HttpClient httpClient)
    {
        // Add deployment service health check
        AddHealthCheck(new DeploymentServiceHealthCheck(_serviceProvider, 
            _serviceProvider.GetService<ILogger<DeploymentServiceHealthCheck>>()));

        // Add RPC health check
        AddHealthCheck(new RpcHealthCheck(rpcUrl, httpClient,
            _serviceProvider.GetService<ILogger<RpcHealthCheck>>()));

        // Add wallet health check
        AddHealthCheck(new WalletHealthCheck(_serviceProvider,
            _serviceProvider.GetService<ILogger<WalletHealthCheck>>()));

        _logger?.LogInformation("Configured default health checks");
    }
}

/// <summary>
/// Health report containing results of all health checks
/// </summary>
public class HealthReport
{
    /// <summary>
    /// Health check entries by name
    /// </summary>
    public IReadOnlyDictionary<string, HealthReportEntry> Entries { get; }

    /// <summary>
    /// Overall health status
    /// </summary>
    public HealthStatus Status { get; }

    /// <summary>
    /// Total duration of all health checks
    /// </summary>
    public TimeSpan TotalDuration { get; }

    /// <summary>
    /// Initialize a new health report
    /// </summary>
    /// <param name="entries">Health check entries</param>
    /// <param name="status">Overall status</param>
    /// <param name="totalDuration">Total duration</param>
    public HealthReport(Dictionary<string, HealthReportEntry> entries, HealthStatus status, TimeSpan totalDuration)
    {
        Entries = entries;
        Status = status;
        TotalDuration = totalDuration;
    }
}

/// <summary>
/// Entry in a health report
/// </summary>
public class HealthReportEntry
{
    /// <summary>
    /// Name of the health check
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Health status
    /// </summary>
    public HealthStatus Status { get; set; }

    /// <summary>
    /// Description of the result
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Exception if the check failed
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// Additional data
    /// </summary>
    public Dictionary<string, object>? Data { get; set; }

    /// <summary>
    /// Duration of the health check
    /// </summary>
    public TimeSpan Duration { get; set; }
}