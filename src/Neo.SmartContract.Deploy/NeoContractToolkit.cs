using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;

namespace Neo.SmartContract.Deploy;

/// <summary>
/// Main entry point for Neo Smart Contract deployment toolkit
/// Provides high-level API for compiling, deploying, and invoking contracts
/// </summary>
public class NeoContractToolkit
{
    private readonly IContractCompiler _compiler;
    private readonly IContractDeployer _deployer;
    private readonly IContractInvoker _invoker;
    private readonly IWalletManager _walletManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<NeoContractToolkit> _logger;

    /// <summary>
    /// Create a new instance of the toolkit
    /// </summary>
    public NeoContractToolkit(
        IContractCompiler compiler,
        IContractDeployer deployer,
        IContractInvoker invoker,
        IWalletManager walletManager,
        IConfiguration configuration,
        ILogger<NeoContractToolkit> logger)
    {
        _compiler = compiler;
        _deployer = deployer;
        _invoker = invoker;
        _walletManager = walletManager;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Approach 1: Compile and deploy a contract from source
    /// </summary>
    /// <param name="compilationOptions">Compilation options</param>
    /// <param name="deploymentOptions">Deployment options</param>
    /// <param name="initParams">Optional initialization parameters</param>
    /// <returns>Deployment result with contract hash</returns>
    public async Task<ContractDeploymentInfo> CompileAndDeployAsync(
        CompilationOptions compilationOptions,
        DeploymentOptions deploymentOptions,
        object[]? initParams = null)
    {
        _logger.LogInformation("Starting compile-and-deploy process for {SourcePath}", compilationOptions.SourcePath);

        // Step 1: Compile the contract
        var compiledContract = await _compiler.CompileAsync(compilationOptions);
        _logger.LogInformation("Contract {Name} compiled successfully", compiledContract.Name);

        // Step 2: Deploy the compiled contract
        var deploymentResult = await _deployer.DeployAsync(compiledContract, deploymentOptions, initParams);
        _logger.LogInformation("Contract {Name} deployed with hash {Hash}",
            compiledContract.Name, deploymentResult.ContractHash);

        return deploymentResult;
    }

    /// <summary>
    /// Approach 2: Deploy from pre-compiled artifacts
    /// </summary>
    /// <param name="nefFilePath">Path to compiled NEF file</param>
    /// <param name="manifestFilePath">Path to contract manifest file</param>
    /// <param name="deploymentOptions">Deployment options</param>
    /// <returns>Deployment result with contract hash</returns>
    public async Task<ContractDeploymentInfo> DeployFromArtifactsAsync(
        string nefFilePath,
        string manifestFilePath,
        DeploymentOptions deploymentOptions)
    {
        _logger.LogInformation("Starting artifact-based deployment for {NefFile}", nefFilePath);

        // Step 1: Load compiled artifacts
        var compiledContract = await _compiler.LoadAsync(nefFilePath, manifestFilePath);
        _logger.LogInformation("Contract artifacts loaded for {Name}", compiledContract.Name);

        // Step 2: Deploy the loaded contract
        var deploymentResult = await _deployer.DeployAsync(compiledContract, deploymentOptions);
        _logger.LogInformation("Contract {Name} deployed with hash {Hash}",
            compiledContract.Name, deploymentResult.ContractHash);

        return deploymentResult;
    }

    /// <summary>
    /// Update an existing contract
    /// </summary>
    /// <param name="contractHash">Hash of existing contract</param>
    /// <param name="compilationOptions">New contract compilation options (for approach 1)</param>
    /// <param name="deploymentOptions">Deployment options</param>
    /// <returns>Update result</returns>
    public async Task<ContractDeploymentInfo> UpdateContractAsync(
        UInt160 contractHash,
        CompilationOptions compilationOptions,
        DeploymentOptions deploymentOptions)
    {
        _logger.LogInformation("Starting contract update for {ContractHash}", contractHash);

        // Step 1: Compile the new version
        var compiledContract = await _compiler.CompileAsync(compilationOptions);
        _logger.LogInformation("New contract version compiled for {Name}", compiledContract.Name);

        // Step 2: Update the contract
        var updateResult = await _deployer.UpdateAsync(compiledContract, contractHash, deploymentOptions);
        _logger.LogInformation("Contract {Hash} updated successfully", contractHash);

        return updateResult;
    }

    /// <summary>
    /// Update an existing contract from artifacts
    /// </summary>
    /// <param name="contractHash">Hash of existing contract</param>
    /// <param name="nefFilePath">Path to new NEF file</param>
    /// <param name="manifestFilePath">Path to new manifest file</param>
    /// <param name="deploymentOptions">Deployment options</param>
    /// <returns>Update result</returns>
    public async Task<ContractDeploymentInfo> UpdateContractFromArtifactsAsync(
        UInt160 contractHash,
        string nefFilePath,
        string manifestFilePath,
        DeploymentOptions deploymentOptions)
    {
        _logger.LogInformation("Starting contract update from artifacts for {ContractHash}", contractHash);

        // Step 1: Load new artifacts
        var compiledContract = await _compiler.LoadAsync(nefFilePath, manifestFilePath);
        _logger.LogInformation("New contract artifacts loaded for {Name}", compiledContract.Name);

        // Step 2: Update the contract
        var updateResult = await _deployer.UpdateAsync(compiledContract, contractHash, deploymentOptions);
        _logger.LogInformation("Contract {Hash} updated successfully", contractHash);

        return updateResult;
    }

    /// <summary>
    /// Invoke a deployed contract method
    /// </summary>
    /// <param name="contractHash">Contract hash to invoke</param>
    /// <param name="method">Method name</param>
    /// <param name="parameters">Method parameters</param>
    /// <returns>Transaction hash</returns>
    public async Task<UInt256> InvokeContractAsync(
        UInt160 contractHash,
        string method,
        params object[] parameters)
    {
        _logger.LogInformation("Invoking contract {ContractHash} method {Method}", contractHash, method);

        var txHash = await _invoker.SendAsync(contractHash, method, parameters);

        _logger.LogInformation("Contract invocation transaction sent: {TxHash}", txHash);

        return txHash;
    }

    /// <summary>
    /// Test invoke a contract method (read-only)
    /// </summary>
    /// <param name="contractHash">Contract hash to test</param>
    /// <param name="method">Method name</param>
    /// <param name="parameters">Method parameters</param>
    /// <returns>Result value</returns>
    public async Task<T?> CallContractAsync<T>(
        UInt160 contractHash,
        string method,
        params object[] parameters)
    {
        _logger.LogInformation("Calling contract {ContractHash} method {Method}", contractHash, method);

        var result = await _invoker.CallAsync<T>(contractHash, method, parameters);

        _logger.LogInformation("Contract call completed");

        return result;
    }

    /// <summary>
    /// Check if a contract exists on the network
    /// </summary>
    /// <param name="contractHash">Contract hash to check</param>
    /// <returns>True if contract exists</returns>
    public async Task<bool> ContractExistsAsync(UInt160 contractHash)
    {
        var networkConfig = _configuration.GetSection("Network").Get<NetworkConfiguration>();
        if (networkConfig == null || string.IsNullOrEmpty(networkConfig.RpcUrl))
        {
            throw new InvalidOperationException("Network configuration not found. Please check appsettings.json");
        }

        return await _deployer.ContractExistsAsync(contractHash, networkConfig.RpcUrl);
    }


    /// <summary>
    /// Load wallet for signing transactions
    /// </summary>
    /// <param name="walletPath">Path to wallet file</param>
    /// <param name="password">Wallet password</param>
    /// <returns>Wallet manager instance</returns>
    public async Task LoadWalletAsync(string walletPath, string password)
    {
        await _walletManager.LoadWalletAsync(walletPath, password);
        _logger.LogInformation("Wallet loaded from {WalletPath}", walletPath);
    }

    /// <summary>
    /// Load wallet from configuration settings
    /// </summary>
    /// <returns>Task</returns>
    public async Task LoadWalletFromConfigurationAsync()
    {
        var networkConfig = _configuration.GetSection("Network").Get<NetworkConfiguration>();
        if (networkConfig?.Wallet == null || string.IsNullOrEmpty(networkConfig.Wallet.WalletPath))
        {
            throw new InvalidOperationException("Wallet configuration not found. Please check appsettings.json");
        }

        await _walletManager.LoadWalletAsync(networkConfig.Wallet.WalletPath, networkConfig.Wallet.Password);
        _logger.LogInformation("Wallet loaded from configuration: {WalletPath}", networkConfig.Wallet.WalletPath);
    }

    /// <summary>
    /// Get deployer account from loaded wallet
    /// </summary>
    /// <param name="accountAddress">Account address (optional - uses default if not specified)</param>
    /// <returns>Account script hash</returns>
    public UInt160 GetDeployerAccount(string? accountAddress = null)
    {
        return _walletManager.GetAccount(accountAddress);
    }
}
