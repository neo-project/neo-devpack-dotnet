using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_NULL(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NULL"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""isNull"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""equalNullA"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":9,""safe"":false},{""name"":""equalNullB"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":16,""safe"":false},{""name"":""equalNotNullA"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":23,""safe"":false},{""name"":""equalNotNullB"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":30,""safe"":false},{""name"":""nullCoalescing"",""parameters"":[{""name"":""code"",""type"":""String""}],""returntype"":""String"",""offset"":37,""safe"":false},{""name"":""nullCollation"",""parameters"":[{""name"":""code"",""type"":""String""}],""returntype"":""String"",""offset"":51,""safe"":false},{""name"":""nullPropertyGT"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Boolean"",""offset"":70,""safe"":false},{""name"":""nullPropertyLT"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Boolean"",""offset"":82,""safe"":false},{""name"":""nullPropertyGE"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Boolean"",""offset"":94,""safe"":false},{""name"":""nullPropertyLE"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Boolean"",""offset"":106,""safe"":false},{""name"":""nullProperty"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Boolean"",""offset"":118,""safe"":false},{""name"":""ifNull"",""parameters"":[{""name"":""obj"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":130,""safe"":false},{""name"":""nullCollationAndCollation"",""parameters"":[{""name"":""code"",""type"":""String""}],""returntype"":""Any"",""offset"":140,""safe"":false},{""name"":""nullCollationAndCollation2"",""parameters"":[{""name"":""code"",""type"":""String""}],""returntype"":""Any"",""offset"":169,""safe"":false},{""name"":""nullType"",""parameters"":[],""returntype"":""Void"",""offset"":210,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOVXAQF4cGgLl0BXAAELeJdAVwABeAuXQFcAAQt4mEBXAAF4C5hAVwEBeErYJAUREoxwaEBXAQF4StgmCkUMBWxpbnV4cGhAVwABeErYJAPKELdAVwABeErYJAPKELVAVwABeErYJAPKELhAVwABeErYJAPKELZAVwABeErYJAPKEJhAVwABeCYECEAJQFcBAUGb9mfOcHhoQZJd6DFK2CYKRQwBe9sw2yhAVwEBQZv2Z85wDAMxMTF4aEHmPxiEeGhBkl3oMUrYJgpFDAF72zDbKEBXAQALcGhK2CQFNAVARUBXAAFARiQ5mA=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("equalNotNullA")]
    public abstract bool? EqualNotNullA(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("equalNotNullB")]
    public abstract bool? EqualNotNullB(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("equalNullA")]
    public abstract bool? EqualNullA(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("equalNullB")]
    public abstract bool? EqualNullB(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("ifNull")]
    public abstract bool? IfNull(object? obj = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isNull")]
    public abstract bool? IsNull(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("nullCoalescing")]
    public abstract string? NullCoalescing(string? code);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("nullCollation")]
    public abstract string? NullCollation(string? code);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("nullCollationAndCollation")]
    public abstract object? NullCollationAndCollation(string? code);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("nullCollationAndCollation2")]
    public abstract object? NullCollationAndCollation2(string? code);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("nullProperty")]
    public abstract bool? NullProperty(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("nullPropertyGE")]
    public abstract bool? NullPropertyGE(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("nullPropertyGT")]
    public abstract bool? NullPropertyGT(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("nullPropertyLE")]
    public abstract bool? NullPropertyLE(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("nullPropertyLT")]
    public abstract bool? NullPropertyLT(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("nullType")]
    public abstract void NullType();

    #endregion

}
