using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_IntegerParse(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_IntegerParse"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testSbyteparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testByteparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":18,""safe"":false},{""name"":""testUshortparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":35,""safe"":false},{""name"":""testShortparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":54,""safe"":false},{""name"":""testUlongparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":75,""safe"":false},{""name"":""testLongparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":106,""safe"":false},{""name"":""testUintparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":145,""safe"":false},{""name"":""testIntparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":168,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARhdG9pAQABDwAAw1cAAXg3AABKAIABgAC7JAM6QFcAAXg3AABKEAEAAbskAzpAVwABeDcAAEoQAgAAAQC7JAM6QFcAAXg3AABKAQCAAgCAAAC7JAM6QFcAAXg3AABKEAQAAAAAAAAAAAEAAAAAAAAAuyQDOkBXAAF4NwAASgMAAAAAAAAAgAQAAAAAAAAAgAAAAAAAAAAAuyQDOkBXAAF4NwAAShADAAAAAAEAAAC7JAM6QFcAAXg3AABKAgAAAIADAAAAgAAAAAC7JAM6QPp9YiI="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEoQAQABuyQDOkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALLT 0000 [32768 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.PUSH0 [1 datoshi]
    /// 09 : OpCode.PUSHINT16 0001 [1 datoshi]
    /// 0C : OpCode.WITHIN [8 datoshi]
    /// 0D : OpCode.JMPIF 03 [2 datoshi]
    /// 0F : OpCode.THROW [512 datoshi]
    /// 10 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteparse")]
    public abstract BigInteger? TestByteparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEoCAAAAgAMAAACAAAAAALskAzpA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALLT 0000 [32768 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 0D : OpCode.PUSHINT64 0000008000000000 [1 datoshi]
    /// 16 : OpCode.WITHIN [8 datoshi]
    /// 17 : OpCode.JMPIF 03 [2 datoshi]
    /// 19 : OpCode.THROW [512 datoshi]
    /// 1A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntparse")]
    public abstract BigInteger? TestIntparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEoDAAAAAAAAAIAEAAAAAAAAAIAAAAAAAAAAALskAzpA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALLT 0000 [32768 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.PUSHINT64 0000000000000080 [1 datoshi]
    /// 11 : OpCode.PUSHINT128 00000000000000800000000000000000 [4 datoshi]
    /// 22 : OpCode.WITHIN [8 datoshi]
    /// 23 : OpCode.JMPIF 03 [2 datoshi]
    /// 25 : OpCode.THROW [512 datoshi]
    /// 26 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongparse")]
    public abstract BigInteger? TestLongparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEoAgAGAALskAzpA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALLT 0000 [32768 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.PUSHINT8 80 [1 datoshi]
    /// 0A : OpCode.PUSHINT16 8000 [1 datoshi]
    /// 0D : OpCode.WITHIN [8 datoshi]
    /// 0E : OpCode.JMPIF 03 [2 datoshi]
    /// 10 : OpCode.THROW [512 datoshi]
    /// 11 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSbyteparse")]
    public abstract BigInteger? TestSbyteparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEoBAIACAIAAALskAzpA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALLT 0000 [32768 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.PUSHINT16 0080 [1 datoshi]
    /// 0B : OpCode.PUSHINT32 00800000 [1 datoshi]
    /// 10 : OpCode.WITHIN [8 datoshi]
    /// 11 : OpCode.JMPIF 03 [2 datoshi]
    /// 13 : OpCode.THROW [512 datoshi]
    /// 14 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortparse")]
    public abstract BigInteger? TestShortparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEoQAwAAAAABAAAAuyQDOkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALLT 0000 [32768 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.PUSH0 [1 datoshi]
    /// 09 : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 12 : OpCode.WITHIN [8 datoshi]
    /// 13 : OpCode.JMPIF 03 [2 datoshi]
    /// 15 : OpCode.THROW [512 datoshi]
    /// 16 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUintparse")]
    public abstract BigInteger? TestUintparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEoQBAAAAAAAAAAAAQAAAAAAAAC7JAM6QA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALLT 0000 [32768 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.PUSH0 [1 datoshi]
    /// 09 : OpCode.PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// 1A : OpCode.WITHIN [8 datoshi]
    /// 1B : OpCode.JMPIF 03 [2 datoshi]
    /// 1D : OpCode.THROW [512 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUlongparse")]
    public abstract BigInteger? TestUlongparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEoQAgAAAQC7JAM6QA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALLT 0000 [32768 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.PUSH0 [1 datoshi]
    /// 09 : OpCode.PUSHINT32 00000100 [1 datoshi]
    /// 0E : OpCode.WITHIN [8 datoshi]
    /// 0F : OpCode.JMPIF 03 [2 datoshi]
    /// 11 : OpCode.THROW [512 datoshi]
    /// 12 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUshortparse")]
    public abstract BigInteger? TestUshortparse(string? s);

    #endregion
}
