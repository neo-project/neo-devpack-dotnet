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
    [DisplayName("callFuncPointer")]
    public abstract BigInteger? CallFuncPointer();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("callFuncPointerWithArg")]
    public abstract BigInteger? CallFuncPointerWithArg();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createFuncPointer")]
    public abstract object? CreateFuncPointer();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createFuncPointerWithArg")]
    public abstract object? CreateFuncPointerWithArg();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("myMethod")]
    public abstract BigInteger? MyMethod();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("myMethodWithArg")]
    public abstract BigInteger? MyMethodWithArg(byte[]? num);

    #endregion

}
