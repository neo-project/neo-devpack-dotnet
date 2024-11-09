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
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.PICKITEM [64 datoshi]
    /// 07 : OpCode.ADD [8 datoshi]
    /// 08 : OpCode.DUP [2 datoshi]
    /// 09 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 0E : OpCode.JMPGE 04 [2 datoshi]
    /// 10 : OpCode.JMP 0A [2 datoshi]
    /// 12 : OpCode.DUP [2 datoshi]
    /// 13 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 18 : OpCode.JMPLE 1E [2 datoshi]
    /// 1A : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 23 : OpCode.AND [8 datoshi]
    /// 24 : OpCode.DUP [2 datoshi]
    /// 25 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2A : OpCode.JMPLE 0C [2 datoshi]
    /// 2C : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 35 : OpCode.SUB [8 datoshi]
    /// 36 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg0hHl4NICeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.CALL 84 [512 datoshi]
    /// 07 : OpCode.LDARG1 [2 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.CALL 80 [512 datoshi]
    /// 0B : OpCode.ADD [8 datoshi]
    /// 0C : OpCode.DUP [2 datoshi]
    /// 0D : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 12 : OpCode.JMPGE 04 [2 datoshi]
    /// 14 : OpCode.JMP 0A [2 datoshi]
    /// 16 : OpCode.DUP [2 datoshi]
    /// 17 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 1C : OpCode.JMPLE 1E [2 datoshi]
    /// 1E : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 27 : OpCode.AND [8 datoshi]
    /// 28 : OpCode.DUP [2 datoshi]
    /// 29 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2E : OpCode.JMPLE 0C [2 datoshi]
    /// 30 : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 39 : OpCode.SUB [8 datoshi]
    /// 3A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("sum2")]
    public abstract BigInteger? Sum2(BigInteger? a);

    #endregion
}
