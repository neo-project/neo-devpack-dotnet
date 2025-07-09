using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.Wallets;
using Neo.SmartContract.Deploy.Extensions;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Services;

namespace Neo.SmartContract.Deploy;

/// <summary>
/// Simplified deployment toolkit for Neo smart contract deployment
/// Provides a fluent API for common deployment scenarios
/// </summary>
public class DeploymentToolkit : IDisposable
{
    private const string GAS_CONTRACT_HASH = "0xd2a4cff31913016155e38e474a2c06d08be276cf";
    private const decimal GAS_DECIMALS = 100_000_000m;
    private const string MAINNET_RPC_URL = "https://rpc10.n3.nspcc.ru:10331";
    private const string TESTNET_RPC_URL = "https://testnet1.neo.coz.io:443";
    private const string LOCAL_RPC_URL = "http://localhost:50012";
    private const string DEFAULT_RPC_URL = "http://localhost:10332";

    private readonly IConfiguration _configuration;
    private readonly ILogger<DeploymentToolkit>? _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IContractCompiler _compiler;
    private readonly IContractDeployer _deployer;
    private readonly IContractInvoker _invoker;
    private readonly IWalletManager _walletManager;
    private volatile string? _currentNetwork = null;
    private volatile string? _wifKey = null;
    private bool _disposed = false;

    /// <summary>
    /// Create a new DeploymentToolkit instance with automatic configuration
    /// </summary>
    /// <param name="configPath">Optional path to configuration file. Defaults to appsettings.json in current directory</param>
    public DeploymentToolkit(string? configPath = null)
    {
        // Build configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory());

        if (!string.IsNullOrEmpty(configPath))
        {
            builder.AddJsonFile(configPath, optional: false);
        }
        else
        {
            builder.AddJsonFile("appsettings.json", optional: true);
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            builder.AddJsonFile($"appsettings.{environment}.json", optional: true);
        }

        builder.AddEnvironmentVariables();
        _configuration = builder.Build();

        // Set up dependency injection
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddNeoContractDeploy(_configuration);
        
        _serviceProvider = services.BuildServiceProvider();
        _logger = _serviceProvider.GetService<ILogger<DeploymentToolkit>>();
        _compiler = _serviceProvider.GetRequiredService<IContractCompiler>();
        _deployer = _serviceProvider.GetRequiredService<IContractDeployer>();
        _invoker = _serviceProvider.GetRequiredService<IContractInvoker>();
        _walletManager = _serviceProvider.GetRequiredService<IWalletManager>();
    }

    /// <summary>
    /// Set the network to use (mainnet, testnet, or custom RPC URL)
    /// </summary>
    /// <param name="network">Network name or RPC URL</param>
    /// <returns>This instance for chaining</returns>
    /// <exception cref="ArgumentException">Thrown when network is invalid</exception>
    public DeploymentToolkit SetNetwork(string network)
    {
        if (string.IsNullOrWhiteSpace(network))
            throw new ArgumentException("Network cannot be null or empty", nameof(network));
        _currentNetwork = network.ToLowerInvariant();

        // Update configuration based on network
        switch (_currentNetwork)
        {
            case "mainnet":
                Environment.SetEnvironmentVariable("Network__RpcUrl", MAINNET_RPC_URL);
                Environment.SetEnvironmentVariable("Network__Network", "mainnet");
                break;

            case "testnet":
                Environment.SetEnvironmentVariable("Network__RpcUrl", TESTNET_RPC_URL);
                Environment.SetEnvironmentVariable("Network__Network", "testnet");
                break;

            case "local":
            case "private":
                Environment.SetEnvironmentVariable("Network__RpcUrl", LOCAL_RPC_URL);
                Environment.SetEnvironmentVariable("Network__Network", "private");
                break;

            default:
                // Assume it's a custom RPC URL
                if (network.StartsWith("http"))
                {
                    Environment.SetEnvironmentVariable("Network__RpcUrl", network);
                    Environment.SetEnvironmentVariable("Network__Network", "custom");
                }
                break;
        }

        _logger?.LogInformation("Network set to: {Network}", _currentNetwork);
        return this;
    }

    /// <summary>
    /// Set the WIF (Wallet Import Format) key for signing transactions
    /// </summary>
    /// <param name="wifKey">The WIF private key</param>
    /// <returns>The deployment toolkit instance for chaining</returns>
    /// <exception cref="ArgumentException">Thrown when WIF key is invalid</exception>
    public DeploymentToolkit SetWifKey(string wifKey)
    {
        if (string.IsNullOrWhiteSpace(wifKey))
            throw new ArgumentException("WIF key cannot be null or empty", nameof(wifKey));

        try
        {
            // Validate the WIF key by attempting to create a KeyPair
            var privateKey = Neo.Wallets.Wallet.GetPrivateKeyFromWIF(wifKey);
            var keyPair = new KeyPair(privateKey);
            var account = Neo.SmartContract.Contract.CreateSignatureContract(keyPair.PublicKey).ScriptHash;

            _wifKey = wifKey;

            _logger?.LogInformation("WIF key set for account: {Account}", account.ToAddress(Neo.ProtocolSettings.Default.AddressVersion));
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid WIF key: {ex.Message}", nameof(wifKey));
        }

        return this;
    }

    /// <summary>
    /// Deploy a contract from source code or project
    /// </summary>
    /// <param name="path">Path to contract project (.csproj) or source file</param>
    /// <param name="initParams">Optional initialization parameters</param>
    /// <returns>Deployment information</returns>
    public async Task<ContractDeploymentInfo> DeployAsync(string path, object[]? initParams = null)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty", nameof(path));

        _logger?.LogInformation("Deploying contract from: {Path}", path);

        // Compile the contract
        var contract = await _compiler.CompileAsync(path);

        // Create deployment options
        var options = CreateDeploymentOptions();

        // Deploy the contract
        return await _deployer.DeployAsync(contract, options, initParams);
    }

    /// <summary>
    /// Deploy a pre-compiled contract from NEF and manifest files
    /// </summary>
    /// <param name="nefPath">Path to NEF file</param>
    /// <param name="manifestPath">Path to manifest file</param>
    /// <param name="initParams">Optional initialization parameters</param>
    /// <returns>Deployment information</returns>
    public async Task<ContractDeploymentInfo> DeployArtifactsAsync(string nefPath, string manifestPath, object[]? initParams = null)
    {
        if (string.IsNullOrWhiteSpace(nefPath))
            throw new ArgumentException("NEF path cannot be null or empty", nameof(nefPath));

        if (string.IsNullOrWhiteSpace(manifestPath))
            throw new ArgumentException("Manifest path cannot be null or empty", nameof(manifestPath));

        _logger?.LogInformation("Deploying contract from artifacts - NEF: {NefPath}, Manifest: {ManifestPath}", nefPath, manifestPath);

        // Load the pre-compiled contract
        var contract = await _compiler.LoadContractAsync(nefPath, manifestPath);

        // Create deployment options
        var options = CreateDeploymentOptions();

        // Deploy the contract
        return await _deployer.DeployAsync(contract, options, initParams);
    }

    /// <summary>
    /// Call a contract method (read-only)
    /// </summary>
    /// <typeparam name="T">Return type</typeparam>
    /// <param name="contractHashOrAddress">Contract hash or address</param>
    /// <param name="method">Method name</param>
    /// <param name="args">Method arguments</param>
    /// <returns>Method return value</returns>
    public async Task<T> CallAsync<T>(string contractHashOrAddress, string method, params object[] args)
    {
        if (string.IsNullOrWhiteSpace(contractHashOrAddress))
            throw new ArgumentException("Contract hash or address cannot be null or empty", nameof(contractHashOrAddress));

        if (string.IsNullOrWhiteSpace(method))
            throw new ArgumentException("Method name cannot be null or empty", nameof(method));

        var contractHash = ParseContractHashOrAddress(contractHashOrAddress);
        var rpcUrl = GetCurrentRpcUrl();

        _logger?.LogInformation("Calling contract method {Method} on {Contract}", method, contractHash);

        return await _invoker.CallAsync<T>(contractHash, method, args, rpcUrl);
    }

    /// <summary>
    /// Invoke a contract method (state-changing transaction)
    /// </summary>
    /// <param name="contractHashOrAddress">Contract hash or address</param>
    /// <param name="method">Method name</param>
    /// <param name="args">Method arguments</param>
    /// <returns>Transaction hash</returns>
    public async Task<UInt256> InvokeAsync(string contractHashOrAddress, string method, params object[] args)
    {
        if (string.IsNullOrWhiteSpace(contractHashOrAddress))
            throw new ArgumentException("Contract hash or address cannot be null or empty", nameof(contractHashOrAddress));

        if (string.IsNullOrWhiteSpace(method))
            throw new ArgumentException("Method name cannot be null or empty", nameof(method));

        if (string.IsNullOrWhiteSpace(_wifKey))
            throw new InvalidOperationException("WIF key must be set before invoking methods. Use SetWifKey().");

        var contractHash = ParseContractHashOrAddress(contractHashOrAddress);
        var options = CreateInvocationOptions();

        _logger?.LogInformation("Invoking contract method {Method} on {Contract}", method, contractHash);

        return await _invoker.InvokeAsync(contractHash, method, args, options);
    }

    /// <summary>
    /// Get the default deployer account
    /// </summary>
    /// <returns>Deployer account script hash</returns>
    /// <exception cref="InvalidOperationException">Thrown when no deployer account is configured</exception>
    public async Task<UInt160> GetDeployerAccountAsync()
    {
        await Task.Delay(1); // Simulate async work
        
        if (!string.IsNullOrEmpty(_wifKey))
        {
            // Use WIF key to get account
            var privateKey = Neo.Wallets.Wallet.GetPrivateKeyFromWIF(_wifKey);
            var keyPair = new KeyPair(privateKey);
            return Neo.SmartContract.Contract.CreateSignatureContract(keyPair.PublicKey).ScriptHash;
        }

        throw new InvalidOperationException("No deployer account configured. Set a WIF key using SetWifKey().");
    }

    /// <summary>
    /// Get the current balance of an account
    /// </summary>
    /// <param name="address">Account address (null for default deployer)</param>
    /// <returns>GAS balance</returns>
    public async Task<decimal> GetGasBalanceAsync(string? address = null)
    {
        UInt160 accountHash;

        if (string.IsNullOrWhiteSpace(address))
        {
            // Use the default deployer account
            accountHash = await GetDeployerAccountAsync();
        }
        else
        {
            // Parse the provided address
            accountHash = ParseContractHashOrAddress(address);
        }

        var rpcUrl = GetCurrentRpcUrl();
        return await _walletManager.GetGasBalanceAsync(accountHash, rpcUrl);
    }

    /// <summary>
    /// Deploy multiple contracts from a manifest file
    /// </summary>
    /// <param name="manifestPath">Path to the deployment manifest JSON file</param>
    /// <returns>Dictionary of contract names to deployment information</returns>
    public async Task<Dictionary<string, ContractDeploymentInfo>> DeployFromManifestAsync(string manifestPath)
    {
        if (string.IsNullOrWhiteSpace(manifestPath))
            throw new ArgumentException("Manifest path cannot be null or empty", nameof(manifestPath));

        _logger?.LogInformation("Deploying contracts from manifest: {ManifestPath}", manifestPath);

        var options = CreateDeploymentOptions();
        var toolkit = new NeoContractToolkit(_serviceProvider);
        
        return await toolkit.DeployFromManifestAsync(manifestPath, options);
    }

    /// <summary>
    /// Check if a contract exists at the given address
    /// </summary>
    /// <param name="contractHashOrAddress">Contract hash or address</param>
    /// <returns>True if contract exists, false otherwise</returns>
    public async Task<bool> ContractExistsAsync(string contractHashOrAddress)
    {
        if (string.IsNullOrWhiteSpace(contractHashOrAddress))
            throw new ArgumentException("Contract hash or address cannot be null or empty", nameof(contractHashOrAddress));

        var contractHash = ParseContractHashOrAddress(contractHashOrAddress);
        var rpcUrl = GetCurrentRpcUrl();

        return await _deployer.ContractExistsAsync(contractHash, rpcUrl);
    }

    #region Private Methods

    private DeploymentOptions CreateDeploymentOptions()
    {
        if (string.IsNullOrWhiteSpace(_wifKey))
            throw new InvalidOperationException("WIF key must be set before deployment. Use SetWifKey().");

        return new DeploymentOptions
        {
            WifKey = _wifKey,
            RpcUrl = GetCurrentRpcUrl(),
            NetworkMagic = GetNetworkMagic(),
            WaitForConfirmation = true,
            VerifyAfterDeploy = false,
            GasLimit = 100_000_000
        };
    }

    private InvocationOptions CreateInvocationOptions()
    {
        if (string.IsNullOrWhiteSpace(_wifKey))
            throw new InvalidOperationException("WIF key must be set before invocation. Use SetWifKey().");

        return new InvocationOptions
        {
            WifKey = _wifKey,
            RpcUrl = GetCurrentRpcUrl(),
            NetworkMagic = GetNetworkMagic(),
            WaitForConfirmation = true,
            GasLimit = 10_000_000
        };
    }

    private UInt160 ParseContractHashOrAddress(string contractHashOrAddress)
    {
        try
        {
            // Try to parse as a hex hash first
            if (contractHashOrAddress.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return UInt160.Parse(contractHashOrAddress);
            }

            // Try to parse as an address
            return contractHashOrAddress.ToScriptHash(ProtocolSettings.Default.AddressVersion);
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid contract hash or address: {contractHashOrAddress}", nameof(contractHashOrAddress), ex);
        }
    }

    private string GetCurrentRpcUrl()
    {
        if (!string.IsNullOrEmpty(_currentNetwork))
        {
            var networks = _configuration.GetSection("Network:Networks").Get<Dictionary<string, NetworkConfiguration>>();
            if (networks != null && networks.TryGetValue(_currentNetwork, out var network))
            {
                return network.RpcUrl;
            }
        }

        // Fallback to default RPC URL
        return _configuration["Network:RpcUrl"] ?? DEFAULT_RPC_URL;
    }

    private uint GetNetworkMagic()
    {
        if (!string.IsNullOrEmpty(_currentNetwork))
        {
            var networks = _configuration.GetSection("Network:Networks").Get<Dictionary<string, NetworkConfiguration>>();
            if (networks != null && networks.TryGetValue(_currentNetwork, out var network))
            {
                return network.NetworkMagic;
            }
        }

        // Return network magic based on current network
        return _currentNetwork?.ToLower() switch
        {
            "mainnet" => 860833102,
            "testnet" => 894710606,
            _ => _configuration.GetValue<uint>("Network:NetworkMagic", 894710606) // Default to testnet
        };
    }

    #endregion

    #region IDisposable Implementation

    /// <summary>
    /// Dispose of the toolkit and its resources
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected dispose method
    /// </summary>
    /// <param name="disposing">True if disposing managed resources</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                if (_serviceProvider is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            _disposed = true;
        }
    }

    #endregion
}