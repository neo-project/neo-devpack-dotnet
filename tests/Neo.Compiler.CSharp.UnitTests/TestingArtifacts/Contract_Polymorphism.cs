using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Polymorphism(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Polymorphism"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":539,""safe"":false},{""name"":""test"",""parameters"":[],""returntype"":""String"",""offset"":547,""safe"":false},{""name"":""test2"",""parameters"":[],""returntype"":""String"",""offset"":553,""safe"":false},{""name"":""abstractTest"",""parameters"":[],""returntype"":""String"",""offset"":559,""safe"":false},{""name"":""mul"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":565,""safe"":false},{""name"":""sumToBeOverriden"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":573,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":508,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1EAlcAA3l6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwABDAl0ZXN0RmluYWxAVwABeDQTeDQai9soDAUudGVzdIvbKEBXAAEMBHRlc3RAVwABeDQODAYudGVzdDKL2yhAVwABDAViYXNlMkBXAAF4NBkMEW92ZXJyaWRlbkFic3RyYWN0i9soQFcAAQwMYWJzdHJhY3RUZXN0QFcAA3l6oEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwADeXqfSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAN6eXg1zwAAAHl6oEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3pKAIAuBCIHSgB/Mg8B/wCRSgB/MgYBAAGfeUoAgC4EIgdKAH8yDwH/AJFKAH8yBgEAAZ94NSv///+eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAN6eXg1Cv7//0BWAgsKeP7//woAAAAAE8BgCwpq/v//CgAAAAATwGFAWBHAI+L9///CIxH+///CIxr+///CI1L+//9ZEcAjef7//8Ij3f7//0A/AaJe"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQZDBFvdmVycmlkZW5BYnN0cmFjdIvbKEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : CALL 19 [512 datoshi]
    /// 06 : PUSHDATA1 6F766572726964656E4162737472616374 'overridenAbstract' [8 datoshi]
    /// 19 : CAT [2048 datoshi]
    /// 1A : CONVERT 28 'ByteString' [8192 datoshi]
    /// 1C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("abstractTest")]
    public abstract string? AbstractTest();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeXqgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : INITSLOT 0003 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG2 [2 datoshi]
    /// 05 : MUL [8 datoshi]
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
    /// 34 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("mul")]
    public abstract BigInteger? Mul(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeXqeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : INITSLOT 0003 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG2 [2 datoshi]
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
    /// 34 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADenl4Nc8AAAB5eqBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ96SgCALgQiB0oAfzIPAf8AkUoAfzIGAQABn3lKAIAuBCIHSgB/Mg8B/wCRSgB/MgYBAAGfeDUr////nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9A
    /// 00 : INITSLOT 0003 [64 datoshi]
    /// 03 : LDARG2 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : LDARG0 [2 datoshi]
    /// 06 : CALL_L CF000000 [512 datoshi]
    /// 0B : LDARG1 [2 datoshi]
    /// 0C : LDARG2 [2 datoshi]
    /// 0D : MUL [8 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : PUSHINT32 00000080 [1 datoshi]
    /// 14 : JMPGE 04 [2 datoshi]
    /// 16 : JMP 0A [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 1E : JMPLE 1E [2 datoshi]
    /// 20 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 29 : AND [8 datoshi]
    /// 2A : DUP [2 datoshi]
    /// 2B : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 30 : JMPLE 0C [2 datoshi]
    /// 32 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 3B : SUB [8 datoshi]
    /// 3C : ADD [8 datoshi]
    /// 3D : DUP [2 datoshi]
    /// 3E : PUSHINT32 00000080 [1 datoshi]
    /// 43 : JMPGE 04 [2 datoshi]
    /// 45 : JMP 0A [2 datoshi]
    /// 47 : DUP [2 datoshi]
    /// 48 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 4D : JMPLE 1E [2 datoshi]
    /// 4F : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 58 : AND [8 datoshi]
    /// 59 : DUP [2 datoshi]
    /// 5A : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 5F : JMPLE 0C [2 datoshi]
    /// 61 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 6A : SUB [8 datoshi]
    /// 6B : LDARG2 [2 datoshi]
    /// 6C : DUP [2 datoshi]
    /// 6D : PUSHINT8 80 [1 datoshi]
    /// 6F : JMPGE 04 [2 datoshi]
    /// 71 : JMP 07 [2 datoshi]
    /// 73 : DUP [2 datoshi]
    /// 74 : PUSHINT8 7F [1 datoshi]
    /// 76 : JMPLE 0F [2 datoshi]
    /// 78 : PUSHINT16 FF00 [1 datoshi]
    /// 7B : AND [8 datoshi]
    /// 7C : DUP [2 datoshi]
    /// 7D : PUSHINT8 7F [1 datoshi]
    /// 7F : JMPLE 06 [2 datoshi]
    /// 81 : PUSHINT16 0001 [1 datoshi]
    /// 84 : SUB [8 datoshi]
    /// 85 : LDARG1 [2 datoshi]
    /// 86 : DUP [2 datoshi]
    /// 87 : PUSHINT8 80 [1 datoshi]
    /// 89 : JMPGE 04 [2 datoshi]
    /// 8B : JMP 07 [2 datoshi]
    /// 8D : DUP [2 datoshi]
    /// 8E : PUSHINT8 7F [1 datoshi]
    /// 90 : JMPLE 0F [2 datoshi]
    /// 92 : PUSHINT16 FF00 [1 datoshi]
    /// 95 : AND [8 datoshi]
    /// 96 : DUP [2 datoshi]
    /// 97 : PUSHINT8 7F [1 datoshi]
    /// 99 : JMPLE 06 [2 datoshi]
    /// 9B : PUSHINT16 0001 [1 datoshi]
    /// 9E : SUB [8 datoshi]
    /// 9F : LDARG0 [2 datoshi]
    /// A0 : CALL_L 2BFFFFFF [512 datoshi]
    /// A5 : ADD [8 datoshi]
    /// A6 : DUP [2 datoshi]
    /// A7 : PUSHINT32 00000080 [1 datoshi]
    /// AC : JMPGE 04 [2 datoshi]
    /// AE : JMP 0A [2 datoshi]
    /// B0 : DUP [2 datoshi]
    /// B1 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// B6 : JMPLE 1E [2 datoshi]
    /// B8 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// C1 : AND [8 datoshi]
    /// C2 : DUP [2 datoshi]
    /// C3 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// C8 : JMPLE 0C [2 datoshi]
    /// CA : PUSHINT64 0000000001000000 [1 datoshi]
    /// D3 : SUB [8 datoshi]
    /// D4 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("sumToBeOverriden")]
    public abstract BigInteger? SumToBeOverriden(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAl0ZXN0RmluYWxA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHDATA1 7465737446696E616C 'testFinal' [8 datoshi]
    /// 0E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("test")]
    public abstract string? Test();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQTeDQai9soDAUudGVzdIvbKEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : CALL 13 [512 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : CALL 1A [512 datoshi]
    /// 09 : CAT [2048 datoshi]
    /// 0A : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0C : PUSHDATA1 2E74657374 '.test' [8 datoshi]
    /// 13 : CAT [2048 datoshi]
    /// 14 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 16 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("test2")]
    public abstract string? Test2();

    #endregion
}
