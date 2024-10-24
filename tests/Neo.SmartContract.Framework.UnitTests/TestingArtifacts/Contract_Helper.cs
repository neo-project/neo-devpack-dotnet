using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Helper(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Helper"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testHexToBytes"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""assertCall"",""parameters"":[{""name"":""value"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":9,""safe"":false},{""name"":""testToBigInteger"",""parameters"":[{""name"":""data"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":32,""safe"":false},{""name"":""modMultiply"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""},{""name"":""modulus"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":39,""safe"":false},{""name"":""modInverse"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""modulus"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":47,""safe"":false},{""name"":""modPow"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""exponent"",""type"":""Integer""},{""name"":""modulus"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":63,""safe"":false},{""name"":""testBigIntegerCast"",""parameters"":[{""name"":""data"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":71,""safe"":false},{""name"":""testBigIntegerParseHexString"",""parameters"":[{""name"":""data"",""type"":""String""}],""returntype"":""Integer"",""offset"":86,""safe"":false},{""name"":""voidAssertCall"",""parameters"":[{""name"":""value"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":94,""safe"":false},{""name"":""testByteToByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":102,""safe"":false},{""name"":""testReverse"",""parameters"":[],""returntype"":""ByteArray"",""offset"":113,""safe"":false},{""name"":""testSbyteToByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":128,""safe"":false},{""name"":""testStringToByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":139,""safe"":false},{""name"":""testConcat"",""parameters"":[],""returntype"":""ByteArray"",""offset"":162,""safe"":false},{""name"":""testRange"",""parameters"":[],""returntype"":""ByteArray"",""offset"":187,""safe"":false},{""name"":""testTake"",""parameters"":[],""returntype"":""ByteArray"",""offset"":205,""safe"":false},{""name"":""testLast"",""parameters"":[],""returntype"":""ByteArray"",""offset"":222,""safe"":false},{""name"":""testToScriptHash"",""parameters"":[],""returntype"":""ByteArray"",""offset"":239,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARhdG9pAQABDwAA/QYBDAYKCwwNDg9AVwABeAiXDAxVVC1FUlJPUi0xMjPhFUBXAAF42yFAVwADeHl6pUBXAAJ5eDQDQFcAAngPeaZAVwADeHl6pkBXAAF42yhK2CYERRDbIUBXAAF4NwAAQFcAAXgIlzlAVwIAEXBoEY1xaUBXAQAMAwECA9swStFwaEBXAgAPcGjbMHFpQFcCAAwLaGVsbG8gd29ybGRwaNswcWlAVwMADAMBAgPbMHAMAwQFBtswcWhpi3JqQFcCAAwDAQID2zBwaBERjHFpQFcCAAwDAQID2zBwaBKNcWlAVwIADAMBAgPbMHBoEo5xaUAMFAECAwQFBgcICQoLDA0OD6q7zN3uQOU1eH8="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAiXDFVULUVSUk9SLTEyM+EVQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHT
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.PUSHDATA1 55542D4552524F522D313233
    /// 14 : OpCode.ASSERTMSG
    /// 15 : OpCode.PUSH5
    /// 16 : OpCode.RET
    /// </remarks>
    [DisplayName("assertCall")]
    public abstract BigInteger? AssertCall(bool? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeA95pkA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHM1
    /// 05 : OpCode.LDARG1
    /// 06 : OpCode.MODPOW
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("modInverse")]
    public abstract BigInteger? ModInverse(BigInteger? value, BigInteger? modulus);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6pUA=
    /// 00 : OpCode.INITSLOT 0003
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.LDARG2
    /// 06 : OpCode.MODMUL
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("modMultiply")]
    public abstract BigInteger? ModMultiply(BigInteger? value, BigInteger? y, BigInteger? modulus);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6pkA=
    /// 00 : OpCode.INITSLOT 0003
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.LDARG2
    /// 06 : OpCode.MODPOW
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("modPow")]
    public abstract BigInteger? ModPow(BigInteger? value, BigInteger? exponent, BigInteger? modulus);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNsoStgmBEUQ2yFA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.CONVERT 28
    /// 06 : OpCode.DUP
    /// 07 : OpCode.ISNULL
    /// 08 : OpCode.JMPIFNOT 04
    /// 0A : OpCode.DROP
    /// 0B : OpCode.PUSH0
    /// 0C : OpCode.CONVERT 21
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("testBigIntegerCast")]
    public abstract BigInteger? TestBigIntegerCast(byte[]? data);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.CALLT 0000
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("testBigIntegerParseHexString")]
    public abstract BigInteger? TestBigIntegerParseHexString(string? data);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEXBoEY1xaUA=
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.PUSH1
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSH1
    /// 07 : OpCode.LEFT
    /// 08 : OpCode.STLOC1
    /// 09 : OpCode.LDLOC1
    /// 0A : OpCode.RET
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
    /// Script: DAoLDA0OD0A=
    /// 00 : OpCode.PUSHDATA1 0A0B0C0D0E0F
    /// 08 : OpCode.RET
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
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.CONVERT 21
    /// 06 : OpCode.RET
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
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHT
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.ASSERT
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("voidAssertCall")]
    public abstract void VoidAssertCall(bool? value);

    #endregion
}
