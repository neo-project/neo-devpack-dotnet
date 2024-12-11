using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Helper(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Helper"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testHexToBytes"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""assertCall"",""parameters"":[{""name"":""value"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":9,""safe"":false},{""name"":""testToBigInteger"",""parameters"":[{""name"":""data"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":34,""safe"":false},{""name"":""testNumEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":41,""safe"":false},{""name"":""testNumNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":48,""safe"":false},{""name"":""modMultiply"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""},{""name"":""modulus"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":55,""safe"":false},{""name"":""modInverse"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""modulus"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":63,""safe"":false},{""name"":""modPow"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""exponent"",""type"":""Integer""},{""name"":""modulus"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":79,""safe"":false},{""name"":""testBigIntegerCast"",""parameters"":[{""name"":""data"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":87,""safe"":false},{""name"":""testBigIntegerParseHexString"",""parameters"":[{""name"":""data"",""type"":""String""}],""returntype"":""Integer"",""offset"":103,""safe"":false},{""name"":""voidAssertCall"",""parameters"":[{""name"":""value"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":111,""safe"":false},{""name"":""testByteToByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":119,""safe"":false},{""name"":""testReverse"",""parameters"":[],""returntype"":""ByteArray"",""offset"":130,""safe"":false},{""name"":""testSbyteToByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":145,""safe"":false},{""name"":""testStringToByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":156,""safe"":false},{""name"":""testConcat"",""parameters"":[],""returntype"":""ByteArray"",""offset"":179,""safe"":false},{""name"":""testRange"",""parameters"":[],""returntype"":""ByteArray"",""offset"":204,""safe"":false},{""name"":""testTake"",""parameters"":[],""returntype"":""ByteArray"",""offset"":222,""safe"":false},{""name"":""testLast"",""parameters"":[],""returntype"":""ByteArray"",""offset"":239,""safe"":false},{""name"":""testToScriptHash"",""parameters"":[],""returntype"":""ByteArray"",""offset"":256,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARhdG9pAQABDwAA/RcBDAYKCwwNDg9AVwABeAiXJBEMDFVULUVSUk9SLTEyM+AVQFcAAXjbIUBXAAJ4ebNAVwACeHm0QFcAA3h5eqVAVwACeXg0A0BXAAJ4D3mmQFcAA3h5eqZAVwABeNsoStgmBUUQQNshQFcAAXg3AABAVwABeAiXOUBXAgARcGgRjXFpQFcBAAwDAQID2zBK0XBoQFcCAA9waNswcWlAVwIADAtoZWxsbyB3b3JsZHBo2zBxaUBXAwAMAwECA9swcAwDBAUG2zBxaGmLcmpAVwIADAMBAgPbMHBoERGMcWlAVwIADAMBAgPbMHBoEo1xaUBXAgAMAwECA9swcGgSjnFpQAwUAQIDBAUGBwgJCgsMDQ4PqrvM3e5ArloZ8w=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAiXJBEMDFVULUVSUk9SLTEyM+AVQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 11 [2 datoshi]
    /// PUSHDATA1 55542D4552524F522D313233 'UT-ERROR-123' [8 datoshi]
    /// ABORTMSG [0 datoshi]
    /// PUSH5 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("assertCall")]
    public abstract BigInteger? AssertCall(bool? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg0A0A=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("modInverse")]
    public abstract BigInteger? ModInverse(BigInteger? value, BigInteger? modulus);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6pUA=
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// MODMUL [32 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("modMultiply")]
    public abstract BigInteger? ModMultiply(BigInteger? value, BigInteger? y, BigInteger? modulus);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6pkA=
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// MODPOW [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("modPow")]
    public abstract BigInteger? ModPow(BigInteger? value, BigInteger? exponent, BigInteger? modulus);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNsoStgmBUUQQNshQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// CONVERT 21 'Integer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerCast")]
    public abstract BigInteger? TestBigIntegerCast(byte[]? data);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerParseHexString")]
    public abstract BigInteger? TestBigIntegerParseHexString(string? data);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEXBoEY1xaUA=
    /// INITSLOT 0200 [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// LEFT [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteToByteArray")]
    public abstract byte[]? TestByteToByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMADAMBAgPbMHAMAwQFBtswcWhpi3JqQA==
    /// INITSLOT 0300 [64 datoshi]
    /// PUSHDATA1 010203 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 040506 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CAT [2048 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testConcat")]
    public abstract byte[]? TestConcat();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAYKCwwNDg9A
    /// PUSHDATA1 0A0B0C0D0E0F [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testHexToBytes")]
    public abstract byte[]? TestHexToBytes();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADAMBAgPbMHBoEo5xaUA=
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHDATA1 010203 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// RIGHT [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLast")]
    public abstract byte[]? TestLast();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmzQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNumEqual")]
    public abstract bool? TestNumEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm0QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// NUMNOTEQUAL [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNumNotEqual")]
    public abstract bool? TestNumNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADAMBAgPbMHBoERGMcWlA
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHDATA1 010203 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// SUBSTR [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRange")]
    public abstract byte[]? TestRange();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADAMBAgPbMErRcGhA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHDATA1 010203 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// DUP [2 datoshi]
    /// REVERSEITEMS [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testReverse")]
    public abstract byte[]? TestReverse();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAD3Bo2zBxaUA=
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHM1 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSbyteToByteArray")]
    public abstract byte[]? TestSbyteToByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADAtoZWxsbyB3b3JsZHBo2zBxaUA=
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHDATA1 68656C6C6F20776F726C64 [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringToByteArray")]
    public abstract byte[]? TestStringToByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADAMBAgPbMHBoEo1xaUA=
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHDATA1 010203 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// LEFT [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testTake")]
    public abstract byte[]? TestTake();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNshQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 21 'Integer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testToBigInteger")]
    public abstract BigInteger? TestToBigInteger(byte[]? data);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DBQBAgMEBQYHCAkKCwwNDg+qu8zd7kA=
    /// PUSHDATA1 0102030405060708090A0B0C0D0E0FAABBCCDDEE [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testToScriptHash")]
    public abstract byte[]? TestToScriptHash();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAiXOUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("voidAssertCall")]
    public abstract void VoidAssertCall(bool? value);

    #endregion
}
