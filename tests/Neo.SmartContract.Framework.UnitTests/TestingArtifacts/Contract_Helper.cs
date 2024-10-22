using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Helper(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Helper"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testHexToBytes"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""assertCall"",""parameters"":[{""name"":""value"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":2,""safe"":false},{""name"":""testToBigInteger"",""parameters"":[{""name"":""data"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":25,""safe"":false},{""name"":""modMultiply"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""},{""name"":""modulus"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":32,""safe"":false},{""name"":""modInverse"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""modulus"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":40,""safe"":false},{""name"":""modPow"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""exponent"",""type"":""Integer""},{""name"":""modulus"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":56,""safe"":false},{""name"":""testBigIntegerCast"",""parameters"":[{""name"":""data"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":64,""safe"":false},{""name"":""testBigIntegerParseHexString"",""parameters"":[{""name"":""data"",""type"":""String""}],""returntype"":""Integer"",""offset"":79,""safe"":false},{""name"":""voidAssertCall"",""parameters"":[{""name"":""value"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":87,""safe"":false},{""name"":""testByteToByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":95,""safe"":false},{""name"":""testReverse"",""parameters"":[],""returntype"":""ByteArray"",""offset"":106,""safe"":false},{""name"":""testSbyteToByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":121,""safe"":false},{""name"":""testStringToByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":132,""safe"":false},{""name"":""testConcat"",""parameters"":[],""returntype"":""ByteArray"",""offset"":155,""safe"":false},{""name"":""testRange"",""parameters"":[],""returntype"":""ByteArray"",""offset"":180,""safe"":false},{""name"":""testTake"",""parameters"":[],""returntype"":""ByteArray"",""offset"":198,""safe"":false},{""name"":""testLast"",""parameters"":[],""returntype"":""ByteArray"",""offset"":215,""safe"":false},{""name"":""testToScriptHash"",""parameters"":[],""returntype"":""ByteArray"",""offset"":232,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":234,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARhdG9pAQABDwAA/Q0BWEBXAAF4CJcMDFVULUVSUk9SLTEyM+EVQFcAAXjbIUBXAAN4eXqlQFcAAnl4NANAVwACeA95pkBXAAN4eXqmQFcAAXjbKErYJgRFENshQFcAAXg3AABAVwABeAiXOUBXAgARcGgRjXFpQFcBAAwDAQID2zBK0XBoQFcCAA9waNswcWlAVwIADAtoZWxsbyB3b3JsZHBo2zBxaUBXAwAMAwECA9swcAwDBAUG2zBxaGmLcmpAVwIADAMBAgPbMHBoERGMcWlAVwIADAMBAgPbMHBoEo1xaUBXAgAMAwECA9swcGgSjnFpQFlAVgIMBgoLDA0OD2AMFAECAwQFBgcICQoLDA0OD6q7zN3uYUCPhy73"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAiXDFVULUVSUk9SLTEyM+EVQA==
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.PUSHT 	-> 1 datoshi
    /// 05 : OpCode.EQUAL 	-> 32 datoshi
    /// 06 : OpCode.PUSHDATA1 55542D4552524F522D313233 	-> 8 datoshi
    /// 14 : OpCode.ASSERTMSG 	-> 1 datoshi
    /// 15 : OpCode.PUSH5 	-> 1 datoshi
    /// 16 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("assertCall")]
    public abstract BigInteger? AssertCall(bool? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeA95pkA=
    /// 00 : OpCode.INITSLOT 0002 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.PUSHM1 	-> 1 datoshi
    /// 05 : OpCode.LDARG1 	-> 2 datoshi
    /// 06 : OpCode.MODPOW 	-> 2048 datoshi
    /// 07 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("modInverse")]
    public abstract BigInteger? ModInverse(BigInteger? value, BigInteger? modulus);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6pUA=
    /// 00 : OpCode.INITSLOT 0003 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.LDARG1 	-> 2 datoshi
    /// 05 : OpCode.LDARG2 	-> 2 datoshi
    /// 06 : OpCode.MODMUL 	-> 32 datoshi
    /// 07 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("modMultiply")]
    public abstract BigInteger? ModMultiply(BigInteger? value, BigInteger? y, BigInteger? modulus);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6pkA=
    /// 00 : OpCode.INITSLOT 0003 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.LDARG1 	-> 2 datoshi
    /// 05 : OpCode.LDARG2 	-> 2 datoshi
    /// 06 : OpCode.MODPOW 	-> 2048 datoshi
    /// 07 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("modPow")]
    public abstract BigInteger? ModPow(BigInteger? value, BigInteger? exponent, BigInteger? modulus);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNsoStgmBEUQ2yFA
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 06 : OpCode.DUP 	-> 2 datoshi
    /// 07 : OpCode.ISNULL 	-> 2 datoshi
    /// 08 : OpCode.JMPIFNOT 04 	-> 2 datoshi
    /// 0A : OpCode.DROP 	-> 2 datoshi
    /// 0B : OpCode.PUSH0 	-> 1 datoshi
    /// 0C : OpCode.CONVERT 21 	-> 8192 datoshi
    /// 0E : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testBigIntegerCast")]
    public abstract BigInteger? TestBigIntegerCast(byte[]? data);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEA=
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.CALLT 0000 	-> 32768 datoshi
    /// 07 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testBigIntegerParseHexString")]
    public abstract BigInteger? TestBigIntegerParseHexString(string? data);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEXBoEY1xaUA=
    /// 00 : OpCode.INITSLOT 0200 	-> 64 datoshi
    /// 03 : OpCode.PUSH1 	-> 1 datoshi
    /// 04 : OpCode.STLOC0 	-> 2 datoshi
    /// 05 : OpCode.LDLOC0 	-> 2 datoshi
    /// 06 : OpCode.PUSH1 	-> 1 datoshi
    /// 07 : OpCode.LEFT 	-> 2048 datoshi
    /// 08 : OpCode.STLOC1 	-> 2 datoshi
    /// 09 : OpCode.LDLOC1 	-> 2 datoshi
    /// 0A : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testByteToByteArray")]
    public abstract byte[]? TestByteToByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testConcat")]
    public abstract byte[]? TestConcat();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WEA=
    /// 00 : OpCode.LDSFLD0 	-> 2 datoshi
    /// 01 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testHexToBytes")]
    public abstract byte[]? TestHexToBytes();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testLast")]
    public abstract byte[]? TestLast();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testRange")]
    public abstract byte[]? TestRange();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testReverse")]
    public abstract byte[]? TestReverse();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testSbyteToByteArray")]
    public abstract byte[]? TestSbyteToByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testStringToByteArray")]
    public abstract byte[]? TestStringToByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testTake")]
    public abstract byte[]? TestTake();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNshQA==
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.CONVERT 21 	-> 8192 datoshi
    /// 06 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testToBigInteger")]
    public abstract BigInteger? TestToBigInteger(byte[]? data);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testToScriptHash")]
    public abstract byte[]? TestToScriptHash();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAiXOUA=
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.PUSHT 	-> 1 datoshi
    /// 05 : OpCode.EQUAL 	-> 32 datoshi
    /// 06 : OpCode.ASSERT 	-> 1 datoshi
    /// 07 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("voidAssertCall")]
    public abstract void VoidAssertCall(bool? value);

    #endregion
}
