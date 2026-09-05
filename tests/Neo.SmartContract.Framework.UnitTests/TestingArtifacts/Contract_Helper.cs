using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Helper(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Helper"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testHexToBytes"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""assertCall"",""parameters"":[{""name"":""value"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":9,""safe"":false},{""name"":""testToBigInteger"",""parameters"":[{""name"":""data"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":38,""safe"":false},{""name"":""testNumEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":45,""safe"":false},{""name"":""testNumNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":52,""safe"":false},{""name"":""modMultiply"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""},{""name"":""modulus"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":59,""safe"":false},{""name"":""modInverse"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""modulus"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":67,""safe"":false},{""name"":""modPow"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""exponent"",""type"":""Integer""},{""name"":""modulus"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":83,""safe"":false},{""name"":""testBigIntegerCast"",""parameters"":[{""name"":""data"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":91,""safe"":false},{""name"":""testBigIntegerParseHexString"",""parameters"":[{""name"":""data"",""type"":""String""}],""returntype"":""Integer"",""offset"":107,""safe"":false},{""name"":""voidAssertCall"",""parameters"":[{""name"":""value"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":115,""safe"":false},{""name"":""testByteToByteArray"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""ByteArray"",""offset"":123,""safe"":false},{""name"":""testReverse"",""parameters"":[],""returntype"":""ByteArray"",""offset"":137,""safe"":false},{""name"":""testSbyteToByteArray"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""ByteArray"",""offset"":152,""safe"":false},{""name"":""testStringToByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":166,""safe"":false},{""name"":""testConcat"",""parameters"":[],""returntype"":""ByteArray"",""offset"":189,""safe"":false},{""name"":""testRange"",""parameters"":[],""returntype"":""ByteArray"",""offset"":214,""safe"":false},{""name"":""testTake"",""parameters"":[],""returntype"":""ByteArray"",""offset"":232,""safe"":false},{""name"":""testLast"",""parameters"":[],""returntype"":""ByteArray"",""offset"":249,""safe"":false},{""name"":""testToScriptHash"",""parameters"":[],""returntype"":""ByteArray"",""offset"":266,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi""]}],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARhdG9pAQABBQAA/SEBDAYKCwwNDg9AVwIBeAiXcAwMVVQtRVJST1ItMTIzcWgkBGngFUBXAAF42yFAVwACeHmzQFcAAnh5tEBXAAN4eXqlQFcAAnl4NANAVwACeA95pkBXAAN4eXqmQFcAAXjbKErYJgVFEEDbIUBXAAF4NwAAQFcAAXgIlzlAVwABeEokBkUMAQARjUBXAQAMAwECA9swStFwaEBXAAF4SiQGRQwBABGNQFcCAAwLaGVsbG8gd29ybGRwaNswcWlAVwMADAMBAgPbMHAMAwQFBtswcWhpi3JqQFcCAAwDAQID2zBwaBERjHFpQFcCAAwDAQID2zBwaBKNcWlAVwIADAMBAgPbMHBoEo5xaUAMFAECAwQFBgcICQoLDA0OD6q7zN3uQF92VSo=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBeAiXcAwMVVQtRVJST1ItMTIzcWgkBGngFUA=
    /// INITSLOT 0201 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// EQUAL [32 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 55542D4552524F522D313233 'UT-ERROR-123' [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// LDLOC1 [2 datoshi]
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
    /// Script: VwABeEokBkUMAQARjUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// JMPIF 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 00 [8 datoshi]
    /// PUSH1 [1 datoshi]
    /// LEFT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteToByteArray")]
    public abstract byte[]? TestByteToByteArray(BigInteger? a);

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
    /// Script: VwABeEokBkUMAQARjUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// JMPIF 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 00 [8 datoshi]
    /// PUSH1 [1 datoshi]
    /// LEFT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSbyteToByteArray")]
    public abstract byte[]? TestSbyteToByteArray(BigInteger? a);

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
