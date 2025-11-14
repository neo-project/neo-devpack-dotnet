using Neo;
using Neo.Extensions;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.Network.RPC.Models;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy;

/// <summary>
/// Provides helper APIs for loading deployment configuration, selecting networks, and deploying/invoking contracts via RPC.
/// </summary>
public sealed class DeploymentToolkit
{
    private readonly Func<Uri, ProtocolSettings, RpcClient> _rpcClientFactory;
    private DeploymentConfiguration _configuration;
    private string? _configPath;
    private string? _networkName;
    private NetworkProfile? _explicitProfile;
    private Uri? _rpcOverride;
    private uint? _networkMagicOverride;
    private byte? _addressVersionOverride;
    private KeyPair? _keyPair;
    private Func<CancellationToken, Task<string?>>? _privateKeyProvider;

    /// <summary>
    /// Initializes a new toolkit instance.
    /// </summary>
    public DeploymentToolkit(DeploymentConfiguration? configuration = null, Func<Uri, ProtocolSettings, RpcClient>? rpcClientFactory = null)
    {
        _configuration = configuration ?? DeploymentConfiguration.Empty;
        _rpcClientFactory = rpcClientFactory ?? DefaultRpcClientFactory;
    }

    /// <summary>
    /// Loads a configuration file and returns a configured toolkit.
    /// </summary>
    public static DeploymentToolkit FromConfigFile(string configPath, Func<Uri, ProtocolSettings, RpcClient>? rpcClientFactory = null)
    {
        var configuration = DeploymentConfiguration.Load(configPath);
        return new DeploymentToolkit(configuration, rpcClientFactory) { _configPath = configPath };
    }

    /// <summary>
    /// Path of the currently loaded configuration file (if any).
    /// </summary>
    public string? ConfigurationPath => _configPath;

    /// <summary>
    /// Gets the current deployment configuration.
    /// </summary>
    public DeploymentConfiguration Configuration => _configuration;

    /// <summary>
    /// Reloads the configuration from the specified file path.
    /// </summary>
    public DeploymentToolkit UseConfigFile(string configPath)
    {
        _configuration = DeploymentConfiguration.Load(configPath);
        _configPath = configPath;
        return this;
    }

    /// <summary>
    /// Selects a network by name.
    /// </summary>
    public DeploymentToolkit UseNetwork(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Network name must be provided.", nameof(name));

        _networkName = name;
        _explicitProfile = null;
        return this;
    }

    /// <summary>
    /// Selects a built-in network profile.
    /// </summary>
    public DeploymentToolkit UseNetwork(NetworkProfile profile)
    {
        _explicitProfile = profile ?? throw new ArgumentNullException(nameof(profile));
        _networkName = profile.Name;
        return this;
    }

    /// <summary>
    /// Overrides the RPC endpoint to use regardless of configuration.
    /// </summary>
    public DeploymentToolkit UseRpcEndpoint(string rpcUrl)
    {
        if (string.IsNullOrWhiteSpace(rpcUrl))
            throw new ArgumentException("RPC endpoint must be provided.", nameof(rpcUrl));

        _rpcOverride = new Uri(rpcUrl, UriKind.Absolute);
        return this;
    }

    /// <summary>
    /// Overrides the network magic used for signer calculations.
    /// </summary>
    public DeploymentToolkit UseNetworkMagic(uint networkMagic)
    {
        _networkMagicOverride = networkMagic;
        return this;
    }

    /// <summary>
    /// Overrides the wallet address version used for signer calculations.
    /// </summary>
    public DeploymentToolkit UseAddressVersion(byte addressVersion)
    {
        _addressVersionOverride = addressVersion;
        return this;
    }

    /// <summary>
    /// Injects an explicit account key pair.
    /// </summary>
    public DeploymentToolkit UseAccount(KeyPair keyPair)
    {
        _keyPair = keyPair ?? throw new ArgumentNullException(nameof(keyPair));
        return this;
    }

    /// <summary>
    /// Uses a private key provided as hex or WIF.
    /// </summary>
    public DeploymentToolkit UsePrivateKey(string privateKey)
    {
        if (string.IsNullOrWhiteSpace(privateKey))
            throw new ArgumentException("Private key must be provided.", nameof(privateKey));

        _keyPair = new KeyPair(DecodePrivateKey(privateKey));
        return this;
    }

    /// <summary>
    /// Registers a custom callback that will be invoked if a private key is required but not configured.
    /// </summary>
    public DeploymentToolkit UsePrivateKeyProvider(Func<CancellationToken, Task<string?>> provider)
    {
        _privateKeyProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        return this;
    }

    /// <summary>
    /// Prompts on the console for a private key if none is configured.
    /// </summary>
    public DeploymentToolkit UseConsolePrivateKeyPrompt(string? prompt = null)
    {
        return UsePrivateKeyProvider(_ =>
        {
            if (!string.IsNullOrEmpty(prompt))
            {
                Console.Write(prompt);
            }
            return Task.FromResult(Console.ReadLine());
        });
    }

    /// <summary>
    /// Deploys the provided artifacts and broadcasts the resulting transaction.
    /// </summary>
    public async Task<DeploymentResult> DeployAsync(ContractArtifacts artifacts, CancellationToken cancellationToken = default)
    {
        if (artifacts is null)
            throw new ArgumentNullException(nameof(artifacts));

        var session = await CreateSessionAsync(cancellationToken).ConfigureAwait(false);
        var signer = session.DefaultSigner;

        using var rpc = session.RpcClient;
        ContractClient contractClient = new(rpc);

        var tx = await contractClient.CreateDeployContractTxAsync(artifacts.Nef.ToArray(), artifacts.Manifest, session.KeyPair)
            .ConfigureAwait(false);

        var txHash = await rpc.SendRawTransactionAsync(tx).ConfigureAwait(false);
        var contractHash = Helper.GetContractHash(tx.Sender, artifacts.Nef.CheckSum, artifacts.Manifest.Name);
        return new DeploymentResult(contractHash, txHash, tx);
    }

    /// <summary>
    /// Creates and relays a transaction that invokes an existing contract.
    /// </summary>
    public async Task<InvocationResult> InvokeAsync(UInt160 contractHash, string operation, object[]? args = null, CancellationToken cancellationToken = default)
    {
        if (contractHash is null)
            throw new ArgumentNullException(nameof(contractHash));
        if (string.IsNullOrWhiteSpace(operation))
            throw new ArgumentException("Operation must be provided.", nameof(operation));

        var session = await CreateSessionAsync(cancellationToken).ConfigureAwait(false);
        using var rpc = session.RpcClient;

        byte[] script;
        using (var sb = new ScriptBuilder())
        {
            sb.EmitDynamicCall(contractHash, operation, args ?? Array.Empty<object>());
            script = sb.ToArray();
        }

        TransactionManagerFactory factory = new(rpc);
        var manager = await factory.MakeTransactionAsync(script, new[] { session.DefaultSigner }).ConfigureAwait(false);
        var tx = await manager.AddSignature(session.KeyPair).SignAsync().ConfigureAwait(false);
        var txHash = await rpc.SendRawTransactionAsync(tx).ConfigureAwait(false);
        return new InvocationResult(txHash, tx);
    }

    /// <summary>
    /// Performs a read-only invocation against the target contract.
    /// </summary>
    public async Task<RpcInvokeResult> TestInvokeAsync(UInt160 contractHash, string operation, object[]? args = null, CancellationToken cancellationToken = default)
    {
        if (contractHash is null)
            throw new ArgumentNullException(nameof(contractHash));
        if (string.IsNullOrWhiteSpace(operation))
            throw new ArgumentException("Operation must be provided.", nameof(operation));

        var session = await CreateSessionAsync(cancellationToken).ConfigureAwait(false);
        using var rpc = session.RpcClient;

        byte[] script;
        using (var sb = new ScriptBuilder())
        {
            sb.EmitDynamicCall(contractHash, operation, args ?? Array.Empty<object>());
            script = sb.ToArray();
        }

        return await rpc.InvokeScriptAsync(script, session.DefaultSigner).ConfigureAwait(false);
    }

    /// <summary>
    /// Resolves the network magic, using config overrides or RPC fallbacks.
    /// </summary>
    public async Task<uint> GetNetworkMagicAsync(CancellationToken cancellationToken = default)
    {
        var network = await ResolveNetworkAsync(cancellationToken).ConfigureAwait(false);
        if (network.NetworkMagic.HasValue)
        {
            return network.NetworkMagic.Value;
        }

        foreach (var uri in network.RpcUris)
        {
            try
            {
                using var rpc = _rpcClientFactory(uri, ProtocolSettings.Default);
                var version = await rpc.GetVersionAsync().ConfigureAwait(false);
                return version.Protocol.Network;
            }
            catch
            {
                // Try next endpoint.
            }
        }

        if (network.Profile?.NetworkMagic is { } profileMagic)
        {
            return profileMagic;
        }

        throw new InvalidOperationException("Unable to determine network magic. Provide a value in the configuration or ensure an RPC endpoint is reachable.");
    }

    private async Task<DeploymentSession> CreateSessionAsync(CancellationToken cancellationToken)
    {
        var network = await ResolveNetworkAsync(cancellationToken).ConfigureAwait(false);

        KeyPair keyPair = await ResolveKeyPairAsync(network, cancellationToken).ConfigureAwait(false);
        var rpcUri = _rpcOverride ?? network.RpcUris.FirstOrDefault()
            ?? throw new InvalidOperationException($"Network '{network.Name}' does not define an RPC endpoint.");

        uint networkMagic = network.NetworkMagic ?? await FetchNetworkMagicAsync(network, rpcUri, cancellationToken).ConfigureAwait(false);
        byte addressVersion = _addressVersionOverride ?? network.ConfigAddressVersion ?? ProtocolSettings.Default.AddressVersion;

        var protocolSettings = ProtocolSettings.Default with
        {
            Network = networkMagic,
            AddressVersion = addressVersion
        };

        var rpcClient = _rpcClientFactory(rpcUri, protocolSettings);
        var sender = Contract.CreateSignatureRedeemScript(keyPair.PublicKey).ToScriptHash();
        var signer = new Signer { Account = sender, Scopes = WitnessScope.CalledByEntry };

        return new DeploymentSession(rpcClient, keyPair, signer);
    }

    private Task<ResolvedNetwork> ResolveNetworkAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var name = _networkName ?? _configuration.DefaultNetwork ?? NetworkProfile.MainNet.Name;

        NetworkConfiguration? configNetwork = _configuration.TryGetNetwork(name, out var networkConfig) ? networkConfig : null;
        NetworkProfile? profile = _explicitProfile ?? (NetworkProfile.TryGetKnown(name, out var knownProfile) ? knownProfile : null);

        var rpcUris = new List<Uri>();
        if (_rpcOverride != null)
        {
            rpcUris.Add(_rpcOverride);
        }
        else
        {
            if (configNetwork?.RpcUris.Count > 0)
            {
                rpcUris.AddRange(configNetwork.RpcUris);
            }

            if (profile?.RpcUris is { Count: > 0 })
            {
                foreach (var uri in profile.RpcUris)
                {
                    if (!rpcUris.Contains(uri))
                    {
                        rpcUris.Add(uri);
                    }
                }
            }
        }

        if (rpcUris.Count == 0)
        {
            throw new InvalidOperationException($"Network '{name}' does not have any RPC endpoints configured. Add one to '{_configPath ?? "deploysettings.json"}' or specify --rpc.");
        }

        return Task.FromResult(new ResolvedNetwork(
            name,
            rpcUris,
            _networkMagicOverride ?? configNetwork?.NetworkMagic ?? profile?.NetworkMagic,
            configNetwork?.AddressVersion,
            configNetwork?.PrivateKey,
            profile));
    }

    private async Task<KeyPair> ResolveKeyPairAsync(ResolvedNetwork network, CancellationToken cancellationToken)
    {
        if (_keyPair != null)
        {
            return _keyPair;
        }

        string? privateKey = network.NetworkPrivateKey;
        if (string.IsNullOrWhiteSpace(privateKey) && _privateKeyProvider != null)
        {
            privateKey = await _privateKeyProvider(cancellationToken).ConfigureAwait(false);
        }

        if (string.IsNullOrWhiteSpace(privateKey))
        {
            throw new InvalidOperationException($"Network '{network.Name}' does not have a configured private key. Supply one via configuration, --private-key, or call UseConsolePrivateKeyPrompt().");
        }

        _keyPair = new KeyPair(DecodePrivateKey(privateKey));
        return _keyPair;
    }

    private async Task<uint> FetchNetworkMagicAsync(ResolvedNetwork network, Uri uri, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (network.NetworkMagic.HasValue)
        {
            return network.NetworkMagic.Value;
        }

        using var rpc = _rpcClientFactory(uri, ProtocolSettings.Default);
        var version = await rpc.GetVersionAsync().ConfigureAwait(false);
        return version.Protocol.Network;
    }

    private static byte[] DecodePrivateKey(string privateKey)
    {
        privateKey = privateKey.Trim();
        if (privateKey.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            privateKey = privateKey[2..];
        }

        if (privateKey.Length == 64 && IsHex(privateKey))
        {
            return Convert.FromHexString(privateKey);
        }

        try
        {
            return Wallet.GetPrivateKeyFromWIF(privateKey);
        }
        catch (FormatException ex)
        {
            throw new FormatException("Private key must be a 32-byte hex string or a valid WIF.", ex);
        }
    }

    private static bool IsHex(string value)
    {
        foreach (var c in value)
        {
            if (!Uri.IsHexDigit(c))
            {
                return false;
            }
        }

        return true;
    }

    private static RpcClient DefaultRpcClientFactory(Uri uri, ProtocolSettings protocolSettings)
    {
        return new RpcClient(uri, protocolSettings: protocolSettings);
    }

    private sealed record ResolvedNetwork(
        string Name,
        IReadOnlyList<Uri> RpcUris,
        uint? NetworkMagic,
        byte? ConfigAddressVersion,
        string? NetworkPrivateKey,
        NetworkProfile? Profile);

    private sealed class DeploymentSession
    {
        public DeploymentSession(RpcClient rpcClient, KeyPair keyPair, Signer signer)
        {
            RpcClient = rpcClient;
            KeyPair = keyPair;
            DefaultSigner = signer;
        }

        public RpcClient RpcClient { get; }
        public KeyPair KeyPair { get; }
        public Signer DefaultSigner { get; }
    }
}
