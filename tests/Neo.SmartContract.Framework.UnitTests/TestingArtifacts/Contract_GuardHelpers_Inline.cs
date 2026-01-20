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

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_GuardHelpers_Inline"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testRequire"",""parameters"":[{""name"":""condition"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":271,""safe"":false},{""name"":""testRequireNotNull"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Void"",""offset"":289,""safe"":false},{""name"":""testRequireNonNegative"",""parameters"":[{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":308,""safe"":false},{""name"":""testRequirePositive"",""parameters"":[{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":318,""safe"":false},{""name"":""testRequireValidAddress"",""parameters"":[{""name"":""address"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":328,""safe"":false},{""name"":""testRequireWitness"",""parameters"":[{""name"":""account"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":338,""safe"":false},{""name"":""testRequireWitnessCustom"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""errorCode"",""type"":""String""}],""returntype"":""Void"",""offset"":360,""safe"":false},{""name"":""testRequireInRange"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Void"",""offset"":371,""safe"":false},{""name"":""testRequireEquals"",""parameters"":[{""name"":""actual"",""type"":""Integer""},{""name"":""expected"",""type"":""Integer""}],""returntype"":""Void"",""offset"":383,""safe"":false},{""name"":""testRequireEqualsCustom"",""parameters"":[{""name"":""actual"",""type"":""Integer""},{""name"":""expected"",""type"":""Integer""},{""name"":""errorCode"",""type"":""String""}],""returntype"":""Void"",""offset"":405,""safe"":false},{""name"":""testRequireCaller"",""parameters"":[{""name"":""expectedCaller"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":417,""safe"":false},{""name"":""testRequireNotEmpty"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Void"",""offset"":427,""safe"":false},{""name"":""testEnsure"",""parameters"":[{""name"":""condition"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":447,""safe"":false},{""name"":""testRevert"",""parameters"":[],""returntype"":""Void"",""offset"":467,""safe"":false},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":482,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0LAlcAAngkBHk6QFcAAngkDgwFUE9TVDp5i9soOkBXAAF4OlcBAnhwaNgmDgwFTlVMTDp5i9soOkBXAAF4ELUmDQwITkVHQVRJVkU6QFcAAXgQtiYRDAxOT1RfUE9TSVRJVkU6QFcBAXhwaNgmBQgiGngMFAAAAAAAAAAAAAAAAAAAAAAAAAAAlyYRDAxJTlZBTElEX0FERFI6QFcAAnhB+CfsjCQEeTpAVwADeHm1JgUIIgV4ercmEQwMT1VUX09GX1JBTkdFOkBXAAN4eZgmBHo6QFcAAUE5U248eJgmEwwOSU5WQUxJRF9DQUxMRVI6QFcAAnhK2CQHyrGqIgRFCCYPDAZFTVBUWTp5i9soOkBXAAEMBkZBSUxFRHg15f7//0BXAAEMB215UGFyYW14NfP+//9AVwABeDX//v//QFcAAXg1Cf///0BXAAF4NRf///9AVwABDApOT19XSVRORVNTeDU3////QFcAAnl4NSz///9AVwADenl4NS7///9AVwACDAlOT1RfRVFVQUx5eDU4////QFcAA3p5eDUs////QFcAAXg1Lf///0BXAAEMCG15U3RyaW5neDU3////QFcAAQwIUE9TVENPTkR4NTz+//9ADAhSRVZFUlRFRDU//v//VwADeDV9/v//eTV3/v//ejVZ/v//DApOT19XSVRORVNTeDWV/v//CECklILD").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAhQT1NUQ09ORHg1PP7//0A=
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 504F5354434F4E44 'POSTCOND' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 3CFEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnsure")]
    public abstract void TestEnsure(bool? condition);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAZGQUlMRUR4NeX+//9A
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 4641494C4544 'FAILED' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L E5FEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequire")]
    public abstract void TestRequire(bool? condition);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDUt////QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 2DFFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireCaller")]
    public abstract void TestRequireCaller(UInt160? expectedCaller);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACDAlOT1RfRVFVQUx5eDU4////QA==
    /// INITSLOT 0002 [64 datoshi]
    /// PUSHDATA1 4E4F545F455155414C 'NOT_EQUAL' [8 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 38FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireEquals")]
    public abstract void TestRequireEquals(BigInteger? actual, BigInteger? expected);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADenl4NSz///9A
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 2CFFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireEqualsCustom")]
    public abstract void TestRequireEqualsCustom(BigInteger? actual, BigInteger? expected, string? errorCode);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADenl4NS7///9A
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 2EFFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireInRange")]
    public abstract void TestRequireInRange(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDX//v//QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L FFFEFFFF [512 datoshi]
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
    /// Script: VwABDAdteVBhcmFteDXz/v//QA==
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 6D79506172616D 'myParam' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L F3FEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireNotNull")]
    public abstract void TestRequireNotNull(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDUJ////QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 09FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequirePositive")]
    public abstract void TestRequirePositive(BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDUX////QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 17FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireValidAddress")]
    public abstract void TestRequireValidAddress(UInt160? address);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDApOT19XSVRORVNTeDU3////QA==
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 4E4F5F5749544E455353 'NO_WITNESS' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 37FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireWitness")]
    public abstract void TestRequireWitness(UInt160? account);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg1LP///0A=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 2CFFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireWitnessCustom")]
    public abstract void TestRequireWitnessCustom(UInt160? account, string? errorCode);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAhSRVZFUlRFRDU//v//
    /// PUSHDATA1 5245564552544544 'REVERTED' [8 datoshi]
    /// CALL_L 3FFEFFFF [512 datoshi]
    /// </remarks>
    [DisplayName("testRevert")]
    public abstract void TestRevert();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeDV9/v//eTV3/v//ejVZ/v//DApOT19XSVRORVNTeDWV/v//CEA=
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 7DFEFFFF [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// CALL_L 77FEFFFF [512 datoshi]
    /// LDARG2 [2 datoshi]
    /// CALL_L 59FEFFFF [512 datoshi]
    /// PUSHDATA1 4E4F5F5749544E455353 'NO_WITNESS' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 95FEFFFF [512 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount);

    #endregion
}
