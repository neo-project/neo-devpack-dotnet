using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_GuardHelpers_Inline(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_GuardHelpers_Inline"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testRequire"",""parameters"":[{""name"":""condition"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":313,""safe"":false},{""name"":""testRequireNotNull"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Void"",""offset"":331,""safe"":false},{""name"":""testRequireNonNegative"",""parameters"":[{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":350,""safe"":false},{""name"":""testRequirePositive"",""parameters"":[{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":360,""safe"":false},{""name"":""testRequireValidAddress"",""parameters"":[{""name"":""address"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":370,""safe"":false},{""name"":""testRequireWitness"",""parameters"":[{""name"":""account"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":380,""safe"":false},{""name"":""testRequireWitnessCustom"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""errorCode"",""type"":""String""}],""returntype"":""Void"",""offset"":402,""safe"":false},{""name"":""testRequireInRange"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Void"",""offset"":413,""safe"":false},{""name"":""testRequireEquals"",""parameters"":[{""name"":""actual"",""type"":""Integer""},{""name"":""expected"",""type"":""Integer""}],""returntype"":""Void"",""offset"":425,""safe"":false},{""name"":""testRequireEqualsCustom"",""parameters"":[{""name"":""actual"",""type"":""Integer""},{""name"":""expected"",""type"":""Integer""},{""name"":""errorCode"",""type"":""String""}],""returntype"":""Void"",""offset"":447,""safe"":false},{""name"":""testRequireCaller"",""parameters"":[{""name"":""expectedCaller"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":459,""safe"":false},{""name"":""testRequireNotEmpty"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Void"",""offset"":469,""safe"":false},{""name"":""testEnsure"",""parameters"":[{""name"":""condition"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":489,""safe"":false},{""name"":""testRevert"",""parameters"":[],""returntype"":""Void"",""offset"":509,""safe"":false},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":525,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""Basic""}}}");

    /// <summary>
    /// Optimization: "Basic"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy44LjErNDdhZjIxODVhNDJlNGYyZThkM2I5NzMwZWM1NWViZDY3YWQuLi4AAAAAAP04AlcAAniqJgR5OkBXAAJ4qiYODAVQT1NUOnmL2yg6QFcAAXg6QFcBAnhwaAuXJg4MBU5VTEw6eYvbKDpAVwABeBC1Jg0MCE5FR0FUSVZFOkBXAAF4ELYmEQwMTk9UX1BPU0lUSVZFOkBXAQF4cGgLlyYFCCIaeAwUAAAAAAAAAAAAAAAAAAAAAAAAAACXJhEMDElOVkFMSURfQUREUjpADBQAAAAAAAAAAAAAAAAAAAAAAAAAAEBXAAJ4Qfgn7IyqJgR5OkBB+CfsjEBXAAN4ebUmBQgiBXh6tyYRDAxPVVRfT0ZfUkFOR0U6QFcAA3h5l6omBHo6QFcAAUE5U248eJgmEwwOSU5WQUxJRF9DQUxMRVI6QEE5U248QFcAAnhK2CQHyhCzIgRFCCYPDAZFTVBUWTp5i9soOkBXAAEMBkZBSUxFRHg1u/7//0BXAAEMB215UGFyYW14Ncz+//9AVwABeDXZ/v//QFcAAXg14/7//0BXAAF4NfH+//9AVwABDApOT19XSVRORVNTeDUp////QFcAAnl4NR7///9AVwADenl4NSf///9AVwACDAlOT1RfRVFVQUx5eDUx////QFcAA3p5eDUl////QFcAAXg1J////0BXAAEMCG15U3RyaW5neDU3////QFcAAQwIUE9TVENPTkR4NRP+//9ADAhSRVZFUlRFRDUX/v//QFcAA3g1Vv7//3k1UP7//3o1Mv7//wwKTk9fV0lUTkVTU3g1hv7//wgiAkC6Bd4q").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAhQT1NUQ09ORHg1E/7//0A=
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 504F5354434F4E44 'POSTCOND' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 13FEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnsure")]
    public abstract void TestEnsure(bool? condition);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAZGQUlMRUR4Nbv+//9A
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 4641494C4544 'FAILED' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L BBFEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequire")]
    public abstract void TestRequire(bool? condition);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDUn////QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 27FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireCaller")]
    public abstract void TestRequireCaller(UInt160? expectedCaller);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACDAlOT1RfRVFVQUx5eDUx////QA==
    /// INITSLOT 0002 [64 datoshi]
    /// PUSHDATA1 4E4F545F455155414C 'NOT_EQUAL' [8 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 31FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireEquals")]
    public abstract void TestRequireEquals(BigInteger? actual, BigInteger? expected);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADenl4NSX///9A
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 25FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireEqualsCustom")]
    public abstract void TestRequireEqualsCustom(BigInteger? actual, BigInteger? expected, string? errorCode);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADenl4NSf///9A
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 27FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireInRange")]
    public abstract void TestRequireInRange(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDXZ/v//QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L D9FEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireNonNegative")]
    public abstract void TestRequireNonNegative(BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAhteVN0cmluZ3g1N////0A=
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 6D79537472696E67 'myString' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 37FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireNotEmpty")]
    public abstract void TestRequireNotEmpty(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAdteVBhcmFteDXM/v//QA==
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 6D79506172616D 'myParam' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L CCFEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireNotNull")]
    public abstract void TestRequireNotNull(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDXj/v//QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L E3FEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequirePositive")]
    public abstract void TestRequirePositive(BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDXx/v//QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L F1FEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireValidAddress")]
    public abstract void TestRequireValidAddress(UInt160? address);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDApOT19XSVRORVNTeDUp////QA==
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 4E4F5F5749544E455353 'NO_WITNESS' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 29FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireWitness")]
    public abstract void TestRequireWitness(UInt160? account);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg1Hv///0A=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 1EFFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireWitnessCustom")]
    public abstract void TestRequireWitnessCustom(UInt160? account, string? errorCode);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAhSRVZFUlRFRDUX/v//QA==
    /// PUSHDATA1 5245564552544544 'REVERTED' [8 datoshi]
    /// CALL_L 17FEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRevert")]
    public abstract void TestRevert();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeDVW/v//eTVQ/v//ejUy/v//DApOT19XSVRORVNTeDWG/v//CCICQA==
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 56FEFFFF [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// CALL_L 50FEFFFF [512 datoshi]
    /// LDARG2 [2 datoshi]
    /// CALL_L 32FEFFFF [512 datoshi]
    /// PUSHDATA1 4E4F5F5749544E455353 'NO_WITNESS' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 86FEFFFF [512 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 02 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount);

    #endregion
}
