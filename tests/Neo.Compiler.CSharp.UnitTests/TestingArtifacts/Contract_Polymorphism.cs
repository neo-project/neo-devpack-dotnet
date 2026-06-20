using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Polymorphism(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Polymorphism"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":447,""safe"":false},{""name"":""test"",""parameters"":[],""returntype"":""String"",""offset"":455,""safe"":false},{""name"":""test2"",""parameters"":[],""returntype"":""String"",""offset"":461,""safe"":false},{""name"":""abstractTest"",""parameters"":[],""returntype"":""String"",""offset"":467,""safe"":false},{""name"":""mul"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":473,""safe"":false},{""name"":""sumToBeOverriden"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":481,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":416,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.0"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP3oAVcAA3l6nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAEMCXRlc3RGaW5hbEBXAAF4NBN4NBqL2ygMBS50ZXN0i9soQFcAAQwEdGVzdEBXAAF4NA4MBi50ZXN0MovbKEBXAAEMBWJhc2UyQFcAAXg0GQwRb3ZlcnJpZGVuQWJzdHJhY3SL2yhAVwABDAxhYnN0cmFjdFRlc3RAVwADeXqgSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcAA3l6n0rKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAN6eXg1mgAAAHl6oErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn55KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ96SsoRMg8B/wCRSgB/MgYBAAGfeUrKETIPAf8AkUoAfzIGAQABn3g1YP///55KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwADenl4NWb+//9AVgILCsf+//8KFwAAABPAYAsKuf7//woJAAAAE8BhQFgRwCM+/v//wiNg/v//wiNp/v//wiOh/v//WRHAI8j+///CIxL///9Alj2H9Q==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQZDBFvdmVycmlkZW5BYnN0cmFjdIvbKEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 19 [512 datoshi]
    /// PUSHDATA1 6F766572726964656E4162737472616374 'overridenAbstract' [8 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("abstractTest")]
    public abstract string? AbstractTest();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeXqgSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQA==
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// MUL [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("mul")]
    public abstract BigInteger? Mul(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeXqeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQA==
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADenl4NZoAAAB5eqBKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+eSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfekrKETIPAf8AkUoAfzIGAQABn3lKyhEyDwH/AJFKAH8yBgEAAZ94NWD///+eSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQA==
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 9A000000 [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// MUL [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// LDARG2 [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPLE 0F [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 7F [1 datoshi]
    /// JMPLE 06 [2 datoshi]
    /// PUSHINT16 0001 [1 datoshi]
    /// SUB [8 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPLE 0F [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 7F [1 datoshi]
    /// JMPLE 06 [2 datoshi]
    /// PUSHINT16 0001 [1 datoshi]
    /// SUB [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 60FFFFFF [512 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("sumToBeOverriden")]
    public abstract BigInteger? SumToBeOverriden(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAl0ZXN0RmluYWxA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 7465737446696E616C 'testFinal' [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test")]
    public abstract string? Test();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQTeDQai9soDAUudGVzdIvbKEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 13 [512 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 1A [512 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// PUSHDATA1 2E74657374 '.test' [8 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test2")]
    public abstract string? Test2();

    #endregion
}
