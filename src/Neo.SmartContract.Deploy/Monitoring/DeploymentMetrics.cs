using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Neo.SmartContract.Deploy.Monitoring;

/// <summary>
/// Deployment metrics collector
/// </summary>
public class DeploymentMetrics
{
    private long _totalDeployments;
    private long _successfulDeployments;
    private long _failedDeployments;
    private long _dryRunDeployments;
    private long _totalGasConsumed;
    private readonly ConcurrentDictionary<string, long> _deploymentsByNetwork = new();
    private readonly ConcurrentDictionary<string, ContractMetrics> _contractMetrics = new();

    /// <summary>
    /// Record a deployment attempt
    /// </summary>
    public void RecordDeployment(string contractName, string network, bool success, bool isDryRun, long gasConsumed)
    {
        Interlocked.Increment(ref _totalDeployments);

        if (isDryRun)
        {
            Interlocked.Increment(ref _dryRunDeployments);
        }
        else if (success)
        {
            Interlocked.Increment(ref _successfulDeployments);
            Interlocked.Add(ref _totalGasConsumed, gasConsumed);
        }
        else
        {
            Interlocked.Increment(ref _failedDeployments);
        }

        // Track by network
        _deploymentsByNetwork.AddOrUpdate(network, 1, (_, count) => count + 1);

        // Track by contract
        _contractMetrics.AddOrUpdate(contractName,
            new ContractMetrics { Name = contractName, Deployments = 1, GasConsumed = gasConsumed },
            (_, metrics) =>
            {
                metrics.Deployments++;
                metrics.GasConsumed += gasConsumed;
                if (!success) metrics.Failures++;
                return metrics;
            });
    }

    /// <summary>
    /// Get current metrics snapshot
    /// </summary>
    public MetricsSnapshot GetSnapshot()
    {
        return new MetricsSnapshot
        {
            Timestamp = DateTime.UtcNow,
            TotalDeployments = _totalDeployments,
            SuccessfulDeployments = _successfulDeployments,
            FailedDeployments = _failedDeployments,
            DryRunDeployments = _dryRunDeployments,
            TotalGasConsumed = _totalGasConsumed,
            DeploymentsByNetwork = new Dictionary<string, long>(_deploymentsByNetwork),
            ContractMetrics = _contractMetrics.Values.ToList()
        };
    }

    /// <summary>
    /// Reset all metrics
    /// </summary>
    public void Reset()
    {
        _totalDeployments = 0;
        _successfulDeployments = 0;
        _failedDeployments = 0;
        _dryRunDeployments = 0;
        _totalGasConsumed = 0;
        _deploymentsByNetwork.Clear();
        _contractMetrics.Clear();
    }
}

/// <summary>
/// Metrics for a specific contract
/// </summary>
public class ContractMetrics
{
    public string Name { get; set; } = string.Empty;
    public long Deployments { get; set; }
    public long Failures { get; set; }
    public long GasConsumed { get; set; }
    public DateTime LastDeployment { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Snapshot of deployment metrics at a point in time
/// </summary>
public class MetricsSnapshot
{
    public DateTime Timestamp { get; set; }
    public long TotalDeployments { get; set; }
    public long SuccessfulDeployments { get; set; }
    public long FailedDeployments { get; set; }
    public long DryRunDeployments { get; set; }
    public long TotalGasConsumed { get; set; }
    public Dictionary<string, long> DeploymentsByNetwork { get; set; } = new();
    public List<ContractMetrics> ContractMetrics { get; set; } = new();

    public double SuccessRate => TotalDeployments > 0 ?
        (double)SuccessfulDeployments / (TotalDeployments - DryRunDeployments) * 100 : 0;

    public double AverageGasPerDeployment => SuccessfulDeployments > 0 ?
        (double)TotalGasConsumed / SuccessfulDeployments : 0;
}
