using Neo.SmartContract.Native;
using System.ComponentModel;

namespace Neo.SmartContract.Testing.Native;

public abstract class CryptoLib : SmartContract
{
    #region Compiled data

    public static readonly Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""CryptoLib"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""bls12381Add"",""parameters"":[{""name"":""x"",""type"":""InteropInterface""},{""name"":""y"",""type"":""InteropInterface""}],""returntype"":""InteropInterface"",""offset"":0,""safe"":true},{""name"":""bls12381Deserialize"",""parameters"":[{""name"":""data"",""type"":""ByteArray""}],""returntype"":""InteropInterface"",""offset"":7,""safe"":true},{""name"":""bls12381Equal"",""parameters"":[{""name"":""x"",""type"":""InteropInterface""},{""name"":""y"",""type"":""InteropInterface""}],""returntype"":""Boolean"",""offset"":14,""safe"":true},{""name"":""bls12381Mul"",""parameters"":[{""name"":""x"",""type"":""InteropInterface""},{""name"":""mul"",""type"":""ByteArray""},{""name"":""neg"",""type"":""Boolean""}],""returntype"":""InteropInterface"",""offset"":21,""safe"":true},{""name"":""bls12381Pairing"",""parameters"":[{""name"":""g1"",""type"":""InteropInterface""},{""name"":""g2"",""type"":""InteropInterface""}],""returntype"":""InteropInterface"",""offset"":28,""safe"":true},{""name"":""bls12381Serialize"",""parameters"":[{""name"":""g"",""type"":""InteropInterface""}],""returntype"":""ByteArray"",""offset"":35,""safe"":true},{""name"":""keccak256"",""parameters"":[{""name"":""data"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":42,""safe"":true},{""name"":""murmur32"",""parameters"":[{""name"":""data"",""type"":""ByteArray""},{""name"":""seed"",""type"":""Integer""}],""returntype"":""ByteArray"",""offset"":49,""safe"":true},{""name"":""ripemd160"",""parameters"":[{""name"":""data"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":56,""safe"":true},{""name"":""sha256"",""parameters"":[{""name"":""data"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":63,""safe"":true},{""name"":""verifyWithECDsa"",""parameters"":[{""name"":""message"",""type"":""ByteArray""},{""name"":""pubkey"",""type"":""ByteArray""},{""name"":""signature"",""type"":""ByteArray""},{""name"":""curve"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":70,""safe"":true}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":null}");

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
    public abstract bool VerifyWithECDsa(byte[] message, byte[] pubkey, byte[] signature, NamedCurve curve);

    #endregion

    #region Constructor for internal use only

    protected CryptoLib(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
