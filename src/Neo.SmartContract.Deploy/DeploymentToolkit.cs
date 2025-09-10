using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Neo;
using Neo.Wallets;
using Neo.Network.RPC;
using Neo.Extensions;
using Neo.VM;
using Neo.SmartContract.Native;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Manifest;
using System.Numerics;

namespace Neo.SmartContract.Deploy;

/// <summary>
/// Simplified deployment toolkit for Neo smart contract deployment (PR 1 - Basic Framework)
/// Note: This is a minimal implementation. Full functionality will be added in subsequent PRs.
/// </summary>
public class DeploymentToolkit : IDisposable
{
    private const string MAINNET_RPC_URL = "https://rpc10.n3.nspcc.ru:10331";
    private const string TESTNET_RPC_URL = "http://seed2t5.neo.org:20332";
    private const string LOCAL_RPC_URL = "http://localhost:50012";
    private const string DEFAULT_RPC_URL = "http://localhost:10332";

    private readonly IConfiguration _configuration;
    private volatile string? _currentNetwork = null;
    private volatile string? _wifKey = null;
    private volatile string? _rpcUrlOverride = null;
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
                _rpcUrlOverride = MAINNET_RPC_URL;
                Environment.SetEnvironmentVariable("Network__RpcUrl", MAINNET_RPC_URL);
                break;

            case "testnet":
                _rpcUrlOverride = TESTNET_RPC_URL;
                Environment.SetEnvironmentVariable("Network__RpcUrl", TESTNET_RPC_URL);
                break;

            case "local":
            case "private":
                _rpcUrlOverride = LOCAL_RPC_URL;
                Environment.SetEnvironmentVariable("Network__RpcUrl", LOCAL_RPC_URL);
                break;

            default:
                // Assume it's a custom RPC URL
                if (network.StartsWith("http"))
                {
                    _rpcUrlOverride = network;
                    Environment.SetEnvironmentVariable("Network__RpcUrl", network);
                }
                break;
        }

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
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid WIF key: {ex.Message}", nameof(wifKey));
        }

        return this;
    }

    /// <summary>
    /// Deploy a contract from source code or project (Stub - Implementation in PR 2)
    /// </summary>
    /// <param name="path">Path to contract project (.csproj) or source file</param>
    /// <param name="initParams">Optional initialization parameters</param>
    /// <returns>Deployment information</returns>
    public async Task<ContractDeploymentInfo> DeployAsync(string path, object[]? initParams = null)
    {
        await Task.Delay(1); // Simulate async work
        throw new NotImplementedException("DeployAsync will be implemented in PR 2 - Full Deployment Functionality");
    }

    /// <summary>
    /// Deploy a pre-compiled contract from NEF and manifest files (Stub - Implementation in PR 2)
    /// </summary>
    /// <param name="nefPath">Path to NEF file</param>
    /// <param name="manifestPath">Path to manifest file</param>
    /// <param name="initParams">Optional initialization parameters</param>
    /// <returns>Deployment information</returns>
    public async Task<ContractDeploymentInfo> DeployArtifactsAsync(string nefPath, string manifestPath, object[]? initParams = null, bool waitForConfirmation = false, int confirmationRetries = 30, int confirmationDelaySeconds = 5)
    {
        if (string.IsNullOrWhiteSpace(nefPath) || string.IsNullOrWhiteSpace(manifestPath))
            throw new ArgumentException("nefPath and manifestPath are required.");

        if (!File.Exists(nefPath) || !File.Exists(manifestPath))
            throw new FileNotFoundException("NEF or manifest file not found.");

        if (string.IsNullOrEmpty(_wifKey))
            throw new InvalidOperationException("WIF key not set. Call SetWifKey() first.");

        var nefBytes = await File.ReadAllBytesAsync(nefPath);
        var manifestJson = await File.ReadAllTextAsync(manifestPath);

        // Compute expected contract hash
        var nef = NefFile.Parse(nefBytes, verify: true);
        var manifest = ContractManifest.FromJson((Neo.Json.JObject)Neo.Json.JToken.Parse(manifestJson)!);
        var sender = await GetDeployerAccountAsync();
        var expectedHash = Neo.SmartContract.Helper.GetContractHash(sender, nef.CheckSum, manifest.Name);

        // Build deploy script
        var script = BuildDeployScript(nefBytes, manifestJson, initParams);

        var rpcUrl = GetCurrentRpcUrl();
        using var rpc = new RpcClient(new Uri(rpcUrl), null, null, ProtocolSettings.Default);

        // Build transaction
        var signer = new Signer { Account = sender, Scopes = WitnessScope.CalledByEntry };
        var tm = await TransactionManager.MakeTransactionAsync(rpc, script, [signer]);

        // Sign and send
        var key = Neo.Network.RPC.Utility.GetKeyPair(_wifKey);
        tm.AddSignature(key);
        var tx = await tm.SignAsync();
        var txHash = await rpc.SendRawTransactionAsync(tx);

        if (waitForConfirmation)
        {
            await WaitForConfirmationAsync(rpc, txHash, confirmationRetries, confirmationDelaySeconds);
        }

        return new ContractDeploymentInfo
        {
            TransactionHash = txHash,
            ContractHash = expectedHash
        };
    }

    /// <summary>
    /// Call a contract method (read-only) (Stub - Implementation in PR 2)
    /// </summary>
    /// <typeparam name="T">Return type</typeparam>
    /// <param name="contractHashOrAddress">Contract hash or address</param>
    /// <param name="method">Method name</param>
    /// <param name="args">Method arguments</param>
    /// <returns>Method return value</returns>
    public async Task<T> CallAsync<T>(string contractHashOrAddress, string method, params object[] args)
    {
        var rpcUrl = GetCurrentRpcUrl();
        using var rpc = new RpcClient(new Uri(rpcUrl), null, null, ProtocolSettings.Default);

        var hash = Neo.Network.RPC.Utility.GetScriptHash(contractHashOrAddress, ProtocolSettings.Default);
        var script = BuildContractCallScript(hash, method, CallFlags.ReadOnly, args);
        var result = await rpc.InvokeScriptAsync(script);

        if (result.State.HasFlag(Neo.VM.VMState.FAULT))
            throw new InvalidOperationException($"Call fault: {result.Exception}");

        if (result.Stack == null || result.Stack.Length == 0)
            return default!;

        var item = result.Stack[0];
        object? value = ConvertStackItem<T>(item);
        return (T)value!;
    }

    /// <summary>
    /// Invoke a contract method (state-changing transaction) (Stub - Implementation in PR 2)
    /// </summary>
    /// <param name="contractHashOrAddress">Contract hash or address</param>
    /// <param name="method">Method name</param>
    /// <param name="args">Method arguments</param>
    /// <returns>Transaction hash</returns>
    public async Task<UInt256> InvokeAsync(string contractHashOrAddress, string method, params object[] args)
    {
        if (string.IsNullOrEmpty(_wifKey))
            throw new InvalidOperationException("WIF key not set. Call SetWifKey() first.");

        var rpcUrl = GetCurrentRpcUrl();
        using var rpc = new RpcClient(new Uri(rpcUrl), null, null, ProtocolSettings.Default);

        var sender = await GetDeployerAccountAsync();
        var hash = Neo.Network.RPC.Utility.GetScriptHash(contractHashOrAddress, ProtocolSettings.Default);
        var script = BuildContractCallScript(hash, method, CallFlags.All, args);

        var signer = new Signer { Account = sender, Scopes = WitnessScope.CalledByEntry };
        var tm = await TransactionManager.MakeTransactionAsync(rpc, script, [signer]);

        var key = Neo.Network.RPC.Utility.GetKeyPair(_wifKey);
        tm.AddSignature(key);
        var tx = await tm.SignAsync();
        return await rpc.SendRawTransactionAsync(tx);
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
    /// Get the current balance of an account (Stub - Implementation in PR 2)
    /// </summary>
    /// <param name="address">Account address (null for default deployer)</param>
    /// <returns>GAS balance</returns>
    public async Task<decimal> GetGasBalanceAsync(string? address = null)
    {
        var rpcUrl = GetCurrentRpcUrl();
        using var rpc = new RpcClient(new Uri(rpcUrl), null, null, ProtocolSettings.Default);

        UInt160 account = !string.IsNullOrEmpty(address)
            ? Neo.Network.RPC.Utility.GetScriptHash(address, ProtocolSettings.Default)
            : await GetDeployerAccountAsync();

        var nep17 = new Nep17API(rpc);
        var balance = await nep17.BalanceOfAsync(NativeContract.GAS.Hash, account);
        var decimals = await nep17.DecimalsAsync(NativeContract.GAS.Hash);
        var factor = BigInteger.Pow(10, (int)decimals);
        return (decimal)balance / (decimal)factor;
    }

    /// <summary>
    /// Deploy multiple contracts from a manifest file (Stub - Implementation in PR 2)
    /// </summary>
    /// <param name="manifestPath">Path to the deployment manifest JSON file</param>
    /// <returns>Dictionary of contract names to deployment information</returns>
    public async Task<Dictionary<string, ContractDeploymentInfo>> DeployFromManifestAsync(string manifestPath)
    {
        await Task.CompletedTask;
        throw new NotSupportedException("Deploying multiple contracts from a manifest is not supported in this minimal API.");
    }

    /// <summary>
    /// Check if a contract exists at the given address (Stub - Implementation in PR 2)
    /// </summary>
    /// <param name="contractHashOrAddress">Contract hash or address</param>
    /// <returns>True if contract exists, false otherwise</returns>
    public async Task<bool> ContractExistsAsync(string contractHashOrAddress)
    {
        var rpcUrl = GetCurrentRpcUrl();
        using var rpc = new RpcClient(new Uri(rpcUrl), null, null, ProtocolSettings.Default);
        try
        {
            var hash = Neo.Network.RPC.Utility.GetScriptHash(contractHashOrAddress, ProtocolSettings.Default).ToString();
            var _ = await rpc.GetContractStateAsync(hash);
            return true;
        }
        catch
        {
            return false;
        }
    }

    #region Private Methods

    private string GetCurrentRpcUrl()
    {
        // Highest priority: explicit override set by SetNetwork()
        if (!string.IsNullOrEmpty(_rpcUrlOverride))
        {
            return _rpcUrlOverride!;
        }

        if (!string.IsNullOrEmpty(_currentNetwork))
        {
            var networks = _configuration.GetSection("Network:Networks").Get<Dictionary<string, NetworkConfiguration>>();
            if (networks != null && networks.TryGetValue(_currentNetwork, out var network))
            {
                return network.RpcUrl;
            }
        }

        // Fallback to default RPC URL (env var configured earlier may not be visible in configuration)
        var envRpc = Environment.GetEnvironmentVariable("Network__RpcUrl");
        if (!string.IsNullOrWhiteSpace(envRpc)) return envRpc;

        return _configuration["Network:RpcUrl"] ?? DEFAULT_RPC_URL;
    }

    private async Task<uint> GetNetworkMagicAsync()
    {
        // Check if NetworkMagic is explicitly configured
        if (!string.IsNullOrEmpty(_currentNetwork))
        {
            var networks = _configuration.GetSection("Network:Networks").Get<Dictionary<string, NetworkConfiguration>>();
            if (networks != null && networks.TryGetValue(_currentNetwork, out var network) && network.NetworkMagic.HasValue)
            {
                return network.NetworkMagic.Value;
            }
        }

        // Check configuration for NetworkMagic
        var configuredMagic = _configuration.GetValue<uint?>("Network:NetworkMagic", null);
        if (configuredMagic.HasValue)
        {
            return configuredMagic.Value;
        }

        // Retrieve from RPC
        try
        {
            var rpcUrl = GetCurrentRpcUrl();
            using var rpcClient = new RpcClient(new Uri(rpcUrl), null, null, ProtocolSettings.Default);
            var version = await rpcClient.GetVersionAsync();
            return version.Protocol.Network;
        }
        catch (Exception)
        {
            // Fallback to known values based on network name
            return _currentNetwork?.ToLower() switch
            {
                "mainnet" => 860833102,
                "testnet" => 894710606,
                _ => 894710606 // Default to testnet
            };
        }
    }

    #endregion

    #region Helpers

    private static async Task<bool> WaitForConfirmationAsync(RpcClient rpc, UInt256 txHash, int retries, int delaySeconds)
    {
        for (int i = 0; i < retries; i++)
        {
            try
            {
                var height = await rpc.GetTransactionHeightAsync(txHash.ToString());
                if (height > 0)
                    return true;
            }
            catch
            {
                // Not yet confirmed or node not returning height
            }
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        }
        return false;
    }

    private static byte[] BuildDeployScript(byte[] nefBytes, string manifestJson, object[]? initParams)
    {
        using var sb = new ScriptBuilder();
        // Build args in reverse order and PACK
        if (initParams is { Length: > 0 })
        {
            // data (pack array if multiple)
            for (int i = initParams.Length - 1; i >= 0; i--) sb.EmitPush(initParams[i]!);
            sb.EmitPush(initParams.Length);
            sb.Emit(OpCode.PACK);
            // manifest
            sb.EmitPush(manifestJson);
            // nef bytes
            sb.EmitPush(nefBytes);
            // pack [nef, manifest, data]
            sb.EmitPush(3);
            sb.Emit(OpCode.PACK);
        }
        else
        {
            // manifest
            sb.EmitPush(manifestJson);
            // nef bytes
            sb.EmitPush(nefBytes);
            // pack [nef, manifest]
            sb.EmitPush(2);
            sb.Emit(OpCode.PACK);
        }

        // call ContractManagement.deploy
        sb.EmitPush(CallFlags.All);
        sb.EmitPush("deploy");
        sb.EmitPush(NativeContract.ContractManagement.Hash);
        sb.EmitSysCall(ApplicationEngine.System_Contract_Call);

        return sb.ToArray();
    }

    private static byte[] BuildContractCallScript(UInt160 scriptHash, string method, CallFlags flags, params object[] args)
    {
        using var sb = new ScriptBuilder();
        if (args is { Length: > 0 })
        {
            for (int i = args.Length - 1; i >= 0; i--) sb.EmitPush(args[i]!);
            sb.EmitPush(args.Length);
            sb.Emit(OpCode.PACK);
        }
        else
        {
            sb.Emit(OpCode.NEWARRAY0);
        }
        sb.EmitPush((byte)flags);
        sb.EmitPush(method);
        sb.EmitPush(scriptHash);
        sb.EmitSysCall(ApplicationEngine.System_Contract_Call);
        return sb.ToArray();
    }

    private static object? ConvertStackItem<T>(Neo.VM.Types.StackItem item)
    {
        var target = typeof(T);
        if (target == typeof(string)) return item.GetString();
        if (target == typeof(bool)) return item.GetBoolean();
        if (target == typeof(int)) return (int)item.GetInteger();
        if (target == typeof(long)) return (long)item.GetInteger();
        if (target == typeof(BigInteger)) return item.GetInteger();
        if (target == typeof(byte[])) return item.GetSpan().ToArray();
        if (target == typeof(UInt160)) return new UInt160(item.GetSpan());
        if (target == typeof(UInt256)) return new UInt256(item.GetSpan());
        return item.GetString();
    }

    #endregion

    #region Minimal Models (PR1)

    internal class NetworkConfiguration
    {
        public string RpcUrl { get; set; } = string.Empty;
        public uint? NetworkMagic { get; set; }
    }

    public record ContractDeploymentInfo
    {
        public UInt256? TransactionHash { get; init; }
        public UInt160? ContractHash { get; init; }
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
            // no managed resources to dispose

            _disposed = true;
        }
    }

    #endregion
}
