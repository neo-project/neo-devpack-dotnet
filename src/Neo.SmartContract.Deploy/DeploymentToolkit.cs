using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Services;
using Neo.Wallets;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy;

/// <summary>
/// Streamlined deployment toolkit providing a simplified API for Neo smart contract deployment with automatic configuration
/// </summary>
public class DeploymentToolkit
{
    private readonly NeoContractToolkit _toolkit;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DeploymentToolkit> _logger;
    private bool _walletLoaded = false;
    private string? _currentNetwork = null;

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
    public DeploymentToolkit SetNetwork(string network)
    {
        _currentNetwork = network.ToLowerInvariant();

        // Update configuration based on network
        var configSection = _configuration.GetSection("Network");

        switch (_currentNetwork)
        {
            case "mainnet":
                Environment.SetEnvironmentVariable("Network__RpcUrl", "https://rpc10.n3.nspcc.ru:10331");
                Environment.SetEnvironmentVariable("Network__Network", "mainnet");
                break;

            case "testnet":
                Environment.SetEnvironmentVariable("Network__RpcUrl", "https://rpc10.n3.neotracker.io:443");
                Environment.SetEnvironmentVariable("Network__Network", "testnet");
                break;

            case "local":
            case "private":
                Environment.SetEnvironmentVariable("Network__RpcUrl", "http://localhost:50012");
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
    public async Task<ContractDeploymentInfo> Deploy(string path, object[]? initParams = null)
    {
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
    public async Task<ContractDeploymentInfo> DeployArtifacts(string nefPath, string manifestPath, object[]? initParams = null)
    {
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
    public async Task<T> Call<T>(string contractHashOrAddress, string method, params object[] args)
    {
        var contractHash = ParseContractHash(contractHashOrAddress);
        return await _toolkit.CallContractAsync<T>(contractHash, method, args);
    }

    /// <summary>
    /// Invoke a contract method (state-changing transaction)
    /// </summary>
    public async Task<UInt256> Invoke(string contractHashOrAddress, string method, params object[] args)
    {
        await EnsureWalletLoaded();

        var contractHash = ParseContractHash(contractHashOrAddress);
        return await _toolkit.InvokeContractAsync(contractHash, method, args);
    }

    /// <summary>
    /// Update an existing contract
    /// </summary>
    public async Task<ContractDeploymentInfo> Update(string contractHashOrAddress, string path)
    {
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
        var gasHash = UInt160.Parse("0xd2a4cff31913016155e38e474a2c06d08be276cf");
        var balance = await Call<System.Numerics.BigInteger>(gasHash.ToString(), "balanceOf", account);

        // GAS has 8 decimals
        return (decimal)balance / 100_000_000m;
    }

    /// <summary>
    /// Deploy multiple contracts from a manifest file
    /// </summary>
    /// <param name="manifestPath">Path to the deployment manifest JSON file</param>
    /// <returns>Dictionary of contract names to deployment information</returns>
    public async Task<Dictionary<string, ContractDeploymentInfo>> DeployFromManifest(string manifestPath)
    {
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
    public async Task<bool> ContractExists(string contractHashOrAddress)
    {
        var contractHash = ParseAddress(contractHashOrAddress);
        var deployer = _serviceProvider.GetRequiredService<IContractDeployer>();
        var rpcUrl = GetCurrentRpcUrl();

        return await deployer.ContractExistsAsync(contractHash, rpcUrl);
    }

    /// <summary>
    /// Deploy contract from pre-compiled artifacts
    /// </summary>
    /// <param name="nefPath">Path to .nef file</param>
    /// <param name="manifestPath">Path to .manifest.json file</param>
    /// <param name="initParams">Optional initialization parameters</param>
    /// <returns>Deployment information</returns>
    public async Task<ContractDeploymentInfo> DeployFromArtifacts(string nefPath, string manifestPath, object[]? initParams = null)
    {
        return await DeployArtifacts(nefPath, manifestPath, initParams);
    }

    #region Private Methods

    private async Task LoadWalletIfConfigured()
    {
        try
        {
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
    }

    private async Task EnsureWalletLoaded()
    {
        if (!_walletLoaded)
        {
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
        catch
        {
            throw new ArgumentException($"Invalid address: {address}");
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
        return _configuration["Network:RpcUrl"] ?? "http://localhost:10332";
    }

    #endregion
}
