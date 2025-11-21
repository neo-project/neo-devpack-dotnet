using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Neo;
using Neo.Network.RPC;
using Neo.SmartContract;

namespace Neo.SmartContract.Deploy;

/// <summary>
/// Deployment toolkit for Neo smart contract deployment.
/// </summary>
public partial class DeploymentToolkit : IDisposable
{
    private const string DefaultRpcUrl = "http://localhost:10332";
    private const int RpcUnknownContractCode = -102;

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
    private bool _networkMagicRetryPending;
    private bool _disposed = false;
    private Dictionary<string, NetworkConfiguration> _configuredNetworks = new(StringComparer.OrdinalIgnoreCase);
    private WifOrigin _wifOrigin = WifOrigin.None;
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

    private void Initialize(IConfiguration configuration, DeploymentOptions? options, IRpcClientFactory? rpcClientFactory)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _rpcClientFactory = rpcClientFactory ?? new DefaultRpcClientFactory();
        _options = (options ?? new DeploymentOptions()).Clone();

        LoadConfiguredNetworks();
        _networkProfile = ResolveInitialNetworkProfile();
        ApplyConfiguredWifForNetwork(_networkProfile.Identifier, force: true);
        _networkMagicFetchedFromRpc = false;
        _networkMagicFallbackActive = false;
        _networkMagicLastAttemptUtc = DateTime.MinValue;
        _networkMagicRetryPending = false;
    }

    private void EnsureNotDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(DeploymentToolkit));
    }

    private ToolkitSnapshot CaptureState()
        => new(
            _networkProfile,
            _options.Clone(),
            _wifKey,
            _wifOrigin,
            _protocolSettings,
            _networkMagicFetchedFromRpc,
            _networkMagicFallbackActive,
            _networkMagicLastAttemptUtc,
            _networkMagicRetryPending);

    private void RestoreState(ToolkitSnapshot snapshot)
    {
        _networkProfile = snapshot.Network;
        _options = snapshot.Options.Clone();
        _options.Network = _networkProfile;
        _wifKey = snapshot.Wif;
        _wifOrigin = snapshot.WifOrigin;
        _protocolSettings = snapshot.ProtocolSettings;
        _networkMagicFetchedFromRpc = snapshot.NetworkMagicResolved;
        _networkMagicFallbackActive = snapshot.NetworkMagicFallbackActive;
        _networkMagicLastAttemptUtc = snapshot.NetworkMagicLastAttemptUtc;
        _networkMagicRetryPending = snapshot.NetworkMagicRetryPending;
    }

    private IDisposable UseTemporaryWif(string? wif)
    {
        if (string.IsNullOrWhiteSpace(wif))
            return DisposableAction.Empty;

        EnsureNotDisposed();
        var previous = _wifKey;
        var previousOrigin = _wifOrigin;
        SetWifKey(wif);

        return new DisposableAction(() =>
        {
            _wifKey = previous;
            _wifOrigin = previousOrigin;
        });
    }

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
            _wifKey = null;
            _protocolSettings = null;

            _disposed = true;
        }
    }

    private byte ResolveAddressVersionForCompilation()
    {
        if (_options.Network?.AddressVersion is byte optionAddress)
        {
            return optionAddress;
        }

        if (_networkProfile.AddressVersion.HasValue)
        {
            return _networkProfile.AddressVersion.Value;
        }

        if (NetworkProfile.TryGetKnown(_networkProfile.Identifier, out var known) && known.AddressVersion.HasValue)
        {
            return known.AddressVersion.Value;
        }

        return ProtocolSettings.Default.AddressVersion;
    }

    private enum WifOrigin
    {
        None = 0,
        Configuration = 1,
        Manual = 2
    }

    private readonly record struct ToolkitSnapshot(
        NetworkProfile Network,
        DeploymentOptions Options,
        string? Wif,
        WifOrigin WifOrigin,
        ProtocolSettings? ProtocolSettings,
        bool NetworkMagicResolved,
        bool NetworkMagicFallbackActive,
        DateTime NetworkMagicLastAttemptUtc,
        bool NetworkMagicRetryPending);

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

    internal class NetworkConfiguration
    {
        public string? RpcUrl { get; set; }
        public uint? NetworkMagic { get; set; }
        public byte? AddressVersion { get; set; }
        public string? Wif { get; set; }
    }

    public record ContractDeploymentInfo
    {
        public UInt256? TransactionHash { get; init; }
        public UInt160? ContractHash { get; init; }
    }

    private readonly record struct ConfirmationPolicy(
        bool WaitForConfirmation,
        int ConfirmationRetries,
        int ConfirmationDelaySeconds);
}
