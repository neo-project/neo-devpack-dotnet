using Neo;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Neo.SmartContract.Deploy;

/// <summary>
/// Represents a built-in network profile with known RPC endpoints and network magic.
/// </summary>
public sealed class NetworkProfile
{
    private NetworkProfile(string name, uint? networkMagic, params string[] rpcUrls)
    {
        Name = name;
        NetworkMagic = networkMagic;
        var uriList = new List<Uri>();
        if (rpcUrls is not null)
        {
            foreach (var url in rpcUrls)
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    continue;
                }

                uriList.Add(new Uri(url, UriKind.Absolute));
            }
        }

        RpcUris = uriList.AsReadOnly();
    }

    /// <summary>
    /// The canonical network name (e.g. mainnet, testnet).
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the known network magic for this profile (if any).
    /// </summary>
    public uint? NetworkMagic { get; }

    /// <summary>
    /// Gets the default RPC endpoints associated with this profile.
    /// </summary>
    public IReadOnlyList<Uri> RpcUris { get; }

    /// <summary>
    /// Default profile for Neo N3 MainNet.
    /// </summary>
    public static NetworkProfile MainNet { get; } = new("mainnet", 860833102u,
        "https://mainnet1.neo.org:443",
        "https://mainnet2.neo.org:443",
        "https://mainnet3.neo.org:443");

    /// <summary>
    /// Default profile for Neo N3 TestNet.
    /// </summary>
    public static NetworkProfile TestNet { get; } = new("testnet", 894710606u,
        "https://testnet1.neo.org:443",
        "https://testnet2.neo.org:443");

    /// <summary>
    /// Default profile for local/private networks.
    /// </summary>
    public static NetworkProfile DevNet { get; } = new("devnet", ProtocolSettings.Default.Network,
        "http://localhost:50012");

    private static readonly Dictionary<string, NetworkProfile> s_knownProfiles = new(StringComparer.OrdinalIgnoreCase)
    {
        [MainNet.Name] = MainNet,
        [TestNet.Name] = TestNet,
        [DevNet.Name] = DevNet
    };

    /// <summary>
    /// Returns true when a known profile exists for the provided <paramref name="name"/>.
    /// </summary>
    public static bool TryGetKnown(string name, [NotNullWhen(true)] out NetworkProfile? profile) => s_knownProfiles.TryGetValue(name, out profile);

    /// <summary>
    /// Enumerates known profiles.
    /// </summary>
    public static IEnumerable<NetworkProfile> GetKnownProfiles() => s_knownProfiles.Values;
}
