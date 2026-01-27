using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Crypto(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Crypto"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""SHA256"",""parameters"":[{""name"":""value"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""RIPEMD160"",""parameters"":[{""name"":""value"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":12,""safe"":false},{""name"":""murmur32"",""parameters"":[{""name"":""value"",""type"":""ByteArray""},{""name"":""seed"",""type"":""Integer""}],""returntype"":""ByteArray"",""offset"":24,""safe"":false},{""name"":""secp256r1VerifySignatureWithMessage"",""parameters"":[{""name"":""message"",""type"":""ByteArray""},{""name"":""pubkey"",""type"":""PublicKey""},{""name"":""signature"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":37,""safe"":false},{""name"":""secp256r1VerifyKeccakSignatureWithMessage"",""parameters"":[{""name"":""message"",""type"":""ByteArray""},{""name"":""pubkey"",""type"":""PublicKey""},{""name"":""signature"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":53,""safe"":false},{""name"":""secp256k1VerifySignatureWithMessage"",""parameters"":[{""name"":""message"",""type"":""ByteArray""},{""name"":""pubkey"",""type"":""PublicKey""},{""name"":""signature"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":69,""safe"":false},{""name"":""secp256k1VerifyKeccakSignatureWithMessage"",""parameters"":[{""name"":""message"",""type"":""ByteArray""},{""name"":""pubkey"",""type"":""PublicKey""},{""name"":""signature"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":85,""safe"":false},{""name"":""recoverSecp256K1"",""parameters"":[{""name"":""messageHash"",""type"":""ByteArray""},{""name"":""signature"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":101,""safe"":false},{""name"":""verifyWithEd25519"",""parameters"":[{""name"":""message"",""type"":""ByteArray""},{""name"":""pubkey"",""type"":""ByteArray""},{""name"":""signature"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":114,""safe"":false},{""name"":""bls12381Serialize"",""parameters"":[{""name"":""data"",""type"":""Any""}],""returntype"":""ByteArray"",""offset"":130,""safe"":false},{""name"":""bls12381Deserialize"",""parameters"":[{""name"":""data"",""type"":""ByteArray""}],""returntype"":""Any"",""offset"":138,""safe"":false},{""name"":""bls12381Equal"",""parameters"":[{""name"":""x"",""type"":""Any""},{""name"":""y"",""type"":""Any""}],""returntype"":""Any"",""offset"":146,""safe"":false},{""name"":""bls12381Add"",""parameters"":[{""name"":""x"",""type"":""Any""},{""name"":""y"",""type"":""Any""}],""returntype"":""Any"",""offset"":155,""safe"":false},{""name"":""bls12381Mul"",""parameters"":[{""name"":""x"",""type"":""Any""},{""name"":""mul"",""type"":""ByteArray""},{""name"":""neg"",""type"":""Boolean""}],""returntype"":""Any"",""offset"":164,""safe"":false},{""name"":""bls12381Pairing"",""parameters"":[{""name"":""g1"",""type"":""Any""},{""name"":""g2"",""type"":""Any""}],""returntype"":""Any"",""offset"":174,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""bls12381Add"",""bls12381Deserialize"",""bls12381Equal"",""bls12381Mul"",""bls12381Pairing"",""bls12381Serialize"",""murmur32"",""recoverSecp256K1"",""ripemd160"",""sha256"",""verifyWithECDsa"",""verifyWithEd25519""]}],""trusts"":[],""extra"":{""Version"":""3.9.0"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAwb9XWrEYlohBNhCjWhKIbN4LZscgZzaGEyNTYBAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHIJcmlwZW1kMTYwAQABDxv1dasRiWiEE2EKNaEohs3gtmxyCG11cm11cjMyAgABDxv1dasRiWiEE2EKNaEohs3gtmxyD3ZlcmlmeVdpdGhFQ0RzYQQAAQ8b9XWrEYlohBNhCjWhKIbN4LZschByZWNvdmVyU2VjcDI1NksxAgABDxv1dasRiWiEE2EKNaEohs3gtmxyEXZlcmlmeVdpdGhFZDI1NTE5AwABDxv1dasRiWiEE2EKNaEohs3gtmxyEWJsczEyMzgxU2VyaWFsaXplAQABDxv1dasRiWiEE2EKNaEohs3gtmxyE2JsczEyMzgxRGVzZXJpYWxpemUBAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHINYmxzMTIzODFFcXVhbAIAAQ8b9XWrEYlohBNhCjWhKIbN4LZscgtibHMxMjM4MUFkZAIAAQ8b9XWrEYlohBNhCjWhKIbN4LZscgtibHMxMjM4MU11bAMAAQ8b9XWrEYlohBNhCjWhKIbN4LZscg9ibHMxMjM4MVBhaXJpbmcCAAEPAAC3VwABeNsoNwAA2zBAVwABeNsoNwEA2zBAVwACeXjbKDcCANswQFcAAwAXetsoeXjbKDcDAEBXAAMAe3rbKHl42yg3AwBAVwADABZ62yh5eNsoNwMAQFcAAwB6etsoeXjbKDcDAEBXAAJ52yh42yg3BABAVwADetsoedsoeNsoNwUAQFcAAXg3BgBAVwABeDcHAEBXAAJ5eDcIAEBXAAJ5eDcJAEBXAAN6eXg3CgBAVwACeXg3CwBAfEt+cQ==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg3CQBA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0900 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("bls12381Add")]
    public abstract object? Bls12381Add(object? x, object? y = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcHAEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0700 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("bls12381Deserialize")]
    public abstract object? Bls12381Deserialize(byte[]? data);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg3CABA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0800 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("bls12381Equal")]
    public abstract object? Bls12381Equal(object? x, object? y = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADenl4NwoAQA==
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0A00 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("bls12381Mul")]
    public abstract object? Bls12381Mul(object? x, byte[]? mul, bool? neg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg3CwBA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0B00 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("bls12381Pairing")]
    public abstract object? Bls12381Pairing(object? g1, object? g2 = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcGAEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0600 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("bls12381Serialize")]
    public abstract byte[]? Bls12381Serialize(object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXjbKDcCANswQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CALLT 0200 [32768 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("murmur32")]
    public abstract byte[]? Murmur32(byte[]? value, BigInteger? seed);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACedsoeNsoNwQAQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CALLT 0400 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("recoverSecp256K1")]
    public abstract byte[]? RecoverSecp256K1(byte[]? messageHash, byte[]? signature);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNsoNwEA2zBA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CALLT 0100 [32768 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    public abstract byte[]? RIPEMD160(byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADAHp62yh5eNsoNwMAQA==
    /// INITSLOT 0003 [64 datoshi]
    /// PUSHINT8 7A [1 datoshi]
    /// LDARG2 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CALLT 0300 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("secp256k1VerifyKeccakSignatureWithMessage")]
    public abstract bool? Secp256k1VerifyKeccakSignatureWithMessage(byte[]? message, ECPoint? pubkey, byte[]? signature);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADABZ62yh5eNsoNwMAQA==
    /// INITSLOT 0003 [64 datoshi]
    /// PUSHINT8 16 [1 datoshi]
    /// LDARG2 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CALLT 0300 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("secp256k1VerifySignatureWithMessage")]
    public abstract bool? Secp256k1VerifySignatureWithMessage(byte[]? message, ECPoint? pubkey, byte[]? signature);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADAHt62yh5eNsoNwMAQA==
    /// INITSLOT 0003 [64 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// LDARG2 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CALLT 0300 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("secp256r1VerifyKeccakSignatureWithMessage")]
    public abstract bool? Secp256r1VerifyKeccakSignatureWithMessage(byte[]? message, ECPoint? pubkey, byte[]? signature);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADABd62yh5eNsoNwMAQA==
    /// INITSLOT 0003 [64 datoshi]
    /// PUSHINT8 17 [1 datoshi]
    /// LDARG2 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CALLT 0300 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("secp256r1VerifySignatureWithMessage")]
    public abstract bool? Secp256r1VerifySignatureWithMessage(byte[]? message, ECPoint? pubkey, byte[]? signature);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNsoNwAA2zBA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    public abstract byte[]? SHA256(byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADetsoedsoeNsoNwUAQA==
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG2 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG1 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CALLT 0500 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("verifyWithEd25519")]
    public abstract bool? VerifyWithEd25519(byte[]? message, byte[]? pubkey, byte[]? signature);

    #endregion
}
