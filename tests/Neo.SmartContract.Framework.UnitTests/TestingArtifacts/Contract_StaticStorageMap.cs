using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_StaticStorageMap(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_StaticStorageMap"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""put"",""parameters"":[{""name"":""message"",""type"":""String""}],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""get"",""parameters"":[{""name"":""msg"",""type"":""String""}],""returntype"":""Integer"",""offset"":18,""safe"":false},{""name"":""putReadonly"",""parameters"":[{""name"":""message"",""type"":""String""}],""returntype"":""Void"",""offset"":43,""safe"":false},{""name"":""getReadonly"",""parameters"":[{""name"":""msg"",""type"":""String""}],""returntype"":""Integer"",""offset"":61,""safe"":false},{""name"":""put2"",""parameters"":[{""name"":""message"",""type"":""String""}],""returntype"":""Void"",""offset"":86,""safe"":false},{""name"":""get2"",""parameters"":[{""name"":""msg"",""type"":""String""}],""returntype"":""Integer"",""offset"":117,""safe"":false},{""name"":""teststoragemap_Putbyteprefix"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""Void"",""offset"":155,""safe"":false},{""name"":""teststoragemap_Getbyteprefix"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":195,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":241,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0YAVcAARF4XwDBRVOLUEHmPxiEQFcAAXhfAMFFU4tQQZJd6DFK2CYERRDbIUBXAAESeF8BwUVTi1BB5j8YhEBXAAF4XwHBRVOLUEGSXegxStgmBEUQ2yFAVwEBDARkYXRhQZv2Z84SwHATeGjBRVOLUEHmPxiEQFcBAQwEZGF0YUGb9mfOEsBweGjBRVOLUEGSXegxStgmBEUQ2yFAVwEBQZv2Z854EYhOEFHQUBLAcAB7DAV0ZXN0MWjBRVOLUEHmPxiEQFcBAUGb9mfOeBGIThBR0FASwHAMBXRlc3QxaMFFU4tQQZJd6DFK2CYERRDbIUBWAgwEZGF0YUGb9mfOEsBgDAxyZWFkb25seWRhdGFBm/ZnzhLAYUBlZFJ/"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("get")]
    public abstract BigInteger? Get(string? msg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("get2")]
    public abstract BigInteger? Get2(string? msg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getReadonly")]
    public abstract BigInteger? GetReadonly(string? msg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("put")]
    public abstract void Put(string? message);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("put2")]
    public abstract void Put2(string? message);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("putReadonly")]
    public abstract void PutReadonly(string? message);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("teststoragemap_Getbyteprefix")]
    public abstract BigInteger? Teststoragemap_Getbyteprefix(BigInteger? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("teststoragemap_Putbyteprefix")]
    public abstract void Teststoragemap_Putbyteprefix(BigInteger? x);

    #endregion

}
