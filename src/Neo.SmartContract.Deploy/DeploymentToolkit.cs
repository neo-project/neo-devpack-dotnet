using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.Wallets;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Services;
using Neo.SmartContract.Deploy.Shared;

namespace Neo.SmartContract.Deploy;

/// <summary>
/// Streamlined deployment toolkit providing a simplified API for Neo smart contract deployment with automatic configuration
/// </summary>
public class DeploymentToolkit : IDisposable
{
    private const string GAS_CONTRACT_HASH = "0xd2a4cff31913016155e38e474a2c06d08be276cf";
    private const decimal GAS_DECIMALS = 100_000_000m;
    private const string MAINNET_RPC_URL = "https://rpc10.n3.nspcc.ru:10331";
    private const string TESTNET_RPC_URL = "https://rpc10.n3.neotracker.io:443";
    private const string LOCAL_RPC_URL = "http://localhost:50012";
    private const string DEFAULT_RPC_URL = "http://localhost:10332";

    private readonly NeoContractToolkit _toolkit;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DeploymentToolkit> _logger;
    private readonly SemaphoreSlim _walletLock = new SemaphoreSlim(1, 1);
    private volatile bool _walletLoaded = false;
    private volatile string? _currentNetwork = null;
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

        // Create toolkit with minimal setup
        var toolkitBuilder = NeoContractToolkitBuilder.Create()
            .ConfigureLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
            })
            .ConfigureServices(services =>
            {
                services.AddSingleton(_configuration);
            });

        _toolkit = toolkitBuilder.Build();
        _serviceProvider = toolkitBuilder.ServiceProvider!;
        _logger = _toolkit.GetService<ILogger<DeploymentToolkit>>()!;

        // Auto-load wallet if configured
        _ = LoadWalletIfConfigured();
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
        var configSection = _configuration.GetSection("Network");

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

        _logger.LogInformation($"Network set to: {_currentNetwork}");
        return this;
    }

    /// <summary>
    /// Deploy a contract from source code or project
    /// </summary>
    /// <param name="path">Path to contract project (.csproj) or source file</param>
    /// <param name="initParams">Optional initialization parameters</param>
    /// <returns>Deployment information</returns>
    /// <exception cref="ArgumentException">Thrown when path is invalid</exception>
    /// <exception cref="FileNotFoundException">Thrown when file does not exist</exception>
    public async Task<ContractDeploymentInfo> Deploy(string path, object[]? initParams = null)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty", nameof(path));

        if (!File.Exists(path))
            throw new FileNotFoundException($"Contract file not found: {path}", path);
        await EnsureWalletLoaded();

        // Determine if it's a project or source file
        var isProject = path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase);

        var compilationOptions = new CompilationOptions
        {
            ProjectPath = isProject ? path : null,
            SourcePath = isProject ? null : path,
            OutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "artifacts"),
            ContractName = Path.GetFileNameWithoutExtension(path)
        };

        var deploymentOptions = new DeploymentOptions
        {
            DeployerAccount = _toolkit.GetDeployerAccount() ?? UInt160.Zero,
            GasLimit = _configuration.GetValue<long>("Deployment:GasLimit", 100_000_000),
            WaitForConfirmation = _configuration.GetValue<bool>("Deployment:WaitForConfirmation", true),
            InitialParameters = initParams?.ToList()
        };

        _logger.LogInformation($"Deploying {compilationOptions.ContractName}...");

        var result = await _toolkit.CompileAndDeployAsync(compilationOptions, deploymentOptions);

        if (result.Success)
        {
            _logger.LogInformation($"✅ Contract deployed: {result.ContractHash}");
        }
        else
        {
            _logger.LogError($"❌ Deployment failed: {result.ErrorMessage}");
        }

        return result;
    }

    /// <summary>
    /// Deploy a pre-compiled contract from NEF and manifest files
    /// </summary>
    /// <param name="nefPath">Path to NEF file</param>
    /// <param name="manifestPath">Path to manifest file</param>
    /// <param name="initParams">Optional initialization parameters</param>
    /// <returns>Deployment information</returns>
    /// <exception cref="ArgumentException">Thrown when paths are invalid</exception>
    /// <exception cref="FileNotFoundException">Thrown when files do not exist</exception>
    public async Task<ContractDeploymentInfo> DeployArtifacts(string nefPath, string manifestPath, object[]? initParams = null)
    {
        if (string.IsNullOrWhiteSpace(nefPath))
            throw new ArgumentException("NEF path cannot be null or empty", nameof(nefPath));

        if (string.IsNullOrWhiteSpace(manifestPath))
            throw new ArgumentException("Manifest path cannot be null or empty", nameof(manifestPath));

        if (!File.Exists(nefPath))
            throw new FileNotFoundException($"NEF file not found: {nefPath}", nefPath);

        if (!File.Exists(manifestPath))
            throw new FileNotFoundException($"Manifest file not found: {manifestPath}", manifestPath);
        await EnsureWalletLoaded();

        var deploymentOptions = new DeploymentOptions
        {
            DeployerAccount = _toolkit.GetDeployerAccount() ?? UInt160.Zero,
            GasLimit = _configuration.GetValue<long>("Deployment:GasLimit", 100_000_000),
            WaitForConfirmation = _configuration.GetValue<bool>("Deployment:WaitForConfirmation", true),
            InitialParameters = initParams?.ToList()
        };

        _logger.LogInformation($"Deploying from artifacts...");

        var result = await _toolkit.DeployFromArtifactsAsync(nefPath, manifestPath, deploymentOptions);

        if (result.Success)
        {
            _logger.LogInformation($"✅ Contract deployed: {result.ContractHash}");
        }
        else
        {
            _logger.LogError($"❌ Deployment failed: {result.ErrorMessage}");
        }

        return result;
    }

    /// <summary>
    /// Call a contract method (read-only)
    /// </summary>
    /// <typeparam name="T">Return type</typeparam>
    /// <param name="contractHashOrAddress">Contract hash or address</param>
    /// <param name="method">Method name</param>
    /// <param name="args">Method arguments</param>
    /// <returns>Method return value</returns>
    /// <exception cref="ArgumentException">Thrown when parameters are invalid</exception>
    public async Task<T> Call<T>(string contractHashOrAddress, string method, params object[] args)
    {
        if (string.IsNullOrWhiteSpace(contractHashOrAddress))
            throw new ArgumentException("Contract hash or address cannot be null or empty", nameof(contractHashOrAddress));

        if (string.IsNullOrWhiteSpace(method))
            throw new ArgumentException("Method name cannot be null or empty", nameof(method));
        var contractHash = ParseContractHash(contractHashOrAddress);
        return await _toolkit.CallContractAsync<T>(contractHash, method, args);
    }

    /// <summary>
    /// Invoke a contract method (state-changing transaction)
    /// </summary>
    /// <param name="contractHashOrAddress">Contract hash or address</param>
    /// <param name="method">Method name</param>
    /// <param name="args">Method arguments</param>
    /// <returns>Transaction hash</returns>
    /// <exception cref="ArgumentException">Thrown when parameters are invalid</exception>
    public async Task<UInt256> Invoke(string contractHashOrAddress, string method, params object[] args)
    {
        if (string.IsNullOrWhiteSpace(contractHashOrAddress))
            throw new ArgumentException("Contract hash or address cannot be null or empty", nameof(contractHashOrAddress));

        if (string.IsNullOrWhiteSpace(method))
            throw new ArgumentException("Method name cannot be null or empty", nameof(method));
        await EnsureWalletLoaded();

        var contractHash = ParseContractHash(contractHashOrAddress);
        return await _toolkit.InvokeContractAsync(contractHash, method, args);
    }

    /// <summary>
    /// Update an existing contract
    /// </summary>
    /// <param name="contractHashOrAddress">Contract hash or address to update</param>
    /// <param name="path">Path to new contract project or source</param>
    /// <returns>Update result</returns>
    /// <exception cref="ArgumentException">Thrown when parameters are invalid</exception>
    /// <exception cref="FileNotFoundException">Thrown when file does not exist</exception>
    public async Task<ContractDeploymentInfo> Update(string contractHashOrAddress, string path)
    {
        if (string.IsNullOrWhiteSpace(contractHashOrAddress))
            throw new ArgumentException("Contract hash or address cannot be null or empty", nameof(contractHashOrAddress));

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty", nameof(path));

        if (!File.Exists(path))
            throw new FileNotFoundException($"Contract file not found: {path}", path);
        await EnsureWalletLoaded();

        var contractHash = ParseContractHash(contractHashOrAddress);
        var isProject = path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase);

        var compilationOptions = new CompilationOptions
        {
            ProjectPath = isProject ? path : null,
            SourcePath = isProject ? null : path,
            OutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "artifacts"),
            ContractName = Path.GetFileNameWithoutExtension(path)
        };

        var deploymentOptions = new DeploymentOptions
        {
            DeployerAccount = _toolkit.GetDeployerAccount() ?? UInt160.Zero,
            GasLimit = _configuration.GetValue<long>("Deployment:GasLimit", 100_000_000),
            WaitForConfirmation = _configuration.GetValue<bool>("Deployment:WaitForConfirmation", true)
        };

        _logger.LogInformation($"Updating contract {contractHash}...");

        // Compile first
        var compiler = _serviceProvider.GetRequiredService<IContractCompiler>();
        var compiled = await compiler.CompileAsync(compilationOptions);

        // Then update
        var updateService = _serviceProvider.GetRequiredService<IContractUpdateService>();
        var updateResult = await updateService.UpdateContractAsync(
            compilationOptions.ContractName,
            _currentNetwork ?? "custom",
            compiled.NefBytes,
            compiled.Manifest.ToJson().ToString(),
            null,
            null);

        var result = new ContractDeploymentInfo
        {
            Success = updateResult.Success,
            ContractHash = contractHash,
            TransactionHash = updateResult.TransactionHash,
            ErrorMessage = updateResult.ErrorMessage
        };

        if (result.Success)
        {
            _logger.LogInformation($"✅ Contract updated: {result.ContractHash}");
        }
        else
        {
            _logger.LogError($"❌ Update failed: {result.ErrorMessage}");
        }

        return result;
    }

    /// <summary>
    /// Get the default deployer account
    /// </summary>
    /// <returns>Deployer account script hash</returns>
    /// <exception cref="InvalidOperationException">Thrown when no deployer account is configured</exception>
    public async Task<UInt160> GetDeployerAccount()
    {
        await EnsureWalletLoaded();
        var account = _toolkit.GetDeployerAccount();
        if (account == null || account == UInt160.Zero)
            throw new InvalidOperationException("No deployer account configured");
        return account;
    }

    /// <summary>
    /// Get the current balance of an account
    /// </summary>
    /// <param name="address">Account address (null for default deployer)</param>
    /// <returns>GAS balance</returns>
    /// <exception cref="ArgumentException">Thrown when address is invalid</exception>
    public async Task<decimal> GetGasBalance(string? address = null)
    {
        UInt160 account;
        if (string.IsNullOrEmpty(address))
        {
            account = await GetDeployerAccount();
        }
        else
        {
            account = ParseAddress(address);
        }

        // Call GAS token contract
        var gasHash = UInt160.Parse(GAS_CONTRACT_HASH);
        var balance = await Call<System.Numerics.BigInteger>(gasHash.ToString(), "balanceOf", account);

        // GAS has 8 decimals
        return (decimal)balance / GAS_DECIMALS;
    }

    /// <summary>
    /// Deploy multiple contracts from a manifest file
    /// </summary>
    /// <param name="manifestPath">Path to the deployment manifest JSON file</param>
    /// <returns>Dictionary of contract names to deployment information</returns>
    /// <exception cref="ArgumentException">Thrown when manifestPath is invalid</exception>
    /// <exception cref="FileNotFoundException">Thrown when manifest file does not exist</exception>
    public async Task<Dictionary<string, ContractDeploymentInfo>> DeployFromManifest(string manifestPath)
    {
        if (string.IsNullOrWhiteSpace(manifestPath))
            throw new ArgumentException("Manifest path cannot be null or empty", nameof(manifestPath));

        if (!File.Exists(manifestPath))
            throw new FileNotFoundException($"Manifest file not found: {manifestPath}", manifestPath);
        await EnsureWalletLoaded();

        var deploymentOptions = new DeploymentOptions
        {
            DeployerAccount = _toolkit.GetDeployerAccount() ?? UInt160.Zero,
            GasLimit = _configuration.GetValue<long>("Deployment:GasLimit", 100_000_000),
            WaitForConfirmation = _configuration.GetValue<bool>("Deployment:WaitForConfirmation", true)
        };

        _logger.LogInformation($"Deploying contracts from manifest: {manifestPath}");

        var result = await _toolkit.DeployFromManifestAsync(manifestPath, deploymentOptions);

        // Convert MultiContractDeploymentResult to simplified dictionary
        var deploymentMap = new Dictionary<string, ContractDeploymentInfo>();

        foreach (var deployment in result.SuccessfulDeployments)
        {
            deploymentMap[deployment.ContractName] = deployment;
        }

        if (result.FailedDeployments.Any())
        {
            var failures = string.Join(", ", result.FailedDeployments.Select(f => $"{f.ContractName}: {f.Reason}"));
            _logger.LogWarning($"Some deployments failed: {failures}");
        }

        return deploymentMap;
    }

    /// <summary>
    /// Check if a contract exists at the given address
    /// </summary>
    /// <param name="contractHashOrAddress">Contract hash or address</param>
    /// <returns>True if contract exists, false otherwise</returns>
    /// <exception cref="ArgumentException">Thrown when contractHashOrAddress is invalid</exception>
    public async Task<bool> ContractExists(string contractHashOrAddress)
    {
        if (string.IsNullOrWhiteSpace(contractHashOrAddress))
            throw new ArgumentException("Contract hash or address cannot be null or empty", nameof(contractHashOrAddress));
        var contractHash = ParseAddress(contractHashOrAddress);
        var deployer = _serviceProvider.GetRequiredService<IContractDeployer>();
        var rpcUrl = GetCurrentRpcUrl();

        return await deployer.ContractExistsAsync(contractHash, rpcUrl);
    }


    #region Private Methods

    private async Task LoadWalletIfConfigured()
    {
        await _walletLock.WaitAsync();
        try
        {
            if (_walletLoaded) return;

            var walletPath = _configuration["Wallet:Path"];
            var walletPassword = _configuration["Wallet:Password"];

            if (!string.IsNullOrEmpty(walletPath) && !string.IsNullOrEmpty(walletPassword))
            {
                _logger.LogInformation($"Auto-loading wallet from configuration...");
                await _toolkit.LoadWalletAsync(walletPath, walletPassword);
                _walletLoaded = true;
                _logger.LogInformation($"Wallet loaded successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Failed to auto-load wallet: {ex.Message}");
        }
        finally
        {
            _walletLock.Release();
        }
    }

    private async Task EnsureWalletLoaded()
    {
        if (_walletLoaded) return;

        await _walletLock.WaitAsync();
        try
        {
            if (_walletLoaded) return; // Double-check after acquiring lock

            // Try to load from environment variables
            var walletPath = Environment.GetEnvironmentVariable("NEO_WALLET_PATH");
            var walletPassword = Environment.GetEnvironmentVariable("NEO_WALLET_PASSWORD");

            if (!string.IsNullOrEmpty(walletPath) && !string.IsNullOrEmpty(walletPassword))
            {
                await _toolkit.LoadWalletAsync(walletPath, walletPassword);
                _walletLoaded = true;
                _logger.LogInformation($"Wallet loaded from environment variables");
            }
            else
            {
                throw new InvalidOperationException(
                    "No wallet loaded. Set wallet configuration in appsettings.json or " +
                    "NEO_WALLET_PATH and NEO_WALLET_PASSWORD environment variables.");
            }
        }
        finally
        {
            _walletLock.Release();
        }
    }

    private UInt160 ParseContractHash(string value)
    {
        // Try to parse as hash first
        if (UInt160.TryParse(value, out var hash))
        {
            return hash;
        }

        // Try to parse as address
        try
        {
            return ParseAddress(value);
        }
        catch
        {
            throw new ArgumentException($"Invalid contract hash or address: {value}");
        }
    }

    private UInt160 ParseAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be null or empty", nameof(address));

        try
        {
            // Try common address versions
            if (address.StartsWith("N")) // N3
                return address.ToScriptHash((byte)53);
            if (address.StartsWith("A")) // Legacy
                return address.ToScriptHash((byte)23);

            // Default to N3
            return address.ToScriptHash((byte)53);
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid address format: {address}", nameof(address), ex);
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
                if (_serviceProvider is IDisposable disposableProvider)
                {
                    disposableProvider.Dispose();
                }

                // Clear any cached RPC clients if using RpcClientFactory
                if (_serviceProvider != null)
                {
                    try
                    {
                        var rpcFactory = _serviceProvider.GetService(typeof(IRpcClientFactory)) as RpcClientFactory;
                        rpcFactory?.ClearPool();
                    }
                    catch
                    {
                        // Ignore errors during disposal
                    }
                }

                // Dispose of the semaphore
                _walletLock?.Dispose();
            }

            _disposed = true;
        }
    }

    #endregion
}
