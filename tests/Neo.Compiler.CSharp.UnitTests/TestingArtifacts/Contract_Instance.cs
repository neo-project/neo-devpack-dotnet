using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Instance(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Instance"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":178,""safe"":false},{""name"":""sum2"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":189,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMlXAAJ5eBDOnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwABeBAQ0HhKEM5OnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ8QUNBFQFcAAnl4NIR5eDSAnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AEBHASjSBI0j///8QEcBKNXb///8isUA/HXmK"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXgQzp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : PICKITEM [64 datoshi]
    /// 07 : ADD [8 datoshi]
    /// 08 : DUP [2 datoshi]
    /// 09 : PUSHINT32 00000080 [1 datoshi]
    /// 0E : JMPGE 04 [2 datoshi]
    /// 10 : JMP 0A [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 18 : JMPLE 1E [2 datoshi]
    /// 1A : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 23 : AND [8 datoshi]
    /// 24 : DUP [2 datoshi]
    /// 25 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2A : JMPLE 0C [2 datoshi]
    /// 2C : PUSHINT64 0000000001000000 [1 datoshi]
    /// 35 : SUB [8 datoshi]
    /// 36 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg0hHl4NICeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : CALL 84 [512 datoshi]
    /// 07 : LDARG1 [2 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : CALL 80 [512 datoshi]
    /// 0B : ADD [8 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : PUSHINT32 00000080 [1 datoshi]
    /// 12 : JMPGE 04 [2 datoshi]
    /// 14 : JMP 0A [2 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 1C : JMPLE 1E [2 datoshi]
    /// 1E : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 27 : AND [8 datoshi]
    /// 28 : DUP [2 datoshi]
    /// 29 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2E : JMPLE 0C [2 datoshi]
    /// 30 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 39 : SUB [8 datoshi]
    /// 3A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("sum2")]
    public abstract BigInteger? Sum2(BigInteger? a);

    #endregion
}
