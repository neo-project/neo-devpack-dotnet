using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy.HealthChecks.Interfaces;

/// <summary>
/// Interface for health check implementations
/// </summary>
public interface IHealthCheck
{
    /// <summary>
    /// Name of the health check
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Perform the health check
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Health check result</returns>
    Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of a health check
/// </summary>
public class HealthCheckResult
{
    /// <summary>
    /// Health status
    /// </summary>
    public HealthStatus Status { get; set; }

    /// <summary>
    /// Optional description of the health check result
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Exception if the health check failed
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// Additional data about the health check
    /// </summary>
    public Dictionary<string, object>? Data { get; set; }

    /// <summary>
    /// Duration of the health check
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Create a healthy result
    /// </summary>
    /// <param name="description">Optional description</param>
    /// <param name="data">Optional data</param>
    /// <returns>Healthy result</returns>
    public static HealthCheckResult Healthy(string? description = null, Dictionary<string, object>? data = null)
    {
        return new HealthCheckResult
        {
            Status = HealthStatus.Healthy,
            Description = description,
            Data = data
        };
    }

    /// <summary>
    /// Create a degraded result
    /// </summary>
    /// <param name="description">Optional description</param>
    /// <param name="exception">Optional exception</param>
    /// <param name="data">Optional data</param>
    /// <returns>Degraded result</returns>
    public static HealthCheckResult Degraded(string? description = null, Exception? exception = null, Dictionary<string, object>? data = null)
    {
        return new HealthCheckResult
        {
            Status = HealthStatus.Degraded,
            Description = description,
            Exception = exception,
            Data = data
        };
    }

    /// <summary>
    /// Create an unhealthy result
    /// </summary>
    /// <param name="description">Optional description</param>
    /// <param name="exception">Optional exception</param>
    /// <param name="data">Optional data</param>
    /// <returns>Unhealthy result</returns>
    public static HealthCheckResult Unhealthy(string? description = null, Exception? exception = null, Dictionary<string, object>? data = null)
    {
        return new HealthCheckResult
        {
            Status = HealthStatus.Unhealthy,
            Description = description,
            Exception = exception,
            Data = data
        };
    }
}

/// <summary>
/// Health status enumeration
/// </summary>
public enum HealthStatus
{
    /// <summary>
    /// The health check determined that the component is healthy
    /// </summary>
    Healthy = 0,

    /// <summary>
    /// The health check determined that the component is degraded
    /// </summary>
    Degraded = 1,

    /// <summary>
    /// The health check determined that the component is unhealthy
    /// </summary>
    Unhealthy = 2
}