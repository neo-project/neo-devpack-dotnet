using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Neo.SmartContract.Deploy.Monitoring;

/// <summary>
/// Metrics collector for deployment operations
/// </summary>
public class DeploymentMetrics
{
    private readonly ConcurrentDictionary<string, MetricData> _metrics;
    private readonly ConcurrentDictionary<string, DeploymentRecord> _deployments;
    private readonly Timer _cleanupTimer;
    private readonly TimeSpan _retentionPeriod;

    /// <summary>
    /// Initialize a new instance of DeploymentMetrics
    /// </summary>
    /// <param name="retentionPeriod">How long to keep metrics (default: 24 hours)</param>
    public DeploymentMetrics(TimeSpan? retentionPeriod = null)
    {
        _metrics = new ConcurrentDictionary<string, MetricData>();
        _deployments = new ConcurrentDictionary<string, DeploymentRecord>();
        _retentionPeriod = retentionPeriod ?? TimeSpan.FromHours(24);
        
        // Set up cleanup timer to run every hour
        _cleanupTimer = new Timer(CleanupOldMetrics, null, TimeSpan.FromHours(1), TimeSpan.FromHours(1));
    }

    /// <summary>
    /// Record a deployment attempt
    /// </summary>
    /// <param name="contractName">Name of the contract</param>
    /// <param name="contractHash">Hash of the deployed contract</param>
    /// <param name="success">Whether the deployment was successful</param>
    /// <param name="duration">Duration of the deployment</param>
    /// <param name="error">Error message if failed</param>
    /// <returns>Deployment ID</returns>
    public string RecordDeployment(string contractName, string contractHash, bool success, TimeSpan duration, string? error = null)
    {
        var deploymentId = Guid.NewGuid().ToString();
        var record = new DeploymentRecord
        {
            Id = deploymentId,
            ContractName = contractName,
            ContractHash = contractHash,
            Success = success,
            Duration = duration,
            Timestamp = DateTime.UtcNow,
            Error = error
        };

        _deployments[deploymentId] = record;

        // Update metrics
        var metricKey = $"deployment_{contractName}";
        _metrics.AddOrUpdate(metricKey, 
            new MetricData { Count = 1, SuccessCount = success ? 1 : 0, TotalDuration = duration },
            (_, existing) =>
            {
                existing.Count++;
                if (success) existing.SuccessCount++;
                existing.TotalDuration += duration;
                return existing;
            });

        return deploymentId;
    }

    /// <summary>
    /// Record a contract invocation
    /// </summary>
    /// <param name="contractHash">Contract hash</param>
    /// <param name="method">Method name</param>
    /// <param name="success">Whether the invocation was successful</param>
    /// <param name="duration">Duration of the invocation</param>
    public void RecordInvocation(string contractHash, string method, bool success, TimeSpan duration)
    {
        var metricKey = $"invocation_{contractHash}_{method}";
        _metrics.AddOrUpdate(metricKey,
            new MetricData { Count = 1, SuccessCount = success ? 1 : 0, TotalDuration = duration },
            (_, existing) =>
            {
                existing.Count++;
                if (success) existing.SuccessCount++;
                existing.TotalDuration += duration;
                return existing;
            });
    }

    /// <summary>
    /// Record a custom metric
    /// </summary>
    /// <param name="category">Metric category</param>
    /// <param name="name">Metric name</param>
    /// <param name="value">Metric value</param>
    public void RecordMetric(string category, string name, double value)
    {
        var metricKey = $"{category}_{name}";
        _metrics.AddOrUpdate(metricKey,
            new MetricData { Count = 1, Value = value, TotalValue = value },
            (_, existing) =>
            {
                existing.Count++;
                existing.Value = value;
                existing.TotalValue += value;
                return existing;
            });
    }

    /// <summary>
    /// Start a timed operation
    /// </summary>
    /// <param name="operationName">Name of the operation</param>
    /// <returns>Timed operation handle</returns>
    public TimedOperation StartTimer(string operationName)
    {
        return new TimedOperation(this, operationName);
    }

    /// <summary>
    /// Get deployment statistics
    /// </summary>
    /// <returns>Deployment statistics</returns>
    public DeploymentStatistics GetStatistics()
    {
        var deploymentMetrics = _metrics.Where(kvp => kvp.Key.StartsWith("deployment_")).ToList();
        var invocationMetrics = _metrics.Where(kvp => kvp.Key.StartsWith("invocation_")).ToList();

        var stats = new DeploymentStatistics
        {
            TotalDeployments = deploymentMetrics.Sum(m => m.Value.Count),
            SuccessfulDeployments = deploymentMetrics.Sum(m => m.Value.SuccessCount),
            TotalInvocations = invocationMetrics.Sum(m => m.Value.Count),
            SuccessfulInvocations = invocationMetrics.Sum(m => m.Value.SuccessCount),
            AverageDeploymentTime = deploymentMetrics.Any() 
                ? TimeSpan.FromMilliseconds(deploymentMetrics.Average(m => m.Value.AverageDuration.TotalMilliseconds))
                : TimeSpan.Zero,
            AverageInvocationTime = invocationMetrics.Any()
                ? TimeSpan.FromMilliseconds(invocationMetrics.Average(m => m.Value.AverageDuration.TotalMilliseconds))
                : TimeSpan.Zero,
            RecentDeployments = _deployments.Values
                .OrderByDescending(d => d.Timestamp)
                .Take(10)
                .ToList()
        };

        return stats;
    }

    /// <summary>
    /// Get metrics for a specific category
    /// </summary>
    /// <param name="category">Category name</param>
    /// <returns>Metrics for the category</returns>
    public Dictionary<string, MetricData> GetMetricsByCategory(string category)
    {
        return _metrics
            .Where(kvp => kvp.Key.StartsWith($"{category}_"))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    /// Get deployment history
    /// </summary>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Deployment records</returns>
    public List<DeploymentRecord> GetDeploymentHistory(int limit = 100)
    {
        return _deployments.Values
            .OrderByDescending(d => d.Timestamp)
            .Take(limit)
            .ToList();
    }

    /// <summary>
    /// Clear all metrics
    /// </summary>
    public void Clear()
    {
        _metrics.Clear();
        _deployments.Clear();
    }

    private void CleanupOldMetrics(object? state)
    {
        var cutoffTime = DateTime.UtcNow - _retentionPeriod;
        
        // Remove old deployments
        var oldDeployments = _deployments
            .Where(kvp => kvp.Value.Timestamp < cutoffTime)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in oldDeployments)
        {
            _deployments.TryRemove(key, out _);
        }
    }

    /// <summary>
    /// Dispose of resources
    /// </summary>
    public void Dispose()
    {
        _cleanupTimer?.Dispose();
    }
}

/// <summary>
/// Metric data
/// </summary>
public class MetricData
{
    /// <summary>
    /// Number of occurrences
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Number of successful occurrences
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Current value
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Total accumulated value
    /// </summary>
    public double TotalValue { get; set; }

    /// <summary>
    /// Total duration
    /// </summary>
    public TimeSpan TotalDuration { get; set; }

    /// <summary>
    /// Average duration
    /// </summary>
    public TimeSpan AverageDuration => Count > 0 ? TimeSpan.FromMilliseconds(TotalDuration.TotalMilliseconds / Count) : TimeSpan.Zero;

    /// <summary>
    /// Success rate
    /// </summary>
    public double SuccessRate => Count > 0 ? (double)SuccessCount / Count : 0;

    /// <summary>
    /// Average value
    /// </summary>
    public double AverageValue => Count > 0 ? TotalValue / Count : 0;
}

/// <summary>
/// Deployment record
/// </summary>
public class DeploymentRecord
{
    /// <summary>
    /// Unique deployment ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Contract name
    /// </summary>
    public string ContractName { get; set; } = string.Empty;

    /// <summary>
    /// Contract hash
    /// </summary>
    public string ContractHash { get; set; } = string.Empty;

    /// <summary>
    /// Whether the deployment was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Duration of the deployment
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Timestamp of the deployment
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Error message if failed
    /// </summary>
    public string? Error { get; set; }
}

/// <summary>
/// Deployment statistics
/// </summary>
public class DeploymentStatistics
{
    /// <summary>
    /// Total number of deployments
    /// </summary>
    public int TotalDeployments { get; set; }

    /// <summary>
    /// Number of successful deployments
    /// </summary>
    public int SuccessfulDeployments { get; set; }

    /// <summary>
    /// Total number of invocations
    /// </summary>
    public int TotalInvocations { get; set; }

    /// <summary>
    /// Number of successful invocations
    /// </summary>
    public int SuccessfulInvocations { get; set; }

    /// <summary>
    /// Average deployment time
    /// </summary>
    public TimeSpan AverageDeploymentTime { get; set; }

    /// <summary>
    /// Average invocation time
    /// </summary>
    public TimeSpan AverageInvocationTime { get; set; }

    /// <summary>
    /// Recent deployment records
    /// </summary>
    public List<DeploymentRecord> RecentDeployments { get; set; } = new();

    /// <summary>
    /// Deployment success rate
    /// </summary>
    public double DeploymentSuccessRate => TotalDeployments > 0 ? (double)SuccessfulDeployments / TotalDeployments : 0;

    /// <summary>
    /// Invocation success rate
    /// </summary>
    public double InvocationSuccessRate => TotalInvocations > 0 ? (double)SuccessfulInvocations / TotalInvocations : 0;
}

/// <summary>
/// Timed operation for automatic metric recording
/// </summary>
public class TimedOperation : IDisposable
{
    private readonly DeploymentMetrics _metrics;
    private readonly string _operationName;
    private readonly Stopwatch _stopwatch;
    private bool _disposed;

    internal TimedOperation(DeploymentMetrics metrics, string operationName)
    {
        _metrics = metrics;
        _operationName = operationName;
        _stopwatch = Stopwatch.StartNew();
    }

    /// <summary>
    /// Complete the operation successfully
    /// </summary>
    public void Success()
    {
        if (_disposed) return;
        
        _stopwatch.Stop();
        _metrics.RecordMetric("operation", _operationName, _stopwatch.ElapsedMilliseconds);
        _disposed = true;
    }

    /// <summary>
    /// Complete the operation with failure
    /// </summary>
    public void Failure()
    {
        if (_disposed) return;
        
        _stopwatch.Stop();
        _metrics.RecordMetric("operation_failed", _operationName, _stopwatch.ElapsedMilliseconds);
        _disposed = true;
    }

    /// <summary>
    /// Dispose of the operation (records as success if not already completed)
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            Success();
        }
    }
}