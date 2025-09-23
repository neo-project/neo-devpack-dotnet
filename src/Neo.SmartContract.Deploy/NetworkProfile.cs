using Neo;
using System;

namespace Neo.SmartContract.Deploy;

/// <summary>
/// Represents a configured Neo network endpoint.
/// </summary>
public sealed record NetworkProfile
{
    public NetworkProfile(string identifier, string rpcUrl, uint? networkMagic = null, byte? addressVersion = null)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Identifier cannot be null or empty.", nameof(identifier));
        if (string.IsNullOrWhiteSpace(rpcUrl))
            throw new ArgumentException("RPC url cannot be null or empty.", nameof(rpcUrl));

        Identifier = identifier;
        RpcUrl = rpcUrl;
        NetworkMagic = networkMagic;
        AddressVersion = addressVersion;
    }

    public string Identifier { get; init; }

    public string RpcUrl { get; init; }

    public uint? NetworkMagic { get; init; }

    public byte? AddressVersion { get; init; }

    public Uri RpcUri => new(RpcUrl, UriKind.Absolute);

    public byte EffectiveAddressVersion => AddressVersion ?? ProtocolSettings.Default.AddressVersion;

    public static NetworkProfile MainNet { get; } = new("mainnet", "https://rpc10.n3.nspcc.ru:10331", 860833102, 0x35);
    public static NetworkProfile TestNet { get; } = new("testnet", "http://seed2t5.neo.org:20332", 894710606, 0x35);
    public static NetworkProfile Local { get; } = new("local", "http://localhost:50012");
    public static NetworkProfile Private { get; } = new("private", "http://localhost:50012");

    public static bool TryGetKnown(string identifier, out NetworkProfile profile)
    {
        NetworkProfile? result = identifier?.ToLowerInvariant() switch
        {
            "mainnet" => MainNet,
            "testnet" => TestNet,
            "local" => Local,
            "private" => Private,
            _ => null
        };

        if (result is null)
        {
            profile = null!;
            return false;
        }

        profile = result;
        return true;
    }
}
