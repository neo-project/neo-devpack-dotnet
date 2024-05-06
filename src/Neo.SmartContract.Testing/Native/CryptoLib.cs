using Neo.SmartContract.Native;
using System.ComponentModel;

namespace Neo.SmartContract.Testing.Native;

public abstract class CryptoLib : SmartContract
{
    #region Compiled data
    public static Manifest.ContractManifest Manifest { get; } =
        NativeContract.CryptoLib.GetContractState(ProtocolSettings.Default, uint.MaxValue).Manifest;


    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("bls12381Add")]
    public abstract object Bls12381Add(object x, object y);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("bls12381Deserialize")]
    public abstract object Bls12381Deserialize(byte[] data);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("bls12381Equal")]
    public abstract bool Bls12381Equal(object x, object y);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("bls12381Mul")]
    public abstract object Bls12381Mul(object x, byte[] mul, bool neg);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("bls12381Pairing")]
    public abstract object Bls12381Pairing(object g1, object g2);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("bls12381Serialize")]
    public abstract byte[] Bls12381Serialize(object g);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("keccak256")]
    public abstract byte[] Keccak256(byte[] data);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("murmur32")]
    public abstract byte[] Murmur32(byte[] data, uint seed);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("ripemd160")]
    public abstract byte[] Ripemd160(byte[] data);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("sha256")]
    public abstract byte[] Sha256(byte[] data);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("verifyWithECDsa")]
    public abstract bool VerifyWithECDsa(byte[] message, byte[] pubkey, byte[] signature, NamedCurveHash curveHash);

    #endregion

    #region Constructor for internal use only

    protected CryptoLib(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
