using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.Wallets;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;

namespace Neo.SmartContract.Deploy;

/// <summary>
/// Provides a fluent API for Neo smart contract operations
/// </summary>
public class NeoContractToolkit : IAsyncDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IContractCompiler _compiler;
    private readonly IContractDeployer _deployer;
    private readonly IContractInvoker _invoker;
    private readonly IWalletManager _walletManager;
    private readonly ILogger<NeoContractToolkit>? _logger;

    /// <summary>
    /// Initialize a new instance of NeoContractToolkit
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    public NeoContractToolkit(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _compiler = serviceProvider.GetRequiredService<IContractCompiler>();
        _deployer = serviceProvider.GetRequiredService<IContractDeployer>();
        _invoker = serviceProvider.GetRequiredService<IContractInvoker>();
        _walletManager = serviceProvider.GetRequiredService<IWalletManager>();
        _logger = serviceProvider.GetService<ILogger<NeoContractToolkit>>();
    }

    /// <summary>
    /// Compile a smart contract from source
    /// </summary>
    /// <param name="projectPath">Path to the contract project</param>
    /// <returns>Compiled contract</returns>
    public async Task<CompiledContract> CompileAsync(string projectPath)
    {
        _logger?.LogInformation("Compiling contract from: {ProjectPath}", projectPath);
        return await _compiler.CompileAsync(projectPath);
    }

    /// <summary>
    /// Load a pre-compiled contract
    /// </summary>
    /// <param name="nefPath">Path to NEF file</param>
    /// <param name="manifestPath">Path to manifest file</param>
    /// <returns>Compiled contract</returns>
    public async Task<CompiledContract> LoadContractAsync(string nefPath, string manifestPath)
    {
        _logger?.LogInformation("Loading contract from NEF: {NefPath}, Manifest: {ManifestPath}", nefPath, manifestPath);
        return await _compiler.LoadContractAsync(nefPath, manifestPath);
    }

    /// <summary>
    /// Deploy a compiled contract
    /// </summary>
    /// <param name="contract">Compiled contract</param>
    /// <param name="options">Deployment options</param>
    /// <param name="initParams">Initialization parameters</param>
    /// <returns>Deployment information</returns>
    public async Task<ContractDeploymentInfo> DeployAsync(CompiledContract contract, DeploymentOptions options, object[]? initParams = null)
    {
        _logger?.LogInformation("Deploying contract: {ContractName}", contract.Name);
        return await _deployer.DeployAsync(contract, options, initParams);
    }

    /// <summary>
    /// Deploy a contract from source
    /// </summary>
    /// <param name="projectPath">Path to the contract project</param>
    /// <param name="options">Deployment options</param>
    /// <param name="initParams">Initialization parameters</param>
    /// <returns>Deployment information</returns>
    public async Task<ContractDeploymentInfo> DeployFromSourceAsync(string projectPath, DeploymentOptions options, object[]? initParams = null)
    {
        var contract = await CompileAsync(projectPath);
        return await DeployAsync(contract, options, initParams);
    }

    /// <summary>
    /// Deploy a contract from pre-compiled files
    /// </summary>
    /// <param name="nefPath">Path to NEF file</param>
    /// <param name="manifestPath">Path to manifest file</param>
    /// <param name="options">Deployment options</param>
    /// <param name="initParams">Initialization parameters</param>
    /// <returns>Deployment information</returns>
    public async Task<ContractDeploymentInfo> DeployFromFilesAsync(string nefPath, string manifestPath, DeploymentOptions options, object[]? initParams = null)
    {
        var contract = await LoadContractAsync(nefPath, manifestPath);
        return await DeployAsync(contract, options, initParams);
    }

    /// <summary>
    /// Check if a contract exists
    /// </summary>
    /// <param name="contractHash">Contract hash</param>
    /// <param name="rpcUrl">RPC URL</param>
    /// <returns>True if contract exists</returns>
    public async Task<bool> ContractExistsAsync(UInt160 contractHash, string rpcUrl)
    {
        return await _deployer.ContractExistsAsync(contractHash, rpcUrl);
    }

    /// <summary>
    /// Call a contract method (read-only)
    /// </summary>
    /// <typeparam name="T">Return type</typeparam>
    /// <param name="contractHash">Contract hash</param>
    /// <param name="method">Method name</param>
    /// <param name="rpcUrl">RPC URL</param>
    /// <param name="args">Method arguments</param>
    /// <returns>Method result</returns>
    public async Task<T> CallAsync<T>(UInt160 contractHash, string method, string rpcUrl, params object[] args)
    {
        _logger?.LogInformation("Calling contract method: {Method} on {Contract}", method, contractHash);
        return await _invoker.CallAsync<T>(contractHash, method, args, rpcUrl);
    }

    /// <summary>
    /// Invoke a contract method (state-changing)
    /// </summary>
    /// <param name="contractHash">Contract hash</param>
    /// <param name="method">Method name</param>
    /// <param name="options">Invocation options</param>
    /// <param name="args">Method arguments</param>
    /// <returns>Transaction hash</returns>
    public async Task<UInt256> InvokeAsync(UInt160 contractHash, string method, InvocationOptions options, params object[] args)
    {
        _logger?.LogInformation("Invoking contract method: {Method} on {Contract}", method, contractHash);
        return await _invoker.InvokeAsync(contractHash, method, args, options);
    }

    /// <summary>
    /// Get account from WIF key
    /// </summary>
    /// <param name="wifKey">WIF private key</param>
    /// <returns>Account</returns>
    public Account GetAccountFromWif(string wifKey)
    {
        return _walletManager.GetAccountFromWif(wifKey);
    }

    /// <summary>
    /// Get account address
    /// </summary>
    /// <param name="account">Account</param>
    /// <returns>Address</returns>
    public string GetAccountAddress(Account account)
    {
        return _walletManager.GetAccountAddress(account);
    }

    /// <summary>
    /// Get GAS balance
    /// </summary>
    /// <param name="accountHash">Account hash</param>
    /// <param name="rpcUrl">RPC URL</param>
    /// <returns>GAS balance</returns>
    public async Task<decimal> GetGasBalanceAsync(UInt160 accountHash, string rpcUrl)
    {
        return await _walletManager.GetGasBalanceAsync(accountHash, rpcUrl);
    }

    /// <summary>
    /// Deploy multiple contracts from a manifest
    /// </summary>
    /// <param name="manifestPath">Path to deployment manifest JSON</param>
    /// <param name="defaultOptions">Default deployment options</param>
    /// <returns>Dictionary of contract names to deployment info</returns>
    public async Task<Dictionary<string, ContractDeploymentInfo>> DeployFromManifestAsync(string manifestPath, DeploymentOptions defaultOptions)
    {
        _logger?.LogInformation("Deploying contracts from manifest: {ManifestPath}", manifestPath);
        
        // Read and parse the manifest file
        var manifestJson = await System.IO.File.ReadAllTextAsync(manifestPath);
        var manifest = System.Text.Json.JsonSerializer.Deserialize<DeploymentManifest>(manifestJson);
        
        if (manifest?.Contracts == null || manifest.Contracts.Count == 0)
        {
            throw new ArgumentException("Manifest contains no contracts to deploy");
        }

        var results = new Dictionary<string, ContractDeploymentInfo>();
        
        foreach (var contractDef in manifest.Contracts)
        {
            try
            {
                _logger?.LogInformation("Deploying contract: {ContractName}", contractDef.Name);
                
                // Merge options
                var options = MergeOptions(defaultOptions, contractDef);
                
                // Compile or load the contract
                CompiledContract contract;
                if (!string.IsNullOrEmpty(contractDef.ProjectPath))
                {
                    contract = await CompileAsync(contractDef.ProjectPath);
                }
                else if (!string.IsNullOrEmpty(contractDef.NefPath) && !string.IsNullOrEmpty(contractDef.ManifestPath))
                {
                    contract = await LoadContractAsync(contractDef.NefPath, contractDef.ManifestPath);
                }
                else
                {
                    throw new ArgumentException($"Contract {contractDef.Name} must specify either ProjectPath or both NefPath and ManifestPath");
                }
                
                // Deploy the contract
                var deployInfo = await DeployAsync(contract, options, contractDef.InitParams?.ToArray());
                results[contractDef.Name] = deployInfo;
                
                _logger?.LogInformation("Successfully deployed {ContractName} at {ContractHash}", 
                    contractDef.Name, deployInfo.ContractHash);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to deploy contract: {ContractName}", contractDef.Name);
                if (!manifest.ContinueOnError)
                {
                    throw;
                }
            }
        }
        
        return results;
    }

    private DeploymentOptions MergeOptions(DeploymentOptions defaultOptions, ContractDefinition contractDef)
    {
        return new DeploymentOptions
        {
            DeployerAccount = contractDef.DeployerAccount ?? defaultOptions.DeployerAccount,
            WifKey = contractDef.WifKey ?? defaultOptions.WifKey,
            RpcUrl = contractDef.RpcUrl ?? defaultOptions.RpcUrl,
            NetworkMagic = contractDef.NetworkMagic ?? defaultOptions.NetworkMagic,
            GasLimit = contractDef.GasLimit ?? defaultOptions.GasLimit,
            WaitForConfirmation = contractDef.WaitForConfirmation ?? defaultOptions.WaitForConfirmation,
            VerifyAfterDeploy = contractDef.VerifyAfterDeploy ?? defaultOptions.VerifyAfterDeploy,
            VerificationDelayMs = contractDef.VerificationDelayMs ?? defaultOptions.VerificationDelayMs,
            DryRun = contractDef.DryRun ?? defaultOptions.DryRun,
            InitialParameters = contractDef.InitParams ?? defaultOptions.InitialParameters,
            DefaultNetworkFee = contractDef.NetworkFee ?? defaultOptions.DefaultNetworkFee,
            ValidUntilBlockOffset = contractDef.ValidUntilBlockOffset ?? defaultOptions.ValidUntilBlockOffset,
            ConfirmationRetries = defaultOptions.ConfirmationRetries,
            ConfirmationDelaySeconds = defaultOptions.ConfirmationDelaySeconds
        };
    }

    /// <summary>
    /// Dispose of resources
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_serviceProvider is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    private class DeploymentManifest
    {
        public List<ContractDefinition> Contracts { get; set; } = new();
        public bool ContinueOnError { get; set; }
    }

    private class ContractDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string? ProjectPath { get; set; }
        public string? NefPath { get; set; }
        public string? ManifestPath { get; set; }
        public List<object>? InitParams { get; set; }
        public UInt160? DeployerAccount { get; set; }
        public string? WifKey { get; set; }
        public string? RpcUrl { get; set; }
        public uint? NetworkMagic { get; set; }
        public long? GasLimit { get; set; }
        public bool? WaitForConfirmation { get; set; }
        public bool? VerifyAfterDeploy { get; set; }
        public int? VerificationDelayMs { get; set; }
        public bool? DryRun { get; set; }
        public long? NetworkFee { get; set; }
        public uint? ValidUntilBlockOffset { get; set; }
    }
}