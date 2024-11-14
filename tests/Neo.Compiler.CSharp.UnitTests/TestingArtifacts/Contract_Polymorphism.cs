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
    /// Script: VwADeXqfSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : INITSLOT 0003 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG2 [2 datoshi]
    /// 05 : SUB [8 datoshi]
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
