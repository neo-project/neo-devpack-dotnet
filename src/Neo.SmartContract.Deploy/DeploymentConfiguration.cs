using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Neo.SmartContract.Deploy;

/// <summary>
/// Represents the JSON configuration for a deployment project.
/// </summary>
public sealed class DeploymentConfiguration
{
    private readonly Dictionary<string, NetworkConfiguration> _networks;

    private DeploymentConfiguration(string? sourcePath)
    {
        SourcePath = sourcePath;
        _networks = new Dictionary<string, NetworkConfiguration>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// The path the configuration was loaded from (if any).
    /// </summary>
    public string? SourcePath { get; }

    /// <summary>
    /// Gets or sets the default network name.
    /// </summary>
    public string? DefaultNetwork { get; init; }

    /// <summary>
    /// Network definitions keyed by their friendly name.
    /// </summary>
    public IReadOnlyDictionary<string, NetworkConfiguration> Networks => _networks;

    /// <summary>
    /// Returns an empty configuration.
    /// </summary>
    public static DeploymentConfiguration Empty { get; } = new(null);

    /// <summary>
    /// Loads a configuration file from disk.
    /// </summary>
    public static DeploymentConfiguration Load(string configPath)
    {
        if (string.IsNullOrWhiteSpace(configPath))
            throw new ArgumentException("Configuration path must be provided.", nameof(configPath));

        if (!File.Exists(configPath))
            throw new FileNotFoundException($"Deployment configuration '{configPath}' was not found.", configPath);

        using var stream = File.OpenRead(configPath);
        using var document = JsonDocument.Parse(stream);
        var root = document.RootElement;
        if (root.TryGetProperty("deployment", out var deploymentElement) && deploymentElement.ValueKind == JsonValueKind.Object)
        {
            root = deploymentElement;
        }

        return FromJson(root, configPath);
    }

    internal static DeploymentConfiguration FromJson(JsonElement root, string? sourcePath)
    {
        if (root.TryGetProperty("deployment", out var deploymentElement) && deploymentElement.ValueKind == JsonValueKind.Object)
        {
            root = deploymentElement;
        }

        var configuration = new DeploymentConfiguration(sourcePath)
        {
            DefaultNetwork = root.TryGetProperty("defaultNetwork", out var defaultNetwork)
                ? defaultNetwork.GetString()
                : null
        };

        if (root.TryGetProperty("networks", out var networksElement) && networksElement.ValueKind == JsonValueKind.Object)
        {
            foreach (var networkProperty in networksElement.EnumerateObject())
            {
                configuration._networks[networkProperty.Name] = NetworkConfiguration.Parse(networkProperty.Name, networkProperty.Value, sourcePath);
            }
        }

        return configuration;
    }

    /// <summary>
    /// Attempts to get a network configuration by name.
    /// </summary>
    public bool TryGetNetwork(string name, out NetworkConfiguration network)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            network = null!;
            return false;
        }

        return _networks.TryGetValue(name, out network!);
    }
}

/// <summary>
/// Represents a single network entry in the deployment configuration.
/// </summary>
public sealed class NetworkConfiguration
{
    private readonly List<Uri> _rpcUris;

    private NetworkConfiguration(string name, IEnumerable<Uri> uris, uint? networkMagic, byte? addressVersion, string? privateKey)
    {
        Name = name;
        _rpcUris = uris.ToList();
        NetworkMagic = networkMagic;
        AddressVersion = addressVersion;
        PrivateKey = string.IsNullOrWhiteSpace(privateKey) ? null : privateKey;
    }

    /// <summary>
    /// Network display name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// RPC endpoints configured for this network.
    /// </summary>
    public IReadOnlyList<Uri> RpcUris => _rpcUris;

    /// <summary>
    /// Optional network magic override.
    /// </summary>
    public uint? NetworkMagic { get; }

    /// <summary>
    /// Optional address version override.
    /// </summary>
    public byte? AddressVersion { get; }

    /// <summary>
    /// Private key (hex or WIF) associated with this network entry.
    /// </summary>
    public string? PrivateKey { get; }

    internal static NetworkConfiguration Parse(string networkName, JsonElement element, string? sourcePath)
    {
        var uris = new List<Uri>();

        void AppendUri(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
            {
                throw new FormatException($"Invalid RPC endpoint '{value}' in network '{networkName}' (config: '{sourcePath ?? "in-memory"}').");
            }

            if (uris.All(r => r != uri))
            {
                uris.Add(uri);
            }
        }

        if (element.TryGetProperty("rpcUrls", out var rpcUrlsElement) && rpcUrlsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var rpcUrl in rpcUrlsElement.EnumerateArray())
            {
                AppendUri(rpcUrl.GetString());
            }
        }

        if (element.TryGetProperty("rpcUrl", out var rpcUrlElement) && rpcUrlElement.ValueKind != JsonValueKind.Array)
        {
            AppendUri(rpcUrlElement.GetString());
        }

        uint? networkMagic = null;
        if (element.TryGetProperty("networkMagic", out var magicElement) && magicElement.ValueKind != JsonValueKind.Null && magicElement.ValueKind != JsonValueKind.Undefined)
        {
            if (magicElement.TryGetUInt32(out var magicValue))
            {
                networkMagic = magicValue;
            }
            else
            {
                throw new FormatException($"networkMagic for '{networkName}' must be a number.");
            }
        }

        byte? addressVersion = null;
        if (element.TryGetProperty("addressVersion", out var addressVersionElement) && addressVersionElement.ValueKind != JsonValueKind.Null)
        {
            if (addressVersionElement.TryGetByte(out var version))
            {
                addressVersion = version;
            }
            else
            {
                throw new FormatException($"addressVersion for '{networkName}' must be a byte value.");
            }
        }

        var privateKey = element.TryGetProperty("privateKey", out var privateKeyElement)
            ? privateKeyElement.GetString()
            : null;

        return new NetworkConfiguration(networkName, uris, networkMagic, addressVersion, privateKey);
    }
}

internal static class JsonElementExtensions
{
    public static bool TryGetUInt32(this JsonElement element, out uint value)
    {
        try
        {
            value = element.GetUInt32();
            return true;
        }
        catch
        {
            value = default;
            return false;
        }
    }

    public static bool TryGetByte(this JsonElement element, out byte value)
    {
        try
        {
            value = element.GetByte();
            return true;
        }
        catch
        {
            value = default;
            return false;
        }
    }
}
