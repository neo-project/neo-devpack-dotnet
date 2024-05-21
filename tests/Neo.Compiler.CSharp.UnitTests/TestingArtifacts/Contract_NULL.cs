using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_NULL : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NULL"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""isNull"",""parameters"":[{""name"":""value"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""equalNullA"",""parameters"":[{""name"":""value"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":11,""safe"":false},{""name"":""equalNullB"",""parameters"":[{""name"":""value"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":20,""safe"":false},{""name"":""equalNotNullA"",""parameters"":[{""name"":""value"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":29,""safe"":false},{""name"":""equalNotNullB"",""parameters"":[{""name"":""value"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":38,""safe"":false},{""name"":""nullCoalescing"",""parameters"":[{""name"":""code"",""type"":""String""}],""returntype"":""Any"",""offset"":47,""safe"":false},{""name"":""nullCollation"",""parameters"":[{""name"":""code"",""type"":""String""}],""returntype"":""String"",""offset"":63,""safe"":false},{""name"":""nullPropertyGT"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Boolean"",""offset"":84,""safe"":false},{""name"":""nullPropertyLT"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Boolean"",""offset"":98,""safe"":false},{""name"":""nullPropertyGE"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Boolean"",""offset"":112,""safe"":false},{""name"":""nullPropertyLE"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Boolean"",""offset"":126,""safe"":false},{""name"":""nullProperty"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Boolean"",""offset"":140,""safe"":false},{""name"":""ifNull"",""parameters"":[{""name"":""obj"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":154,""safe"":false},{""name"":""nullCollationAndCollation"",""parameters"":[{""name"":""code"",""type"":""String""}],""returntype"":""Any"",""offset"":171,""safe"":false},{""name"":""nullCollationAndCollation2"",""parameters"":[{""name"":""code"",""type"":""String""}],""returntype"":""Any"",""offset"":202,""safe"":false},{""name"":""nullType"",""parameters"":[],""returntype"":""Void"",""offset"":245,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPAAD9CQFXAQF4cGgLlyICQFcAAQt4lyICQFcAAXgLlyICQFcAAQt4mCICQFcAAXgLmCICQFcBAXhK2CQFERKMcGgiAkBXAQF4StgmCkUMBWxpbnV4cGgiAkBXAAF4StgkA8oQtyICQFcAAXhK2CQDyhC1IgJAVwABeErYJAPKELgiAkBXAAF4StgkA8oQtiICQFcAAXhK2CQDyhCYIgJAVwABeCYHEdsgIgcQ2yAiAkBXAQFBm/ZnznB4aEGSXegxStgmCkUMAXvbMNsoIgJAVwEBQZv2Z85wDAMxMTF4aEHmPxiEeGhBkl3oMUrYJgpFDAF72zDbKCICQFcBAAtwaErYJAY0BiIDRUBXAAFAEz8NVg=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("equalNotNullA")]
    public abstract bool? EqualNotNullA(IList<object>? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("equalNotNullB")]
    public abstract bool? EqualNotNullB(IList<object>? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("equalNullA")]
    public abstract bool? EqualNullA(IList<object>? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("equalNullB")]
    public abstract bool? EqualNullB(IList<object>? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("ifNull")]
    public abstract bool? IfNull(object? obj = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isNull")]
    public abstract bool? IsNull(IList<object>? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("nullCoalescing")]
    public abstract object? NullCoalescing(string? code);

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

    #region Constructor for internal use only

    protected Contract_NULL(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
