using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class C(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""C"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":205,""safe"":false},{""name"":""sumToBeOverriden"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":213,""safe"":false},{""name"":""test"",""parameters"":[],""returntype"":""String"",""offset"":221,""safe"":false},{""name"":""test2"",""parameters"":[],""returntype"":""String"",""offset"":229,""safe"":false},{""name"":""abstractTest"",""parameters"":[],""returntype"":""String"",""offset"":237,""safe"":false},{""name"":""mul"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":245,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":146,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy4xMC4yK2QzZDdjNDNmNWJjMjNmNDJmNDIxMWZmNzQ1MTkwYzgyNDguLi4AAAAAAP3+AFcAA3l6nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAN6eXg00kBXAAEMBHRlc3RAVwABeDQODAYudGVzdDKL2yhAVwABDAViYXNlMkBXAAEMDGFic3RyYWN0VGVzdEBXAAN5eqBKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVgMLCrj///8KKQAAABPAYAq2////CpT///8Khf///xPAYQsKmP///woJAAAAE8BiQFcAAQwEYmFzZUBYEcAjMP///1gRwCNQ////WRHAI1H///9ZEcAjU////1kRwCNo////WhHAI3L///9AisMcxQ==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAxhYnN0cmFjdFRlc3RA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 616273747261637454657374 'abstractTest' [8 datoshi]
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
    /// Script: VwADenl4NNJA
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL D2 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("sumToBeOverriden")]
    public abstract BigInteger? SumToBeOverriden(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAR0ZXN0QA==
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 74657374 'test' [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test")]
    public abstract string? Test();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQODAYudGVzdDKL2yhA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 0E [512 datoshi]
    /// PUSHDATA1 2E7465737432 '.test2' [8 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test2")]
    public abstract string? Test2();

    #endregion
}
