using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neo.SmartContract.Deploy.Interfaces;

namespace Neo.SmartContract.Deploy.HealthChecks;

/// <summary>
/// Health check for wallet availability
/// </summary>
public class WalletHealthCheck : IHealthCheck
{
    private readonly IWalletManager _walletManager;
    private readonly ILogger<WalletHealthCheck> _logger;

    public WalletHealthCheck(
        IWalletManager walletManager,
        ILogger<WalletHealthCheck> logger)
    {
        _walletManager = walletManager;
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var data = new Dictionary<string, object>
            {
                ["WalletLoaded"] = _walletManager.IsWalletLoaded
            };

            if (_walletManager.IsWalletLoaded)
            {
                // Get account count
                var accounts = _walletManager.GetAccounts();
                var accountCount = 0;
                foreach (var _ in accounts)
                {
                    accountCount++;
                }

                data["AccountCount"] = accountCount;

                if (accountCount == 0)
                {
                    stopwatch.Stop();
                    var result = HealthCheckResult.Degraded(
                        "Wallet is loaded but has no accounts",
                        data: data);
                    result.Duration = stopwatch.Elapsed;
                    return Task.FromResult(result);
                }

                // Try to get default account
                try
                {
                    var defaultAccount = _walletManager.GetAccount();
                    data["DefaultAccount"] = defaultAccount.ToString();

                    stopwatch.Stop();
                    var result = HealthCheckResult.Healthy(
                        $"Wallet is loaded with {accountCount} account(s)",
                        data);
                    result.Duration = stopwatch.Elapsed;
                    return Task.FromResult(result);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    var result = HealthCheckResult.Degraded(
                        "Wallet is loaded but default account is not accessible",
                        ex,
                        data);
                    result.Duration = stopwatch.Elapsed;
                    return Task.FromResult(result);
                }
            }
            else
            {
                stopwatch.Stop();
                var result = HealthCheckResult.Degraded(
                    "No wallet is currently loaded",
                    data: data);
                result.Duration = stopwatch.Elapsed;
                return Task.FromResult(result);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Wallet health check failed");

            var result = HealthCheckResult.Unhealthy(
                $"Wallet health check failed: {ex.Message}",
                ex);
            result.Duration = stopwatch.Elapsed;
            return Task.FromResult(result);
        }
    }
}
