using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleInscription(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleInscription"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""addInscription"",""parameters"":[{""name"":""address"",""type"":""Hash160""},{""name"":""inscription"",""type"":""String""}],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""getInscription"",""parameters"":[{""name"":""address"",""type"":""Hash160""}],""returntype"":""String"",""offset"":99,""safe"":true}],""events"":[{""name"":""InscriptionAdded"",""parameters"":[{""name"":""arg1"",""type"":""Hash160""},{""name"":""arg2"",""type"":""String""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Description"":""A sample inscription contract."",""Version"":""0.0.1"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHJXAAJ4Qfgn7IwkMgwtVW5hdXRob3JpemVkOiBDYWxsZXIgaXMgbm90IHRoZSBhZGRyZXNzIG93bmVyOnl4QZv2Z85B5j8YhHl4EsAMEEluc2NyaXB0aW9uQWRkZWRBlQFvYUBXAAF4QZv2Z85Bkl3oMUCCZH/N"));

    #endregion

    #region Events

    public delegate void delInscriptionAdded(UInt160? arg1, string? arg2);

    [DisplayName("InscriptionAdded")]
    public event delInscriptionAdded? OnInscriptionAdded;

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getInscription")]
    public abstract string? GetInscription(UInt160? address);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("addInscription")]
    public abstract void AddInscription(UInt160? address, string? inscription);

    #endregion
}
