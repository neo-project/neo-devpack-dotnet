using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Crypto : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Crypto"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""SHA256"",""parameters"":[{""name"":""value"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""RIPEMD160"",""parameters"":[{""name"":""value"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":14,""safe"":false},{""name"":""murmur32"",""parameters"":[{""name"":""value"",""type"":""ByteArray""},{""name"":""seed"",""type"":""Integer""}],""returntype"":""ByteArray"",""offset"":28,""safe"":false},{""name"":""secp256r1VerifySignatureWithMessage"",""parameters"":[{""name"":""message"",""type"":""ByteArray""},{""name"":""pubkey"",""type"":""PublicKey""},{""name"":""signature"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":43,""safe"":false},{""name"":""secp256k1VerifySignatureWithMessage"",""parameters"":[{""name"":""message"",""type"":""ByteArray""},{""name"":""pubkey"",""type"":""PublicKey""},{""name"":""signature"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":61,""safe"":false},{""name"":""bls12381Serialize"",""parameters"":[{""name"":""data"",""type"":""Any""}],""returntype"":""ByteArray"",""offset"":79,""safe"":false},{""name"":""bls12381Deserialize"",""parameters"":[{""name"":""data"",""type"":""ByteArray""}],""returntype"":""Any"",""offset"":89,""safe"":false},{""name"":""bls12381Equal"",""parameters"":[{""name"":""x"",""type"":""Any""},{""name"":""y"",""type"":""Any""}],""returntype"":""Any"",""offset"":99,""safe"":false},{""name"":""bls12381Add"",""parameters"":[{""name"":""x"",""type"":""Any""},{""name"":""y"",""type"":""Any""}],""returntype"":""Any"",""offset"":110,""safe"":false},{""name"":""bls12381Mul"",""parameters"":[{""name"":""x"",""type"":""Any""},{""name"":""mul"",""type"":""ByteArray""},{""name"":""neg"",""type"":""Boolean""}],""returntype"":""Any"",""offset"":121,""safe"":false},{""name"":""bls12381Pairing"",""parameters"":[{""name"":""g1"",""type"":""Any""},{""name"":""g2"",""type"":""Any""}],""returntype"":""Any"",""offset"":133,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""bls12381Add"",""bls12381Deserialize"",""bls12381Equal"",""bls12381Mul"",""bls12381Pairing"",""bls12381Serialize"",""murmur32"",""ripemd160"",""sha256"",""verifyWithECDsa""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM3Rlc3Rob3N0AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAob9XWrEYlohBNhCjWhKIbN4LZscgZzaGEyNTYBAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHIJcmlwZW1kMTYwAQABDxv1dasRiWiEE2EKNaEohs3gtmxyCG11cm11cjMyAgABDxv1dasRiWiEE2EKNaEohs3gtmxyD3ZlcmlmeVdpdGhFQ0RzYQQAAQ8b9XWrEYlohBNhCjWhKIbN4LZschFibHMxMjM4MVNlcmlhbGl6ZQEAAQ8b9XWrEYlohBNhCjWhKIbN4LZschNibHMxMjM4MURlc2VyaWFsaXplAQABDxv1dasRiWiEE2EKNaEohs3gtmxyDWJsczEyMzgxRXF1YWwCAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHILYmxzMTIzODFBZGQCAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHILYmxzMTIzODFNdWwDAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHIPYmxzMTIzODFQYWlyaW5nAgABDwAAkFcAAXjbKDcAANswIgJAVwABeNsoNwEA2zAiAkBXAAJ5eNsoNwIA2zAiAkBXAAMAF3rbKHl42yg3AwAiAkBXAAMAFnrbKHl42yg3AwAiAkBXAAF4NwQAIgJAVwABeDcFACICQFcAAnl4NwYAIgJAVwACeXg3BwAiAkBXAAN6eXg3CAAiAkBXAAJ5eDcJACICQEaICfM="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("bls12381Add")]
    public abstract object? Bls12381Add(object? x, object? y = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("bls12381Deserialize")]
    public abstract object? Bls12381Deserialize(byte[]? data);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("bls12381Equal")]
    public abstract object? Bls12381Equal(object? x, object? y = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("bls12381Mul")]
    public abstract object? Bls12381Mul(object? x, byte[]? mul, bool? neg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("bls12381Pairing")]
    public abstract object? Bls12381Pairing(object? g1, object? g2 = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("bls12381Serialize")]
    public abstract byte[]? Bls12381Serialize(object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("murmur32")]
    public abstract byte[]? Murmur32(byte[]? value, BigInteger? seed);

    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract byte[]? RIPEMD160(byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("secp256k1VerifySignatureWithMessage")]
    public abstract bool? Secp256k1VerifySignatureWithMessage(byte[]? message, ECPoint? pubkey, byte[]? signature);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("secp256r1VerifySignatureWithMessage")]
    public abstract bool? Secp256r1VerifySignatureWithMessage(byte[]? message, ECPoint? pubkey, byte[]? signature);

    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract byte[]? SHA256(byte[]? value);

    #endregion

    #region Constructor for internal use only

    protected Contract_Crypto(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
