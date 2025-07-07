using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Neo.SmartContract.Deploy.Shared;

namespace Neo.SmartContract.Deploy.HealthChecks;

/// <summary>
/// Health check for RPC connectivity
/// </summary>
public class RpcHealthCheck : IHealthCheck
{
    private readonly IRpcClientFactory _rpcClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RpcHealthCheck> _logger;

    public RpcHealthCheck(
        IRpcClientFactory rpcClientFactory,
        IConfiguration configuration,
        ILogger<RpcHealthCheck> logger)
    {
        _rpcClientFactory = rpcClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var rpcUrl = _configuration["Network:RpcUrl"] ?? "http://localhost:10332";
            var client = _rpcClientFactory.CreateClient(rpcUrl);

            // Check basic connectivity by getting block count
            var blockCount = await client.GetBlockCountAsync();

            // Get version info for additional health data
            var version = await client.GetVersionAsync();

            stopwatch.Stop();

            var data = new Dictionary<string, object>
            {
                ["RpcUrl"] = rpcUrl,
                ["BlockHeight"] = blockCount,
                ["Network"] = version.Protocol.Network,
                ["UserAgent"] = version.UserAgent,
                ["ResponseTime"] = stopwatch.ElapsedMilliseconds
            };

            // Check if node is synced (simple heuristic: block should be recent)
            var timeSinceLastBlock = (DateTime.UtcNow - DateTimeOffset.FromUnixTimeSeconds(blockCount * 15).UtcDateTime);
            if (timeSinceLastBlock > TimeSpan.FromMinutes(5))
            {
                var degradedResult = HealthCheckResult.Degraded(
                    $"RPC node may not be fully synced. Last block was {timeSinceLastBlock.TotalMinutes:F1} minutes ago",
                    data: data);
                degradedResult.Duration = stopwatch.Elapsed;
                return degradedResult;
            }

            var healthyResult = HealthCheckResult.Healthy($"RPC endpoint is healthy. Block height: {blockCount}", data);
            healthyResult.Duration = stopwatch.Elapsed;
            return healthyResult;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "RPC health check failed");

            var unhealthyResult = HealthCheckResult.Unhealthy(
                $"RPC endpoint is not accessible: {ex.Message}",
                ex,
                new Dictionary<string, object>
                {
                    ["RpcUrl"] = _configuration["Network:RpcUrl"] ?? "unknown",
                    ["ErrorType"] = ex.GetType().Name
                });
            unhealthyResult.Duration = stopwatch.Elapsed;
            return unhealthyResult;
        }
    }
}
