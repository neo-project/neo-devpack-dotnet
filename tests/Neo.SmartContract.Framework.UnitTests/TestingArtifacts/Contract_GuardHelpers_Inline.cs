using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_GuardHelpers_Inline(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_GuardHelpers_Inline"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testRequire"",""parameters"":[{""name"":""condition"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":270,""safe"":false},{""name"":""testRequireNotNull"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Void"",""offset"":288,""safe"":false},{""name"":""testRequireNonNegative"",""parameters"":[{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":307,""safe"":false},{""name"":""testRequirePositive"",""parameters"":[{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":317,""safe"":false},{""name"":""testRequireValidAddress"",""parameters"":[{""name"":""address"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":327,""safe"":false},{""name"":""testRequireWitness"",""parameters"":[{""name"":""account"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":337,""safe"":false},{""name"":""testRequireWitnessCustom"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""errorCode"",""type"":""String""}],""returntype"":""Void"",""offset"":359,""safe"":false},{""name"":""testRequireInRange"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Void"",""offset"":370,""safe"":false},{""name"":""testRequireEquals"",""parameters"":[{""name"":""actual"",""type"":""Integer""},{""name"":""expected"",""type"":""Integer""}],""returntype"":""Void"",""offset"":382,""safe"":false},{""name"":""testRequireEqualsCustom"",""parameters"":[{""name"":""actual"",""type"":""Integer""},{""name"":""expected"",""type"":""Integer""},{""name"":""errorCode"",""type"":""String""}],""returntype"":""Void"",""offset"":404,""safe"":false},{""name"":""testRequireCaller"",""parameters"":[{""name"":""expectedCaller"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":416,""safe"":false},{""name"":""testRequireNotEmpty"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Void"",""offset"":426,""safe"":false},{""name"":""testEnsure"",""parameters"":[{""name"":""condition"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":446,""safe"":false},{""name"":""testRevert"",""parameters"":[],""returntype"":""Void"",""offset"":466,""safe"":false},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":481,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.0"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0KAlcAAngkBHk6QFcAAngkDgwFUE9TVDp5i9soOkBXAAF4OlcBAnhwaNgmDgwFTlVMTDp5i9soOkBXAAF4ELUmDQwITkVHQVRJVkU6QFcAAXgQtiYRDAxOT1RfUE9TSVRJVkU6QFcBAXhwaNgmBQgiGngMFAAAAAAAAAAAAAAAAAAAAAAAAAAAlyYRDAxJTlZBTElEX0FERFI6QFcAAnhB+CfsjCQEeTpAVwADeHm1JgUIIgV4ercmEQwMT1VUX09GX1JBTkdFOkBXAAN4eZgmBHo6QFcAAUE5U248eJgmEwwOSU5WQUxJRF9DQUxMRVI6QFcAAnhK2CQGyqoiBEUIJg8MBkVNUFRZOnmL2yg6QFcAAQwGRkFJTEVEeDXm/v//QFcAAQwHbXlQYXJhbXg19P7//0BXAAF4NQD///9AVwABeDUK////QFcAAXg1GP///0BXAAEMCk5PX1dJVE5FU1N4NTj///9AVwACeXg1Lf///0BXAAN6eXg1L////0BXAAIMCU5PVF9FUVVBTHl4NTn///9AVwADenl4NS3///9AVwABeDUu////QFcAAQwIbXlTdHJpbmd4NTj///9AVwABDAhQT1NUQ09ORHg1Pf7//0AMCFJFVkVSVEVENUD+//9XAAN4NX7+//95NXj+//96NVr+//8MCk5PX1dJVE5FU1N4NZb+//8IQNRaCiw=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAhQT1NUQ09ORHg1Pf7//0A=
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 504F5354434F4E44 'POSTCOND' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 3DFEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnsure")]
    public abstract void TestEnsure(bool? condition);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAZGQUlMRUR4Neb+//9A
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 4641494C4544 'FAILED' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L E6FEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequire")]
    public abstract void TestRequire(bool? condition);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDUu////QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 2EFFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireCaller")]
    public abstract void TestRequireCaller(UInt160? expectedCaller);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACDAlOT1RfRVFVQUx5eDU5////QA==
    /// INITSLOT 0002 [64 datoshi]
    /// PUSHDATA1 4E4F545F455155414C 'NOT_EQUAL' [8 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 39FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireEquals")]
    public abstract void TestRequireEquals(BigInteger? actual, BigInteger? expected);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADenl4NS3///9A
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 2DFFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireEqualsCustom")]
    public abstract void TestRequireEqualsCustom(BigInteger? actual, BigInteger? expected, string? errorCode);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADenl4NS////9A
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 2FFFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireInRange")]
    public abstract void TestRequireInRange(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDUA////QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 00FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireNonNegative")]
    public abstract void TestRequireNonNegative(BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAhteVN0cmluZ3g1OP///0A=
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 6D79537472696E67 'myString' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 38FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireNotEmpty")]
    public abstract void TestRequireNotEmpty(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAdteVBhcmFteDX0/v//QA==
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 6D79506172616D 'myParam' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L F4FEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireNotNull")]
    public abstract void TestRequireNotNull(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDUK////QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 0AFFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequirePositive")]
    public abstract void TestRequirePositive(BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDUY////QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 18FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireValidAddress")]
    public abstract void TestRequireValidAddress(UInt160? address);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDApOT19XSVRORVNTeDU4////QA==
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 4E4F5F5749544E455353 'NO_WITNESS' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 38FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireWitness")]
    public abstract void TestRequireWitness(UInt160? account);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg1Lf///0A=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 2DFFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireWitnessCustom")]
    public abstract void TestRequireWitnessCustom(UInt160? account, string? errorCode);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAhSRVZFUlRFRDVA/v//
    /// PUSHDATA1 5245564552544544 'REVERTED' [8 datoshi]
    /// CALL_L 40FEFFFF [512 datoshi]
    /// </remarks>
    [DisplayName("testRevert")]
    public abstract void TestRevert();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeDV+/v//eTV4/v//ejVa/v//DApOT19XSVRORVNTeDWW/v//CEA=
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 7EFEFFFF [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// CALL_L 78FEFFFF [512 datoshi]
    /// LDARG2 [2 datoshi]
    /// CALL_L 5AFEFFFF [512 datoshi]
    /// PUSHDATA1 4E4F5F5749544E455353 'NO_WITNESS' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 96FEFFFF [512 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount);

    #endregion
}
