using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Neo.Network.RPC;
using Neo.SmartContract.Deploy.Models;

namespace Neo.SmartContract.Deploy.Shared;

/// <summary>
/// Factory for creating and managing RPC client instances with connection pooling
/// </summary>
public class RpcClientFactory : IRpcClientFactory
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RpcClientFactory> _logger;
    private readonly ConcurrentDictionary<string, RpcClient> _clientPool = new();
    private NetworkConfiguration? _networkConfig;
    private string _currentNetwork = "default";

    public RpcClientFactory(IConfiguration configuration, ILogger<RpcClientFactory> logger)
    {
        _configuration = configuration;
        _logger = logger;
        LoadNetworkConfiguration();
    }

    /// <inheritdoc/>
    public RpcClient CreateClient(string? networkName = null)
    {
        var network = networkName ?? _currentNetwork;
        var rpcUrl = GetRpcUrl(network);

        // Try to reuse existing client for the same URL
        if (_clientPool.TryGetValue(rpcUrl, out var existingClient))
        {
            _logger.LogDebug("Reusing existing RPC client for {RpcUrl}", rpcUrl);
            return existingClient;
        }

        // Create new client
        _logger.LogInformation("Creating new RPC client for {Network} at {RpcUrl}", network, rpcUrl);
        var client = new RpcClient(new Uri(rpcUrl));

        // Add to pool
        _clientPool.TryAdd(rpcUrl, client);

        return client;
    }

    /// <inheritdoc/>
    public string GetRpcUrl(string? networkName = null)
    {
        var network = networkName ?? _currentNetwork;

        // Check environment variable override first
        var envRpcUrl = Environment.GetEnvironmentVariable("Network__RpcUrl");
        if (!string.IsNullOrEmpty(envRpcUrl))
        {
            return envRpcUrl;
        }

        // Load from configuration
        if (_networkConfig == null)
        {
            LoadNetworkConfiguration();
        }

        // Check if network name matches configured network
        if (_networkConfig != null && _networkConfig.Network.Equals(network, StringComparison.OrdinalIgnoreCase))
        {
            return _networkConfig.RpcUrl;
        }

        // Check for direct RPC URL in configuration
        if (!string.IsNullOrEmpty(_networkConfig?.RpcUrl))
        {
            return _networkConfig.RpcUrl;
        }

        // Fallback to well-known networks
        return network.ToLowerInvariant() switch
        {
            "mainnet" => "https://rpc10.n3.nspcc.ru:10331",
            "testnet" => "https://rpc10.n3.neotracker.io:443",
            "local" or "private" => "http://localhost:50012",
            _ => throw new InvalidOperationException($"Unknown network '{network}' and no RPC URL configured")
        };
    }

    /// <inheritdoc/>
    public string GetNetworkName()
    {
        return _currentNetwork;
    }

    /// <summary>
    /// Set the current network
    /// </summary>
    /// <param name="networkName">Network name</param>
    public void SetNetwork(string networkName)
    {
        _currentNetwork = networkName;
        _logger.LogInformation("Current network set to: {Network}", networkName);
    }

    /// <summary>
    /// Clear the client pool
    /// </summary>
    public void ClearPool()
    {
        _clientPool.Clear();
        _logger.LogInformation("RPC client pool cleared");
    }

    private void LoadNetworkConfiguration()
    {
        try
        {
            _networkConfig = _configuration.GetSection("Network").Get<NetworkConfiguration>();
            if (_networkConfig == null)
            {
                _logger.LogWarning("No network configuration found in appsettings.json");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load network configuration");
        }
    }
}
