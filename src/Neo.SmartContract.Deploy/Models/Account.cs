using System;
using Neo;
using Neo.Cryptography.ECC;

namespace Neo.Wallets;

/// <summary>
/// Represents a Neo account
/// </summary>
public class Account
{
    /// <summary>
    /// Private key
    /// </summary>
    public byte[] PrivateKey { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Public key
    /// </summary>
    public ECPoint PublicKey { get; set; } = null!;

    /// <summary>
    /// Script hash of the account
    /// </summary>
    public UInt160 ScriptHash { get; set; } = null!;
}