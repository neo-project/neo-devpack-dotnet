using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neo.SmartContract.Deploy.HealthChecks.Interfaces;

namespace Neo.SmartContract.Deploy.HealthChecks;

/// <summary>
/// Health check for RPC endpoints
/// </summary>
public class RpcHealthCheck : IHealthCheck
{
    private readonly string _rpcUrl;
    private readonly HttpClient _httpClient;
    private readonly ILogger<RpcHealthCheck>? _logger;
    private readonly TimeSpan _timeout;

    /// <inheritdoc />
    public string Name => $"RPC_{new Uri(_rpcUrl).Host}";

    /// <summary>
    /// Initialize a new instance of RpcHealthCheck
    /// </summary>
    /// <param name="rpcUrl">RPC URL to check</param>
    /// <param name="httpClient">HTTP client</param>
    /// <param name="logger">Optional logger</param>
    /// <param name="timeout">Timeout for health check (default: 10 seconds)</param>
    public RpcHealthCheck(string rpcUrl, HttpClient httpClient, ILogger<RpcHealthCheck>? logger = null, TimeSpan? timeout = null)
    {
        _rpcUrl = rpcUrl ?? throw new ArgumentNullException(nameof(rpcUrl));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger;
        _timeout = timeout ?? TimeSpan.FromSeconds(10);
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var data = new Dictionary<string, object>
        {
            ["RpcUrl"] = _rpcUrl
        };

        try
        {
            _logger?.LogDebug("Starting RPC health check for: {RpcUrl}", _rpcUrl);

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(_timeout);

            // Test basic RPC connectivity with getversion call
            var versionInfo = await GetVersionAsync(cts.Token);
            data["Version"] = versionInfo;

            // Test block height
            var blockHeight = await GetBlockCountAsync(cts.Token);
            data["BlockHeight"] = blockHeight;

            // Test network info
            var networkInfo = await GetNetworkInfoAsync(cts.Token);
            data["Network"] = networkInfo;

            stopwatch.Stop();

            // Check if block height is reasonable
            if (blockHeight < 1)
            {
                var result = HealthCheckResult.Unhealthy(
                    $"RPC endpoint {_rpcUrl} returned invalid block height: {blockHeight}",
                    data: data);
                result.Duration = stopwatch.Elapsed;
                return result;
            }

            var healthyResult = HealthCheckResult.Healthy(
                $"RPC endpoint {_rpcUrl} is responding correctly",
                data);
            healthyResult.Duration = stopwatch.Elapsed;
            return healthyResult;
        }
        catch (OperationCanceledException)
        {
            _logger?.LogWarning("RPC health check timed out for: {RpcUrl}", _rpcUrl);
            stopwatch.Stop();
            
            var result = HealthCheckResult.Unhealthy(
                $"RPC endpoint {_rpcUrl} timed out after {_timeout.TotalSeconds} seconds",
                new TimeoutException($"Health check timed out after {_timeout.TotalSeconds} seconds"),
                data);
            result.Duration = stopwatch.Elapsed;
            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogWarning(ex, "RPC health check HTTP request failed for: {RpcUrl}", _rpcUrl);
            stopwatch.Stop();
            
            var result = HealthCheckResult.Unhealthy(
                $"RPC endpoint {_rpcUrl} is not reachable",
                ex,
                data);
            result.Duration = stopwatch.Elapsed;
            return result;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "RPC health check failed for: {RpcUrl}", _rpcUrl);
            stopwatch.Stop();
            
            var result = HealthCheckResult.Unhealthy(
                $"RPC endpoint {_rpcUrl} health check failed",
                ex,
                data);
            result.Duration = stopwatch.Elapsed;
            return result;
        }
    }

    private async Task<Dictionary<string, object>> GetVersionAsync(CancellationToken cancellationToken)
    {
        var response = await SendRpcRequestAsync("getversion", cancellationToken);
        
        if (response.TryGetProperty("tcpport", out var tcpPort))
        {
            return new Dictionary<string, object>
            {
                ["TcpPort"] = tcpPort.GetInt32(),
                ["UserAgent"] = response.GetProperty("useragent").GetString() ?? "unknown",
                ["Nonce"] = response.GetProperty("nonce").GetInt64(),
                ["Protocol"] = response.TryGetProperty("protocol", out var protocol) ? 
                    new
                    {
                        Network = protocol.GetProperty("network").GetInt32(),
                        ValidatorsCount = protocol.GetProperty("validatorscount").GetInt32(),
                        MillisecondsPerBlock = protocol.GetProperty("msperblock").GetInt32(),
                        MaxTracableBlocks = protocol.GetProperty("maxtraceableblocks").GetInt32()
                    } : null
            };
        }

        return new Dictionary<string, object> { ["Raw"] = response.ToString() };
    }

    private async Task<long> GetBlockCountAsync(CancellationToken cancellationToken)
    {
        var response = await SendRpcRequestAsync("getblockcount", cancellationToken);
        return response.GetInt64();
    }

    private async Task<Dictionary<string, object>> GetNetworkInfoAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await SendRpcRequestAsync("getconnectioncount", cancellationToken);
            var connectionCount = response.GetInt32();

            return new Dictionary<string, object>
            {
                ["ConnectionCount"] = connectionCount,
                ["IsConnected"] = connectionCount > 0
            };
        }
        catch
        {
            return new Dictionary<string, object>
            {
                ["ConnectionCount"] = 0,
                ["IsConnected"] = false
            };
        }
    }

    private async Task<JsonElement> SendRpcRequestAsync(string method, CancellationToken cancellationToken, params object[] parameters)
    {
        var request = new
        {
            jsonrpc = "2.0",
            method = method,
            @params = parameters,
            id = 1
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_rpcUrl, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var responseDoc = JsonDocument.Parse(responseJson);

        if (responseDoc.RootElement.TryGetProperty("error", out var error))
        {
            var errorMessage = error.GetProperty("message").GetString();
            throw new InvalidOperationException($"RPC error: {errorMessage}");
        }

        return responseDoc.RootElement.GetProperty("result");
    }

    /// <summary>
    /// Perform a comprehensive RPC health check
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Comprehensive health check result</returns>
    public async Task<HealthCheckResult> CheckHealthComprehensiveAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var data = new Dictionary<string, object>
        {
            ["RpcUrl"] = _rpcUrl
        };

        try
        {
            // Basic health check first
            var basicResult = await CheckHealthAsync(cancellationToken);
            if (basicResult.Status == HealthStatus.Unhealthy)
            {
                return basicResult;
            }

            // Additional checks
            data.Add("BasicCheck", basicResult.Data);

            // Check state service
            var stateServiceHealthy = await CheckStateServiceAsync(cancellationToken);
            data["StateService"] = stateServiceHealthy ? "Healthy" : "Unhealthy";

            // Check response times
            var responseTimes = await MeasureResponseTimesAsync(cancellationToken);
            data["ResponseTimes"] = responseTimes;

            stopwatch.Stop();

            // Evaluate overall health based on response times
            var avgResponseTime = responseTimes["Average"];
            if (avgResponseTime is double avg)
            {
                if (avg > 5000) // > 5 seconds average
                {
                    var result = HealthCheckResult.Degraded(
                        $"RPC endpoint {_rpcUrl} is slow (avg response time: {avg:F0}ms)",
                        data: data);
                    result.Duration = stopwatch.Elapsed;
                    return result;
                }
            }

            var healthyResult = HealthCheckResult.Healthy(
                $"RPC endpoint {_rpcUrl} is healthy and responsive",
                data);
            healthyResult.Duration = stopwatch.Elapsed;
            return healthyResult;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Comprehensive RPC health check failed for: {RpcUrl}", _rpcUrl);
            stopwatch.Stop();
            
            var result = HealthCheckResult.Unhealthy(
                $"Comprehensive RPC health check failed for {_rpcUrl}",
                ex,
                data);
            result.Duration = stopwatch.Elapsed;
            return result;
        }
    }

    private async Task<bool> CheckStateServiceAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await SendRpcRequestAsync("getstateroot", new object[] { 0 }, cancellationToken);
            return response.ValueKind != JsonValueKind.Null;
        }
        catch
        {
            return false;
        }
    }

    private async Task<Dictionary<string, object>> MeasureResponseTimesAsync(CancellationToken cancellationToken)
    {
        var measurements = new List<long>();
        var methods = new[] { "getblockcount", "getversion", "getconnectioncount" };

        foreach (var method in methods)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                await SendRpcRequestAsync(method, cancellationToken);
                sw.Stop();
                measurements.Add(sw.ElapsedMilliseconds);
            }
            catch
            {
                // Ignore individual failures in timing measurements
            }
        }

        if (measurements.Count == 0)
        {
            return new Dictionary<string, object>
            {
                ["Error"] = "No measurements could be taken"
            };
        }

        return new Dictionary<string, object>
        {
            ["Min"] = measurements.Min(),
            ["Max"] = measurements.Max(),
            ["Average"] = measurements.Average(),
            ["Count"] = measurements.Count
        };
    }
}