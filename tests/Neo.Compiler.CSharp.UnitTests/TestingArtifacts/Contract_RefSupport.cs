using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_RefSupport(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_RefSupport"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""incrementRefLocal"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""incrementRefStaticField"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":56,""safe"":false},{""name"":""incrementRefInstanceField"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":72,""safe"":false},{""name"":""produceOutValue"",""parameters"":[],""returntype"":""Integer"",""offset"":99,""safe"":false},{""name"":""produceOutStaticField"",""parameters"":[],""returntype"":""Integer"",""offset"":115,""safe"":false},{""name"":""produceOutInstanceField"",""parameters"":[],""returntype"":""Integer"",""offset"":128,""safe"":false},{""name"":""swapDigits"",""parameters"":[{""name"":""first"",""type"":""Integer""},{""name"":""second"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":155,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":272,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy4xMC4yK2UyY2FjZTgzZTQ1YTNkZDJlY2U5YjRmNjNkMTU1YjMyNjkuLi4AAAAAAP0VAVcAAXhgWEphNAZZYFhAVwABWUqcSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfYUVAVwABeGJaSmFKYjTMWWJaQFcBAXgRwHBoY1sQzkphSmQ0tllbEFHQaBDOQBBKZTQGXmVdQFcAAQB7ZkAQYhBKZkpiNPJeYlpAVwEAEBHAcGhnBxBKZkpkNN1eXwcQUdBoEM5AVwACeGcIeWcJXwlKZwtfCEpnCjRUXwpnCF8LZwlfCBqgSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfXwmeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcBAl8KcF8LZwpoZwtAVgwQYkBUuMn5").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeBHAcGhjWxDOSmFKZDS2WVsQUdBoEM5A
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STSFLD3 [2 datoshi]
    /// LDSFLD3 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD1 [2 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD4 [2 datoshi]
    /// CALL B6 [512 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// LDSFLD3 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("incrementRefInstanceField")]
    public abstract BigInteger? IncrementRefInstanceField(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeGBYSmE0BllgWEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD1 [2 datoshi]
    /// CALL 06 [512 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("incrementRefLocal")]
    public abstract BigInteger? IncrementRefLocal(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeGJaSmFKYjTMWWJaQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STSFLD2 [2 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD1 [2 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD2 [2 datoshi]
    /// CALL CC [512 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// STSFLD2 [2 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("incrementRefStaticField")]
    public abstract BigInteger? IncrementRefStaticField(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEBHAcGhnBxBKZkpkNN1eXwcQUdBoEM5A
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STSFLD 07 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD6 [2 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD4 [2 datoshi]
    /// CALL DD [512 datoshi]
    /// LDSFLD6 [2 datoshi]
    /// LDSFLD 07 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("produceOutInstanceField")]
    public abstract BigInteger? ProduceOutInstanceField();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGIQSmZKYjTyXmJaQA==
    /// PUSH0 [1 datoshi]
    /// STSFLD2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD6 [2 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD2 [2 datoshi]
    /// CALL F2 [512 datoshi]
    /// LDSFLD6 [2 datoshi]
    /// STSFLD2 [2 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("produceOutStaticField")]
    public abstract BigInteger? ProduceOutStaticField();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EEplNAZeZV1A
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD5 [2 datoshi]
    /// CALL 06 [512 datoshi]
    /// LDSFLD6 [2 datoshi]
    /// STSFLD5 [2 datoshi]
    /// LDSFLD5 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("produceOutValue")]
    public abstract BigInteger? ProduceOutValue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeGcIeWcJXwlKZwtfCEpnCjRUXwpnCF8LZwlfCBqgSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfXwmeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// STSFLD 09 [2 datoshi]
    /// LDSFLD 09 [2 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD 0B [2 datoshi]
    /// LDSFLD 08 [2 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD 0A [2 datoshi]
    /// CALL 54 [512 datoshi]
    /// LDSFLD 0A [2 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// STSFLD 09 [2 datoshi]
    /// LDSFLD 08 [2 datoshi]
    /// PUSH10 [1 datoshi]
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
    /// LDSFLD 09 [2 datoshi]
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
    [DisplayName("swapDigits")]
    public abstract BigInteger? SwapDigits(BigInteger? first, BigInteger? second);

    #endregion
}
