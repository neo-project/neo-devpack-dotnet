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
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT16 0001 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteparse")]
    public abstract BigInteger? TestByteparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEoCAAAAgAMAAACAAAAAALskAzpA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// PUSHINT64 0000008000000000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntparse")]
    public abstract BigInteger? TestIntparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEoDAAAAAAAAAIAEAAAAAAAAAIAAAAAAAAAAALskAzpA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// PUSHINT128 00000000000000800000000000000000 [4 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongparse")]
    public abstract BigInteger? TestLongparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEoAgAGAALskAzpA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 80 [1 datoshi]
    /// PUSHINT16 8000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSbyteparse")]
    public abstract BigInteger? TestSbyteparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEoBAIACAIAAALskAzpA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT16 0080 [1 datoshi]
    /// PUSHINT32 00800000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortparse")]
    public abstract BigInteger? TestShortparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEoQAwAAAAABAAAAuyQDOkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUintparse")]
    public abstract BigInteger? TestUintparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEoQBAAAAAAAAAAAAQAAAAAAAAC7JAM6QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUlongparse")]
    public abstract BigInteger? TestUlongparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEoQAgAAAQC7JAM6QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT32 00000100 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUshortparse")]
    public abstract BigInteger? TestUshortparse(string? s);

    #endregion
}
