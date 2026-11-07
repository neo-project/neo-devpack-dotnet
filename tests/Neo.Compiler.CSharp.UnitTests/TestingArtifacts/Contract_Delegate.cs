using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Delegate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Delegate"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sumFunc"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testDelegate"",""parameters"":[],""returntype"":""Void"",""offset"":94,""safe"":false},{""name"":""testConditionalInvoke"",""parameters"":[{""name"":""isNull"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":130,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABBQAA/XUBVwECCgsAAABweXhoNkBXAAJ4eZ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwACeHmeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcDAArV////cGhyFhVqNnEMBVN1bTogaTcAAIvbKEHP50eWQFcDARARwHB4JgULIgcKbQAAAHFpStgkEmg1rAAAAGg1pgAAAFASUjZK2CYERRByagBkoErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2gQzp5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwACeBqgSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfeZ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwABeEoQzpxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9OEFDQQHseQPo=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECCgsAAABweXhoNkA=
    /// INITSLOT 0102 [64 datoshi]
    /// PUSHA 0B000000 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("sumFunc")]
    public abstract BigInteger? SumFunc(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBEBHAcHgmBQsiBwptAAAAcWlK2CQSaDWsAAAAaDWmAAAAUBJSNkrYJgRFEHJqAGSgSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfaBDOnkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// INITSLOT 0301 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// JMP 07 [2 datoshi]
    /// PUSHA 6D000000 [4 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 12 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL_L AC000000 [512 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL_L A6000000 [512 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// CALLA [512 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSHINT8 64 [1 datoshi]
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
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
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
    [DisplayName("testConditionalInvoke")]
    public abstract BigInteger? TestConditionalInvoke(bool? isNull);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMACtX///9waHIWFWo2cQwFU3VtOiBpNwAAi9soQc/nR5ZA
    /// INITSLOT 0300 [64 datoshi]
    /// PUSHA D5FFFFFF [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSH6 [1 datoshi]
    /// PUSH5 [1 datoshi]
    /// LDLOC2 [2 datoshi]
    /// CALLA [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSHDATA1 53756D3A20 [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDelegate")]
    public abstract void TestDelegate();

    #endregion
}
