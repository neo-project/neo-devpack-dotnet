using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_RefLocals(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_RefLocals"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""incrementViaRefLocal"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""rebindRefLocal"",""parameters"":[{""name"":""first"",""type"":""Integer""},{""name"":""second"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":42,""safe"":false},{""name"":""rewriteArrayElement"",""parameters"":[{""name"":""index"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":84,""safe"":false},{""name"":""updateInstanceField"",""parameters"":[{""name"":""start"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":107,""safe"":false},{""name"":""updateStaticField"",""parameters"":[{""name"":""start"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":164,""safe"":false},{""name"":""updateNestedHolder"",""parameters"":[{""name"":""start"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":209,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":272,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy4xMC4yK2UyY2FjZTgzZTQ1YTNkZDJlY2U5YjRmNjNkMTU1YjMyNjkuLi4AAAAAAP0VAVcBAXgVnkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn4B4QFcBAnkSnkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn4F5QFcEARcVExPAcGhyeHMAY2prUdBoeM5AVwMBeBHAcGhyahDOFJ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KahBR0EVoEM5AVwEBeGBYSpxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9gRVhAVwMBeBHAEcBwaBDOcmoQzhOeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSmoQUdBFaBDOEM5AVgEQYEBgRvlz").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeBWeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfgHhA
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH5 [1 datoshi]
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
    /// STARG0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("incrementViaRefLocal")]
    public abstract BigInteger? IncrementViaRefLocal(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECeRKeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfgXlA
    /// INITSLOT 0102 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH2 [1 datoshi]
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
    /// STARG1 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rebindRefLocal")]
    public abstract BigInteger? RebindRefLocal(BigInteger? first, BigInteger? second);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQBFxUTE8BwaHJ4cwBjamtR0Gh4zkA=
    /// INITSLOT 0401 [64 datoshi]
    /// PUSH7 [1 datoshi]
    /// PUSH5 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// STLOC3 [2 datoshi]
    /// PUSHINT8 63 [1 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rewriteArrayElement")]
    public abstract BigInteger? RewriteArrayElement(BigInteger? index);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBeBHAcGhyahDOFJ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KahBR0EVoEM5A
    /// INITSLOT 0301 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH4 [1 datoshi]
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
    /// DUP [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("updateInstanceField")]
    public abstract BigInteger? UpdateInstanceField(BigInteger? start);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBeBHAEcBwaBDOcmoQzhOeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSmoQUdBFaBDOEM5A
    /// INITSLOT 0301 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH3 [1 datoshi]
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
    /// DUP [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("updateNestedHolder")]
    public abstract BigInteger? UpdateNestedHolder(BigInteger? start);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeGBYSpxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9gRVhA
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
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
    /// STSFLD0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("updateStaticField")]
    public abstract BigInteger? UpdateStaticField(BigInteger? start);

    #endregion
}
