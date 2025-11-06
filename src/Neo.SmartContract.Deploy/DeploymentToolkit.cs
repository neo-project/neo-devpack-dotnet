using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
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
using System.Globalization;
using Neo.Compiler;
using CompilationOptions = Neo.Compiler.CompilationOptions;

namespace Neo.SmartContract.Deploy;

/// <summary>
/// Deployment toolkit for Neo smart contract deployment.
/// </summary>
public class DeploymentToolkit : IDisposable
{
    private const string DefaultRpcUrl = "http://localhost:10332";

    private IConfiguration _configuration = default!;
    private IRpcClientFactory _rpcClientFactory = default!;
    private DeploymentOptions _options = default!;
    private readonly bool _optionsExplicitlyProvided;
    private NetworkProfile _networkProfile = default!;
    private volatile string? _wifKey = null;
    private ProtocolSettings? _protocolSettings;
    private bool _networkMagicFetchedFromRpc;
    private bool _networkMagicFallbackActive;
    private DateTime _networkMagicLastAttemptUtc;
    private bool _disposed = false;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    internal static TimeSpan NetworkMagicRetryInterval { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Create a new DeploymentToolkit instance with automatic configuration
    /// </summary>
    /// <param name="configPath">Optional path to configuration file. Defaults to appsettings.json in current directory</param>
    public DeploymentToolkit(string? configPath = null, DeploymentOptions? options = null, IRpcClientFactory? rpcClientFactory = null)
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
        _optionsExplicitlyProvided = options is not null;
        Initialize(builder.Build(), options, rpcClientFactory);
    }

    public DeploymentToolkit(IConfiguration configuration, DeploymentOptions? options = null, IRpcClientFactory? rpcClientFactory = null)
    {
        _optionsExplicitlyProvided = options is not null;
        Initialize(configuration, options, rpcClientFactory);
    }

    public NetworkProfile CurrentNetwork => _networkProfile;

    public DeploymentOptions Options => _options.Clone();

    internal string? CurrentWif => _wifKey;

    private void Initialize(IConfiguration configuration, DeploymentOptions? options, IRpcClientFactory? rpcClientFactory)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _rpcClientFactory = rpcClientFactory ?? new DefaultRpcClientFactory();
        _options = (options ?? new DeploymentOptions()).Clone();

        _networkProfile = ResolveInitialNetworkProfile();
        _networkMagicFetchedFromRpc = false;
        _networkMagicFallbackActive = false;
        _networkMagicLastAttemptUtc = DateTime.MinValue;
    }

    /// <summary>
    /// Set the network to use (mainnet, testnet, or custom RPC URL)
    /// </summary>
    /// <param name="network">Network name or RPC URL</param>
    /// <returns>This instance for chaining</returns>
    /// <exception cref="ArgumentException">Thrown when network is invalid</exception>
    public DeploymentToolkit SetNetwork(string network)
    {
        EnsureNotDisposed();

        if (string.IsNullOrWhiteSpace(network))
            throw new ArgumentException("Network cannot be null or empty", nameof(network));

        return UseNetwork(ResolveNetworkProfile(network));
    }

    public DeploymentToolkit UseNetwork(NetworkProfile profile)
    {
        EnsureNotDisposed();

        _networkProfile = profile ?? throw new ArgumentNullException(nameof(profile));
        _options.Network = profile;
        _protocolSettings = null;
        _networkMagicFetchedFromRpc = false;
        _networkMagicFallbackActive = false;
        _networkMagicLastAttemptUtc = DateTime.MinValue;
        return this;
    }

    public DeploymentToolkit ConfigureOptions(Action<DeploymentOptions> configure)
    {
        EnsureNotDisposed();
        ArgumentNullException.ThrowIfNull(configure);

        var updated = _options.Clone();
        configure(updated);
        _options = updated;

        if (updated.Network is not null)
        {
            UseNetwork(updated.Network);
        }

        return this;
    }

    private NetworkProfile ResolveInitialNetworkProfile()
    {
        var configured = TryResolveConfiguredNetworkProfile();

        if (_optionsExplicitlyProvided && _options.Network is not null)
        {
            return _options.Network;
        }

        if (configured is not null)
        {
            _options.Network = configured;
            return configured;
        }

        if (_options.Network is not null)
        {
            return _options.Network;
        }

        var fallback = CreateDefaultProfile();
        _options.Network = fallback;
        return fallback;
    }

    private NetworkProfile? TryResolveConfiguredNetworkProfile()
    {
        var configuredName = _configuration["Network:Network"];
        if (!string.IsNullOrWhiteSpace(configuredName))
        {
            try
            {
                return ResolveNetworkProfile(configuredName);
            }
            catch (ArgumentException)
            {
                // ignore invalid configuration and fall back
            }
        }

        var configuredUrl = _configuration["Network:RpcUrl"];
        if (!string.IsNullOrWhiteSpace(configuredUrl) && Uri.TryCreate(configuredUrl, UriKind.Absolute, out var uri))
        {
            var normalized = configuredUrl.Trim();
            var magic = _configuration.GetValue<uint?>("Network:NetworkMagic", null);
            var addressVersion = _configuration.GetValue<byte?>("Network:AddressVersion", null);
            return new NetworkProfile(configuredName ?? uri.Host, normalized, magic, addressVersion);
        }

        return null;
    }

    private NetworkProfile ResolveNetworkProfile(string network)
    {
        var trimmed = network.Trim();
        if (Uri.TryCreate(trimmed, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            return new NetworkProfile(uri.Host, trimmed);
        }

        if (NetworkProfile.TryGetKnown(trimmed, out var known))
        {
            return known;
        }

        var networksSection = _configuration.GetSection("Network:Networks");
        var configuredNetworks = networksSection.Get<Dictionary<string, NetworkConfiguration>>();
        if (configuredNetworks != null)
        {
            foreach (var entry in configuredNetworks)
            {
                if (string.Equals(entry.Key, trimmed, StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrWhiteSpace(entry.Value.RpcUrl))
                {
                    return new NetworkProfile(entry.Key, entry.Value.RpcUrl, entry.Value.NetworkMagic, entry.Value.AddressVersion);
                }
            }
        }

        throw new ArgumentException($"Unknown network '{network}'. Provide a known network name or an RPC URL.", nameof(network));
    }

    private static NetworkProfile CreateDefaultProfile()
        => new("default", DefaultRpcUrl);

    /// <summary>
    /// Set the WIF (Wallet Import Format) key for signing transactions
    /// </summary>
    /// <param name="wifKey">The WIF private key</param>
    /// <returns>The deployment toolkit instance for chaining</returns>
    /// <exception cref="ArgumentException">Thrown when WIF key is invalid</exception>
    public DeploymentToolkit SetWifKey(string wifKey)
    {
        EnsureNotDisposed();

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
    /// Compile and deploy a smart contract from source (csproj or single C# file).
    /// </summary>
    /// <param name="path">Path to the project or source file.</param>
    /// <param name="initParams">Optional initialization parameters supplied to the deploy script.</param>
    /// <param name="targetContract">Optional contract name when the project builds multiple contracts.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Deployment information.</returns>
    public virtual async Task<ContractDeploymentInfo> DeployAsync(
        string path,
        object?[]? initParams = null,
        string? targetContract = null,
        CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));

        var fullPath = Path.GetFullPath(path);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException("Contract project or source file not found.", fullPath);

        var compilationOptions = await CreateCompilationOptionsAsync(
            Path.GetFileNameWithoutExtension(fullPath),
            cancellationToken).ConfigureAwait(false);

        var artifacts = await CompileContractsAsync(
            fullPath,
            compilationOptions,
            targetContract,
            cancellationToken).ConfigureAwait(false);

        var artifact = artifacts[0];
        return await DeployCompiledArtifactAsync(artifact, initParams, cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<IReadOnlyList<CompiledContractArtifact>> CompileAsync(
        string path,
        string? targetContract = null,
        CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));

        var fullPath = Path.GetFullPath(path);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException("Contract project or source file not found.", fullPath);

        var options = await CreateCompilationOptionsAsync(
            Path.GetFileNameWithoutExtension(fullPath),
            cancellationToken).ConfigureAwait(false);

        return await CompileContractsAsync(fullPath, options, targetContract, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deploy a pre-compiled contract from NEF and manifest files (Stub - Implementation in PR 2)
    /// </summary>
    /// <param name="nefPath">Path to NEF file</param>
    /// <param name="manifestPath">Path to manifest file</param>
    /// <param name="initParams">Optional initialization parameters</param>
    /// <returns>Deployment information</returns>
    public virtual Task<ContractDeploymentInfo> DeployArtifactsAsync(
        DeploymentArtifactsRequest request,
        CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        ArgumentNullException.ThrowIfNull(request);
        return DeployArtifactsInternalAsync(
            request.NefPath,
            request.ManifestPath,
            request.InitParams ?? Array.Empty<object?>(),
            request.WaitForConfirmation,
            request.ConfirmationRetries,
            request.ConfirmationDelaySeconds,
            cancellationToken,
            request.Signers,
            request.TransactionSignerAsync);
    }

    public virtual Task<ContractDeploymentInfo> DeployArtifactsAsync(
        string nefPath,
        string manifestPath,
        object?[]? initParams = null,
        bool? waitForConfirmation = null,
        int? confirmationRetries = null,
        int? confirmationDelaySeconds = null,
        CancellationToken cancellationToken = default,
        IReadOnlyList<Signer>? signers = null,
        Func<TransactionManager, CancellationToken, Task<Transaction>>? transactionSignerAsync = null)
    {
        EnsureNotDisposed();
        return DeployArtifactsInternalAsync(
            nefPath,
            manifestPath,
            initParams ?? Array.Empty<object?>(),
            waitForConfirmation,
            confirmationRetries,
            confirmationDelaySeconds,
            cancellationToken,
            signers,
            transactionSignerAsync);
    }

    private async Task<ContractDeploymentInfo> DeployArtifactsInternalAsync(
        string nefPath,
        string manifestPath,
        object?[] initParams,
        bool? waitForConfirmation,
        int? confirmationRetries,
        int? confirmationDelaySeconds,
        CancellationToken cancellationToken,
        IReadOnlyList<Signer>? signers,
        Func<TransactionManager, CancellationToken, Task<Transaction>>? transactionSignerAsync)
    {
        EnsureNotDisposed();
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(nefPath))
            throw new ArgumentException("NEF path is required.", nameof(nefPath));

        if (string.IsNullOrWhiteSpace(manifestPath))
            throw new ArgumentException("Manifest path is required.", nameof(manifestPath));

        if (!File.Exists(nefPath))
            throw new FileNotFoundException("NEF file not found.", nefPath);

        if (!File.Exists(manifestPath))
            throw new FileNotFoundException("Manifest file not found.", manifestPath);

        var nefBytes = await File.ReadAllBytesAsync(nefPath, cancellationToken).ConfigureAwait(false);
        var manifestJson = await File.ReadAllTextAsync(manifestPath, cancellationToken).ConfigureAwait(false);

        // Compute expected contract hash
        var nef = NefFile.Parse(nefBytes, verify: true);
        var manifest = ContractManifest.FromJson((Neo.Json.JObject)Neo.Json.JToken.Parse(manifestJson)!);

        var protocolSettings = await GetProtocolSettingsAsync().ConfigureAwait(false);
        var resolvedSigners = ResolveSigners(signers, protocolSettings);

        string? defaultWif = null;
        UInt160 sender;
        if (resolvedSigners.Count > 0)
        {
            sender = resolvedSigners[0].Account;
        }
        else
        {
            defaultWif = EnsureWif();
            var keyPair = Neo.Network.RPC.Utility.GetKeyPair(defaultWif);
            sender = Neo.SmartContract.Contract.CreateSignatureContract(keyPair.PublicKey).ScriptHash;
            resolvedSigners = new[]
            {
                new Signer { Account = sender, Scopes = WitnessScope.CalledByEntry }
            };
        }

        var expectedHash = Neo.SmartContract.Helper.GetContractHash(sender, nef.CheckSum, manifest.Name);

        // Build deploy script
        var script = BuildDeployScript(nefBytes, manifestJson, initParams);

        var policy = ResolveConfirmationPolicy(waitForConfirmation, confirmationRetries, confirmationDelaySeconds);
        var signerDelegate = transactionSignerAsync
            ?? _options.TransactionSignerAsync;

        if (signerDelegate is null)
        {
            if (string.IsNullOrEmpty(defaultWif))
            {
                throw new InvalidOperationException("No signing credentials available. Provide a WIF via SetWifKey(), configure DeploymentOptions.TransactionSignerAsync, or supply a transaction signer when calling DeployArtifactsAsync.");
            }
            var key = Neo.Network.RPC.Utility.GetKeyPair(defaultWif);
            signerDelegate = (tm, ct) =>
            {
                tm.AddSignature(key);
                return tm.SignAsync();
            };
        }

        var signersArray = resolvedSigners is Signer[] direct
            ? direct
            : resolvedSigners.ToArray();

        return await WithRpcClientAsync(async (rpc, _, ct) =>
        {
            ct.ThrowIfCancellationRequested();
            // Build transaction
            var tm = await CreateTransactionManagerAsync(rpc, script.AsMemory(), signersArray, ct).ConfigureAwait(false);

            // Sign and send
            var tx = await signerDelegate(tm, ct).ConfigureAwait(false);
            var txHash = await rpc.SendRawTransactionAsync(tx).ConfigureAwait(false);

            if (policy.WaitForConfirmation)
            {
                var confirmed = await WaitForConfirmationAsync(
                    rpc,
                    txHash,
                    policy.ConfirmationRetries,
                    TimeSpan.FromSeconds(policy.ConfirmationDelaySeconds),
                    ct).ConfigureAwait(false);

                if (!confirmed)
                {
                    throw new TimeoutException($"Transaction {txHash} was not confirmed within the allotted retries.");
                }
            }

            return new ContractDeploymentInfo
            {
                TransactionHash = txHash,
                ContractHash = expectedHash
            };
        }, cancellationToken).ConfigureAwait(false);
    }

    private async Task<CompilationOptions> CreateCompilationOptionsAsync(string baseName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var protocolSettings = await GetProtocolSettingsAsync().ConfigureAwait(false);

        return new CompilationOptions
        {
            AddressVersion = protocolSettings.AddressVersion,
            BaseName = baseName,
            Nullable = NullableContextOptions.Enable,
            Optimize = CompilationOptions.OptimizationType.Basic
        };
    }

    protected virtual CompilationEngine CreateCompilationEngine(CompilationOptions options) => new(options);

    protected virtual Task<TransactionManager> CreateTransactionManagerAsync(
        RpcClient rpcClient,
        ReadOnlyMemory<byte> script,
        Signer[] signers,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return TransactionManager.MakeTransactionAsync(rpcClient, script, signers);
    }

    protected virtual Task<IReadOnlyList<CompiledContractArtifact>> CompileContractsAsync(
        string path,
        CompilationOptions compilationOptions,
        string? targetContractName,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var extension = Path.GetExtension(path).ToLowerInvariant();
        var engine = CreateCompilationEngine(compilationOptions);

        List<CompilationContext> contexts = extension switch
        {
            ".csproj" => engine.CompileProject(path),
            ".cs" => engine.CompileSources(path),
            _ => throw new NotSupportedException($"Unsupported contract source type '{extension}'. Provide a .csproj or .cs file.")
        } ?? [];

        if (contexts.Count == 0)
            throw new InvalidOperationException("Compilation did not produce any smart contract classes.");

        var errors = contexts
            .SelectMany(ctx => ctx.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
            .ToList();

        if (errors.Count > 0)
            throw new InvalidOperationException(BuildCompilationErrorMessage(errors));

        if (!string.IsNullOrWhiteSpace(targetContractName))
        {
            var context = contexts.FirstOrDefault(c => string.Equals(c.ContractName, targetContractName, StringComparison.OrdinalIgnoreCase));
            if (context is null)
                throw new ArgumentException($"Contract '{targetContractName}' was not found in the compilation output.", nameof(targetContractName));
            contexts = new List<CompilationContext> { context };
        }
        else if (contexts.Count > 1)
        {
            var names = string.Join(", ", contexts.Select(c => c.ContractName ?? "<unnamed>"));
            throw new InvalidOperationException($"Multiple contracts were produced ({names}). Provide a target contract name.");
        }

        var baseFolder = Path.GetDirectoryName(path) ?? Directory.GetCurrentDirectory();
        var artifacts = contexts.Select(context =>
        {
            var (nef, manifest, _) = context.CreateResults(baseFolder);
            var name = context.ContractName ?? Path.GetFileNameWithoutExtension(path);
            return new CompiledContractArtifact(name, nef, manifest);
        }).ToList();

        return Task.FromResult<IReadOnlyList<CompiledContractArtifact>>(artifacts);
    }

    private async Task<ContractDeploymentInfo> DeployCompiledArtifactAsync(
        CompiledContractArtifact artifact,
        object?[]? initParams,
        CancellationToken cancellationToken)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);

        try
        {
            var contractName = string.IsNullOrWhiteSpace(artifact.ContractName)
                ? "contract"
                : artifact.ContractName;

            var nefPath = Path.Combine(tempDir, contractName + ".nef");
            var manifestPath = Path.Combine(tempDir, contractName + ".manifest.json");

            await File.WriteAllBytesAsync(nefPath, artifact.Nef.ToArray(), cancellationToken).ConfigureAwait(false);
            await File.WriteAllTextAsync(manifestPath, artifact.Manifest.ToJson().ToString(), cancellationToken).ConfigureAwait(false);

            return await DeployArtifactsAsync(
                nefPath,
                manifestPath,
                initParams,
                waitForConfirmation: null,
                confirmationRetries: null,
                confirmationDelaySeconds: null,
                cancellationToken: cancellationToken,
                signers: null,
                transactionSignerAsync: null).ConfigureAwait(false);
        }
        finally
        {
            try
            {
                if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
            }
            catch
            {
                // ignore cleanup exceptions
            }
        }
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
        EnsureNotDisposed();

        return await WithRpcClientAsync(async (rpc, protocolSettings, ct) =>
        {
            ct.ThrowIfCancellationRequested();
            var hash = Neo.Network.RPC.Utility.GetScriptHash(contractHashOrAddress, protocolSettings);
            var script = BuildContractCallScript(hash, method, CallFlags.ReadOnly, args);
            var result = await rpc.InvokeScriptAsync(script).ConfigureAwait(false);

            if (result.State.HasFlag(Neo.VM.VMState.FAULT))
                throw new InvalidOperationException($"Call fault: {result.Exception}");

            if (result.Stack == null || result.Stack.Length == 0)
                return default!;

            var item = result.Stack[0];
            object? value = ConvertStackItem<T>(item);
            return (T)value!;
        }, CancellationToken.None).ConfigureAwait(false);
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
        EnsureNotDisposed();
        var wif = EnsureWif();
        var sender = await GetDeployerAccountAsync().ConfigureAwait(false);

        return await WithRpcClientAsync(async (rpc, protocolSettings, ct) =>
        {
            ct.ThrowIfCancellationRequested();
            var hash = Neo.Network.RPC.Utility.GetScriptHash(contractHashOrAddress, protocolSettings);
            var script = BuildContractCallScript(hash, method, CallFlags.All, args);

            var signer = new Signer { Account = sender, Scopes = WitnessScope.CalledByEntry };
            var tm = await TransactionManager.MakeTransactionAsync(rpc, script, [signer]).ConfigureAwait(false);

            var key = Neo.Network.RPC.Utility.GetKeyPair(wif);
            tm.AddSignature(key);
            var tx = await tm.SignAsync().ConfigureAwait(false);
            return await rpc.SendRawTransactionAsync(tx).ConfigureAwait(false);
        }, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Get the default deployer account
    /// </summary>
    /// <returns>Deployer account script hash</returns>
    /// <exception cref="InvalidOperationException">Thrown when no deployer account is configured</exception>
    public Task<UInt160> GetDeployerAccountAsync()
    {
        EnsureNotDisposed();
        var wif = EnsureWif();

        var privateKey = Neo.Wallets.Wallet.GetPrivateKeyFromWIF(wif);
        var keyPair = new KeyPair(privateKey);
        var account = Neo.SmartContract.Contract.CreateSignatureContract(keyPair.PublicKey).ScriptHash;
        return Task.FromResult(account);
    }

    /// <summary>
    /// Get the current balance of an account (Stub - Implementation in PR 2)
    /// </summary>
    /// <param name="address">Account address (null for default deployer)</param>
    /// <returns>GAS balance</returns>
    public async Task<decimal> GetGasBalanceAsync(string? address = null)
    {
        EnsureNotDisposed();

        return await WithRpcClientAsync(async (rpc, protocolSettings, ct) =>
        {
            ct.ThrowIfCancellationRequested();
            UInt160 account = !string.IsNullOrEmpty(address)
                ? Neo.Network.RPC.Utility.GetScriptHash(address, protocolSettings)
                : await GetDeployerAccountAsync().ConfigureAwait(false);

            var nep17 = new Nep17API(rpc);
            var balance = await nep17.BalanceOfAsync(NativeContract.GAS.Hash, account).ConfigureAwait(false);
            var decimals = await nep17.DecimalsAsync(NativeContract.GAS.Hash).ConfigureAwait(false);
            var factor = BigInteger.Pow(10, (int)decimals);
            return (decimal)balance / (decimal)factor;
        }, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Deploy multiple contracts from a manifest file (Stub - Implementation in PR 2)
    /// </summary>
    /// <param name="manifestPath">Path to the deployment manifest JSON file</param>
    /// <returns>Dictionary of contract names to deployment information</returns>
    public async Task<Dictionary<string, ContractDeploymentInfo>> DeployFromManifestAsync(string manifestPath, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(manifestPath))
            throw new ArgumentException("Manifest path is required.", nameof(manifestPath));

        if (!File.Exists(manifestPath))
            throw new FileNotFoundException("Deployment manifest file not found.", manifestPath);

        var manifestJson = await File.ReadAllTextAsync(manifestPath, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(manifestJson))
            throw new InvalidOperationException("Deployment manifest file is empty.");

        var manifest = System.Text.Json.JsonSerializer.Deserialize<DeploymentManifestDocument>(manifestJson, JsonOptions)
            ?? throw new InvalidOperationException("Deployment manifest is invalid or could not be parsed.");

        if (manifest.Contracts is null || manifest.Contracts.Count == 0)
            throw new InvalidOperationException("Deployment manifest must contain at least one contract entry.");

        var manifestDirectory = Path.GetDirectoryName(Path.GetFullPath(manifestPath)) ?? Directory.GetCurrentDirectory();

        var originalState = CaptureState();
        var results = new Dictionary<string, ContractDeploymentInfo>(StringComparer.OrdinalIgnoreCase);

        try
        {
            if (!string.IsNullOrWhiteSpace(manifest.Network))
            {
                UseNetwork(ResolveNetworkProfile(manifest.Network));
            }

            var manifestPolicy = ResolveConfirmationPolicy(
                manifest.WaitForConfirmation,
                manifest.ConfirmationRetries,
                manifest.ConfirmationDelaySeconds);

            using var manifestWifScope = UseTemporaryWif(manifest.Wif);

            foreach (var contract in manifest.Contracts)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (string.IsNullOrWhiteSpace(contract.Nef) || string.IsNullOrWhiteSpace(contract.Manifest))
                    throw new InvalidOperationException($"Contract entry '{contract.Name ?? contract.Nef}' is missing required artifact paths.");

                var nefPath = ResolveArtifactPath(manifestDirectory, contract.Nef);
                var contractManifestPath = ResolveArtifactPath(manifestDirectory, contract.Manifest);

                object?[]? initParams = null;
                if (contract.InitParams.ValueKind is JsonValueKind.Array)
                {
                    initParams = ConvertJsonArray(contract.InitParams);
                }
                else if (contract.InitParams.ValueKind is not JsonValueKind.Undefined and not JsonValueKind.Null)
                {
                    throw new InvalidOperationException($"Contract entry '{contract.Name ?? contract.Nef}' initialization parameters must be an array when provided.");
                }

                var contractPolicy = ResolveConfirmationPolicy(
                    contract.WaitForConfirmation,
                    contract.ConfirmationRetries,
                    contract.ConfirmationDelaySeconds,
                    manifestPolicy);

                using var contractWifScope = UseTemporaryWif(contract.Wif);

                var deploymentInfo = await DeployArtifactsAsync(
                    nefPath,
                    contractManifestPath,
                    initParams,
                    contractPolicy.WaitForConfirmation,
                    contractPolicy.ConfirmationRetries,
                    contractPolicy.ConfirmationDelaySeconds,
                    cancellationToken,
                    signers: null,
                    transactionSignerAsync: null).ConfigureAwait(false);

                var key = string.IsNullOrWhiteSpace(contract.Name)
                    ? Path.GetFileNameWithoutExtension(nefPath)
                    : contract.Name;

                if (results.ContainsKey(key))
                    throw new InvalidOperationException($"Duplicate deployment key '{key}' detected in manifest. Provide unique names or NEF paths.");

                results[key] = deploymentInfo;
            }
        }
        finally
        {
            RestoreState(originalState);
        }

        return results;
    }

    /// <summary>
    /// Check if a contract exists at the given address (Stub - Implementation in PR 2)
    /// </summary>
    /// <param name="contractHashOrAddress">Contract hash or address</param>
    /// <returns>True if contract exists, false otherwise</returns>
    public async Task<bool> ContractExistsAsync(string contractHashOrAddress)
    {
        EnsureNotDisposed();

        return await WithRpcClientAsync(async (rpc, protocolSettings, ct) =>
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                var hash = Neo.Network.RPC.Utility.GetScriptHash(contractHashOrAddress, protocolSettings).ToString();
                _ = await rpc.GetContractStateAsync(hash).ConfigureAwait(false);
                return true;
            }
            catch
            {
                return false;
            }
        }, CancellationToken.None).ConfigureAwait(false);
    }

    #region Private Methods

    private string GetCurrentRpcUrl() => _networkProfile.RpcUrl;

    private void EnsureNotDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(DeploymentToolkit));
    }

    private string EnsureWif()
    {
        if (!string.IsNullOrEmpty(_wifKey))
            return _wifKey;

        throw new InvalidOperationException("WIF key not set. Call SetWifKey() first.");
    }

    private ToolkitSnapshot CaptureState()
        => new(
            _networkProfile,
            _options.Clone(),
            _wifKey,
            _protocolSettings,
            _networkMagicFetchedFromRpc,
            _networkMagicFallbackActive,
            _networkMagicLastAttemptUtc);

    private void RestoreState(ToolkitSnapshot snapshot)
    {
        _networkProfile = snapshot.Network;
        _options = snapshot.Options.Clone();
        _options.Network = _networkProfile;
        _wifKey = snapshot.Wif;
        _protocolSettings = snapshot.ProtocolSettings;
        _networkMagicFetchedFromRpc = snapshot.NetworkMagicResolved;
        _networkMagicFallbackActive = snapshot.NetworkMagicFallbackActive;
        _networkMagicLastAttemptUtc = snapshot.NetworkMagicLastAttemptUtc;
    }

    private IDisposable UseTemporaryWif(string? wif)
    {
        if (string.IsNullOrWhiteSpace(wif))
            return DisposableAction.Empty;

        EnsureNotDisposed();
        var previous = _wifKey;
        SetWifKey(wif);

        return new DisposableAction(() =>
        {
            if (!string.IsNullOrWhiteSpace(previous))
            {
                _wifKey = previous;
            }
            else
            {
                _wifKey = null;
            }
        });
    }

    private ConfirmationPolicy ResolveConfirmationPolicy(
        bool? waitForConfirmation,
        int? confirmationRetries,
        int? confirmationDelaySeconds,
        ConfirmationPolicy? fallback = null)
    {
        var baseline = fallback ?? new ConfirmationPolicy(
            _options.WaitForConfirmation,
            _options.ConfirmationRetries,
            _options.ConfirmationDelaySeconds);

        var wait = waitForConfirmation ?? baseline.WaitForConfirmation;
        var retries = confirmationRetries ?? baseline.ConfirmationRetries;
        var delay = confirmationDelaySeconds ?? baseline.ConfirmationDelaySeconds;

        if (retries < 0)
            throw new ArgumentOutOfRangeException(nameof(confirmationRetries), "Confirmation retries cannot be negative.");
        if (delay < 0)
            throw new ArgumentOutOfRangeException(nameof(confirmationDelaySeconds), "Confirmation delay cannot be negative.");

        return new ConfirmationPolicy(wait, retries, delay);
    }

    private IReadOnlyList<Signer> ResolveSigners(
        IReadOnlyList<Signer>? explicitSigners,
        ProtocolSettings protocolSettings)
    {
        if (explicitSigners is { Count: > 0 })
            return explicitSigners;

        if (_options.SignerProvider is not null)
        {
            var provided = _options.SignerProvider(protocolSettings);
            if (provided is { Count: > 0 })
                return provided;
        }

        return Array.Empty<Signer>();
    }

    private async Task<TResult> WithRpcClientAsync<TResult>(
        Func<RpcClient, ProtocolSettings, CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken)
    {
        EnsureNotDisposed();
        ArgumentNullException.ThrowIfNull(action);
        cancellationToken.ThrowIfCancellationRequested();

        var protocolSettings = await GetProtocolSettingsAsync().ConfigureAwait(false);
        using var rpc = _rpcClientFactory.Create(_networkProfile.RpcUri, protocolSettings);
        return await action(rpc, protocolSettings, cancellationToken).ConfigureAwait(false);
    }

    private async Task<uint> GetNetworkMagicAsync()
    {
        EnsureNotDisposed();

        var configuredMagic = _configuration.GetValue<uint?>("Network:NetworkMagic", null);
        var hasKnownProfile = NetworkProfile.TryGetKnown(_networkProfile.Identifier, out var knownProfile);
        var now = DateTime.UtcNow;

        if (_networkProfile.NetworkMagic.HasValue && _networkMagicFetchedFromRpc)
            return _networkProfile.NetworkMagic.Value;

        var shouldAttemptRpc = !_networkMagicFallbackActive
            || _networkMagicLastAttemptUtc == DateTime.MinValue
            || now - _networkMagicLastAttemptUtc >= NetworkMagicRetryInterval;

        if (shouldAttemptRpc)
        {
            _networkMagicLastAttemptUtc = now;
            var rpcResult = await TryResolveNetworkMagicFromRpcAsync().ConfigureAwait(false);
            if (rpcResult.HasValue)
                return rpcResult.Value;
        }

        var fallbackTimestamp = shouldAttemptRpc
            ? now
            : (_networkMagicLastAttemptUtc == DateTime.MinValue ? now : _networkMagicLastAttemptUtc);

        if (_networkProfile.NetworkMagic.HasValue)
        {
            var optionMagic = _options.Network?.NetworkMagic;
            var profileMagic = _networkProfile.NetworkMagic.Value;

            if ((optionMagic.HasValue && optionMagic.Value == profileMagic) ||
                (configuredMagic.HasValue && configuredMagic.Value == profileMagic) ||
                (hasKnownProfile && knownProfile!.NetworkMagic.HasValue && knownProfile.NetworkMagic.Value == profileMagic))
            {
                _networkMagicFetchedFromRpc = false;
                _networkMagicFallbackActive = true;
                _networkMagicLastAttemptUtc = fallbackTimestamp;
                return profileMagic;
            }
        }

        if (configuredMagic.HasValue)
        {
            _networkProfile = _networkProfile with { NetworkMagic = configuredMagic.Value };
            _options.Network = _networkProfile;
            _networkMagicFetchedFromRpc = false;
            _networkMagicFallbackActive = true;
            _networkMagicLastAttemptUtc = fallbackTimestamp;
            return configuredMagic.Value;
        }

        var networksSection = _configuration.GetSection("Network:Networks");
        var configuredNetworks = networksSection.Get<Dictionary<string, NetworkConfiguration>>();
        if (configuredNetworks is not null)
        {
            foreach (var entry in configuredNetworks)
            {
                if (string.Equals(entry.Key, _networkProfile.Identifier, StringComparison.OrdinalIgnoreCase) && entry.Value.NetworkMagic.HasValue)
                {
                    var magic = entry.Value.NetworkMagic.Value;
                    _networkProfile = _networkProfile with
                    {
                        NetworkMagic = magic,
                        AddressVersion = entry.Value.AddressVersion ?? _networkProfile.AddressVersion
                    };
                    _options.Network = _networkProfile;
                    _networkMagicFetchedFromRpc = false;
                    _networkMagicFallbackActive = true;
                    _networkMagicLastAttemptUtc = fallbackTimestamp;
                    return magic;
                }
            }
        }

        if (hasKnownProfile && knownProfile!.NetworkMagic.HasValue)
        {
            _networkProfile = _networkProfile with
            {
                NetworkMagic = knownProfile.NetworkMagic,
                AddressVersion = knownProfile.AddressVersion ?? _networkProfile.AddressVersion
            };
            _options.Network = _networkProfile;
            _networkMagicFetchedFromRpc = false;
            _networkMagicFallbackActive = true;
            _networkMagicLastAttemptUtc = fallbackTimestamp;
            return knownProfile.NetworkMagic.Value;
        }

        if (_networkProfile.NetworkMagic.HasValue)
        {
            _networkMagicFetchedFromRpc = false;
            _networkMagicFallbackActive = true;
            _networkMagicLastAttemptUtc = fallbackTimestamp;
            return _networkProfile.NetworkMagic.Value;
        }

        const uint defaultMagic = 894710606; // TestNet magic fallback
        _networkProfile = _networkProfile with { NetworkMagic = defaultMagic };
        _options.Network = _networkProfile;
        _networkMagicFetchedFromRpc = false;
        _networkMagicFallbackActive = true;
        _networkMagicLastAttemptUtc = fallbackTimestamp;
        return defaultMagic;
    }

    private async Task<uint?> TryResolveNetworkMagicFromRpcAsync()
    {
        try
        {
            var rpcUrl = GetCurrentRpcUrl();
            using var rpcClient = _rpcClientFactory.Create(new Uri(rpcUrl), ProtocolSettings.Default);
            var version = await rpcClient.GetVersionAsync().ConfigureAwait(false);
            var magic = version.Protocol.Network;
            var addressVersion = version.Protocol.AddressVersion;

            _networkProfile = _networkProfile with
            {
                NetworkMagic = magic,
                AddressVersion = addressVersion == 0 ? _networkProfile.AddressVersion : addressVersion
            };
            _options.Network = _networkProfile;
            _networkMagicFetchedFromRpc = true;
            _networkMagicFallbackActive = false;
            _networkMagicLastAttemptUtc = DateTime.UtcNow;
            return magic;
        }
        catch
        {
            return null;
        }
    }

    private async Task<ProtocolSettings> GetProtocolSettingsAsync()
    {
        EnsureNotDisposed();

        if (_protocolSettings is not null)
            return _protocolSettings;

        var magic = await GetNetworkMagicAsync();
        var baseSettings = ProtocolSettings.Default;
        var addressVersion = _networkProfile.AddressVersion ?? baseSettings.AddressVersion;
        _protocolSettings = baseSettings with
        {
            Network = magic,
            AddressVersion = addressVersion
        };

        return _protocolSettings;
    }

    #endregion

    #region Helpers

    private static async Task<bool> WaitForConfirmationAsync(RpcClient rpc, UInt256 txHash, int retries, TimeSpan delay, CancellationToken cancellationToken)
    {
        for (int i = 0; i < retries; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
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
            await Task.Delay(delay, cancellationToken);
        }
        return false;
    }

    private static object?[] ConvertJsonArray(JsonElement array)
    {
        var values = new object?[array.GetArrayLength()];
        var index = 0;
        foreach (var element in array.EnumerateArray())
        {
            values[index++] = ConvertJsonValue(element);
        }
        return values;
    }

    private static object? ConvertJsonValue(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.String => element.GetString(),
        JsonValueKind.Number => ConvertJsonNumberValue(element),
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Null => null,
        JsonValueKind.Array => ConvertJsonArrayParameter(element),
        JsonValueKind.Object => TryConvertContractParameter(element, out var parameter)
            ? parameter
            : ConvertJsonObjectParameter(element),
        _ => throw new InvalidOperationException($"Unsupported JSON value '{element.GetRawText()}' in deployment manifest.")
    };

    private static object ConvertJsonNumberValue(JsonElement element)
    {
        var raw = element.GetRawText();
        if (raw.IndexOfAny(new[] { '.', 'e', 'E' }) >= 0)
            throw new InvalidOperationException($"Only integer values are supported in deployment parameters. Value '{raw}' is not an integer.");

        if (!BigInteger.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            throw new InvalidOperationException($"Unable to parse numeric value '{raw}' in deployment parameters.");

        if (value >= long.MinValue && value <= long.MaxValue)
            return (long)value;

        return value;
    }

    private static bool TryConvertContractParameter(JsonElement element, out ContractParameter parameter)
    {
        parameter = default!;
        if (element.ValueKind != JsonValueKind.Object)
            return false;

        if (!element.TryGetProperty("type", out var typeProperty) || typeProperty.ValueKind != JsonValueKind.String)
            return false;

        try
        {
            var raw = element.GetRawText();
            if (Neo.Json.JToken.Parse(raw) is Neo.Json.JObject obj)
            {
                parameter = ContractParameter.FromJson(obj);
                return true;
            }
        }
        catch
        {
            // Ignore parse errors so fallback handlers can process the payload.
        }

        return false;
    }

    private static ContractParameter ConvertJsonArrayParameter(JsonElement array)
    {
        var parameter = new ContractParameter(ContractParameterType.Array);
        var items = new List<ContractParameter>(array.GetArrayLength());
        foreach (var child in array.EnumerateArray())
        {
            items.Add(ConvertJsonElementToContractParameter(child));
        }
        parameter.Value = items;
        return parameter;
    }

    private static ContractParameter ConvertJsonObjectParameter(JsonElement element)
    {
        var parameter = new ContractParameter(ContractParameterType.Map);
        var entries = new List<KeyValuePair<ContractParameter, ContractParameter>>();
        foreach (var property in element.EnumerateObject())
        {
            var key = new ContractParameter(ContractParameterType.String) { Value = property.Name };
            var value = ConvertJsonElementToContractParameter(property.Value);
            entries.Add(new KeyValuePair<ContractParameter, ContractParameter>(key, value));
        }
        parameter.Value = entries;
        return parameter;
    }

    private static ContractParameter ConvertJsonElementToContractParameter(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.Null => new ContractParameter(ContractParameterType.Any) { Value = null },
        JsonValueKind.True => new ContractParameter(ContractParameterType.Boolean) { Value = true },
        JsonValueKind.False => new ContractParameter(ContractParameterType.Boolean) { Value = false },
        JsonValueKind.String => new ContractParameter(ContractParameterType.String) { Value = element.GetString()! },
        JsonValueKind.Number => ConvertJsonNumberParameter(element),
        JsonValueKind.Array => ConvertJsonArrayParameter(element),
        JsonValueKind.Object => TryConvertContractParameter(element, out var parameter)
            ? parameter
            : ConvertJsonObjectParameter(element),
        _ => throw new InvalidOperationException($"Unsupported JSON element '{element.GetRawText()}' in deployment manifest.")
    };

    private static ContractParameter ConvertJsonNumberParameter(JsonElement element)
    {
        var value = ConvertJsonNumberValue(element);
        return value switch
        {
            long l => new ContractParameter(ContractParameterType.Integer) { Value = new BigInteger(l) },
            BigInteger bigInteger => new ContractParameter(ContractParameterType.Integer) { Value = bigInteger },
            _ => throw new InvalidOperationException($"Unsupported numeric value '{value}' in deployment parameters.")
        };
    }

    private static string ResolveArtifactPath(string baseDirectory, string path)
    {
        if (Path.IsPathRooted(path))
            return Path.GetFullPath(path);

        return Path.GetFullPath(Path.Combine(baseDirectory, path));
    }

    private static string BuildCompilationErrorMessage(IEnumerable<Diagnostic> diagnostics)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Compilation failed with the following diagnostics:");
        foreach (var diagnostic in diagnostics)
        {
            sb.AppendLine(diagnostic.ToString());
        }
        return sb.ToString();
    }

    public sealed record CompiledContractArtifact(string ContractName, NefFile Nef, ContractManifest Manifest);

    private sealed record DeploymentManifestDocument
    {
        public string? Network { get; init; }
        public string? Wif { get; init; }
        public bool? WaitForConfirmation { get; init; }
        public int? ConfirmationRetries { get; init; }
        public int? ConfirmationDelaySeconds { get; init; }
        public List<DeploymentManifestContract> Contracts { get; init; } = new();
    }

    private sealed record DeploymentManifestContract
    {
        public string? Name { get; init; }
        public string? Nef { get; init; }
        public string? Manifest { get; init; }
        public JsonElement InitParams { get; init; }
        public string? Wif { get; init; }
        public bool? WaitForConfirmation { get; init; }
        public int? ConfirmationRetries { get; init; }
        public int? ConfirmationDelaySeconds { get; init; }
    }

    private static byte[] BuildDeployScript(byte[] nefBytes, string manifestJson, object?[]? initParams)
    {
        using var sb = new ScriptBuilder();
        // Build args in reverse order and PACK
        if (initParams is { Length: > 0 })
        {
            // data (pack array if multiple)
            for (int i = initParams.Length - 1; i >= 0; i--)
            {
                var value = initParams[i];
                if (value is null)
                {
                    sb.Emit(OpCode.PUSHNULL);
                }
                else
                {
                    sb.EmitPush(value);
                }
            }
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

    private readonly record struct ToolkitSnapshot(
        NetworkProfile Network,
        DeploymentOptions Options,
        string? Wif,
        ProtocolSettings? ProtocolSettings,
        bool NetworkMagicResolved,
        bool NetworkMagicFallbackActive,
        DateTime NetworkMagicLastAttemptUtc);

    private readonly record struct ConfirmationPolicy(
        bool WaitForConfirmation,
        int ConfirmationRetries,
        int ConfirmationDelaySeconds);

    private sealed class DisposableAction : IDisposable
    {
        private readonly Action _onDispose;
        private bool _disposed;
        public static IDisposable Empty => new DisposableAction(static () => { });

        public DisposableAction(Action onDispose)
        {
            _onDispose = onDispose ?? throw new ArgumentNullException(nameof(onDispose));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _onDispose();
        }
    }

    #endregion

    #region Minimal Models (PR1)

    internal class NetworkConfiguration
    {
        public string RpcUrl { get; set; } = string.Empty;
        public uint? NetworkMagic { get; set; }
        public byte? AddressVersion { get; set; }
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
