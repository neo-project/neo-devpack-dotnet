using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class CryptoLib : SmartContract
{
    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("bls12381Add")]
    public abstract object? Bls12381Add(object? x, object? y);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("bls12381Deserialize")]
    public abstract object? Bls12381Deserialize(byte[]? data);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("bls12381Equal")]
    public abstract bool? Bls12381Equal(object? x, object? y);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("bls12381Mul")]
    public abstract object? Bls12381Mul(object? x, byte[]? mul, bool? neg);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("bls12381Pairing")]
    public abstract object? Bls12381Pairing(object? g1, object? g2);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("bls12381Serialize")]
    public abstract byte[]? Bls12381Serialize(object? g);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("murmur32")]
    public abstract byte[] Murmur32(byte[]? data, BigInteger? seed);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("ripemd160")]
    public abstract byte[] Ripemd160(byte[]? data);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("sha256")]
    public abstract byte[] Sha256(byte[]? data);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("verifyWithECDsa")]
    public abstract bool VerifyWithECDsa(byte[]? message, byte[]? pubkey, byte[]? signature, BigInteger? curve);

    #endregion

    #region Constructor for internal use only

    protected CryptoLib(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
