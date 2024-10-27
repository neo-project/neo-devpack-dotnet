using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Pointers(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Pointers"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""createFuncPointer"",""parameters"":[],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""myMethod"",""parameters"":[],""returntype"":""Integer"",""offset"":6,""safe"":false},{""name"":""callFuncPointer"",""parameters"":[],""returntype"":""Integer"",""offset"":9,""safe"":false},{""name"":""createFuncPointerWithArg"",""parameters"":[],""returntype"":""Any"",""offset"":21,""safe"":false},{""name"":""myMethodWithArg"",""parameters"":[{""name"":""num"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":27,""safe"":false},{""name"":""callFuncPointerWithArg"",""parameters"":[],""returntype"":""Integer"",""offset"":34,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADUKBgAAAEAAe0BXAQAK+v///3BoNkAKBgAAAEBXAAF42yFAVwEACvb///9wDAMLFiHbMGg2QPVJbRI="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACvr///9waDZA
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSHA FAFFFFFF [4 datoshi]
    /// 08 : OpCode.STLOC0 [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.CALLA [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("callFuncPointer")]
    public abstract BigInteger? CallFuncPointer();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACvb///9wDAsWIdswaDZA
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSHA F6FFFFFF [4 datoshi]
    /// 08 : OpCode.STLOC0 [2 datoshi]
    /// 09 : OpCode.PUSHDATA1 0B1621 [8 datoshi]
    /// 0E : OpCode.CONVERT 'Buffer' [8192 datoshi]
    /// 10 : OpCode.LDLOC0 [2 datoshi]
    /// 11 : OpCode.CALLA [512 datoshi]
    /// 12 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("callFuncPointerWithArg")]
    public abstract BigInteger? CallFuncPointerWithArg();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CgYAAABA
    /// 00 : OpCode.PUSHA 06000000 [4 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("createFuncPointer")]
    public abstract object? CreateFuncPointer();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CgYAAABA
    /// 00 : OpCode.PUSHA 06000000 [4 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("createFuncPointerWithArg")]
    public abstract object? CreateFuncPointerWithArg();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: AHtA
    /// 00 : OpCode.PUSHINT8 7B [1 datoshi]
    /// 02 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("myMethod")]
    public abstract BigInteger? MyMethod();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNshQA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CONVERT 'Integer' [8192 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("myMethodWithArg")]
    public abstract BigInteger? MyMethodWithArg(byte[]? num);

    #endregion
}
