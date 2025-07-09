using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Neo.SmartContract.Deploy.Monitoring;

/// <summary>
/// Performance monitor for tracking deployment toolkit operations
/// </summary>
public class PerformanceMonitor
{
    private readonly ILogger<PerformanceMonitor>? _logger;
    private readonly ConcurrentDictionary<string, PerformanceCounter> _counters;
    private readonly ConcurrentDictionary<string, List<PerformanceSample>> _samples;
    private readonly Timer _aggregationTimer;
    private readonly int _maxSamplesPerCounter;
    private readonly TimeSpan _aggregationInterval;

    /// <summary>
    /// Initialize a new instance of PerformanceMonitor
    /// </summary>
    /// <param name="logger">Optional logger</param>
    /// <param name="maxSamplesPerCounter">Maximum samples to keep per counter (default: 1000)</param>
    /// <param name="aggregationInterval">Interval for aggregating metrics (default: 1 minute)</param>
    public PerformanceMonitor(ILogger<PerformanceMonitor>? logger = null, int maxSamplesPerCounter = 1000, TimeSpan? aggregationInterval = null)
    {
        _logger = logger;
        _counters = new ConcurrentDictionary<string, PerformanceCounter>();
        _samples = new ConcurrentDictionary<string, List<PerformanceSample>>();
        _maxSamplesPerCounter = maxSamplesPerCounter;
        _aggregationInterval = aggregationInterval ?? TimeSpan.FromMinutes(1);
        
        _aggregationTimer = new Timer(AggregateMetrics, null, _aggregationInterval, _aggregationInterval);
    }

    /// <summary>
    /// Track a performance metric
    /// </summary>
    /// <param name="category">Category of the metric</param>
    /// <param name="operation">Operation name</param>
    /// <param name="value">Value to track</param>
    /// <param name="unit">Unit of measurement</param>
    public void Track(string category, string operation, double value, string unit = "ms")
    {
        var key = $"{category}.{operation}";
        var sample = new PerformanceSample
        {
            Timestamp = DateTime.UtcNow,
            Value = value,
            Unit = unit
        };

        // Add sample
        _samples.AddOrUpdate(key,
            new List<PerformanceSample> { sample },
            (_, list) =>
            {
                lock (list)
                {
                    list.Add(sample);
                    // Keep only recent samples
                    if (list.Count > _maxSamplesPerCounter)
                    {
                        list.RemoveRange(0, list.Count - _maxSamplesPerCounter);
                    }
                }
                return list;
            });

        // Update counter
        _counters.AddOrUpdate(key,
            new PerformanceCounter { Count = 1, Total = value, Min = value, Max = value, LastValue = value },
            (_, counter) =>
            {
                lock (counter)
                {
                    counter.Count++;
                    counter.Total += value;
                    counter.Min = Math.Min(counter.Min, value);
                    counter.Max = Math.Max(counter.Max, value);
                    counter.LastValue = value;
                }
                return counter;
            });
    }

    /// <summary>
    /// Start tracking an operation
    /// </summary>
    /// <param name="category">Category of the operation</param>
    /// <param name="operation">Operation name</param>
    /// <returns>Performance tracker</returns>
    public PerformanceTracker StartTracking(string category, string operation)
    {
        return new PerformanceTracker(this, category, operation);
    }

    /// <summary>
    /// Get performance statistics
    /// </summary>
    /// <param name="category">Optional category filter</param>
    /// <returns>Performance statistics</returns>
    public Dictionary<string, PerformanceStatistics> GetStatistics(string? category = null)
    {
        var result = new Dictionary<string, PerformanceStatistics>();
        var keys = _counters.Keys.Where(k => category == null || k.StartsWith($"{category}."));

        foreach (var key in keys)
        {
            if (_counters.TryGetValue(key, out var counter) && _samples.TryGetValue(key, out var samples))
            {
                List<PerformanceSample> samplesCopy;
                lock (samples)
                {
                    samplesCopy = samples.ToList();
                }

                var stats = new PerformanceStatistics
                {
                    Key = key,
                    Count = counter.Count,
                    Average = counter.Count > 0 ? counter.Total / counter.Count : 0,
                    Min = counter.Min,
                    Max = counter.Max,
                    Total = counter.Total,
                    LastValue = counter.LastValue,
                    Percentiles = CalculatePercentiles(samplesCopy.Select(s => s.Value).ToList()),
                    RecentSamples = samplesCopy.TakeLast(10).ToList()
                };

                result[key] = stats;
            }
        }

        return result;
    }

    /// <summary>
    /// Get performance report
    /// </summary>
    /// <returns>Formatted performance report</returns>
    public string GetReport()
    {
        var stats = GetStatistics();
        var report = new List<string>
        {
            "=== Performance Report ===",
            $"Generated at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC",
            ""
        };

        var categories = stats.Keys
            .Select(k => k.Split('.')[0])
            .Distinct()
            .OrderBy(c => c);

        foreach (var category in categories)
        {
            report.Add($"Category: {category}");
            report.Add(new string('-', 50));

            var categoryStats = stats.Where(kvp => kvp.Key.StartsWith($"{category}."));
            foreach (var (key, stat) in categoryStats)
            {
                var operation = key.Substring(category.Length + 1);
                report.Add($"  {operation}:");
                report.Add($"    Count: {stat.Count:N0}");
                report.Add($"    Average: {stat.Average:F2} ms");
                report.Add($"    Min: {stat.Min:F2} ms");
                report.Add($"    Max: {stat.Max:F2} ms");
                report.Add($"    P50: {stat.Percentiles.P50:F2} ms");
                report.Add($"    P95: {stat.Percentiles.P95:F2} ms");
                report.Add($"    P99: {stat.Percentiles.P99:F2} ms");
                report.Add("");
            }
        }

        return string.Join(Environment.NewLine, report);
    }

    /// <summary>
    /// Reset all counters
    /// </summary>
    public void Reset()
    {
        _counters.Clear();
        _samples.Clear();
        _logger?.LogInformation("Performance counters reset");
    }

    /// <summary>
    /// Export metrics to dictionary
    /// </summary>
    /// <returns>Metrics dictionary</returns>
    public Dictionary<string, object> Export()
    {
        var result = new Dictionary<string, object>();
        var stats = GetStatistics();

        foreach (var (key, stat) in stats)
        {
            result[$"{key}.count"] = stat.Count;
            result[$"{key}.average"] = stat.Average;
            result[$"{key}.min"] = stat.Min;
            result[$"{key}.max"] = stat.Max;
            result[$"{key}.p50"] = stat.Percentiles.P50;
            result[$"{key}.p95"] = stat.Percentiles.P95;
            result[$"{key}.p99"] = stat.Percentiles.P99;
        }

        return result;
    }

    private void AggregateMetrics(object? state)
    {
        try
        {
            var stats = GetStatistics();
            
            foreach (var (key, stat) in stats)
            {
                if (stat.Count > 0)
                {
                    _logger?.LogDebug("Performance metric {Key}: Count={Count}, Avg={Average:F2}ms, P95={P95:F2}ms", 
                        key, stat.Count, stat.Average, stat.Percentiles.P95);
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error aggregating performance metrics");
        }
    }

    private Percentiles CalculatePercentiles(List<double> values)
    {
        if (values.Count == 0)
        {
            return new Percentiles();
        }

        values.Sort();
        
        return new Percentiles
        {
            P50 = GetPercentile(values, 0.50),
            P75 = GetPercentile(values, 0.75),
            P90 = GetPercentile(values, 0.90),
            P95 = GetPercentile(values, 0.95),
            P99 = GetPercentile(values, 0.99)
        };
    }

    private double GetPercentile(List<double> sortedValues, double percentile)
    {
        var index = (int)Math.Ceiling(percentile * sortedValues.Count) - 1;
        return sortedValues[Math.Max(0, Math.Min(index, sortedValues.Count - 1))];
    }

    /// <summary>
    /// Dispose of resources
    /// </summary>
    public void Dispose()
    {
        _aggregationTimer?.Dispose();
    }
}

/// <summary>
/// Performance counter
/// </summary>
internal class PerformanceCounter
{
    public long Count { get; set; }
    public double Total { get; set; }
    public double Min { get; set; } = double.MaxValue;
    public double Max { get; set; } = double.MinValue;
    public double LastValue { get; set; }
}

/// <summary>
/// Performance sample
/// </summary>
public class PerformanceSample
{
    /// <summary>
    /// Timestamp of the sample
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Value of the sample
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Unit of measurement
    /// </summary>
    public string Unit { get; set; } = "ms";
}

/// <summary>
/// Performance statistics
/// </summary>
public class PerformanceStatistics
{
    /// <summary>
    /// Metric key
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Number of samples
    /// </summary>
    public long Count { get; set; }

    /// <summary>
    /// Average value
    /// </summary>
    public double Average { get; set; }

    /// <summary>
    /// Minimum value
    /// </summary>
    public double Min { get; set; }

    /// <summary>
    /// Maximum value
    /// </summary>
    public double Max { get; set; }

    /// <summary>
    /// Total sum
    /// </summary>
    public double Total { get; set; }

    /// <summary>
    /// Last recorded value
    /// </summary>
    public double LastValue { get; set; }

    /// <summary>
    /// Percentile values
    /// </summary>
    public Percentiles Percentiles { get; set; } = new();

    /// <summary>
    /// Recent samples
    /// </summary>
    public List<PerformanceSample> RecentSamples { get; set; } = new();
}

/// <summary>
/// Percentile values
/// </summary>
public class Percentiles
{
    /// <summary>
    /// 50th percentile (median)
    /// </summary>
    public double P50 { get; set; }

    /// <summary>
    /// 75th percentile
    /// </summary>
    public double P75 { get; set; }

    /// <summary>
    /// 90th percentile
    /// </summary>
    public double P90 { get; set; }

    /// <summary>
    /// 95th percentile
    /// </summary>
    public double P95 { get; set; }

    /// <summary>
    /// 99th percentile
    /// </summary>
    public double P99 { get; set; }
}

/// <summary>
/// Performance tracker for automatic timing
/// </summary>
public class PerformanceTracker : IDisposable
{
    private readonly PerformanceMonitor _monitor;
    private readonly string _category;
    private readonly string _operation;
    private readonly Stopwatch _stopwatch;
    private bool _disposed;

    internal PerformanceTracker(PerformanceMonitor monitor, string category, string operation)
    {
        _monitor = monitor;
        _category = category;
        _operation = operation;
        _stopwatch = Stopwatch.StartNew();
    }

    /// <summary>
    /// Stop tracking and record the metric
    /// </summary>
    public void Stop()
    {
        if (_disposed) return;
        
        _stopwatch.Stop();
        _monitor.Track(_category, _operation, _stopwatch.Elapsed.TotalMilliseconds);
        _disposed = true;
    }

    /// <summary>
    /// Dispose (automatically stops tracking)
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            Stop();
        }
    }
}