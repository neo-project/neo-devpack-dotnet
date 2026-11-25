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

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_GuardHelpers_Inline"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testRequire"",""parameters"":[{""name"":""condition"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":291,""safe"":false},{""name"":""testRequireNotNull"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Void"",""offset"":309,""safe"":false},{""name"":""testRequireNonNegative"",""parameters"":[{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":328,""safe"":false},{""name"":""testRequirePositive"",""parameters"":[{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":338,""safe"":false},{""name"":""testRequireValidAddress"",""parameters"":[{""name"":""address"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":348,""safe"":false},{""name"":""testRequireWitness"",""parameters"":[{""name"":""account"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":358,""safe"":false},{""name"":""testRequireWitnessCustom"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""errorCode"",""type"":""String""}],""returntype"":""Void"",""offset"":380,""safe"":false},{""name"":""testRequireInRange"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Void"",""offset"":391,""safe"":false},{""name"":""testRequireEquals"",""parameters"":[{""name"":""actual"",""type"":""Integer""},{""name"":""expected"",""type"":""Integer""}],""returntype"":""Void"",""offset"":403,""safe"":false},{""name"":""testRequireEqualsCustom"",""parameters"":[{""name"":""actual"",""type"":""Integer""},{""name"":""expected"",""type"":""Integer""},{""name"":""errorCode"",""type"":""String""}],""returntype"":""Void"",""offset"":425,""safe"":false},{""name"":""testRequireCaller"",""parameters"":[{""name"":""expectedCaller"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":437,""safe"":false},{""name"":""testRequireNotEmpty"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Void"",""offset"":447,""safe"":false},{""name"":""testEnsure"",""parameters"":[{""name"":""condition"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":467,""safe"":false},{""name"":""testRevert"",""parameters"":[],""returntype"":""Void"",""offset"":487,""safe"":false},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":502,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.11.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0fAlcAAngkBHk6QFcAAngkFQwFUE9TVDp5StgmBUUMAIvbKDpAVwABeDpXAQJ4cGjYJhUMBU5VTEw6eUrYJgVFDACL2yg6QFcAAXgQtSYNDAhORUdBVElWRTpAVwABeBC2JhEMDE5PVF9QT1NJVElWRTpAVwEBeHBo2CYFCCIaeAwUAAAAAAAAAAAAAAAAAAAAAAAAAACXJhEMDElOVkFMSURfQUREUjpAVwACeEH4J+yMJAR5OkBXAAN4ebUmBQgiBXh6tyYRDAxPVVRfT0ZfUkFOR0U6QFcAA3h5mCYEejpAVwABQTlTbjx4mCYTDA5JTlZBTElEX0NBTExFUjpAVwACeErYJAbKqiIERQgmFgwGRU1QVFk6eUrYJgVFDACL2yg6QFcAAQwGRkFJTEVEeDXR/v//QFcAAQwHbXlQYXJhbXg15v7//0BXAAF4Nfn+//9AVwABeDUD////QFcAAXg1Ef///0BXAAEMCk5PX1dJVE5FU1N4NTH///9AVwACeXg1Jv///0BXAAN6eXg1KP///0BXAAIMCU5PVF9FUVVBTHl4NTL///9AVwADenl4NSb///9AVwABeDUn////QFcAAQwIbXlTdHJpbmd4NTH///9AVwABDAhQT1NUQ09ORHg1KP7//0AMCFJFVkVSVEVENTL+//9XAAN4NXf+//95NXH+//96NVP+//8MCk5PX1dJVE5FU1N4NY/+//8IQFYmfLY=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAhQT1NUQ09ORHg1KP7//0A=
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 504F5354434F4E44 'POSTCOND' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 28FEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnsure")]
    public abstract void TestEnsure(bool? condition);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAZGQUlMRUR4NdH+//9A
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 4641494C4544 'FAILED' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L D1FEFFFF [512 datoshi]
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
    /// Script: VwACDAlOT1RfRVFVQUx5eDUy////QA==
    /// INITSLOT 0002 [64 datoshi]
    /// PUSHDATA1 4E4F545F455155414C 'NOT_EQUAL' [8 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 32FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireEquals")]
    public abstract void TestRequireEquals(BigInteger? actual, BigInteger? expected);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADenl4NSb///9A
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 26FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireEqualsCustom")]
    public abstract void TestRequireEqualsCustom(BigInteger? actual, BigInteger? expected, string? errorCode);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADenl4NSj///9A
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 28FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireInRange")]
    public abstract void TestRequireInRange(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDX5/v//QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L F9FEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireNonNegative")]
    public abstract void TestRequireNonNegative(BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAhteVN0cmluZ3g1Mf///0A=
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 6D79537472696E67 'myString' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 31FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireNotEmpty")]
    public abstract void TestRequireNotEmpty(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAdteVBhcmFteDXm/v//QA==
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 6D79506172616D 'myParam' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L E6FEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireNotNull")]
    public abstract void TestRequireNotNull(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDUD////QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 03FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequirePositive")]
    public abstract void TestRequirePositive(BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDUR////QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 11FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireValidAddress")]
    public abstract void TestRequireValidAddress(UInt160? address);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDApOT19XSVRORVNTeDUx////QA==
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 4E4F5F5749544E455353 'NO_WITNESS' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 31FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireWitness")]
    public abstract void TestRequireWitness(UInt160? account);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg1Jv///0A=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 26FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRequireWitnessCustom")]
    public abstract void TestRequireWitnessCustom(UInt160? account, string? errorCode);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAhSRVZFUlRFRDUy/v//
    /// PUSHDATA1 5245564552544544 'REVERTED' [8 datoshi]
    /// CALL_L 32FEFFFF [512 datoshi]
    /// </remarks>
    [DisplayName("testRevert")]
    public abstract void TestRevert();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeDV3/v//eTVx/v//ejVT/v//DApOT19XSVRORVNTeDWP/v//CEA=
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 77FEFFFF [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// CALL_L 71FEFFFF [512 datoshi]
    /// LDARG2 [2 datoshi]
    /// CALL_L 53FEFFFF [512 datoshi]
    /// PUSHDATA1 4E4F5F5749544E455353 'NO_WITNESS' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 8FFEFFFF [512 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount);

    #endregion
}
