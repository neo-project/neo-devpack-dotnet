using System.Threading;
using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy.HealthChecks;

/// <summary>
/// Interface for health check implementations
/// </summary>
public interface IHealthCheck
{
    /// <summary>
    /// Perform the health check
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Health check result</returns>
    Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Health check result
/// </summary>
public class HealthCheckResult
{
    /// <summary>
    /// Health status
    /// </summary>
    public HealthStatus Status { get; set; }

    /// <summary>
    /// Description of the health check result
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Additional data about the health check
    /// </summary>
    public Dictionary<string, object> Data { get; set; } = new();

    /// <summary>
    /// Exception if health check failed
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// Duration of the health check
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Create a healthy result
    /// </summary>
    public static HealthCheckResult Healthy(string description = "Healthy", Dictionary<string, object>? data = null)
    {
        return new HealthCheckResult
        {
            Status = HealthStatus.Healthy,
            Description = description,
            Data = data ?? new Dictionary<string, object>()
        };
    }

    /// <summary>
    /// Create a degraded result
    /// </summary>
    public static HealthCheckResult Degraded(string description, Exception? exception = null, Dictionary<string, object>? data = null)
    {
        return new HealthCheckResult
        {
            Status = HealthStatus.Degraded,
            Description = description,
            Exception = exception,
            Data = data ?? new Dictionary<string, object>()
        };
    }

    /// <summary>
    /// Create an unhealthy result
    /// </summary>
    public static HealthCheckResult Unhealthy(string description, Exception? exception = null, Dictionary<string, object>? data = null)
    {
        return new HealthCheckResult
        {
            Status = HealthStatus.Unhealthy,
            Description = description,
            Exception = exception,
            Data = data ?? new Dictionary<string, object>()
        };
    }
}

/// <summary>
/// Health status
/// </summary>
public enum HealthStatus
{
    /// <summary>
    /// The component is healthy
    /// </summary>
    Healthy = 0,

    /// <summary>
    /// The component is degraded but functional
    /// </summary>
    Degraded = 1,

    /// <summary>
    /// The component is unhealthy
    /// </summary>
    Unhealthy = 2
}
