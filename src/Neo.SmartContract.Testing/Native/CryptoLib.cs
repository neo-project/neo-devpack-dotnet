using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class CryptoLib : Neo.SmartContract.Testing.SmartContract
{
    #region Safe methods
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract object bls12381Add(object x, object y);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract object bls12381Deserialize(byte[] data);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract bool bls12381Equal(object x, object y);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract object bls12381Mul(object x, byte[] mul, bool neg);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract object bls12381Pairing(object g1, object g2);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract byte[] bls12381Serialize(object g);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract byte[] murmur32(byte[] data, BigInteger seed);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract byte[] ripemd160(byte[] data);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract byte[] sha256(byte[] data);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract bool verifyWithECDsa(byte[] message, byte[] pubkey, byte[] signature, BigInteger curve);
    #endregion
    #region Constructor for internal use only
    protected CryptoLib(Neo.SmartContract.Testing.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) {}
    #endregion
}