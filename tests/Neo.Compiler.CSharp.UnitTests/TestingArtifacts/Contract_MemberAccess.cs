using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_MemberAccess(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_MemberAccess"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testMain"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""testComplexAssignment"",""parameters"":[],""returntype"":""Void"",""offset"":59,""safe"":false},{""name"":""testStaticComplexAssignment"",""parameters"":[],""returntype"":""Void"",""offset"":268,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":332,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABBQAA/VMBVwEAFQwFaGVsbG8QE8BwaBDONwAAQc/nR5YMA21zZ0HP50eWaBHOQc/nR5ZoNAhBz+dHlkBXAAEMAEBXAgBDcRUMBWhlbGxvEBPAcGg0Zw+zOWhKEM4PSg8qHEsCAAAAgCoUQ2kyBUUi+wwIT3ZlcmZsb3c6oU5QEFHQEbM5aDRksao5aDRuDAZoZWxsbzKXOWhKEc5K2CYFRQwADAIzM4vbKE4RUNAMCGhlbGxvMjMzlzlAVwABeEoQzp1KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9OUBBR0EBXAAF4ShLOeBLOk04SUNBAVwABeEoRzkrYJgVFDAAMATKL2yhOEVDQQBBgWLGqOVicSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfYFgRszlZqjlZCKxhWTlZCJOxYVmqOUBWAhNgCWFAxTJBcw==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAQ3EVDAVoZWxsbxATwHBoNGcPszloShDOD0oPKhxLAgAAAIAqFENpMgVFIvsMCE92ZXJmbG93OqFOUBBR0BGzOWg0ZLGqOWg0bgwGaGVsbG8ylzloShHOStgmBUUMAAwCMzOL2yhOEVDQDAhoZWxsbzIzM5c5QA==
    /// INITSLOT 0200 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH5 [1 datoshi]
    /// PUSHDATA1 68656C6C6F 'hello' [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 67 [512 datoshi]
    /// PUSHM1 [1 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// ASSERT [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSHM1 [1 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 1C [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPNE 14 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 4F766572666C6F77 'Overflow' [8 datoshi]
    /// THROW [512 datoshi]
    /// DIV [8 datoshi]
    /// TUCK [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// PUSH1 [1 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// ASSERT [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 64 [512 datoshi]
    /// NZ [4 datoshi]
    /// NOT [4 datoshi]
    /// ASSERT [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 6E [512 datoshi]
    /// PUSHDATA1 68656C6C6F32 'hello2' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// PUSHDATA1 3333 '33' [8 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// TUCK [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// PUSHDATA1 68656C6C6F323333 'hello233' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testComplexAssignment")]
    public abstract void TestComplexAssignment();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAFQwFaGVsbG8QE8BwaBDONwAAQc/nR5YMA21zZ0HP50eWaBHOQc/nR5ZoNAhBz+dHlkA=
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH5 [1 datoshi]
    /// PUSHDATA1 68656C6C6F 'hello' [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// PUSHDATA1 6D7367 'msg' [8 datoshi]
    /// SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 08 [512 datoshi]
    /// SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMain")]
    public abstract void TestMain();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGBYsao5WJxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9gWBGzOVmqOVkIrGFZOVkIk7FhWao5QA==
    /// PUSH0 [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// NZ [4 datoshi]
    /// NOT [4 datoshi]
    /// ASSERT [1 datoshi]
    /// LDSFLD0 [2 datoshi]
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
    /// LDSFLD0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// ASSERT [1 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// NOT [4 datoshi]
    /// ASSERT [1 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// BOOLOR [8 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// ASSERT [1 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// XOR [8 datoshi]
    /// NZ [4 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// NOT [4 datoshi]
    /// ASSERT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticComplexAssignment")]
    public abstract void TestStaticComplexAssignment();

    #endregion
}
