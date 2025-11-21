using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;

namespace Neo.SmartContract.Deploy;

public partial class DeploymentToolkit
{
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
        _networkMagicRetryPending = false;
        ApplyConfiguredWifForNetwork(_networkProfile.Identifier);
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

    private void LoadConfiguredNetworks()
    {
        var networksSection = _configuration.GetSection("Network:Networks");
        var configuredNetworks = networksSection.Get<Dictionary<string, NetworkConfiguration>>();

        if (configuredNetworks is null || configuredNetworks.Count == 0)
        {
            _configuredNetworks = new Dictionary<string, NetworkConfiguration>(StringComparer.OrdinalIgnoreCase);
        }
        else
        {
            _configuredNetworks = new Dictionary<string, NetworkConfiguration>(configuredNetworks.Count, StringComparer.OrdinalIgnoreCase);
            foreach (var entry in configuredNetworks)
            {
                var key = entry.Key?.Trim();
                if (string.IsNullOrWhiteSpace(key))
                    continue;

                _configuredNetworks[key] = entry.Value ?? new NetworkConfiguration();
            }
        }

        var configuredName = _configuration["Network:Network"];
        if (!string.IsNullOrWhiteSpace(configuredName))
        {
            var normalized = configuredName.Trim();
            if (!_configuredNetworks.TryGetValue(normalized, out var config))
            {
                config = new NetworkConfiguration();
                _configuredNetworks[normalized] = config;
            }

            config.Wif ??= _configuration["Network:Wif"];

            var networkMagic = _configuration.GetValue<uint?>("Network:NetworkMagic", null);
            if (networkMagic.HasValue && !config.NetworkMagic.HasValue)
            {
                config.NetworkMagic = networkMagic;
            }

            var addressVersion = _configuration.GetValue<byte?>("Network:AddressVersion", null);
            if (addressVersion.HasValue && !config.AddressVersion.HasValue)
            {
                config.AddressVersion = addressVersion;
            }

            var rpcUrl = _configuration["Network:RpcUrl"];
            if (!string.IsNullOrWhiteSpace(rpcUrl))
            {
                config.RpcUrl = rpcUrl.Trim();
            }
        }
    }

    private NetworkConfiguration GetOrCreateConfiguredNetwork(string identifier)
    {
        if (!_configuredNetworks.TryGetValue(identifier, out var config))
        {
            config = new NetworkConfiguration();
            _configuredNetworks[identifier] = config;
        }

        return config;
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
            var identifier = configuredName ?? uri.Host;
            var profile = new NetworkProfile(identifier, normalized, magic, addressVersion);

            var config = GetOrCreateConfiguredNetwork(identifier);
            config.RpcUrl ??= normalized;
            if (magic.HasValue && !config.NetworkMagic.HasValue)
                config.NetworkMagic = magic;
            if (addressVersion.HasValue && !config.AddressVersion.HasValue)
                config.AddressVersion = addressVersion;
            var configuredWif = _configuration["Network:Wif"];
            if (!string.IsNullOrWhiteSpace(configuredWif) && string.IsNullOrWhiteSpace(config.Wif))
                config.Wif = configuredWif;

            return profile;
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

        if (_configuredNetworks.TryGetValue(trimmed, out var configured))
        {
            var rpcUrl = configured.RpcUrl?.Trim();
            if (!string.IsNullOrWhiteSpace(rpcUrl))
            {
                return new NetworkProfile(trimmed, rpcUrl, configured.NetworkMagic, configured.AddressVersion);
            }

            if (NetworkProfile.TryGetKnown(trimmed, out var knownProfile))
            {
                return knownProfile with
                {
                    NetworkMagic = configured.NetworkMagic ?? knownProfile.NetworkMagic,
                    AddressVersion = configured.AddressVersion ?? knownProfile.AddressVersion
                };
            }

            throw new ArgumentException($"Network '{network}' is configured without an RpcUrl.", nameof(network));
        }

        if (NetworkProfile.TryGetKnown(trimmed, out var known))
        {
            return known;
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
        SetWifKeyInternal(wifKey, WifOrigin.Manual);
        return this;
    }

    private void ApplyConfiguredWifForNetwork(string identifier, bool force = false)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return;

        if (_configuredNetworks.TryGetValue(identifier, out var configured) &&
            !string.IsNullOrWhiteSpace(configured.Wif))
        {
            ApplyConfiguredWif(configured.Wif!, identifier, force);
        }
    }

    private void ApplyConfiguredWif(string wif, string identifier, bool force)
    {
        if (!force && _wifOrigin == WifOrigin.Manual)
            return;

        try
        {
            SetWifKeyInternal(wif, WifOrigin.Configuration);
        }
        catch (ArgumentException ex)
        {
            throw new InvalidOperationException($"Configured WIF for network '{identifier}' is invalid: {ex.Message}", ex);
        }
    }

    private void SetWifKeyInternal(string wifKey, WifOrigin origin)
    {
        EnsureNotDisposed();

        if (string.IsNullOrWhiteSpace(wifKey))
            throw new ArgumentException("WIF key cannot be null or empty.", nameof(wifKey));

        var normalized = wifKey.Trim();

        try
        {
            // Validate the WIF key by attempting to create a KeyPair
            var privateKey = Neo.Wallets.Wallet.GetPrivateKeyFromWIF(normalized);
            var keyPair = new Neo.Wallets.KeyPair(privateKey);
            _ = Neo.SmartContract.Contract.CreateSignatureContract(keyPair.PublicKey).ScriptHash;

            _wifKey = normalized;
            _wifOrigin = origin;
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid WIF key: {ex.Message}", nameof(wifKey));
        }
    }

    private string EnsureWif()
    {
        if (!string.IsNullOrEmpty(_wifKey))
            return _wifKey;

        throw new InvalidOperationException("WIF key not set. Call SetWifKey() first.");
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

            _networkMagicRetryPending = true;
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

        if (_configuredNetworks.TryGetValue(_networkProfile.Identifier, out var configured) && configured.NetworkMagic.HasValue)
        {
            var magic = configured.NetworkMagic.Value;
            _networkProfile = _networkProfile with
            {
                NetworkMagic = magic,
                AddressVersion = configured.AddressVersion ?? _networkProfile.AddressVersion
            };
            _options.Network = _networkProfile;
            _networkMagicFetchedFromRpc = false;
            _networkMagicFallbackActive = true;
            _networkMagicLastAttemptUtc = fallbackTimestamp;
            return magic;
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

        throw new InvalidOperationException($"Unable to determine network magic for '{_networkProfile.Identifier}'. Configure the value via DeploymentOptions, appsettings.json, or allow the toolkit to reach the RPC node so it can be discovered automatically.");
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
            _networkMagicRetryPending = false;
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
        {
            if (!_networkMagicRetryPending)
                return _protocolSettings;

            if (_networkMagicLastAttemptUtc != DateTime.MinValue &&
                DateTime.UtcNow - _networkMagicLastAttemptUtc < NetworkMagicRetryInterval)
            {
                return _protocolSettings;
            }

            _protocolSettings = null;
        }

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

    private string GetCurrentRpcUrl() => _networkProfile.RpcUrl;
}
