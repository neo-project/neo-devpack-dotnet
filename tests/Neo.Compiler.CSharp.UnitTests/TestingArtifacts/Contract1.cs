using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract1(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract1"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_001"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""testVoid"",""parameters"":[],""returntype"":""Void"",""offset"":14,""safe"":false},{""name"":""testArgs1"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""ByteArray"",""offset"":27,""safe"":false},{""name"":""testArgs2"",""parameters"":[{""name"":""a"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":48,""safe"":false},{""name"":""testArgs3"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":53,""safe"":false},{""name"":""testArgs4"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":108,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAANNXAQAMBAECAwTbMHBoQFcBAAwEAQIDBNswcEBXAQEMBAECAwPbMHB4SmgTUdBFaEBXAAF4QFcAAngSnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+AeEBXAAJ4Ep5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfgHh5nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9A94cdyg=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBDAQBAgMD2zBweEpoE1HQRWhA
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : PUSHDATA1 01020303 [8 datoshi]
    /// 09 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 0B : STLOC0 [2 datoshi]
    /// 0C : LDARG0 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : LDLOC0 [2 datoshi]
    /// 0F : PUSH3 [1 datoshi]
    /// 10 : ROT [2 datoshi]
    /// 11 : SETITEM [8192 datoshi]
    /// 12 : DROP [2 datoshi]
    /// 13 : LDLOC0 [2 datoshi]
    /// 14 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testArgs1")]
    public abstract byte[]? TestArgs1(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testArgs2")]
    public abstract byte[]? TestArgs2(byte[]? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeBKeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn4B4QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : ADD [8 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : PUSHINT32 00000080 [1 datoshi]
    /// 0C : JMPGE 04 [2 datoshi]
    /// 0E : JMP 0A [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 16 : JMPLE 1E [2 datoshi]
    /// 18 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 21 : AND [8 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 28 : JMPLE 0C [2 datoshi]
    /// 2A : PUSHINT64 0000000001000000 [1 datoshi]
    /// 33 : SUB [8 datoshi]
    /// 34 : STARG0 [2 datoshi]
    /// 35 : LDARG0 [2 datoshi]
    /// 36 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testArgs3")]
    public abstract BigInteger? TestArgs3(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeBKeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn4B4eZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : ADD [8 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : PUSHINT32 00000080 [1 datoshi]
    /// 0C : JMPGE 04 [2 datoshi]
    /// 0E : JMP 0A [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 16 : JMPLE 1E [2 datoshi]
    /// 18 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 21 : AND [8 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 28 : JMPLE 0C [2 datoshi]
    /// 2A : PUSHINT64 0000000001000000 [1 datoshi]
    /// 33 : SUB [8 datoshi]
    /// 34 : STARG0 [2 datoshi]
    /// 35 : LDARG0 [2 datoshi]
    /// 36 : LDARG1 [2 datoshi]
    /// 37 : ADD [8 datoshi]
    /// 38 : DUP [2 datoshi]
    /// 39 : PUSHINT32 00000080 [1 datoshi]
    /// 3E : JMPGE 04 [2 datoshi]
    /// 40 : JMP 0A [2 datoshi]
    /// 42 : DUP [2 datoshi]
    /// 43 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 48 : JMPLE 1E [2 datoshi]
    /// 4A : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 53 : AND [8 datoshi]
    /// 54 : DUP [2 datoshi]
    /// 55 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 5A : JMPLE 0C [2 datoshi]
    /// 5C : PUSHINT64 0000000001000000 [1 datoshi]
    /// 65 : SUB [8 datoshi]
    /// 66 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testArgs4")]
    public abstract BigInteger? TestArgs4(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADAQBAgME2zBwQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHDATA1 01020304 [8 datoshi]
    /// 09 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 0B : STLOC0 [2 datoshi]
    /// 0C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testVoid")]
    public abstract void TestVoid();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADAQBAgME2zBwaEA=
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHDATA1 01020304 [8 datoshi]
    /// 09 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 0B : STLOC0 [2 datoshi]
    /// 0C : LDLOC0 [2 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_001")]
    public abstract byte[]? UnitTest_001();

    #endregion
}
